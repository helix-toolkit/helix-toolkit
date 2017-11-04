using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX.Direct2D1;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public class Button2D : Clickable2D
    {
        protected override IRenderable2D CreateRenderCore(IRenderHost host)
        {
            throw new NotImplementedException();
        }

        protected override void OnRenderTargetChanged(RenderTarget newTarget)
        {
            throw new NotImplementedException();
        }

        public class DefaultButtonRenderer : GroupElement2D
        {
            public DefaultButtonRenderer()
            {
                Children.Add(new RectangleModel2D() { Fill = new Media.SolidColorBrush(Media.Colors.Gray) });
            }
        }
    }
}
