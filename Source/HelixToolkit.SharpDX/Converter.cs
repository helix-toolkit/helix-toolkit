using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX;

public static class Converter
{
    public static IList<int>? ToCollection(this IntCollection collection)
    {
        if (collection is null)
        {
            return null;
        }

        return collection.ToList();
    }

    public static float[]? ToFloatArray(this IList<double>? collection)
    {
        if (collection is null)
        {
            return null;
        }

        var result = new float[collection.Count];

        for (int i = 0; i < collection.Count; i++)
        {
            result[i] = (float)collection[i];
        }

        return result;
    }

    public static MeshGeometry3D ToMeshGeometry3D(this MeshGeometry3D mesh)
    {
        var mg = new MeshGeometry3D()
        {
            Normals = mesh.Normals is not null ? new Vector3Collection(mesh.Normals) : null,
            Positions = mesh.Positions is not null ? new Vector3Collection(mesh.Positions) : null,
            TextureCoordinates = mesh.TextureCoordinates is not null ? new Vector2Collection(mesh.TextureCoordinates) : null,
            TriangleIndices = mesh.TriangleIndices is not null ? new IntCollection(mesh.TriangleIndices) : null,
            Tangents = mesh.Tangents is not null ? new Vector3Collection(mesh.Tangents) : null,
            BiTangents = mesh.BiTangents is not null ? new Vector3Collection(mesh.BiTangents) : null
        };

        return mg;
    }

    public static MeshGeometry3D ToMeshGeometry3D(this MeshBuilder builder)
    {
        var mg = new MeshGeometry3D()
        {
            Normals = builder.Normals is not null ? new Vector3Collection(builder.Normals) : null,
            Positions = builder.Positions is not null ? new Vector3Collection(builder.Positions) : null,
            TextureCoordinates = builder.TextureCoordinates is not null ? new Vector2Collection(builder.TextureCoordinates) : null,
            TriangleIndices = builder.TriangleIndices is not null ? new IntCollection(builder.TriangleIndices) : null,
            Tangents = builder.Tangents is not null ? new Vector3Collection(builder.Tangents) : null,
            BiTangents = builder.BiTangents is not null ? new Vector3Collection(builder.BiTangents) : null
        };

        return mg;
    }

    public static Geometry.MeshGeometry3D ToWndMeshGeometry3D(this MeshGeometry3D mesh)
    {
        return new Geometry.MeshGeometry3D()
        {
            Normals = mesh.Normals,
            Positions = mesh.Positions ?? new(),
            TextureCoordinates = mesh.TextureCoordinates,
            TriangleIndices = mesh.TriangleIndices ?? new(),
            Tangents = mesh.Tangents,
            BiTangents = mesh.BiTangents
        };
    }
}
