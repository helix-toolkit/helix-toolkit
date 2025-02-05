#if false
#elif WINUI
using Microsoft.UI.Xaml.Media;
#elif WPF
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

public interface ITextBlock : IBackground
{
    Brush Foreground
    {
        set; get;
    }
}
