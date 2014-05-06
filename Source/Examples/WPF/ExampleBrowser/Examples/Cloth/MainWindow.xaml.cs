// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ClothDemo
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Simulates cloth physics.")]
    public partial class MainWindow : Window
    {
        public GeometryModel3D FlagModel { get; set; }
        public Flag Flag { get; private set; }

        private readonly Stopwatch watch;
        private readonly Thread integratorThread;

        public MainWindow()
        {
            InitializeComponent();

            CreateFlag();

            DataContext = this;
            Loaded += MainWindow_Loaded;

            watch = new Stopwatch();
            watch.Start();
            integratorThread = new Thread(IntegrationWorker);
            integratorThread.Start();

            CompositionTarget.Rendering += this.OnCompositionTargetRendering;
            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            integratorThread.Abort();
        }

        private void IntegrationWorker()
        {
            while (true)
            {
                double dt = 1.0 * watch.ElapsedTicks / Stopwatch.Frequency;
                watch.Restart();
                Flag.Update(dt);
            }
        }

        void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            Flag.Transfer();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomExtents();
        }

        private void CreateFlag()
        {
            Flag = new Flag("Examples/Cloth/FlagOfNorway.png");
            Flag.Init();

            FlagModel = new GeometryModel3D
                            {
                                Geometry = Flag.Mesh,
                                Material = Flag.Material,
                                BackMaterial = Flag.Material
                            };
        }
    }
}