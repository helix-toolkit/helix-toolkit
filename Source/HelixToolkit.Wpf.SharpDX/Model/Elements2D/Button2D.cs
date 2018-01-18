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
    public class Button2D : ContentElement2D
    {
        public Button2D()
        {
            Content2D = new DefaultButtonRenderer();
        }

        protected override IRenderable2D CreateRenderCore(IDevice2DProxy host)
        {
            return null;
        }

        protected override void OnRenderTargetChanged(RenderTarget newTarget)
        {

        }

        public class DefaultButtonRenderer : Canvas2D
        {
            public DefaultButtonRenderer()
            {
                Children.Add(new RectangleModel2D() { Fill = new Media.SolidColorBrush(Media.Colors.Gray), Width = 100, Height = 30 });
            }
        }
    }
}
