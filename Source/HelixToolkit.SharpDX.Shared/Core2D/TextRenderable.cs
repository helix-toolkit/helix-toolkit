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
            var rect = new RectangleF(-Rect.Width / 2, -Rect.Height / 2, Rect.Width, Rect.Height);
            RenderTarget.DrawText(Text, new TextFormat(TextFactory, Font, FontWeight.Bold, FontStyle.Normal, 14), 
               rect, Brush, D2D.DrawTextOptions.None);
            RenderTarget.DrawRectangle(rect, Brush);
        }

        public override void Dispose()
        {
            base.Dispose();
            Disposer.RemoveAndDispose(ref brush);           
        }
    }
}
