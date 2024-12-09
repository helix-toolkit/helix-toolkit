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

/// <summary>
/// Use to apply style for mover button from Generic.xaml/>
/// </summary>
/// <seealso cref="Button2D" />
public sealed class MoverButton2D : Button2D
{
    static MoverButton2D()
    {
#if WPF
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(MoverButton2D), new FrameworkPropertyMetadata(typeof(MoverButton2D)));
#endif
    }

    public MoverButton2D()
    {
        DefaultStyleKey = typeof(MoverButton2D);

        SetDefaultStyle();
    }

    private void SetDefaultStyle()
    {
#if WINUI
        var backgroupUIColor = UIColors.Transparent;
        this.Background = new SolidColorBrush(backgroupUIColor);

        this.CornerRadius = 0;
        this.Width = 12;
        this.Height = 12;
        this.BorderThickness = new UIThickness(2, 2, 2, 2);
        this.BorderBrush = new SolidColorBrush(UIColor.FromArgb(255, 255, 125, 0));
        this.Margin = new Thickness(8, 8, 8, 8);
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

            var backgroupUIColor = UIColors.DarkOrange;
            this.Background = new SolidColorBrush(backgroupUIColor);
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
