using System;
using System.Collections.Generic;
using System.Text;
using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX;
using HelixToolkit.Wpf.SharpDX;

namespace HelixToolkit.SharpDX.Core2D
{
    public class TextRenderable : Renderable2DBase
    {
        public string Text { set; get; } = "Text";

        public global::SharpDX.RectangleF Rect { set; get; } = new RectangleF(0, 0, 100, 100);

        private D2D.Brush brush = null;
        public D2D.Brush Brush
        {
            set
            {
                brush = value;
            }
            get
            {
                return brush;
            }
        }

        public string Font { set; get; } = "Arial";

        public Color Foreground { set; get; } = Color.Black;

        public int FontSize { set; get; } = 14;

        private Factory TextFactory = new Factory(FactoryType.Isolated);

        protected override void OnTargetChanged(D2D.RenderTarget target)
        {
            Disposer.RemoveAndDispose(ref brush);
            if(target ==null || target.IsDisposed)
            {
                return;
            }
            Brush = new D2D.SolidColorBrush(target, Foreground);
        }


        protected override void OnRender(IRenderMatrices matrices)
        {
            RenderTarget.DrawText(Text, new TextFormat(TextFactory, Font, FontWeight.Bold, FontStyle.Normal, 14), Rect, Brush, D2D.DrawTextOptions.None);
        }

        public override void Dispose()
        {
            base.Dispose();
            Disposer.RemoveAndDispose(ref brush);           
        }
    }
}
