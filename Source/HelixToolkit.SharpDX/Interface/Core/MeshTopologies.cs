namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public static class MeshTopologies
{
    /// <summary>
    /// Gets the topologies.
    /// </summary>
    /// <value>
    /// The topologies.
    /// </value>
    public static IEnumerable<MeshTopologyEnum> Topologies
    {
        get
        {
            yield return MeshTopologyEnum.PNTriangles;
            yield return MeshTopologyEnum.PNQuads;
        }
    }
}
