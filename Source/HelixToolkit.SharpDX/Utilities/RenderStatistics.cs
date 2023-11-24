using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public sealed class RenderStatistics : IRenderStatistics
{
    const string LineBreak = "\n---------\n";
    /// <summary>
    /// Gets the FPS statistics.
    /// </summary>
    /// <value>
    /// The FPS statistics.
    /// </value>
    public IFrameStatistics FPSStatistics { get; } = new FrameStatistics();
    /// <summary>
    /// Gets the render latency statistics.
    /// </summary>
    /// <value>
    /// The latency statistics.
    /// </value>
    public IFrameStatistics LatencyStatistics { get; } = new FrameStatistics();

    /// <summary>
    /// Gets the number of model3d per frame.
    /// </summary>
    /// <value>
    /// The number model3d.
    /// </value>
    public int NumModel3D { internal set; get; } = 0;
    /// <summary>
    /// Gets the number of render core3d per frame.
    /// </summary>
    /// <value>
    /// The number core3 d.
    /// </value>
    public int NumCore3D { internal set; get; } = 0;
    /// <summary>
    /// Gets the number triangles rendered in geometry model
    /// </summary>
    /// <value>
    /// The number triangles.
    /// </value>
    public int NumTriangles { internal set; get; } = 0;
    /// <summary>
    /// Gets the number draw calls per frame
    /// </summary>
    /// <value>
    /// The number draw calls.
    /// </value>
    public int NumDrawCalls { internal set; get; } = 0;
    /// <summary>
    /// Gets the frustum test time.
    /// </summary>
    /// <value>
    /// The frustum test time.
    /// </value>
    public float FrustumTestTime { internal set; get; } = 0;
    /// <summary>
    /// Gets or sets the camera.
    /// </summary>
    /// <value>
    /// The camera.
    /// </value>
    public ICamera? Camera
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the frame detail.
    /// </summary>
    /// <value>
    /// The frame detail.
    /// </value>
    public RenderDetail FrameDetail { set; get; } = RenderDetail.None;

    public string GetDetailString()
    {
        return GetDetailString(FrameDetail);
    }

    public string GetDetailString(RenderDetail detail)
    {
        if (detail == RenderDetail.None)
        {
            return string.Empty;
        }
        var s = string.Empty;
        if ((detail & RenderDetail.FPS) == RenderDetail.FPS)
        {
            s += GetFPS();
        }
        if ((detail & RenderDetail.Statistics) == RenderDetail.Statistics)
        {
            s += GetStatistics();
        }
        if ((detail & RenderDetail.TriangleInfo) == RenderDetail.TriangleInfo)
        {
            s += GetTriangleCount();
        }
        if ((detail & RenderDetail.Camera) == RenderDetail.Camera)
        {
            s += GetCamera();
        }
        return s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetFPS()
    {
        return $"FPS:{Math.Round(FPSStatistics.AverageFrequency, 2)}" + LineBreak;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetStatistics()
    {
        return $"Render(ms): {Math.Round(LatencyStatistics.AverageValue, 4)}\n" +
            $"NumModel3D: {NumModel3D}\n" +
            $"NumCore3D: {NumCore3D}\n" +
            $"NumDrawCalls: {NumDrawCalls}"
            + LineBreak;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetTriangleCount()
    {
        return $"NumTriangle: {NumTriangles}" + LineBreak;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetCamera()
    {
        return Camera == null ? string.Empty : "Camera:\n" + Camera.ToString() + LineBreak;
    }
    /// <summary>
    /// Resets this instance.
    /// </summary>
    public void Reset()
    {
        FPSStatistics.Reset();
        LatencyStatistics.Reset();
        NumTriangles = NumCore3D = NumModel3D = NumDrawCalls = 0;
        FrustumTestTime = 0;
    }
    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return GetDetailString(RenderDetail.FPS | RenderDetail.Statistics);
    }
}
