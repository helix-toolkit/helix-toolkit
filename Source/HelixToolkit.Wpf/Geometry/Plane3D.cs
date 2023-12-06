using System.Globalization;

namespace HelixToolkit.Wpf.Geometry;

/// <summary>
/// Represents a plane in three-dimensional space.
/// </summary>
public sealed class Plane3D : HelixToolkit.Plane3D
{

    public new string Normal
    {
        get
        {
            return Vector3ToString(base.Normal);
        }

        set
        {
            base.Normal = StringToVector3(value);
        }
    }

    public new string Position
    {
        get
        {
            return Vector3ToString(base.Position);
        }

        set
        {
            base.Position = StringToVector3(value);
        }
    }

    private static string Vector3ToString(System.Numerics.Vector3 v)
    {
        return v.X.ToString(CultureInfo.InvariantCulture) + "," + v.Y.ToString(CultureInfo.InvariantCulture) + "," + v.Z.ToString(CultureInfo.InvariantCulture);
    }

    private static System.Numerics.Vector3 StringToVector3(string str)
    {
        string[] parts = str.Split(',');

        float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
        float y = float.Parse(parts[1], CultureInfo.InvariantCulture);
        float z = float.Parse(parts[2], CultureInfo.InvariantCulture);

        return new System.Numerics.Vector3(x, y, z);
    }
}
