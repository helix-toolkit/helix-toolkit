using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Extensions;
    using Model.Scene2D;

    public class FrameStatisticsModel2D : Element2D
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

        protected override void OnAttached()
        {
            base.OnAttached();
            foregroundChanged = backgroundChanged = true;
        }

        protected override SceneNode2D OnCreateSceneNode()
        {
            return new FrameStatisticsNode2D();
        }

        protected override void OnUpdate(IRenderContext2D context)
        {
            base.OnUpdate(context);
            if (foregroundChanged)
            {
                (SceneNode as FrameStatisticsNode2D).Foreground = Foreground != null ? Foreground.ToD2DBrush(context.DeviceContext) : null;
                foregroundChanged = false;
            }
            if (backgroundChanged)
            {
                (SceneNode as FrameStatisticsNode2D).Background = Background != null ? Background.ToD2DBrush(context.DeviceContext) : null;
                backgroundChanged = false;
            }
        }
    }
}
