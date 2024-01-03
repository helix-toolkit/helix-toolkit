using System.Numerics;

namespace HelixToolkit.Geometry;

/// <summary>
/// Represents a 3d rectangle.
/// </summary>
public sealed class Rect3D
{
    public static readonly Rect3D Empty = new();

    private Vector3 location;

    private Vector3 size;

    public Rect3D()
    {
        this.location = Vector3.Zero;
        this.size = Vector3.Zero;
    }

    public Rect3D(float x, float y, float z, float sizeX, float sizeY, float sizeZ)
    {
        this.location = new Vector3(x, y, z);
        this.size = new Vector3(sizeX, sizeY, sizeZ);
    }

    public Rect3D(Vector3 location, Vector3 size)
    {
        this.location = location;
        this.size = size;
    }

    public Vector3 Location
    {
        get
        {
            return this.location;
        }

        set
        {
            this.location = value;
        }
    }

    public float X
    {
        get
        {
            return this.location.X;
        }

        set
        {
            this.location.X = value;
        }
    }

    public float Y
    {
        get
        {
            return this.location.Y;
        }

        set
        {
            this.location.Y = value;
        }
    }

    public float Z
    {
        get
        {
            return this.location.Z;
        }

        set
        {
            this.location.Z = value;
        }
    }

    public Vector3 Size
    {
        get
        {
            return this.size;
        }

        set
        {
            this.size = value;
        }
    }

    public float SizeX
    {
        get
        {
            return this.size.X;
        }

        set
        {
            this.size.X = value;
        }
    }

    public float SizeY
    {
        get
        {
            return this.size.Y;
        }

        set
        {
            this.size.Y = value;
        }
    }

    public float SizeZ
    {
        get
        {
            return this.size.Z;
        }

        set
        {
            this.size.Z = value;
        }
    }

    /// <summary>
    /// Get the center point of <see cref="Rect3D"/>
    /// </summary>
    /// <returns>The center point of given Rect3D</returns>
    public Vector3 GetCenter()
    {
        return new Vector3(
            this.X + this.SizeX / 2,
            this.Y + this.SizeY / 2,
            this.Z + this.SizeZ / 2);
    }

    /// <summary>
    /// Expands the size of <see cref="Rect3D"/>
    /// in 3 directions by the given amount
    /// </summary>
    /// <param name="rect3d">The given Rect3D</param>
    /// <param name="expand">Amount of expansion</param>
    /// <returns>A newly expanded Rect3D</returns>
    public static Rect3D Expand(Rect3D rect3d, float expand)
    {
        if (rect3d == Rect3D.Empty
           || float.IsNaN(expand)
           || float.IsInfinity(expand)
           || float.IsNegativeInfinity(expand))
            return rect3d;
        float x = rect3d.X - expand;
        float y = rect3d.Y - expand;
        float z = rect3d.Z - expand;
        float sizeX = rect3d.SizeX + 2 * expand;
        float sizeY = rect3d.SizeY + 2 * expand;
        float sizeZ = rect3d.SizeZ + 2 * expand;
        return new Rect3D(x, y, z, sizeX, sizeY, sizeZ);
    }

    /// <summary>
    /// Merge a collection of <see cref="Rect3D"/>
    /// into one total <see cref="Rect3D"/>
    /// to be 
    /// </summary>
    /// <param name="rects">Collection Rect3D</param>
    /// <returns>A newly total Rect3D</returns>
    public static Rect3D Merge(IEnumerable<Rect3D> rects)
    {
        Rect3D result = Rect3D.Empty;
        foreach (var rect in rects)
        {
            result.Union(rect);
        }
        return result;
    }

    /// <summary>
    /// Update this rectangle to be the union of this and rect.
    /// </summary>
    /// <param name="rect">The given Rect3D.</param>
    /// <returns></returns>
    public void Union(Rect3D rect)
    {
        if (this.location == Vector3.Zero && this.size == Vector3.Zero)
        {
            this.location = rect.location;
            this.size = rect.size;
            return;
        }

        if (rect.location != Vector3.Zero || rect.size != Vector3.Zero)
        {
            Vector3 min = Vector3.Min(this.location, rect.location);
            Vector3 max = Vector3.Max(this.location + this.size, rect.location + rect.size);

            this.size = max - min;
            this.location = min;
        }
    }
}
