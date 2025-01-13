using HelixToolkit.SharpDX.Model.Scene;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Use this model to keep update rendering in each frame.
/// <para>Default behavior for render host is lazy rendering. 
/// Only property changes trigger a render inside render loop. 
/// However, sometimes user want to keep updating rendering each frame while doing shader animation using TimeStamp.
/// Use this model to invalidate rendering in each frame and keep render host busy.
/// </para>
/// </summary>
public sealed class ContinuousRender3D : Element3D
{
    protected override SceneNode OnCreateSceneNode()
    {
        return new ContinuousRenderNode();
    }
}
