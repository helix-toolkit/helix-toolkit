using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{

    /// <summary>
    /// 
    /// </summary>
    public struct RenderParameter
    {
        /// <summary>
        /// The render target view
        /// </summary>
        public RenderTargetView RenderTargetView;
        /// <summary>
        /// The depth stencil view
        /// </summary>
        public DepthStencilView DepthStencilView;
        /// <summary>
        /// The viewport region
        /// </summary>
        public ViewportF ViewportRegion;
        /// <summary>
        /// The scissor region
        /// </summary>
        public Rectangle ScissorRegion;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct RenderParameter2D
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IRenderer : IDisposable
    {
        /// <summary>
        /// Default ImmediateContext. Same as Device.ImmediateContext.
        /// <para>Used for update global variables</para>
        /// </summary>
        DeviceContextProxy ImmediateContext { get; }
        /// <summary>
        /// Update scene graph, return the renderables which will be rendered in this frame
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        IEnumerable<IRenderable> UpdateSceneGraph(IRenderContext context, IEnumerable<IRenderable> renderables);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <param name="parameter"></param>
        void UpdateGlobalVariables(IRenderContext context, IEnumerable<IRenderable> renderables, ref RenderParameter parameter);
        /// <summary>
        /// Run actual rendering for render cores.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <param name="parameter"></param>
        /// <param name="deviceContext"></param>
        void RenderScene(IRenderContext context, DeviceContextProxy deviceContext, IEnumerable<IRenderCore> renderables, ref RenderParameter parameter);
        /// <summary>
        /// Update scene graph not related to rendering. Can be run parallel with the <see cref="RenderScene(IRenderContext, DeviceContextProxy, IEnumerable{IRenderCore}, ref RenderParameter)"/>
        /// <para>Warning: Dependency properties are thread affinity. Do not get/set any dependency property in this function.</para>
        /// </summary>
        /// <param name="renderables"></param>
        /// <returns></returns>
        void UpdateNotRenderParallel(IEnumerable<IRenderable> renderables);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <param name="parameter"></param>
        void Render2D(IRenderContext2D context, IEnumerable<IRenderable2D> renderables, ref RenderParameter2D parameter);
    }
}
