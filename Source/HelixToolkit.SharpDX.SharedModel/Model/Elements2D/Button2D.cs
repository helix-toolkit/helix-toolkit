#if false
#elif WINUI
using Microsoft.UI.Xaml.Media;
#elif WPF
using System.Windows;
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

public class Button2D : Clickable2D
{
    static Button2D()
    {
#if false
#elif WINUI
#elif WPF
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Button2D), new FrameworkPropertyMetadata(typeof(Button2D)));
#else
#error Unknown framework
#endif
    }

    public Button2D()
    {
#if false
#elif WINUI
        DefaultStyleKey = typeof(Button2D);
#elif WPF
#else
#error Unknown framework
#endif

        SetDefaultStyle();
    }

    private void SetDefaultStyle()
    {
#if false
#elif WINUI
        var backgroupColor = System.Drawing.SystemColors.ControlDark;
        var backgroupUIColor = UIColor.FromArgb(backgroupColor.A, backgroupColor.R, backgroupColor.G, backgroupColor.B);
        this.Background = new SolidColorBrush(backgroupUIColor);

        var foregroupColor = System.Drawing.SystemColors.ControlText;
        var foregroupUIColor = UIColor.FromArgb(foregroupColor.A, foregroupColor.R, foregroupColor.G, foregroupColor.B);
        this.Foreground = new SolidColorBrush(foregroupUIColor);

        this.CornerRadius = 2;
        this.BorderThickness = new UIThickness(0, 0, 0, 0);
#elif WPF
#else
#error Unknown framework
#endif
    }

#if false
#elif WINUI
    private Brush? _previousBackgroundBrush;
#elif WPF
#else
#error Unknown framework
#endif

    protected override void OnMouseOverChanged(bool newValue, bool oldValue)
    {
        base.OnMouseOverChanged(newValue, oldValue);

#if false
#elif WINUI
        if (newValue)
        {
            this._previousBackgroundBrush = this.Background;

            var overColor = System.Drawing.SystemColors.Highlight;
            var overUIColor = UIColor.FromArgb(overColor.A, overColor.R, overColor.G, overColor.B);
            this.Background = new SolidColorBrush(overUIColor);
        }
        else
        {
            if (this._previousBackgroundBrush is not null)
            {
                this.Background = this._previousBackgroundBrush;
            }
        }
#elif WPF
#else
#error Unknown framework
#endif
    }
}
