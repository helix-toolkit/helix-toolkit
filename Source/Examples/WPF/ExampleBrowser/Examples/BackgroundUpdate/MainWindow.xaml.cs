namespace BackgroundUpdateDemo
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Updates the visual model in a background thread.")]
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private CancellationTokenSource source;

        Material mat2 = MaterialHelper.CreateMaterial(Colors.Red);

        int n = 10;

        private int count1;

        private int count2;

        private int count3;

        private int count4;

        private int runningWorkers;

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += this.MainWindowLoaded;
            this.Closing += this.MainWindowClosing;
            this.AddPoints = true;
            this.AddFrozenGeometry = true;
            this.AddFrozenModel = true;
            this.AddToModelGroup = true;
        }

        public bool AddPoints { get; set; }

        public bool AddFrozenGeometry { get; set; }

        public bool AddFrozenModel { get; set; }

        public bool AddToModelGroup { get; set; }

        public int Count1
        {
            get
            {
                return this.count1;
            }
            set
            {
                this.count1 = value;
                this.OnPropertyChanged("Count1");
            }
        }

        public int Count2
        {
            get
            {
                return this.count2;
            }
            set
            {
                this.count2 = value;
                this.OnPropertyChanged("Count2");
            }
        }

        public int Count3
        {
            get
            {
                return this.count3;
            }
            set
            {
                this.count3 = value;
                this.OnPropertyChanged("Count3");
            }
        }

        public int Count4
        {
            get
            {
                return this.count4;
            }
            set
            {
                this.count4 = value;
                this.OnPropertyChanged("Count4");
            }
        }
        void MainWindowClosing(object sender, CancelEventArgs e)
        {
            this.source.Cancel();
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Materials.Gold.Freeze();

            this.source = new CancellationTokenSource();
            Task.Factory.StartNew(this.Worker1, this.Dispatcher, source.Token);
            Task.Factory.StartNew(this.Worker2, this.Dispatcher, source.Token);
            Task.Factory.StartNew(this.Worker3, this.Dispatcher, source.Token);
            Task.Factory.StartNew(this.Worker4, this.Dispatcher, source.Token);
        }

        private void Worker1(object d)
        {
            var dispatcher = (Dispatcher)d;
            Interlocked.Increment(ref this.runningWorkers);
            while (!this.source.IsCancellationRequested)
            {
                if (!this.AddPoints || this.runningWorkers < 4)
                {
                    Thread.Yield();
                    continue;
                }

                for (int i = 1; i <= n && this.AddPoints; i++)
                {
                    for (int j = 1; j <= n && this.AddPoints; j++)
                    {
                        for (int k = 1; k <= n && this.AddPoints; k++)
                        {
                            dispatcher.Invoke(new Action<Point3D, ModelVisual3D>(this.Add), new Point3D(-i, j, k), this.model1);
                        }
                    }
                }

                dispatcher.Invoke((Action)(() => this.Count1++));
                dispatcher.Invoke(new Action<ModelVisual3D>(this.Clear), this.model1);
            }
        }

        private void Worker2(object d)
        {
            var dispatcher = (Dispatcher)d;
            Interlocked.Increment(ref this.runningWorkers);
            while (!this.source.IsCancellationRequested)
            {
                if (!this.AddFrozenGeometry || this.runningWorkers < 4)
                {
                    Thread.Yield();
                    continue;
                }

                for (int i = 1; i <= n; i++)
                {
                    var b = new MeshBuilder();
                    for (int j = 1; j <= n; j++)
                    {
                        for (int k = 1; k <= n; k++)
                        {
                            b.AddBox(new Point3D(i, j, k), 0.8, 0.8, 0.8);
                        }
                    }

                    dispatcher.Invoke(new Action<MeshGeometry3D, Material, ModelVisual3D>(this.Add), b.ToMesh(true), mat2, model2);
                }

                dispatcher.Invoke((Action)(() => this.Count2++));

                dispatcher.Invoke(new Action<ModelVisual3D>(this.Clear), this.model2);
            }
        }

        private void Worker3(object d)
        {
            var dispatcher = (Dispatcher)d;
            var m = MaterialHelper.CreateMaterial(Colors.Green);
            Interlocked.Increment(ref this.runningWorkers);
            while (!this.source.IsCancellationRequested)
            {
                if (!this.AddFrozenModel || this.runningWorkers < 4)
                {
                    Thread.Yield();
                    continue;
                }

                for (int i = 1; i <= n; i++)
                {
                    var b = new MeshBuilder();
                    for (int j = 1; j <= n; j++)
                    {
                        for (int k = 1; k <= n; k++)
                        {
                            b.AddBox(new Point3D(i, j, -k), 0.8, 0.8, 0.8);
                        }
                    }

                    var box = new GeometryModel3D { Geometry = b.ToMesh(false), Material = m };
                    box.Freeze();

                    dispatcher.Invoke(new Action<Model3D, ModelVisual3D>(this.Add), box, this.model3);
                }

                dispatcher.Invoke((Action)(() => this.Count3++));
                dispatcher.Invoke(new Action<ModelVisual3D>(this.Clear), model3);
            }
        }

        private void Worker4(object d)
        {
            var dispatcher = (Dispatcher)d;
            var m = MaterialHelper.CreateMaterial(Colors.Gold);
            Model3DGroup mg = null;
            dispatcher.Invoke(new Action(() => this.model4.Content = mg = new Model3DGroup()));

            Interlocked.Increment(ref this.runningWorkers);

            while (!this.source.IsCancellationRequested)
            {
                if (!this.AddToModelGroup || this.runningWorkers < 4)
                {
                    Thread.Yield();
                    continue;
                }

                for (int i = 1; i <= n; i++)
                {
                    var b = new MeshBuilder();
                    for (int j = 1; j <= n; j++)
                    {
                        for (int k = 1; k <= n; k++)
                        {
                            b.AddBox(new Point3D(-i, j, -k), 0.8, 0.8, 0.8);
                        }
                    }

                    var box = new GeometryModel3D { Geometry = b.ToMesh(false), Material = m };
                    box.Freeze();
                    dispatcher.Invoke(new Action(() => mg.Children.Add(box)));
                }
                dispatcher.Invoke((Action)(() => this.Count4++));
                dispatcher.Invoke((Action)(() => mg.Children.Clear()));
            }
        }

        private void Clear(ModelVisual3D visual)
        {
            visual.Children.Clear();
        }

        private void Add(Point3D p, ModelVisual3D visual)
        {
            // Create the box visual on the UI thread
            var box = new BoxVisual3D { Center = p, Width = 0.8, Height = 0.8, Length = 0.8 };
            visual.Children.Add(box);
        }

        private void Add(MeshGeometry3D g, Material m, ModelVisual3D visual)
        {
            var box = new GeometryModel3D { Geometry = g, Material = m };
            visual.Children.Add(new ModelVisual3D { Content = box });
        }

        private void Add(Model3D model, ModelVisual3D visual)
        {
            visual.Children.Add(new ModelVisual3D { Content = model });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
