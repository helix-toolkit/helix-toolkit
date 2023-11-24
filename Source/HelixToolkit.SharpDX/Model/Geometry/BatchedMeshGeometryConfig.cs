using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX;

public struct BatchedMeshGeometryConfig : IBatchedGeometry
{
    public Geometry3D Geometry
    {
        private set; get;
    }
    public Matrix ModelTransform
    {
        private set; get;
    }
    public int MaterialIndex
    {
        private set; get;
    }
    public BatchedMeshGeometryConfig(Geometry3D geometry, Matrix modelTransform, int materialIndex)
    {
        Geometry = geometry;
        ModelTransform = modelTransform;
        MaterialIndex = materialIndex;
    }
}
