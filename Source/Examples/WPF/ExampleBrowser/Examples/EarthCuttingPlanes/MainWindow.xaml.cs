using System.Windows;

namespace EarthCuttingPlanesDemo
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Clouds = MaterialHelper.CreateImageMaterial("pack://application:,,,/Examples/Earth/clouds.jpg", 0.5);
            this.DataContext = this;
        }

        public static readonly DependencyProperty CloudsProperty = DependencyProperty.Register(
            "Clouds", typeof(Material), typeof(MainWindow), new UIPropertyMetadata(null));
        public Material Clouds
        {
            get { return (Material)GetValue(CloudsProperty); }
            set { SetValue(CloudsProperty, value); }
        }
    }
}
