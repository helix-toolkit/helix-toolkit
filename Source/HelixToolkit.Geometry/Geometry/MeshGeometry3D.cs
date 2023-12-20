namespace HelixToolkit.Geometry;

/// <summary>
/// TODO
/// </summary>
public sealed class MeshGeometry3D
{
    public Vector3Collection Positions { get; set; } = new Vector3Collection();

    public Vector3Collection? Normals { get; set; }

    public Vector2Collection? TextureCoordinates { get; set; }

    public Vector3Collection? Tangents { get; set; }

    public Vector3Collection? BiTangents { get; set; }

    public IntCollection TriangleIndices { get; set; } = new IntCollection();
}
