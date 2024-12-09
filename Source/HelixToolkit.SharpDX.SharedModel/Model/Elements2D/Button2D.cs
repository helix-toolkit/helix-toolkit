#if WINUI
using Microsoft.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

public class Button2D : Clickable2D
{
    static Button2D()
    {
#if WPF
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Button2D), new FrameworkPropertyMetadata(typeof(Button2D)));
#endif
    }

    public Button2D()
    {
#if WINUI
        DefaultStyleKey = typeof(Button2D);
#endif

        SetDefaultStyle();
    }

    private void SetDefaultStyle()
    {
#if WINUI
        var backgroupColor = System.Drawing.SystemColors.ControlDark;
        var backgroupUIColor = UIColor.FromArgb(backgroupColor.A, backgroupColor.R, backgroupColor.G, backgroupColor.B);
        this.Background = new SolidColorBrush(backgroupUIColor);

        var foregroupColor = System.Drawing.SystemColors.ControlText;
        var foregroupUIColor = UIColor.FromArgb(foregroupColor.A, foregroupColor.R, foregroupColor.G, foregroupColor.B);
        this.Foreground = new SolidColorBrush(foregroupUIColor);

        this.CornerRadius = 2;
        this.BorderThickness = new UIThickness(0, 0, 0, 0);
#endif
    }

#if WINUI
    private Brush? _previousBackgroundBrush;
#endif

    protected override void OnMouseOverChanged(bool newValue, bool oldValue)
    {
        base.OnMouseOverChanged(newValue, oldValue);

#if WINUI
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
#endif
    }
}
