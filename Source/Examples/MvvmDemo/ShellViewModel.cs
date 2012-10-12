// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using HelixToolkit.Wpf;

namespace MvvmDemo
{
    using System.ComponentModel.Composition;

    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; NotifyOfPropertyChange(() => Title); }
        }

        public ObservableCollection<Visual3D> Objects { get; set; }

        public ShellViewModel()
        {
            Title = "MVVM demo (using Caliburn.Micro)";
            Objects = new ObservableCollection<Visual3D>();
            Objects.CollectionChanged += this.Objects_CollectionChanged;

            // Initialize with two objects
            this.Add();
            this.Add();
        }

        void Objects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => CanAdd);
            NotifyOfPropertyChange(() => CanRemove);
        }

        public bool CanAdd
        {
            get
            {
                return Objects.Count < 10;
            }
        }

        public void Add()
        {
            if (Objects.Count == 0)
                Objects.Add(new DefaultLights());
            Objects.Add(new BoxVisual3D { Center = GetRandomPoint() });
        }

        private Random r = new Random();

        private Point3D GetRandomPoint()
        {
            int d = 10;
            return new Point3D(r.Next(d) - d / 2, r.Next(d) - d / 2, r.Next(d) - d / 2);
        }

        public bool CanRemove
        {
            get { return Objects.Count > 0; }
        }

        public void Remove()
        {
            Objects.RemoveAt(Objects.Count - 1);
        }

        public void Exit()
        {
            this.TryClose();
        }
    }
}