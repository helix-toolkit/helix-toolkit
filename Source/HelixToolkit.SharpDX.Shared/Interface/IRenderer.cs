using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core;
    using Model.Scene;
    using Model.Scene2D;
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
        /// <summary>
        /// 
        /// </summary>
        public bool RenderLight;
        /// <summary>
        /// 
        /// </summary>
        public bool UpdatePerFrameData;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct RenderParameter2D
    {
        public global::SharpDX.Direct2D1.Bitmap1 RenderTarget;
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IRenderer
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
        /// <param name="results"></param>
        /// <returns></returns>
        void UpdateSceneGraph(IRenderContext context, List<SceneNode> renderables, List<SceneNode> results);

        /// <summary>
        /// Update scene graph, return the 2D renderables which will be rendered in this frame
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <returns></returns>
        void UpdateSceneGraph2D(IRenderContext2D context, List<SceneNode2D> renderables);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <param name="parameter"></param>
        void UpdateGlobalVariables(IRenderContext context, List<RenderCore> renderables, ref RenderParameter parameter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        void SetRenderTargets(ref RenderParameter parameter);

        /// <summary>
        /// Render pre processings, such as Shadow pass etc.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <param name="parameter"></param>
        void RenderPreProc(IRenderContext context, List<RenderCore> renderables, ref RenderParameter parameter);

        /// <summary>
        /// Render post processing, such as bloom effects etc.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <param name="parameter"></param>
        void RenderPostProc(IRenderContext context, List<RenderCore> renderables, ref RenderParameter parameter);

        /// <summary>
        /// Run actual rendering for render cores.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <param name="parameter"></param>
        void RenderScene(IRenderContext context, List<RenderCore> renderables, ref RenderParameter parameter);
        /// <summary>
        /// Update scene graph not related to rendering. Can be run parallel with the <see cref="RenderScene(IRenderContext, List{RenderCore}, ref RenderParameter)"/>
        /// <para>Warning: Dependency properties are thread affinity. Do not get/set any dependency property in this function.</para>
        /// </summary>
        /// <param name="renderables"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        void UpdateNotRenderParallel(IRenderContext context, List<SceneNode> renderables);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <param name="parameter"></param>
        void RenderScene2D(IRenderContext2D context, List<SceneNode2D> renderables, ref RenderParameter2D parameter);
    }
}
