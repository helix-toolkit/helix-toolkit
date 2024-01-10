using System.Runtime.CompilerServices;

namespace HelixToolkit.Wpf;

public static class Converter
{
    #region Array
    public static float[]? ToFloatArray(this double[]? array)
    {
        if (array is null)
        {
            return null;
        }

        var result = new float[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            result[i] = (float)array[i];
        }

        return result;
    }

    public static double[]? ToDoubleArray(this float[]? array)
    {
        if (array is null)
        {
            return null;
        }

        var result = new double[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            result[i] = array[i];
        }

        return result;
    }
    #endregion
    #region System.Numerics.Vector2
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Point ToWndPoint(this System.Numerics.Vector2 vector)
    {
        return new System.Windows.Point(vector.X, vector.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector2 ToVector(this System.Windows.Point vector)
    {
        return new System.Numerics.Vector2((float)vector.X, (float)vector.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Size ToWndSize(this System.Numerics.Vector2 vector)
    {
        return new System.Windows.Size(vector.X, vector.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector2 ToVector(this System.Windows.Size size)
    {
        return new System.Numerics.Vector2((float)size.Width, (float)size.Height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Vector ToWndVector(this System.Numerics.Vector2 vector)
    {
        return new System.Windows.Vector(vector.X, vector.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector2 ToVector(this System.Windows.Vector vector)
    {
        return new System.Numerics.Vector2((float)vector.X, (float)vector.Y);
    }
    #endregion

    #region System.Numerics.Vector3
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Point3D ToWndPoint3D(this System.Numerics.Vector3 vector)
    {
        return new System.Windows.Media.Media3D.Point3D(vector.X, vector.Y, vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 ToVector3(this System.Windows.Media.Media3D.Point3D vector)
    {
        return new System.Numerics.Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Vector3D ToWndVector3D(this System.Numerics.Vector3 vector)
    {
        return new System.Windows.Media.Media3D.Vector3D(vector.X, vector.Y, vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 ToVector3(this System.Windows.Media.Media3D.Vector3D vector)
    {
        return new System.Numerics.Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Size3D ToWndSize3D(this System.Numerics.Vector3 vector)
    {
        return new System.Windows.Media.Media3D.Size3D(vector.X, vector.Y, vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 ToVector3(this System.Windows.Media.Media3D.Size3D size)
    {
        return new System.Numerics.Vector3((float)size.X, (float)size.Y, (float)size.Z);
    }
    #endregion

    #region System.Numerics.Vector4
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Point4D ToWnPoint4D(this System.Numerics.Vector4 vector)
    {
        return new System.Windows.Media.Media3D.Point4D(vector.X, vector.Y, vector.Z, vector.W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector4 ToVector4(this System.Windows.Media.Media3D.Point4D vector)
    {
        return new System.Numerics.Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, (float)vector.W);
    }
    #endregion

    #region System.Numerics.Matrix4x4
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Matrix3D ToWndMatrix3D(this System.Numerics.Matrix4x4 matrix)
    {
        return new System.Windows.Media.Media3D.Matrix3D(
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M32, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Matrix4x4 ToMatrix(this System.Windows.Media.Media3D.Matrix3D matrix)
    {
        return new System.Numerics.Matrix4x4(
            (float)matrix.M11, (float)matrix.M12, (float)matrix.M13, (float)matrix.M14,
            (float)matrix.M21, (float)matrix.M22, (float)matrix.M23, (float)matrix.M24,
            (float)matrix.M31, (float)matrix.M32, (float)matrix.M33, (float)matrix.M34,
            (float)matrix.OffsetX, (float)matrix.OffsetY, (float)matrix.OffsetZ, (float)matrix.M44);
    }
    #endregion

    #region System.Windows.Media.Media3D.Vector3D
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Point3D ToWndPoint3D(this System.Windows.Media.Media3D.Vector3D vector)
    {
        return new System.Windows.Media.Media3D.Point3D(vector.X, vector.Y, vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Vector3D ToWndVector3D(this System.Windows.Media.Media3D.Point3D vector)
    {
        return new System.Windows.Media.Media3D.Vector3D(vector.X, vector.Y, vector.Z);
    }
    #endregion

    #region HelixToolkit.Maths.BoundingBox
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Rect3D ToWnRect3D(this HelixToolkit.Maths.BoundingBox boundingBox)
    {
        return new System.Windows.Media.Media3D.Rect3D(boundingBox.Minimum.ToWndPoint3D(), boundingBox.Size.ToWndSize3D());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HelixToolkit.Maths.BoundingBox ToBoundingBox(this System.Windows.Media.Media3D.Rect3D rect)
    {
        return new HelixToolkit.Maths.BoundingBox(rect.Location.ToVector3(), rect.Location.ToVector3() + rect.Size.ToVector3());
    }
    #endregion

    #region System.Numerics.Plane
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HelixToolkit.Wpf.Plane3D ToPlane3D(this System.Numerics.Plane plane)
    {
        return new HelixToolkit.Wpf.Plane3D((plane.Normal * plane.D).ToWndPoint3D(), plane.Normal.ToWndVector3D());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Plane ToPlane(this HelixToolkit.Wpf.Plane3D plane)
    {
        return Maths.PlaneHelper.Create(plane.Position.ToVector3(), plane.Normal.ToVector3());
    }
    #endregion

    #region HelixToolkit.Maths.Ray
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HelixToolkit.Wpf.Ray3D ToRay3D(this HelixToolkit.Maths.Ray ray)
    {
        return new HelixToolkit.Wpf.Ray3D(ray.Position.ToWndPoint3D(), ray.Direction.ToWndVector3D());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HelixToolkit.Maths.Ray ToRay(this HelixToolkit.Wpf.Ray3D ray)
    {
        return new HelixToolkit.Maths.Ray(ray.Origin.ToVector3(), ray.Direction.ToVector3());
    }
    #endregion

    #region Collection
    public static System.Windows.Media.Media3D.Vector3DCollection? ToVector3DCollection(IList<System.Numerics.Vector3>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        var newCollection = new System.Windows.Media.Media3D.Vector3DCollection(collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToWndVector3D());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector3Collection? ToCollection(this System.Windows.Media.Media3D.Vector3DCollection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        var newCollection = new HelixToolkit.Vector3Collection(collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector3());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector3Collection? ToVector3Collection(this IList<System.Windows.Media.Media3D.Point3D>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        var newCollection = new HelixToolkit.Vector3Collection(collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector3());
        }

        return newCollection;
    }

    public static System.Windows.Media.Media3D.Point3DCollection? ToPoint3DCollection(IList<System.Numerics.Vector3>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        var newCollection = new System.Windows.Media.Media3D.Point3DCollection(collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToWndPoint3D());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector3Collection? ToCollection(this System.Windows.Media.Media3D.Point3DCollection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        var newCollection = new HelixToolkit.Vector3Collection(collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector3());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector2Collection? ToVector2Collection(this IList<System.Windows.Point>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        var newCollection = new HelixToolkit.Vector2Collection(collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector());
        }

        return newCollection;
    }

    public static System.Windows.Media.PointCollection? ToPointCollection(IList<System.Numerics.Vector2>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        var newCollection = new System.Windows.Media.PointCollection(collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToWndPoint());
        }

        return newCollection;
    }

    public static HelixToolkit.Vector2Collection? ToCollection(this System.Windows.Media.PointCollection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        var newCollection = new HelixToolkit.Vector2Collection(collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add(collection[i].ToVector());
        }

        return newCollection;
    }

    public static System.Windows.Media.Int32Collection? ToInt32Collection(IList<int>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new System.Windows.Media.Int32Collection(collection);
    }

    public static HelixToolkit.IntCollection? ToCollection(this System.Windows.Media.Int32Collection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new HelixToolkit.IntCollection(collection);
    }

    public static IList<float>? ToFloatCollection(this System.Windows.Media.DoubleCollection collection)
    {
        if (collection is null)
        {
            return null;
        }

        var newCollection = new List<float>(collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add((float)collection[i]);
        }

        return newCollection;
    }

    public static System.Windows.Media.DoubleCollection? ToDoubleCollection(IList<float>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        var newCollection = new System.Windows.Media.DoubleCollection(collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            newCollection.Add((double)collection[i]);
        }

        return newCollection;
    }

    public static System.Windows.Media.Media3D.MeshGeometry3D ToMeshGeometry3D(this HelixToolkit.Geometry.MeshGeometry3D mesh, bool freeze = false)
    {
        var mg = new System.Windows.Media.Media3D.MeshGeometry3D()
        {
            Normals = ToVector3DCollection(mesh.Normals),
            Positions = ToPoint3DCollection(mesh.Positions),
            TextureCoordinates = ToPointCollection(mesh.TextureCoordinates),
            TriangleIndices = ToInt32Collection(mesh.TriangleIndices)
        };

        if (freeze)
        {
            mg.Freeze();
        }

        return mg;
    }

    public static HelixToolkit.Geometry.MeshGeometry3D ToWndMeshGeometry3D(this System.Windows.Media.Media3D.MeshGeometry3D mesh)
    {
        return new HelixToolkit.Geometry.MeshGeometry3D()
        {
            Normals = mesh.Normals?.ToCollection(),
            Positions = mesh.Positions?.ToCollection() ?? new HelixToolkit.Vector3Collection(),
            TextureCoordinates = mesh.TextureCoordinates?.ToCollection(),
            TriangleIndices = mesh.TriangleIndices.ToCollection() ?? new HelixToolkit.IntCollection()
        };
    }
    #endregion 
}
