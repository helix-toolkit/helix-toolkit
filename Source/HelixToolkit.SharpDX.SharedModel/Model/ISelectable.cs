#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public interface ISelectable
{

    bool IsSelected
    {
        get; set;
    }
}
