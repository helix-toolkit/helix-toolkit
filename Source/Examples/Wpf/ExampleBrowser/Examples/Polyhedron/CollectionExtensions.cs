using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Polyhedron;

public static class CollectionExtensions
{
    public static void Add(this Int32Collection c, params int[] values)
    {
        foreach (var i in values)
            c.Add(i);
    }

    public static void Add(this Point3DCollection c, double x, double y, double z)
    {
        c.Add(new Point3D(x, y, z));
    }
}
