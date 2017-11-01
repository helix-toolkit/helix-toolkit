using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using HelixToolkit.Wpf.SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public class TextRenderable : Renderable2DBase
    {
        public string Text { set; get; } = "Text";

        public D2D.Brush Foreground = null;

        public string Font { set; get; } = "Arial";

        public int FontSize { set; get; } = 12;

        public FontWeight FontWeight { set; get; } = FontWeight.Normal;

        public FontStyle FontStyle { set; get; } = FontStyle.Normal;

        public D2D.DrawTextOptions DrawingOptions { set; get; } = D2D.DrawTextOptions.None;

        private Factory TextFactory = new Factory(FactoryType.Isolated);

        protected override bool CanRender(D2D.RenderTarget target)
        {
            return base.CanRender(target) && Foreground != null;
        }

        protected override void OnRender(IRenderMatrices matrices)
        {
            RenderTarget.DrawText(Text, new TextFormat(TextFactory, Font, FontWeight, FontStyle, FontSize), 
               LocalDrawingRect, Foreground, DrawingOptions);
        }

        public override void Dispose()
        {
            Disposer.RemoveAndDispose(ref Foreground);
            base.Dispose();
        }
    }
}
