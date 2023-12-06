using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.ShaderManager;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Used to hold shared variables for Lights per scene
/// </summary>
public sealed class Light3DSceneShared : DisposeObject
{
    public readonly LightsBufferModel LightModels = new();

    private IBufferProxy? buffer;
    /// <summary>
    /// 
    /// </summary>
    public Light3DSceneShared(IConstantBufferPool pool)
    {
        buffer = pool.Register(DefaultBufferNames.LightCB, LightsBufferModel.SizeInBytes);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UploadToBuffer(DeviceContextProxy context)
    {
        LightModels.UploadToBuffer(buffer, context);
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref buffer);
        base.OnDispose(disposeManagedResources);
    }
}
