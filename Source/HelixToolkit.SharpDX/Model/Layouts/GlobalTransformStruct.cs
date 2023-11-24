using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct GlobalTransformStruct
{
    /// <summary>
    /// The view matrix
    /// </summary>
    public Matrix View;
    /// <summary>
    /// The projection matrix
    /// </summary>
    public Matrix Projection;
    /// <summary>
    /// The view projection matrix
    /// </summary>
    public Matrix ViewProjection;
    /// <summary>
    /// The frustum [fov,asepct-ratio,near,far]  
    /// </summary>
    public Vector4 Frustum;
    /// <summary>
    /// The viewport [w,h,1/w,1/h]      
    /// </summary>
    public Vector4 Viewport;
    /// <summary>
    /// Render target resolution [w, h, 1/w, 1/h]
    /// </summary>
    public Vector4 Resolution;
    /// <summary>
    /// The eye position
    /// </summary>
    public Vector3 EyePos;
    public uint SSAOEnabled;
    public float SSAOBias;
    public float SSAOIntensity;
    public float TimeStamp;
    public bool IsPerspective;
    public float OITWeightPower;
    public float OITWeightDepthSlope;
    public int OITWeightMode;
    public float DpiScale;
    public const int SizeInBytes = 4 * (4 * 4 * 3 + 4 * 6);
}
