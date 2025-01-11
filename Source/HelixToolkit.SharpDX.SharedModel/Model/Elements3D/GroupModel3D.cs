using HelixToolkit.SharpDX;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// 
/// </summary>
/// <seealso cref="GroupElement3D" />
/// <seealso cref="IHitable" />
public class GroupModel3D : GroupElement3D, IHitable, IVisible
{
}
