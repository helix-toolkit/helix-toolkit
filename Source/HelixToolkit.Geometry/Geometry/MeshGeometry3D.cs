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

    public MeshGeometry3D Clone()
    {
        var clone = new MeshGeometry3D
        {
            Positions = new Vector3Collection(this.Positions),
            TriangleIndices = new IntCollection(this.TriangleIndices)
        };

        if (this.Normals != null)
            clone.Normals = new Vector3Collection(this.Normals);

        if (this.TextureCoordinates != null)
            clone.TextureCoordinates = new Vector2Collection(this.TextureCoordinates);

        if (this.Tangents != null)
            clone.Tangents = new Vector3Collection(this.Tangents);

        if (this.BiTangents != null)
            clone.BiTangents = new Vector3Collection(this.BiTangents);

        return clone;
    }
}
