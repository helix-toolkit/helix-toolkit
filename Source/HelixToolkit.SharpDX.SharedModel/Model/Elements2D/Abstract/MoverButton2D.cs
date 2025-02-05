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

/// <summary>
/// Use to apply style for mover button from Generic.xaml/>
/// </summary>
/// <seealso cref="Button2D" />
public sealed class MoverButton2D : Button2D
{
    static MoverButton2D()
    {
#if false
#elif WINUI
#elif WPF
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(MoverButton2D), new FrameworkPropertyMetadata(typeof(MoverButton2D)));
#else
#error Unknown framework
#endif
    }

    public MoverButton2D()
    {
        DefaultStyleKey = typeof(MoverButton2D);

        SetDefaultStyle();
    }

    private void SetDefaultStyle()
    {
#if false
#elif WINUI
        var backgroupUIColor = UIColors.Transparent;
        this.Background = new SolidColorBrush(backgroupUIColor);

        this.CornerRadius = 0;
        this.Width = 12;
        this.Height = 12;
        this.BorderThickness = new UIThickness(2, 2, 2, 2);
        this.BorderBrush = new SolidColorBrush(UIColor.FromArgb(255, 255, 125, 0));
        this.Margin = new Thickness(8, 8, 8, 8);
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
#elif WPF
#else
#error Unknown framework
#endif
    }
}
