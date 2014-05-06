// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit examples">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EarthCuttingPlanesDemo
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Applies cutting planes to the Earth.")]
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty CloudsProperty = DependencyProperty.Register(
            "Clouds", typeof(Material), typeof(MainWindow), new UIPropertyMetadata(null));

        public MainWindow()
        {
            this.InitializeComponent();
            this.Clouds = MaterialHelper.CreateImageMaterial("pack://application:,,,/Examples/Earth/clouds.jpg", 0.5);
            this.DataContext = this;
        }

        public Material Clouds
        {
            get { return (Material)GetValue(CloudsProperty); }
            set { SetValue(CloudsProperty, value); }
        }
    }
}
