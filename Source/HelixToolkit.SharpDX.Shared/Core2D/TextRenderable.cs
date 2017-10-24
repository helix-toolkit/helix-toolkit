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

        public int FontSize { set; get; } = 12;

        public FontWeight FontWeight { set; get; } = FontWeight.Normal;

        public FontStyle FontStyle { set; get; } = FontStyle.Normal;

        public D2D.DrawTextOptions DrawingOptions { set; get; } = D2D.DrawTextOptions.None;

        private Factory TextFactory = new Factory(FactoryType.Isolated);

        protected override void OnTargetChanged(D2D.RenderTarget target)
        {
            base.OnTargetChanged(target);
            Disposer.RemoveAndDispose(ref brush);
            if(target ==null || target.IsDisposed)
            {
                return;
            }
            Brush = new D2D.SolidColorBrush(target, Foreground);
        }


        protected override void OnRender(IRenderMatrices matrices)
        {
            RenderTarget.DrawText(Text, new TextFormat(TextFactory, Font, FontWeight, FontStyle, FontSize), 
               LocalDrawingRect, Brush, DrawingOptions);
        }

        public override void Dispose()
        {
            base.Dispose();
            Disposer.RemoveAndDispose(ref brush);           
        }
    }
}
