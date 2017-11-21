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

        public InstanceBufferModel InstanceBuffer { set; get; }

        public GeometryBufferModel GeometryBuffer{ set; get; }

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

        protected override bool CanRender()
        {
            return base.CanRender() && GeometryBuffer != null;
        }
    }
}
