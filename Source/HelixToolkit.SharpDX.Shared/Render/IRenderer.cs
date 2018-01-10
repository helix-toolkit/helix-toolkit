using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{

    public class RenderParameter
    {
        public RenderTargetView target;
        public DepthStencilView depthStencil;
        public ViewportF ViewportRegion;
        public Rectangle ScissorRegion;
    }

    public class RenderParameter2D
    {

    }

    public interface IRenderer
    {
        void Render(IRenderContext context, IEnumerable<IRenderable> renderables, RenderParameter parameter);
        void Render2D(IRenderContext2D context, IEnumerable<IRenderable2D> renderables, RenderParameter2D parameter);
    }
}
