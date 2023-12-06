namespace HelixToolkit.SharpDX;

public class BatchedMeshHitTestResult : HitTestResult
{
    public int MeshConfigIndex
    {
        get;
    } = -1;

    public BatchedMeshGeometryConfig Config
    {
        get;
    }

    public BatchedMeshHitTestResult(int idx, ref BatchedMeshGeometryConfig config, HitTestResult result)
    {
        MeshConfigIndex = idx;
        Config = config;
        ShallowCopy(result);
    }
}
