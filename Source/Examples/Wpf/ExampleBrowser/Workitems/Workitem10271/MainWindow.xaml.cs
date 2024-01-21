using DependencyPropertyGenerator;
using HelixToolkit;
using HelixToolkit.Wpf;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Workitem10271;

/// <summary>
/// Interaction logic for the main window.
/// </summary>
[ExampleBrowser.Example("Work item 10271", "Quad Normals")]
[DependencyProperty<MeshGeometry3D>("MbTri", DefaultValueExpression = "BuildTri()")]
[DependencyProperty<MeshGeometry3D>("MbQuad", DefaultValueExpression = "BuildQuad()")]
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = this;
    }

    private static object BuildTri()
    {
        var mb = new MeshBuilder(true, false);
        var p0 = new Vector3(0, 0, 0);
        var p1 = new Vector3(1, 0, 0);
        var p2 = new Vector3(1, 1, 0);
        mb.AddTriangle(p0, p1, p2);
        mb.Normals?.ToList().ForEach(x => System.Diagnostics.Trace.WriteLine(x.ToString()));
        return mb.ToMesh().ToWndMeshGeometry3D();
    }

    private static object BuildQuad()
    {
        var mb = new MeshBuilder(true, false);
        var p0 = new Vector3(0, 0, 0);
        var p1 = new Vector3(1, 0, 0);
        var p2 = new Vector3(1, 1, 0);
        var p3 = new Vector3(0, 1, 0);
        mb.AddQuad(p0, p1, p2, p3);
        mb.Normals?.ToList().ForEach(x => System.Diagnostics.Trace.WriteLine(x.ToString()));
        return mb.ToMesh().ToWndMeshGeometry3D();
    }
}
