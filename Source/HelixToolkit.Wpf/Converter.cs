using System.Runtime.CompilerServices;

namespace HelixToolkit.Wpf;

public static class Converter
{
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
    public static System.Windows.Media.Media3D.Point4D ToWnPoint4D(this System.Numerics.Vector4 vector)
    {
        return new System.Windows.Media.Media3D.Point4D(vector.X, vector.Y, vector.Z, vector.W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector4 ToVector4(this System.Windows.Media.Media3D.Point4D vector)
    {
        return new System.Numerics.Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, (float)vector.W);
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Windows.Media.Media3D.Rect3D ToWnRect3D(this HelixToolkit.Geometry.Rect3D rect)
    {
        return new System.Windows.Media.Media3D.Rect3D(rect.X, rect.Y, rect.Z, rect.SizeX, rect.SizeY, rect.SizeZ);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HelixToolkit.Geometry.Rect3D ToRect3D(this System.Windows.Media.Media3D.Rect3D rect)
    {
        return new HelixToolkit.Geometry.Rect3D((float)rect.X, (float)rect.Y, (float)rect.Z, (float)rect.SizeX, (float)rect.SizeY, (float)rect.SizeZ);
    }

    public static System.Windows.Media.Media3D.Vector3DCollection? ToVector3DCollection(IList<System.Numerics.Vector3>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new System.Windows.Media.Media3D.Vector3DCollection(collection.Select(v => v.ToWndVector3D()));
    }

    public static Vector3Collection? ToCollection(this System.Windows.Media.Media3D.Vector3DCollection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new Vector3Collection(collection.Select(t => t.ToVector3()));
    }

    public static System.Windows.Media.Media3D.Point3DCollection? ToPoint3DCollection(IList<System.Numerics.Vector3>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new System.Windows.Media.Media3D.Point3DCollection(collection.Select(v => v.ToWndPoint3D()));
    }

    public static Vector3Collection? ToCollection(this System.Windows.Media.Media3D.Point3DCollection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new Vector3Collection(collection.Select(t => t.ToVector3()));
    }

    public static System.Windows.Media.PointCollection? ToPointCollection(IList<System.Numerics.Vector2>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new System.Windows.Media.PointCollection(collection.Select(v => v.ToWndPoint()));
    }

    public static Vector2Collection? ToCollection(this System.Windows.Media.PointCollection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new Vector2Collection(collection.Select(t => t.ToVector()));
    }

    public static System.Windows.Media.Int32Collection? ToInt32Collection(IList<int>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new System.Windows.Media.Int32Collection(collection);
    }

    public static IntCollection? ToCollection(this System.Windows.Media.Int32Collection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new IntCollection(collection);
    }

    public static IList<float>? ToFloatCollection(this System.Windows.Media.DoubleCollection collection)
    {
        if (collection is null)
        {
            return null;
        }

        return collection.Select(t => (float)t).ToList();
    }

    public static System.Windows.Media.DoubleCollection? ToDoubleCollection(IList<float>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new System.Windows.Media.DoubleCollection(collection.Select(t => (double)t));
    }

    public static System.Windows.Media.Media3D.MeshGeometry3D ToMeshGeometry3D(this Geometry.MeshGeometry3D mesh, bool freeze = false)
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

    public static Geometry.MeshGeometry3D ToWndMeshGeometry3D(this System.Windows.Media.Media3D.MeshGeometry3D mesh)
    {
        return new Geometry.MeshGeometry3D()
        {
            Normals = mesh.Normals?.ToCollection(),
            Positions = mesh.Positions?.ToCollection() ?? new Vector3Collection(),
            TextureCoordinates = mesh.TextureCoordinates?.ToCollection(),
            TriangleIndices = mesh.TriangleIndices.ToCollection() ?? new IntCollection()
        };
    }
}
