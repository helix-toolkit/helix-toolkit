// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using BatchRender.Properties;

namespace BatchRender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel vm = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += TimerTick;
            timer.Start();
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Settings.Default.Save();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            vm.CheckStatus();
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var lb = (ListBox)sender;
                var jobsToRemove = lb.SelectedItems.Cast<Job>().ToList();
                foreach (var job in jobsToRemove)
                {
                    vm.Jobs.Remove(job);
                }
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data as DataObject;
            if (data != null && data.ContainsFileDropList())
            {
                foreach (var path in data.GetFileDropList())
                {
                    if (File.Exists(path))
                        vm.Add(path);
                    else
                        vm.Search(path);
                }
            }
        }
    }

    public class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private bool changeExecutable;

        public MainViewModel()
        {
            if (String.IsNullOrEmpty(Executable))
            {
                Executable = @"C:\Program Files (x86)\Kerkythea Rendering System\Kerkythea.exe";
                if (!File.Exists(Executable))
                {
                    Executable = @"C:\Program Files\Kerkythea Rendering System\Kerkythea.exe";
                }
            }
            if (!File.Exists(Executable))
                ChangeExecutable = true;

            Jobs = new ObservableCollection<Job>();
            ProcessList = new ObservableCollection<Process>();
            Processes = 1;
        }

        public ObservableCollection<Process> ProcessList { get; set; }

        public static string Executable
        {
            get { return Settings.Default.Executable; }
            set { Settings.Default.Executable = value; }
        }

        public bool ChangeExecutable
        {
            get { return changeExecutable; }
            set
            {
                changeExecutable = value;
                RaisePropertyChanged("ExecutableVisibility");
            }
        }

        public Visibility ExecutableVisibility
        {
            get { return ChangeExecutable ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IsRunning { get; set; }
        public int Processes { get; set; }

        public ObservableCollection<Job> Jobs { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Executable" && !File.Exists(Executable))
                {
                    return "Executable does not exist.";
                }
                return null;
            }
        }

        public string Error
        {
            get
            {
                if (!File.Exists(Executable))
                {
                    return "Executable does not exist.";
                }
                return null;
            }
        }

        public void Search(string dir)
        {
            foreach (string file in Directory.EnumerateFiles(dir, "*.xml"))
            {
                var job = new Job { FullPath = file };
                Jobs.Add(job);
            }
        }

        public void Add(string filename)
        {
            var job = new Job { FullPath = filename };
            Jobs.Add(job);
        }

        public void StartOne()
        {
            if (Jobs.Count == 0)
            {
                return;
            }
            Job job = Jobs.First();
            //Interlocked.Increment(ref runningJobs);
            //RaisePropertyChanged("RunningJobs");
            Jobs.Remove(job);
            job.Execute();
            ProcessList.Add(job.Process);
            // job.Process.Exited += OnJobFinished;
        }

        //private void OnJobFinished(object sender, EventArgs eventArgs)
        //{
        //    //Interlocked.Decrement(ref runningJobs);
        //    //RaisePropertyChanged("RunningJobs");
        //}

        public void CheckStatus()
        {
            List<Process> remove = ProcessList.Where(p => p.HasExited).ToList();
            foreach (Process p in remove)
            {
                ProcessList.Remove(p);
            }

            if (ProcessList.Count < Processes && IsRunning)
            {
                StartOne();
            }
        }
    }

    public class Job
    {
        public string FullPath { get; set; }
        public Process Process { get; set; }

        public override string ToString()
        {
            return FullPath;
        }

        public void Execute()
        {
            string input = Path.GetFileName(FullPath);
            string output = Path.ChangeExtension(input, ".png");
            var psi = new ProcessStartInfo(MainViewModel.Executable)
                          {
                              WorkingDirectory = Path.GetDirectoryName(FullPath),
                              Arguments = input + " -o " + output,
                              WindowStyle = ProcessWindowStyle.Minimized,
                              CreateNoWindow = true
                          };
            Process = Process.Start(psi);
        }
    }
}