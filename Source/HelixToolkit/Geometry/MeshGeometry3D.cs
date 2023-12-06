using System.Numerics;
using System.Runtime.Serialization;

namespace HelixToolkit.Geometry;

/// <summary>
/// TODO
/// </summary>
public sealed class MeshGeometry3D
{
    public IList<Vector3> Positions { get; set; } = new List<Vector3>();

    public IList<Vector3>? Normals { get; set; }

    public IList<Vector2>? TextureCoordinates { get; set; }

    public IList<Vector3>? Tangents { get; set; }

    public IList<Vector3>? BiTangents { get; set; }

    public IList<int> TriangleIndices { get; set; } = new List<int>();
}
