using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System.Numerics;
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
    public partial class DeviceContextProxy
    {
        #region Set targets

        /// <summary>
        ///
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTargets(DX11RenderBufferProxyBase buffer)
        {
            buffer.SetDefaultRenderTargets(this);
        }

        /// <summary>
        /// Sets the render target.
        /// </summary>
        /// <param name="dsv">The DSV.</param>
        /// <param name="renderTarget">The render target.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTarget(DepthStencilView dsv, RenderTargetView renderTarget)
        {
            deviceContext.OutputMerger.SetRenderTargets(dsv, renderTarget);
        }

        /// <summary>
        /// Sets the render targets.
        /// </summary>
        /// <param name="dsv">The DSV.</param>
        /// <param name="renderTarget">The render target.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTargets(DepthStencilView dsv, RenderTargetView[] renderTarget)
        {
            deviceContext.OutputMerger.SetRenderTargets(dsv, renderTarget);
        }
        /// <summary>
        /// Sets the stream output target.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetStreamOutputTarget(Buffer buffer, int offset = 0)
        {
            deviceContext.StreamOutput.SetTarget(buffer, offset);
        }
        /// <summary>
        /// Sets the stream output target.
        /// </summary>
        /// <param name="bufferBindings">The buffer bindings.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetStreamOutputTarget(StreamOutputBufferBinding[] bufferBindings)
        {
            deviceContext.StreamOutput.SetTargets(bufferBindings);
        }
        private static readonly RenderTargetView[] ZeroRenderTargetArray = new RenderTargetView[0];

        /// <summary>
        /// Sets the depth stencil only. This will clear all render target bindings and only binds depth stencil view to output merger.
        /// </summary>
        /// <param name="dsv">The DSV.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDepthStencilOnly(DepthStencilView dsv)
        {
            deviceContext.OutputMerger.SetRenderTargets(dsv, ZeroRenderTargetArray);
        }
        #endregion Set targets

        #region Get targets

        /// <summary>
        /// Gets the depth stencil view.
        /// </summary>
        /// <param name="depthStencilViewRef">The depth stencil view reference.</param>
        /// <remarks>
        ///     Any returned interfaces will have their reference count incremented by one. Applications
        ///     should call {{IUnknown::Release}} on the returned interfaces when they are no
        ///     longer needed to avoid memory leaks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetDepthStencilView(out DepthStencilView depthStencilViewRef)
        {
            deviceContext.OutputMerger.GetRenderTargets(out depthStencilViewRef);
        }

        /// <summary>
        /// Gets the render targets.
        /// </summary>
        /// <param name="numViews">The number views.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Any returned interfaces will have their reference count incremented by one. Applications
        ///     should call {{IUnknown::Release}} on the returned interfaces when they are no
        ///     longer needed to avoid memory leaks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RenderTargetView[] GetRenderTargets(int numViews)
        {
            return deviceContext.OutputMerger.GetRenderTargets(numViews);
        }

        /// <summary>
        /// Gets the render targets.
        /// </summary>
        /// <param name="numViews">The number views.</param>
        /// <param name="depthStencilViewRef">The depth stencil view reference.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Any returned interfaces will have their reference count incremented by one. Applications
        ///     should call {{IUnknown::Release}} on the returned interfaces when they are no
        ///     longer needed to avoid memory leaks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RenderTargetView[] GetRenderTargets(int numViews, out DepthStencilView depthStencilViewRef)
        {
            return deviceContext.OutputMerger.GetRenderTargets(numViews, out depthStencilViewRef);
        }

        /// <summary>
        /// Gets the unordered access views.
        /// </summary>
        /// <param name="startSlot">Index of the first element in the zero-based array to return (ranges from 0 to D3D11_PS_CS_UAV_REGISTER_COUNT - 1).</param>
        /// <param name="count"> Number of views to get (ranges from 0 to D3D11_PS_CS_UAV_REGISTER_COUNT - StartSlot).</param>
        /// <returns></returns>
        /// <remarks>
        ///     Any returned interfaces will have their reference count incremented by one. Applications
        ///     should call IUnknown::Release on the returned interfaces when they are no longer
        ///     needed to avoid memory leaks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnorderedAccessView[] GetUnorderedAccessViews(int startSlot, int count)
        {
            return deviceContext.OutputMerger.GetUnorderedAccessViews(startSlot, count);
        }

        #endregion Get targets

        #region Clear Targets

        /// <summary>
        /// Clears the render targets.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="color">The color.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTargets(DX11RenderBufferProxyBase buffer, Color4 color)
        {
            buffer.ClearRenderTarget(this, color);
        }

        /// <summary>
        /// Clears the depth stencil view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="clearFlag">The clear flag.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="stencil">The stencil.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearDepthStencilView(DepthStencilView view, DepthStencilClearFlags clearFlag, float depth, byte stencil)
        {
            deviceContext.ClearDepthStencilView(view, clearFlag, depth, stencil);
        }

        /// <summary>
        /// Clears the render target view.
        /// </summary>
        /// <param name="renderTargetViewRef">The render target view reference.</param>
        /// <param name="colorRGBA">A 4-component array that represents the color to fill the render target with.</param>
        /// <remarks>
        ///     Applications that wish to clear a render target to a specific integer value bit
        ///     pattern should render a screen-aligned quad instead of using this method. The
        ///     reason for this is because this method accepts as input a floating point value,
        ///     which may not have the same bit pattern as the original integer. Differences
        ///     between Direct3D 9 and Direct3D 11/10: Unlike Direct3D 9, the full extent of
        ///     the resource view is always cleared. Viewport and scissor settings are not applied.
        ///     ?When using D3D_FEATURE_LEVEL_9_x, ClearRenderTargetView only clears the first
        ///     array slice in the render target view. This can impact (for example) cube map
        ///     rendering scenarios. Applications should create a render target view for each
        ///     face or array slice, then clear each view individually.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTargetView(RenderTargetView renderTargetViewRef, Color4 colorRGBA)
        {
            deviceContext.ClearRenderTargetView(renderTargetViewRef, colorRGBA);
        }

        /// <summary>
        ///
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTagetBindings()
        {
            deviceContext.OutputMerger.ResetTargets();
        }

        /// <summary>
        /// Clears an unordered access resource with bit-precise values.
        /// </summary>
        /// <param name="unorderedAccessViewRef">The unordered access view reference.</param>
        /// <param name="values">The values.</param>
        /// <remarks>
        ///     This API copies the lower ni bits from each array element i to the corresponding
        ///     channel, where ni is the number of bits in the ith channel of the resource format
        ///     (for example, R8G8B8_FLOAT has 8 bits for the first 3 channels). This works on
        ///     any UAV with no format conversion. For a raw or structured buffer view, only
        ///     the first array element value is used.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearUnorderedAccessView(UnorderedAccessView unorderedAccessViewRef, Int4 values)
        {
            deviceContext.ClearUnorderedAccessView(unorderedAccessViewRef, values);
        }

        /// <summary>
        /// Clears an unordered access resource with a float value.
        /// </summary>
        /// <param name="unorderedAccessViewRef">The unordered access view reference.</param>
        /// <param name="values">The values.</param>
        /// <remarks>
        ///     This API works on FLOAT, UNORM, and SNORM unordered access views (UAVs), with
        ///     format conversion from FLOAT to *NORM where appropriate. On other UAVs, the operation
        ///     is invalid and the call will not reach the driver.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearUnorderedAccessView(UnorderedAccessView unorderedAccessViewRef, Vector4 values)
        {
            deviceContext.ClearUnorderedAccessView(unorderedAccessViewRef, values.ToRaw());
        }
        #endregion Clear Targets
    }
}
