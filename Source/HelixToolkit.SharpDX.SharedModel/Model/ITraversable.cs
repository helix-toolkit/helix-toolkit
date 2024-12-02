#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public interface ITraversable
{
    IList<ITraversable> Items
    {
        get;
    }
}
