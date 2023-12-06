using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using Transform3D = System.Windows.Media.Media3D.Transform3D;

namespace GroupElementDemo;

public class MeshDataModel
{
    public required Geometry3D Geometry { set; get; }
    public required Material Material { set; get; }
    public required Transform3D Transform { set; get; }
}
