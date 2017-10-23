using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.SharpDX.Core2D;

namespace HelixToolkit.Wpf.SharpDX
{
    public class TextModel2D : Element2D
    {
        protected override IRenderable2D CreateRenderCore(IRenderHost host)
        {
            return new TextRenderable();
        }

        protected override void OnRender(RenderContext context)
        {
            renderCore.Render(context, RenderHost.D2DControls.D2DTarget);
        }
    }
}
