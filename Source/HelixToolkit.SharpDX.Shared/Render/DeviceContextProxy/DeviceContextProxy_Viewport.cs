using SharpDX;
using System.Runtime.CompilerServices;

#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Render
    {
        public partial class DeviceContextProxy
        {
            #region Viewport and Scissors

            /// <summary>
            /// Sets the scissor rectangle.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="top">The top.</param>
            /// <param name="right">The right.</param>
            /// <param name="bottom">The bottom.</param>
            /// <remarks>
            ///     All scissor rects must be set atomically as one operation. Any scissor rects
            ///     not defined by the call are disabled.The scissor rectangles will only be used
            ///     if ScissorEnable is set to true in the rasterizer state (see SharpDX.Direct3D11.RasterizerStateDescription).Which
            ///     scissor rectangle to use is determined by the SV_ViewportArrayIndex semantic
            ///     output by a geometry shader (see shader semantic syntax). If a geometry shader
            ///     does not make use of the SV_ViewportArrayIndex semantic then Direct3D will use
            ///     the first scissor rectangle in the array.Each scissor rectangle in the array
            ///     corresponds to a viewport in an array of viewports (see SharpDX.Direct3D11.RasterizerStage.SetViewports(SharpDX.Mathematics.Interop.RawViewportF[],System.Int32)).
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetScissorRectangle(int left, int top, int right, int bottom)
            {
                deviceContext.Rasterizer.SetScissorRectangle(left, top, right, bottom);
            }

            /// <summary>
            /// Binds a set of scissor rectangles to the rasterizer stage.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="scissorRectangles">The scissor rectangles.</param>
            /// <remarks>
            ///     All scissor rects must be set atomically as one operation. Any scissor rects
            ///     not defined by the call are disabled.The scissor rectangles will only be used
            ///     if ScissorEnable is set to true in the rasterizer state (see SharpDX.Direct3D11.RasterizerStateDescription).Which
            ///     scissor rectangle to use is determined by the SV_ViewportArrayIndex semantic
            ///     output by a geometry shader (see shader semantic syntax). If a geometry shader
            ///     does not make use of the SV_ViewportArrayIndex semantic then Direct3D will use
            ///     the first scissor rectangle in the array.Each scissor rectangle in the array
            ///     corresponds to a viewport in an array of viewports (see SharpDX.Direct3D11.RasterizerStage.SetViewports(SharpDX.Mathematics.Interop.RawViewportF[],System.Int32)).
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetScissorRectangles<T>(params T[] scissorRectangles) where T : struct
            {
                deviceContext.Rasterizer.SetScissorRectangles(scissorRectangles);
            }

            /// <summary>
            /// Binds a single viewport to the rasterizer stage.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="minZ">The minimum z.</param>
            /// <param name="maxZ">The maximum z.</param>
            /// <remarks>
            ///     All viewports must be set atomically as one operation. Any viewports not defined
            ///     by the call are disabled.Which viewport to use is determined by the SV_ViewportArrayIndex
            ///     semantic output by a geometry shader; if a geometry shader does not specify the
            ///     semantic, Direct3D will use the first viewport in the array.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetViewport(float x, float y, float width, float height, float minZ = 0, float maxZ = 1)
            {
                deviceContext.Rasterizer.SetViewport(x, y, width, height, minZ, maxZ);
            }

            /// <summary>
            /// Binds a single viewport to the rasterizer stage.
            /// </summary>
            /// <param name="viewport">The viewport.</param>
            /// <remarks>
            ///     All viewports must be set atomically as one operation. Any viewports not defined
            ///     by the call are disabled.Which viewport to use is determined by the SV_ViewportArrayIndex
            ///     semantic output by a geometry shader; if a geometry shader does not specify the
            ///     semantic, Direct3D will use the first viewport in the array.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetViewport(ref Viewport viewport)
            {
                deviceContext.Rasterizer.SetViewport(viewport);
            }

            /// <summary>
            /// Binds a single viewport to the rasterizer stage.
            /// </summary>
            /// <param name="viewport">The viewport.</param>
            /// <remarks>
            ///     All viewports must be set atomically as one operation. Any viewports not defined
            ///     by the call are disabled.Which viewport to use is determined by the SV_ViewportArrayIndex
            ///     semantic output by a geometry shader; if a geometry shader does not specify the
            ///     semantic, Direct3D will use the first viewport in the array.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetViewport(ref ViewportF viewport)
            {
                deviceContext.Rasterizer.SetViewport(viewport);
            }

            #endregion Viewport and Scissors
        }
    }

}
