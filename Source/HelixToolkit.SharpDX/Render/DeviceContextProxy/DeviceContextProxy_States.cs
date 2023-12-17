using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Mathematics.Interop;
using System.Runtime.CompilerServices;
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;

namespace HelixToolkit.SharpDX.Render;

public partial class DeviceContextProxy
{
    /// <summary>
    /// Sets the state of the raster.
    /// </summary>
    /// <param name="rasterState">State of the raster.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetRasterState(RasterizerStateProxy? rasterState)
    {
        if (AutoSkipRedundantStateSetting && currRasterState == rasterState)
        {
            return;
        }

        if (deviceContext is null || rasterState is null)
        {
            return;
        }

        deviceContext.Rasterizer.State = rasterState;
        currRasterState = rasterState;
    }

    /// <summary>
    /// Sets the state of the depth stencil.
    /// </summary>
    /// <param name="depthStencilState">State of the depth stencil.</param>
    /// <param name="stencilRef">The stencil reference.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetDepthStencilState(DepthStencilStateProxy depthStencilState, int stencilRef = 0)
    {
        if (AutoSkipRedundantStateSetting && currDepthStencilState == depthStencilState && currStencilRef == stencilRef)
        {
            return;
        }
        deviceContext?.OutputMerger.SetDepthStencilState(depthStencilState, stencilRef);
        currDepthStencilState = depthStencilState;
        currStencilRef = stencilRef;
    }

    /// <summary>
    /// Sets the state of the blend.
    /// </summary>
    /// <param name="blendState">State of the blend.</param>
    /// <param name="blendFactor">The blend factor.</param>
    /// <param name="sampleMask">The sample mask.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBlendState(BlendStateProxy blendState, Color4? blendFactor = null, int sampleMask = -1)
    {
        if (AutoSkipRedundantStateSetting && currBlendState == blendState && blendFactor == currBlendFactor && currSampleMask == sampleMask)
        {
            return;
        }
        deviceContext?.OutputMerger.SetBlendState(blendState, blendFactor?.ToStruct<Color4, RawColor4>(), sampleMask);
        currBlendState = blendState;
        currBlendFactor = blendFactor;
        currSampleMask = sampleMask == -1 ? uint.MaxValue : (uint)sampleMask;
    }

    /// <summary>
    /// Sets the state of the blend.
    /// </summary>
    /// <param name="blendState">State of the blend.</param>
    /// <param name="blendFactor">The blend factor.</param>
    /// <param name="sampleMask">The sample mask.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBlendState(BlendStateProxy blendState, Color4? blendFactor = null, uint sampleMask = uint.MaxValue)
    {
        if (AutoSkipRedundantStateSetting && currBlendState == blendState && blendFactor == currBlendFactor && currSampleMask == sampleMask)
        {
            return;
        }
        deviceContext?.OutputMerger.SetBlendState(blendState, blendFactor?.ToStruct<Color4, RawColor4>(), sampleMask);
        currBlendState = blendState;
        currBlendFactor = blendFactor;
        currSampleMask = sampleMask;
    }
}
