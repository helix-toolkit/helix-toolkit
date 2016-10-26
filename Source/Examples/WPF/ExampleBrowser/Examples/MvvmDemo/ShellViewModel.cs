// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MvvmDemo
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;
    using HelixToolkit.Wpf;
    using ModelViewer;

    public class ShellViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title { get; }

        public ObservableCollection<Visual3D> Objects { get; set; }

        public ShellViewModel()
        {
            this.Title = "MVVM demo";
            this.Objects = new ObservableCollection<Visual3D>();
            this.AddCommand = new DelegateCommand(this.Add, this.CanAdd);
            this.RemoveCommand = new DelegateCommand(this.Remove, this.CanRemove);

            // Initialize with two objects
            this.Add();
            this.Add();
        }
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        public bool CanAdd()
        {
            return this.Objects.Count < 10;
        }

        public void Add()
        {
            if (this.Objects.Count == 0)
            {
                this.Objects.Add(new DefaultLights());
            }

            this.Objects.Add(new BoxVisual3D { Center = this.GetRandomPoint() });
        }

        private readonly Random r = new Random();

        private Point3D GetRandomPoint()
        {
            int d = 10;
            return new Point3D(this.r.Next(d) - d / 2, this.r.Next(d) - d / 2, this.r.Next(d) - d / 2);
        }

        public bool CanRemove()
        {
            return this.Objects.Count > 0;
        }

        public void Remove()
        {
            this.Objects.RemoveAt(this.Objects.Count - 1);
        }
    }
}