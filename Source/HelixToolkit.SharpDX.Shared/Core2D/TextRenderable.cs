using System;
using System.Collections.Generic;
using System.Text;
using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX;
using HelixToolkit.Wpf.SharpDX;
using Media = System.Windows.Media;
using HelixToolkit.Wpf.SharpDX.Extensions;

namespace HelixToolkit.SharpDX.Core2D
{
    public class TextRenderable : Renderable2DBase
    {
        public string Text { set; get; } = "Text";

        private Media.Brush foreground;
        public Media.Brush Foreground
        {
            set
            {
                if(foreground != value)
                {
                    foreground = value;
                    foregroundChanged = true;
                    Disposer.RemoveAndDispose(ref foregroundBrush);
                }                
            }
            get
            {
                return foreground;
            }
        }

        private bool foregroundChanged = true;
        private D2D.Brush foregroundBrush = null;

        public string Font { set; get; } = "Arial";

        public int FontSize { set; get; } = 12;

        public FontWeight FontWeight { set; get; } = FontWeight.Normal;

        public FontStyle FontStyle { set; get; } = FontStyle.Normal;

        public D2D.DrawTextOptions DrawingOptions { set; get; } = D2D.DrawTextOptions.None;

        private Factory TextFactory = new Factory(FactoryType.Isolated);

        protected override void OnTargetChanged(D2D.RenderTarget target)
        {
            Disposer.RemoveAndDispose(ref foregroundBrush);
            foregroundChanged = true;
            base.OnTargetChanged(target);
        }

        protected override bool CanRender(D2D.RenderTarget target)
        {
            return base.CanRender(target) && Foreground != null;
        }

        protected override void OnRender(IRenderMatrices matrices)
        {
            if (foregroundChanged)
            {
                foregroundBrush = Foreground.ToD2DBrush(RenderTarget);
                foregroundChanged = false;
            }
            RenderTarget.DrawText(Text, new TextFormat(TextFactory, Font, FontWeight, FontStyle, FontSize), 
               LocalDrawingRect, foregroundBrush, DrawingOptions);
        }

        public override void Dispose()
        {
            Disposer.RemoveAndDispose(ref foregroundBrush);
            foregroundChanged = true;
            base.Dispose();
        }
    }
}
