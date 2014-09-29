namespace PointsAndLinesBinding
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Binding listening to NotifyCollectionChanged.")]
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ObservableCollection<Point3D> linePoints = new ObservableCollection<Point3D>();
        private readonly ObservableCollection<Point3D> points = new ObservableCollection<Point3D>();
        private int n;
        private bool isAnimating = true;
        Stopwatch watch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            watch.Start();

            N = 100;
            DataContext = this;

            CompositionTarget.Rendering += this.OnCompositionTargetRendering;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int N
        {
            get { return n; }
            set { n = value; RaisePropertyChanged(); }
        }

        public bool ShowLinesVisual3D { get; set; }

        public bool ShowPointsVisual3D { get; set; }

        public ObservableCollection<Point3D> LinePoints
        {
            get
            {
                return this.linePoints;
            }
        }

        public ObservableCollection<Point3D> Points
        {
            get
            {
                return this.points;
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            if (!ShowLinesVisual3D)
            {
                this.LinePoints.Clear();
            }
            if (!ShowPointsVisual3D)
            {
                this.Points.Clear();
            }
            if (isAnimating ||
               (ShowLinesVisual3D && this.LinePoints.Count != N) ||
               (ShowPointsVisual3D && this.Points.Count != N))
            {
                var newPoints = GeneratePoints(N, watch.ElapsedMilliseconds * 0.001);
                Points.Clear();
                LinePoints.Clear();
                if (ShowPointsVisual3D)
                {
                    for (int i = 0; i < n; i++)
                    {
                        Points.Add(newPoints[i]);
                    }
                }
                if (ShowLinesVisual3D)
                {
                    for (int i = 0; i < n; i++)
                    {
                        LinePoints.Add(newPoints[i]);
                    }
                }
            }
        }

        public Point3D[] GeneratePoints(int n, double time)
        {
            var result = new Point3D[n];
            double R = 2;
            double r = 0.5;
            for (int i = 0; i < n; i++)
            {
                double t = Math.PI * 2 * i / (n - 1);
                double u = t * 24 + time * 5;
                var pt = new Point3D(Math.Cos(t) * (R + r * Math.Cos(u)), Math.Sin(t) * (R + r * Math.Cos(u)), r * Math.Sin(u));
                result[i] =pt;
                if (i > 0 && i < n - 1)
                    result[i] =pt;
            }
            return result;
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
