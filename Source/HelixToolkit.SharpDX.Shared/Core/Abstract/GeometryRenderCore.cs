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

        public IElementsBufferModel InstanceBuffer { set; get; }

        public IGeometryBufferModel GeometryBuffer{ set; get; }

        private RasterizerStateDescription rasterDescription = new RasterizerStateDescription()
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.None,
        };
        public RasterizerStateDescription RasterDescription
        {
            set
            {
                rasterDescription = value;
                CreateRasterState(value, false);
            }
            get
            {
                return RasterDescription;
            }
        }

        private void CreateRasterState(RasterizerStateDescription description, bool force)
        {
            rasterDescription = description;
            if (!IsAttached && !force)
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
                CreateRasterState(rasterDescription, true);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Set all necessary states and buffers
        /// </summary>
        /// <param name="context"></param>
        protected override void SetStatesAndVariables(IRenderMatrices context)
        {
            base.SetStatesAndVariables(context);
            SetRasterState(context.DeviceContext);
        }
        /// <summary>
        /// Attach vertex buffer routine
        /// </summary>
        /// <param name="context"></param>
        protected override void OnAttachBuffers(DeviceContext context)
        {
            GeometryBuffer.AttachBuffers(context, this.VertexLayout, 0);
            InstanceBuffer?.AttachBuffer(context, 1);
        }

        protected override bool CanRender()
        {
            return base.CanRender() && GeometryBuffer != null;
        }

        protected override void PostRender(IRenderMatrices context)
        {
            base.PostRender(context);
            InstanceBuffer?.ResetHasElementsVariable();
        }
        /// <summary>
        /// Draw call
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instanceModel"></param>
        protected virtual void OnDraw(DeviceContext context, IElementsBufferModel instanceModel)
        {
            if (GeometryBuffer.IndexBuffer != null)
            {
                if (instanceModel == null || !instanceModel.HasElements)
                {
                    context.DrawIndexed(GeometryBuffer.IndexBuffer.Count, GeometryBuffer.IndexBuffer.Offset, 0);
                }
                else
                {
                    context.DrawIndexedInstanced(GeometryBuffer.IndexBuffer.Count, instanceModel.Buffer.Count, GeometryBuffer.IndexBuffer.Offset, 0, instanceModel.Buffer.Offset);
                }
            }
            else if (GeometryBuffer.VertexBuffer != null)
            {
                if (instanceModel == null || !instanceModel.HasElements)
                {
                    context.Draw(GeometryBuffer.VertexBuffer.Count, GeometryBuffer.VertexBuffer.Offset);
                }
                else
                {
                    context.DrawInstanced(GeometryBuffer.VertexBuffer.Count, instanceModel.Buffer.Count,
                        GeometryBuffer.VertexBuffer.Offset, instanceModel.Buffer.Offset);
                }
            }
        }
    }
}
