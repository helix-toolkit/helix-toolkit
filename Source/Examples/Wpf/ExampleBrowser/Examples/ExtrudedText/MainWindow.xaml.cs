using HelixToolkit;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace ExtrudedText;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("ExtrudedText", "Demonstrates extruding text.")]
public partial class MainWindow : Window
{
    private MeshGeometry3D textGeometry;

    public MainWindow()
    {
        this.InitializeComponent();
        var builder = new MeshBuilder(false, false);
        builder.ExtrudeText(
            "Helix Toolkit",
            "Arial",
            FontStyles.Normal,
            FontWeights.Bold,
            20,
            new Vector3D(1, 0, 0),
            new Point3D(0, 0, 0),
            new Point3D(0, 0, 1));

        this.textGeometry = builder.ToMesh().ToMeshGeometry3D(true);
        this.DataContext = this;
    }

    public MeshGeometry3D TextGeometry
    {
        get
        {
            return this.textGeometry;
        }
    }
}
