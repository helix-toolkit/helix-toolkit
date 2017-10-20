using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.SharpDX.Shared.D2DControls;

namespace HelixToolkit.Wpf.SharpDX
{
    public class D2DTextModel : Element2D
    {
        protected override ID2DRenderable CreateRenderCore(IRenderHost host)
        {
            return new TextRenderable();
        }

        protected override void OnRender(RenderContext context)
        {
            renderCore.Render(context, RenderHost.D2DControls.D2DTarget);
        }
    }
}
