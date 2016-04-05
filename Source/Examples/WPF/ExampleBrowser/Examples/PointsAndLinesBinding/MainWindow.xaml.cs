// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for the main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PointsAndLinesBinding
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for the main window.
    /// </summary>
    [Example(null, "Binding listening to NotifyCollectionChanged.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public partial class MainWindow : INotifyPropertyChanged
    {
        private Point3DCollection linePoints = new Point3DCollection();

        private Point3DCollection points = new Point3DCollection();

        private readonly Stopwatch watch = new Stopwatch();

        private int numberOfPoints;

        public MainWindow()
        {
            this.InitializeComponent();
            this.watch.Start();

            this.NumberOfPoints = 100;
            this.DataContext = this;

            CompositionTarget.Rendering += this.OnCompositionTargetRendering;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int NumberOfPoints
        {
            get
            {
                return this.numberOfPoints;
            }

            set
            {
                this.numberOfPoints = value;
                this.RaisePropertyChanged("NumberOfPoints");
            }
        }

        public bool ShowLinesVisual3D { get; set; }

        public bool ShowPointsVisual3D { get; set; }

        public Point3DCollection LinePoints
        {
            get
            {
                return this.linePoints;
            }
            set
            {
                this.linePoints = value;
                this.RaisePropertyChanged("LinePoints");
            }
        }

        public Point3DCollection Points
        {
            get
            {
                return this.points;
            }

            set
            {
                this.points = value;
                this.RaisePropertyChanged("Points");
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            if (!this.ShowLinesVisual3D)
            {
                this.LinePoints.Clear();
            }

            if (!this.ShowPointsVisual3D)
            {
                this.Points.Clear();
            }

            if (this.ShowLinesVisual3D || this.ShowPointsVisual3D)
            {
                var newPoints = PointsAndLinesDemo.MainWindow.GeneratePoints(this.NumberOfPoints, this.watch.ElapsedMilliseconds * 0.001).ToArray();
                if (this.ShowPointsVisual3D)
                {
                    this.Points = new Point3DCollection(newPoints);
                }

                if (this.ShowLinesVisual3D)
                {
                    this.LinePoints = new Point3DCollection(newPoints);
                }
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
