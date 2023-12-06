using HelixToolkit;
using HelixToolkit.SharpDX;
using SharpDX;

namespace TemplateDemo;

public class Sphere : Shape
{
    private static readonly Geometry3D geometry;

    static Sphere()
    {
        var b1 = new MeshBuilder();
        b1.AddSphere(new Vector3(0, 0, 0).ToVector(), 0.5f);
        geometry = b1.ToMesh().ToMeshGeometry3D();
    }

    protected override Geometry3D GetGeometry()
    {
        return geometry;
    }
}
