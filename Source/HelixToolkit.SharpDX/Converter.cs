using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX;

public static class Converter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static global::SharpDX.Vector2 ToDxVector(this System.Numerics.Vector2 vector)
    {
        return new global::SharpDX.Vector2(vector.X, vector.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector2 ToVector(this global::SharpDX.Vector2 vector)
    {
        return new System.Numerics.Vector2(vector.X, vector.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static global::SharpDX.Vector3 ToDxVector(this System.Numerics.Vector3 vector)
    {
        return new global::SharpDX.Vector3(vector.X, vector.Y, vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector3 ToVector(this global::SharpDX.Vector3 vector)
    {
        return new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static global::SharpDX.Vector4 ToDxVector(this System.Numerics.Vector4 vector)
    {
        return new global::SharpDX.Vector4(vector.X, vector.Y, vector.Z, vector.W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Vector4 ToVector(this global::SharpDX.Vector4 vector)
    {
        return new System.Numerics.Vector4(vector.X, vector.Y, vector.Z, vector.W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static global::SharpDX.Matrix ToDxMatrix(this System.Numerics.Matrix4x4 matrix)
    {
        return new global::SharpDX.Matrix(
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Numerics.Matrix4x4 ToMatrix(this global::SharpDX.Matrix matrix)
    {
        return new System.Numerics.Matrix4x4(
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44);
    }

    public static IList<System.Numerics.Vector2>? ToCollection(this Vector2Collection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return collection.Select(t => t.ToVector()).ToList();
    }

    public static Vector2Collection? ToVector2Collection(IList<System.Numerics.Vector2>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new Vector2Collection(collection.Select(t => t.ToDxVector()));
    }

    public static IList<System.Numerics.Vector3>? ToCollection(this Vector3Collection? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return collection.Select(t => t.ToVector()).ToList();
    }

    public static Vector3Collection? ToVector3Collection(IList<System.Numerics.Vector3>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new Vector3Collection(collection.Select(t => t.ToDxVector()));
    }

    public static HelixToolkit.SharpDX.IntCollection? ToInt32Collection(IList<int>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        return new HelixToolkit.SharpDX.IntCollection(collection);
    }

    public static IList<int>? ToCollection(this HelixToolkit.SharpDX.IntCollection collection)
    {
        if (collection is null)
        {
            return null;
        }

        return collection.ToList();
    }

    public static HelixToolkit.SharpDX.MeshGeometry3D ToMeshGeometry3D(this HelixToolkit.Geometry.MeshGeometry3D mesh)
    {
        var mg = new HelixToolkit.SharpDX.MeshGeometry3D()
        {
            Normals = ToVector3Collection(mesh.Normals),
            Positions = ToVector3Collection(mesh.Positions),
            TextureCoordinates = ToVector2Collection(mesh.TextureCoordinates),
            TriangleIndices = ToInt32Collection(mesh.TriangleIndices),
            Tangents = ToVector3Collection(mesh.Tangents),
            BiTangents = ToVector3Collection(mesh.BiTangents)
        };

        return mg;
    }

    public static HelixToolkit.Geometry.MeshGeometry3D ToWndMeshGeometry3D(this HelixToolkit.SharpDX.MeshGeometry3D mesh)
    {
        return new HelixToolkit.Geometry.MeshGeometry3D()
        {
            Normals = mesh.Normals?.ToCollection(),
            Positions = mesh.Positions?.ToCollection() ?? new List<System.Numerics.Vector3>(),
            TextureCoordinates = mesh.TextureCoordinates?.ToCollection(),
            TriangleIndices = mesh.TriangleIndices?.ToCollection() ?? new List<int>(),
            Tangents = mesh.Tangents?.ToCollection(),
            BiTangents = mesh.BiTangents?.ToCollection()
        };
    }
}
