using HelixToolkit.Mathematics;
using System.Runtime.CompilerServices;

#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Utilities;

    public partial class DeviceContextProxy
    {
        /// <summary>
        /// Sets the state of the raster.
        /// </summary>
        /// <param name="rasterState">State of the raster.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRasterState(RasterizerStateProxy rasterState)
        {
            if (AutoSkipRedundantStateSetting && currRasterState == rasterState)
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
            deviceContext.OutputMerger.SetDepthStencilState(depthStencilState, stencilRef);
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
            deviceContext.OutputMerger.SetBlendState(blendState, blendFactor, sampleMask);
            currBlendState = blendState;
            currBlendFactor = blendFactor;
            currSampleMask = sampleMask == -1 ? int.MaxValue : (uint)sampleMask;
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
            deviceContext.OutputMerger.SetBlendState(blendState, blendFactor, sampleMask);
            currBlendState = blendState;
            currBlendFactor = blendFactor;
            currSampleMask = sampleMask;
        }

        /// <summary>
        /// Restore all default settings.
        /// </summary>
        /// <remarks>
        ///     This method resets any device context to the default settings. This sets all
        ///     input/output resource slots, shaders, input layouts, predications, scissor rectangles,
        ///     depth-stencil state, rasterizer state, blend state, sampler state, and viewports
        ///     to null. The primitive topology is set to UNDEFINED.For a scenario where you
        ///     would like to clear a list of commands recorded so far, call SharpDX.Direct3D11.DeviceContext.FinishCommandListInternal(SharpDX.Mathematics.Interop.RawBool,SharpDX.Direct3D11.CommandList@)
        ///     and throw away the resulting SharpDX.Direct3D11.CommandList.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearState()
        {
            deviceContext.ClearState();
            currRasterState = null;
            currBlendState = null;
            currDepthStencilState = null;
            currBlendFactor = null;
            currSampleMask = uint.MaxValue;
            currStencilRef = 0;
        }
    }
}
