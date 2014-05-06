// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PointsAndLinesDemo
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    using Petzold.Media3D;

    using _3DTools;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Renders text and lines.")]
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public bool ShowLinesVisual3D { get; set; }
        public bool ShowPointsVisual3D { get; set; }
        public bool ShowScreenSpaceLines3D { get; set; }
        public bool ShowWireLines { get; set; }
        public Point3DCollection Points { get; set; }
        private int n;
        public int N
        {
            get { return n; }
            set { n = value; RaisePropertyChanged("N"); }
        }

        Stopwatch watch = new Stopwatch();

        private bool isAnimating = false;

        private LinesVisual3D lines;
        private PointsVisual3D points;
        private ScreenSpaceLines3D screenSpaceLines;
        private WireLines wireLines;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            watch.Start();

            N = 10000;
            DataContext = this;

            CompositionTarget.Rendering += this.OnCompositionTargetRendering;
        }

        void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            if (ShowLinesVisual3D && lines == null)
            {
                lines = new LinesVisual3D { Color = Colors.Blue };
                view1.Children.Add(lines);
            }
            if (!ShowLinesVisual3D && lines != null)
            {
                lines.IsRendering = false;
                view1.Children.Remove(lines);
                lines = null;
            }
            if (ShowPointsVisual3D && points == null)
            {
                points = new PointsVisual3D { Color = Colors.Red, Size = 6 };
                view1.Children.Add(points);
            }
            if (!ShowPointsVisual3D && points != null)
            {
                points.IsRendering = false;
                view1.Children.Remove(points);
                points = null;
            }
            if (ShowScreenSpaceLines3D && screenSpaceLines == null)
            {
                screenSpaceLines = new ScreenSpaceLines3D { Color = Colors.Green };
                view1.Children.Add(screenSpaceLines);
            }
            if (!ShowScreenSpaceLines3D && screenSpaceLines != null)
            {
                view1.Children.Remove(screenSpaceLines);
                screenSpaceLines = null;
            }
            if (ShowWireLines && wireLines == null)
            {
                wireLines = new WireLines { Color = Colors.Pink };
                view1.Children.Add(wireLines);
            }
            if (!ShowWireLines && wireLines != null)
            {
                view1.Children.Remove(wireLines);
                wireLines = null;
            }

            if (Points == null || Points.Count != N || isAnimating)
            {
                Points = GeneratePoints(N, watch.ElapsedMilliseconds * 0.001);
                RaisePropertyChanged("Points");
            }

            if (lines != null)
                lines.Points = Points;
            if (points != null)
                points.Points = Points;
            if (screenSpaceLines != null)
                screenSpaceLines.Points = Points;
            if (wireLines != null)
                wireLines.Lines = Points;
        }

        public Point3DCollection GeneratePoints(int n, double time)
        {
            var result = new Point3DCollection(n);
            double R = 2;
            double r = 0.5;
            for (int i = 0; i < n; i++)
            {
                double t = Math.PI * 2 * i / (n - 1);
                double u = t * 24 + time * 5;
                var pt = new Point3D(Math.Cos(t) * (R + r * Math.Cos(u)), Math.Sin(t) * (R + r * Math.Cos(u)), r * Math.Sin(u));
                result.Add(pt);
                if (i > 0 && i < n - 1)
                    result.Add(pt);
            }
            return result;
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}