using SharpDX;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public abstract class GeometryRenderCore : RenderCoreBase
    {
        private RasterizerState rasterState = null;
        public RasterizerState RasterState { get { return rasterState; } }
        public InputLayout VertexLayout { private set; get; }
        public EffectTechnique EffectTechnique { private set; get; }

        public InstanceBufferModel InstanceBuffer { set; get; }

        public Geometry3D Geometry { set; get; }
        public BufferModel GeometryBuffer { set; get; }

        public virtual void CreateRasterState(Device device, RasterizerStateDescription description)
        {
            RemoveAndDispose(ref rasterState);
            rasterState = Collect(new RasterizerState(device, description));
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
