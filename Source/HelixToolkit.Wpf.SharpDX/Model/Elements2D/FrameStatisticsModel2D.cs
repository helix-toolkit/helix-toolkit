using System;
using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using Extensions;

    public class FrameStatisticsModel2D : Element2DCore
    {
        public static readonly DependencyProperty ForegroundProperty
            = DependencyProperty.Register("Foreground", typeof(Brush), typeof(FrameStatisticsModel2D),
        new PropertyMetadata(new SolidColorBrush(Colors.Black), (d, e) =>
        {
            var model = (d as FrameStatisticsModel2D);
            model.foregroundChanged = true;
        }));

        public Brush Foreground
        {
            set
            {
                SetValue(ForegroundProperty, value);
            }
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }
        }

        public static readonly DependencyProperty BackgroundProperty
            = DependencyProperty.Register("Background", typeof(Brush), typeof(FrameStatisticsModel2D),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(64, 32, 32, 32)), (d, e) =>
                {
                    var model = (d as FrameStatisticsModel2D);
                    model.backgroundChanged = true;
                }));

        public Brush Background
        {
            set
            {
                SetValue(BackgroundProperty, value);
            }
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
        }

        private bool foregroundChanged = true;
        private bool backgroundChanged = true;

        public FrameStatisticsModel2D()
        {
            HorizontalAlignmentInternal = HorizontalAlignment.Right;
            VerticalAlignmentInternal = VerticalAlignment.Top;
            EnableBitmapCacheInternal = false;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            foregroundChanged = backgroundChanged = true;
            return base.OnAttach(host);
        }

        protected override IRenderCore2D CreateRenderCore()
        {
            return new FrameStatisticsRenderCore();
        }

        public override void Update(IRenderContext2D context)
        {
            base.Update(context);
            if (foregroundChanged)
            {
                (RenderCore as FrameStatisticsRenderCore).Foreground = Foreground != null ? Foreground.ToD2DBrush(context.DeviceContext) : null;
                foregroundChanged = false;
            }
            if (backgroundChanged)
            {
                (RenderCore as FrameStatisticsRenderCore).Background = Background != null ? Background.ToD2DBrush(context.DeviceContext) : null;
                backgroundChanged = false;
            }
        }

        protected override bool CanHitTest()
        {
            return false;
        }

        protected override bool OnHitTest(ref global::SharpDX.Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            return false;
        }
    }
}
