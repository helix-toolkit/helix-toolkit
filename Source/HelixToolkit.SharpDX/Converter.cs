using CommunityToolkit.Diagnostics;
using HelixToolkit.Geometry;
using System.Diagnostics;
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
    /// <summary>
    /// Converts the geometry to a <see cref="SharpDX.MeshGeometry3D"/>. Equivalent to MeshBuilder.ToMesh().ToMeshGeometry3D().
    /// All internal mesh builder data are directly assigned to the <see cref="SharpDX.MeshGeometry3D"/> without copying.
    /// User must call <see cref="MeshBuilder.Reset"/> to reset and reuse the mesh builder object to create new meshes.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MeshGeometry3D ToMeshGeometry3D(this MeshBuilder builder)
    {
        return builder.ToMesh().ToMeshGeometry3D();
    }
    /// <summary>
    /// Converts the <see cref="SharpDX.MeshGeometry3D"/> to a <see cref="Geometry.MeshGeometry3D"/>.
    /// All internal mesh builder data are directly assigned to the <see cref="Geometry.MeshGeometry3D"/> without copying.
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Converts the <see cref="Geometry.MeshGeometry3D"/> to a <see cref="SharpDX.MeshGeometry3D"/>.
    /// All internal mesh builder data are directly assigned to the <see cref="SharpDX.MeshGeometry3D"/> without copying.
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public static MeshGeometry3D ToMeshGeometry3D(this Geometry.MeshGeometry3D mesh)
    {
        return new MeshGeometry3D()
        {
            Positions = mesh.Positions,
            Normals = mesh.Normals,
            TextureCoordinates = mesh.TextureCoordinates,
            Tangents = mesh.Tangents,
            BiTangents = mesh.BiTangents,
            TriangleIndices= mesh.TriangleIndices
        };
    }
}
