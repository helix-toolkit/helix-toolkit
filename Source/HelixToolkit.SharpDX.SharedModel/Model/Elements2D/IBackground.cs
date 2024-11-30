#if WINUI
using Microsoft.UI.Xaml.Media;
#else
using System.Windows.Media;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

public interface IBackground
{
    Brush Background
    {
        set; get;
    }
}
