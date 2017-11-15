using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;
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

        public virtual void SetRasterState(Device device, RasterizerStateDescription description)
        {
            RemoveAndDispose(ref rasterState);
            rasterState = Collect(new RasterizerState(device, description));
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
    }
}
