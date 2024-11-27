using HelixToolkit.SharpDX;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// 
/// </summary>
/// <seealso cref="GroupElement3D" />
/// <seealso cref="IHitable" />
public class GroupModel3D : GroupElement3D, IHitable, IVisible
{
}
