using SharpDX.DXGI;

namespace HelixToolkit.SharpDX.Utilities;

public interface IRenderStatistics
{
    IFrameStatistics FPSStatistics
    {
        get;
    }
    IFrameStatistics LatencyStatistics
    {
        get;
    }
    int NumModel3D
    {
        get;
    }
    int NumCore3D
    {
        get;
    }
    int NumTriangles
    {
        get;
    }
    int NumDrawCalls
    {
        get;
    }
    float FrustumTestTime
    {
        get;
    }
    RenderDetail FrameDetail
    {
        set; get;
    }
    ICamera? Camera
    {
        set; get;
    }
    string GetDetailString();
    void Reset();
}
