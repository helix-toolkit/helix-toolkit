using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene2D;

#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Extensions;
using Microsoft.UI.Xaml.Media;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows.Media;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#else
#error Unknown framework
#endif

public class FrameStatisticsModel2D : Element2D
{
#if false
#elif WINUI
    new
#elif WPF
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty ForegroundProperty
        = DependencyProperty.Register("Foreground", typeof(Brush), typeof(FrameStatisticsModel2D),
    new PropertyMetadata(new SolidColorBrush(UIColors.Black), (d, e) =>
    {
        if (d is FrameStatisticsModel2D model)
        {
            model.foregroundChanged = true;
        }
    }));

#if false
#elif WINUI
    new
#elif WPF
#else
#error Unknown framework
#endif
    public Brush? Foreground
    {
        set
        {
            SetValue(ForegroundProperty, value);
        }
        get
        {
            return (Brush?)GetValue(ForegroundProperty);
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty BackgroundProperty
        = DependencyProperty.Register("Background", typeof(Brush), typeof(FrameStatisticsModel2D),
            new PropertyMetadata(new SolidColorBrush(UIColor.FromArgb(64, 32, 32, 32)), (d, e) =>
            {
                if (d is FrameStatisticsModel2D model)
                {
                    model.backgroundChanged = true;
                }
            }));

#if false
#elif WINUI
    new
#elif WPF
#else
#error Unknown framework
#endif
    public Brush? Background
    {
        set
        {
            SetValue(BackgroundProperty, value);
        }
        get
        {
            return (Brush?)GetValue(BackgroundProperty);
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

    protected override void OnUpdate(RenderContext2D context)
    {
        base.OnUpdate(context);

        if (foregroundChanged)
        {
            if (SceneNode is FrameStatisticsNode2D node)
            {
                node.Foreground = Foreground?.ToD2DBrush(context.DeviceContext);
            }

            foregroundChanged = false;
        }

        if (backgroundChanged)
        {
            if (SceneNode is FrameStatisticsNode2D node)
            {
                node.Background = Background?.ToD2DBrush(context.DeviceContext);
            }

            backgroundChanged = false;
        }
    }
}
