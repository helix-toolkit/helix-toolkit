namespace TessellationDemo
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Transform3DGroup = System.Windows.Media.Media3D.Transform3DGroup;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }

}
