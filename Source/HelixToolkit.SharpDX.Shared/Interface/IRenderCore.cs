using SharpDX;
using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public interface IRenderCore : IGUID
    {
        event EventHandler<bool> OnInvalidateRenderer;

        Matrix ModelMatrix { set; get; }
        Effect Effect { get; }
        Device Device { get; }
        /// <summary>
        /// If render core is attached or not
        /// </summary>
        bool IsAttached { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="technique"></param>
        void Attach(IRenderTechnique technique);
        /// <summary>
        /// 
        /// </summary>
        void Detach();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Render(IRenderMatrices context);
        /// <summary>
        /// Unsubscribe all OnInvalidateRenderer event handler;
        /// </summary>
        void ResetInvalidateHandler();
    }

    public interface IGeometryRenderCore
    {
        InputLayout VertexLayout { get; }
        IElementsBufferModel InstanceBuffer { set; get; }
        IGeometryBufferModel GeometryBuffer { set; get; }
        RasterizerStateDescription RasterDescription { set; get; }
        bool SetRasterState(DeviceContext context);
    }

    public interface IMaterialRenderCore
    {
        IMaterial Material { set; get; }
        bool RenderDiffuseMap { set; get; }
        bool RenderDiffuseAlphaMap { set; get; }
        bool RenderNormalMap { set; get; }
        bool RenderDisplacementMap { set; get; }
        bool HasShadowMap { set; get; }
    }
}
