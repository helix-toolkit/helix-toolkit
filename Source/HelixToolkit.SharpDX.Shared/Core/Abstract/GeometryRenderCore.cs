using SharpDX;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public abstract class GeometryRenderCore : RenderCoreBase, IGeometryRenderCore
    {
        private RasterizerState rasterState = null;
        public RasterizerState RasterState { get { return rasterState; } }
        public InputLayout VertexLayout { private set; get; }
        public EffectTechnique EffectTechnique { private set; get; }

        public IInstanceBufferModel InstanceBuffer { set; get; }

        public IGeometryBufferModel GeometryBuffer{ set; get; }

        private RasterizerStateDescription rasterDescription;

        public void CreateRasterState(RasterizerStateDescription description)
        {
            rasterDescription = description;
            if (!IsAttached)
            { return; }
            RemoveAndDispose(ref rasterState);
            rasterState = Collect(new RasterizerState(Device, description));
        }

        public bool SetRasterState(DeviceContext context)
        {
            if (rasterState != null)
            {
                context.Rasterizer.State = rasterState;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool OnAttach(IRenderHost host, RenderTechnique technique)
        {
            if(base.OnAttach(host, technique))
            {
                this.VertexLayout = host.EffectsManager.GetLayout(technique);
                this.EffectTechnique = Effect.GetTechniqueByName(technique.Name);
                CreateRasterState(rasterDescription);
                return true;
            }
            return false;
        }

        protected override void PreRender(IRenderMatrices context)
        {
            base.PreRender(context);
            SetRasterState(context.DeviceContext);
            GeometryBuffer.AttachBuffers(context.DeviceContext, this.VertexLayout, 0);
            InstanceBuffer?.AttachBuffer(context.DeviceContext, 1);
        }

        protected override bool CanRender()
        {
            return base.CanRender() && GeometryBuffer != null;
        }

        protected override void PostRender(IRenderMatrices context)
        {
            base.PostRender(context);
            InstanceBuffer?.ResetHasInstanceVariable();
        }
        /// <summary>
        /// Draw call
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instanceModel"></param>
        protected virtual void OnDraw(DeviceContext context, IInstanceBufferModel instanceModel)
        {
            if (GeometryBuffer.IndexBuffer != null)
            {
                if (instanceModel == null || !instanceModel.HasInstance)
                {
                    context.DrawIndexed(GeometryBuffer.IndexBuffer.Count, GeometryBuffer.IndexBuffer.Offset, 0);
                }
                else
                {
                    context.DrawIndexedInstanced(GeometryBuffer.IndexBuffer.Count, instanceModel.InstanceBuffer.Count, GeometryBuffer.IndexBuffer.Offset, 0, instanceModel.InstanceBuffer.Offset);
                }
            }
            else if (GeometryBuffer.VertexBuffer != null)
            {
                if (instanceModel == null || !instanceModel.HasInstance)
                {
                    context.Draw(GeometryBuffer.VertexBuffer.Count, GeometryBuffer.VertexBuffer.Offset);
                }
                else
                {
                    context.DrawInstanced(GeometryBuffer.VertexBuffer.Count, instanceModel.InstanceBuffer.Count,
                        GeometryBuffer.VertexBuffer.Offset, instanceModel.InstanceBuffer.Offset);
                }
            }
        }
    }
}
