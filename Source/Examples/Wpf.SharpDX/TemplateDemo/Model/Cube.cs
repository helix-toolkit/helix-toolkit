using HelixToolkit;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using SharpDX;

namespace TemplateDemo;

public class Cube : Shape
{
    private static readonly Geometry3D geometry;

    static Cube()
    {
        var b1 = new MeshBuilder();
        b1.AddBox(new Vector3(0, 0, 0), 1, 1, 1);
        geometry = b1.ToMeshGeometry3D();
    }

    protected override Geometry3D GetGeometry()
    {
        return geometry;
    }
}
