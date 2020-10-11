// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for the main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PointsAndLinesDemo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using _3DTools;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    using Petzold.Media3D;

    /// <summary>
    /// Interaction logic for the main window.
    /// </summary>
    [Example(null, "Renders text and lines.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly Stopwatch watch = new Stopwatch();

        private int numberOfPoints;

        private LinesVisual3D linesVisual;
        private PointsVisual3D pointsVisual;
        private ScreenSpaceLines3D screenSpaceLines;
        private WireLines wireLines;

        private Point3DCollection points;

        public MainWindow()
        {
            this.InitializeComponent();
            this.watch.Start();

            this.NumberOfPoints = 100;
            this.DataContext = this;

            CompositionTarget.Rendering += this.OnCompositionTargetRendering;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ShowLinesVisual3D { get; set; }

        public bool ShowPointsVisual3D { get; set; }

        public bool ShowScreenSpaceLines3D { get; set; }

        public bool ShowWireLines { get; set; }

        public bool IsMirrored { get; set; }

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

        public static IEnumerable<Point3D> GeneratePoints(int n, double time)
        {
            const double R = 2;
            const double Q = 0.5;
            for (int i = 0; i < n; i++)
            {
                double t = Math.PI * 2 * i / (n - 1);
                double u = (t * 24) + (time * 5);
                var pt = new Point3D(Math.Cos(t) * (R + (Q * Math.Cos(u))), Math.Sin(t) * (R + (Q * Math.Cos(u))), Q * Math.Sin(u));
                yield return pt;
                if (i > 0 && i < n - 1)
                {
                    yield return pt;
                }
            }
        }

        protected void RaisePropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            if (this.ShowLinesVisual3D && this.linesVisual == null)
            {
                this.linesVisual = new LinesVisual3D { Color = Colors.Blue };
                View1.Children.Add(this.linesVisual);
            }

            if (!this.ShowLinesVisual3D && this.linesVisual != null)
            {
                this.linesVisual.IsRendering = false;
                View1.Children.Remove(this.linesVisual);
                this.linesVisual = null;
            }

            if (this.ShowPointsVisual3D && this.pointsVisual == null)
            {
                this.pointsVisual = new PointsVisual3D { Color = Colors.Red, Size = 6 };
                View1.Children.Add(this.pointsVisual);
            }

            if (!this.ShowPointsVisual3D && this.pointsVisual != null)
            {
                this.pointsVisual.IsRendering = false;
                View1.Children.Remove(this.pointsVisual);
                this.pointsVisual = null;
            }

            if (this.ShowScreenSpaceLines3D && this.screenSpaceLines == null)
            {
                this.screenSpaceLines = new ScreenSpaceLines3D { Color = Colors.Green };
                View1.Children.Add(this.screenSpaceLines);
            }

            if (!this.ShowScreenSpaceLines3D && this.screenSpaceLines != null)
            {
                View1.Children.Remove(this.screenSpaceLines);
                this.screenSpaceLines = null;
            }

            if (this.ShowWireLines && this.wireLines == null)
            {
                this.wireLines = new WireLines { Color = Colors.Pink };
                View1.Children.Add(this.wireLines);
            }

            if (!this.ShowWireLines && this.wireLines != null)
            {
                View1.Children.Remove(this.wireLines);
                this.wireLines = null;
            }

            if (this.Points == null || this.Points.Count != this.NumberOfPoints)
            {
                this.Points = new Point3DCollection(GeneratePoints(this.NumberOfPoints, this.watch.ElapsedMilliseconds * 0.001));
            }

            if (this.linesVisual != null)
            {
                this.linesVisual.Points = this.Points;
                this.linesVisual.Transform = IsMirrored ? new ScaleTransform3D(-1, 1, 1) : new ScaleTransform3D(1, 1, 1);
            }

            if (this.pointsVisual != null)
            {
                this.pointsVisual.Points = this.Points;
                this.pointsVisual.Transform = IsMirrored ? new ScaleTransform3D(-1, 1, 1) : new ScaleTransform3D(1, 1, 1);
            }

            if (this.screenSpaceLines != null)
            {
                this.screenSpaceLines.Points = this.Points;
                this.screenSpaceLines.Transform = IsMirrored ? new ScaleTransform3D(-1, 1, 1) : new ScaleTransform3D(1, 1, 1);
            }

            if (this.wireLines != null)
            {
                this.wireLines.Lines = this.Points;
                this.wireLines.Transform = IsMirrored ? new ScaleTransform3D(-1, 1, 1) : new ScaleTransform3D(1, 1, 1);
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}