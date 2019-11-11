using HelixToolkit.Wpf.SharpDX.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MemoryLeakTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Window testWin;
        private DispatcherTimer timer = null;
        private SystemStateParams systemparams = new SystemStateParams();
        private IList<Tuple<string, Type>> ProjectWinPairs = new List<Tuple<string, Type>>();

        public MainWindow()
        {
            InitializeComponent();
            //ProjectWinPairs.Add(new Tuple<string, Type>("CustomShaderDemo", typeof(CustomShaderDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("BoneSkinDemo", typeof(BoneSkinDemo.MainWindow)));
           // ProjectWinPairs.Add(new Tuple<string, Type>("DeferredShadingDemo", typeof(DeferredShadingDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("DynamicTextureDemo", typeof(DynamicTextureDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("InstancingDemo", typeof(InstancingDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("LightingDemo", typeof(LightingDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("OctreeDemo", typeof(OctreeDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("ParticleSystemDemo", typeof(ParticleSystemDemo.MainWindow)));
           // ProjectWinPairs.Add(new Tuple<string, Type>("ScreenSpaceDemo", typeof(ScreenSpaceDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("ShadowMapDemo", typeof(ShadowMapDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("TessellationDemo", typeof(TessellationDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("SimpleDemo", typeof(SimpleDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("MaterialDemo", typeof(MaterialDemo.MainWindow)));
            ProjectWinPairs.Add(new Tuple<string, Type>("EnvironmentMapDemo", typeof(EnvironmentMapDemo.MainWindow)));
            projectCombo.ItemsSource = ProjectWinPairs;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (timer == null)
            {               
                SharpDX.Configuration.EnableObjectTracking = true;
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(0.5);
                timer.Tick += Timer_Tick;
                systemparams.Count = 0;
                timer.Start();
                startButton.Visibility = Visibility.Collapsed;
                stopButton.Visibility = Visibility.Visible;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timer = null;
            startButton.Visibility = Visibility.Visible;
            stopButton.Visibility = Visibility.Collapsed;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (testWin == null)
            {
                CreateWindow();
            }
            else
            {
                testWin.Close();
                testWin = null;
                GC.Collect(0, GCCollectionMode.Forced);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.WaitForFullGCComplete();
                GC.Collect();
                Debug.WriteLine(SharpDX.Diagnostics.ObjectTracker.ReportActiveObjects());
                var log = systemparams.Update();
                paragraph.Inlines.Add(log);
                logTextbox.ScrollToEnd();
            }
        }

        private void CreateWindow()
        {
            if (projectCombo.SelectedIndex == -1)
            {
                testWin = new TestWindow();
            }
            else
            {
                var pair = ProjectWinPairs[projectCombo.SelectedIndex];
                testWin = Activator.CreateInstance(pair.Item2) as Window;
            }
            if (testWin != null)
            {
                testWin.Show();
            }
        }

        internal sealed class SystemStateParams
        {
            
            public long Count = 0;
            public long WorkingSet { private set; get; }
            public long PrivateMemory { private set; get; }
            public long ManagedMemory { private set; get; }
            public long HandleCount { private set; get; }
            public long ThreadCount { private set; get; }
            public bool NeedsUpdate { private set; get; }
            public double ChangePercent = 1; //10%
            public Run Update()
            {
                if(Count == 0)
                {
                    InitialBaseline();
                }
                ++Count;
                var proc = Process.GetCurrentProcess();

                var workingSet = proc.WorkingSet64;
                var privateMemory = proc.PrivateMemorySize64;
                var managedMemory = GC.GetTotalMemory(false);
                var handleCount = proc.HandleCount;
                var threadCount = proc.Threads.Count;
                var run = new Run($"{Count}  Total: {privateMemory / 1000000} MB; Physical: {workingSet / 1000000} MB; Managed: {managedMemory / 1000000} MB; Handle: {handleCount}; Threads: {threadCount};\n");
                if(Changed(PrivateMemory, privateMemory) 
                    || Changed(HandleCount, handleCount) || Changed(ThreadCount, threadCount))
                {
                    run.Foreground = new SolidColorBrush(Colors.Red);
                }
                return run;
            }

            private void InitialBaseline()
            {
                var proc = Process.GetCurrentProcess();

                WorkingSet = proc.WorkingSet64;
                PrivateMemory = proc.PrivateMemorySize64;
                ManagedMemory = GC.GetTotalMemory(false);
                HandleCount = proc.HandleCount;
                ThreadCount = proc.Threads.Count;
            }

            private bool Changed(long orgValue, long newValue)
            {
                return Math.Abs(orgValue - newValue) > orgValue * ChangePercent;
            }
        }
    }
}
