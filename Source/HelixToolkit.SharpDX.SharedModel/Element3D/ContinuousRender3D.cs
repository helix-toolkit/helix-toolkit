#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Model.Scene;
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
}
