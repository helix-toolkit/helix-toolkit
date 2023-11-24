using HelixToolkit.SharpDX.Render;
using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IDynamicReflector
{
    bool IsDynamicScene
    {
        set; get;
    }
    bool EnableReflector
    {
        set; get;
    }
    Vector3 Center
    {
        set; get;
    }
    int FaceSize
    {
        set; get;
    }
    float NearField
    {
        set; get;
    }
    float FarField
    {
        set; get;
    }
    bool IsLeftHanded
    {
        set; get;
    }
    void BindCubeMap(DeviceContextProxy deviceContext);
    void UnBindCubeMap(DeviceContextProxy deviceContext);
}
