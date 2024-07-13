using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HelixToolkit.Wpf;

namespace MeshBuilderSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ExampleBrowser.Example("MeshBuilderSample", "Demonstrates the mesh build-in MeshBuilder.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public MeshGeometry3D GlassGeometry
        {
            get
            {
                var builder = new MeshBuilder(true, true);
                var profile = new[] { new Point(0, 0.4), new Point(0.06, 0.36), new Point(0.1, 0.1), new Point(0.34, 0.1), new Point(0.4, 0.14), new Point(0.5, 0.5), new Point(0.7, 0.56), new Point(1, 0.46) };
                builder.AddRevolvedGeometry(profile.ToVector2Collection()!, null, new Vector3(0, 0, 0), new Vector3(0, 0, 1), 100);
                return builder.ToMesh().ToWndMeshGeometry3D(true);
            }
        }
        public MeshGeometry3D Tube
        {
            get
            {
                var builder = new MeshBuilder(true, true);
                var profile = new Vector2[] { new Vector2(-3, 2), new Vector2(3, 2), new Vector2(3, -1), new Vector2(-3, -1) };
                builder.AddTube(
                    new Vector3[] { new Vector3(), new Vector3(0, 0, 1) },
                    null, null, profile,null,
                    false, true);

                return builder.ToMesh().ToWndMeshGeometry3D(true);
            }
        }
    }
}
