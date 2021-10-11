
#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
#if !COREWPF && !WINUI
    using Model.Scene;
#endif
    /// <summary>
    /// 
    /// </summary>
    public class EnvironmentMap3D : Element3D
    {
        /// <summary>
        /// The texture property
        /// </summary>
        public static readonly DependencyProperty TextureProperty = DependencyProperty.Register("Texture", typeof(TextureModel), typeof(EnvironmentMap3D),
            new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as EnvironmentMapNode).Texture = (TextureModel)e.NewValue;
            }));
        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public TextureModel Texture
        {
            set
            {
                SetValue(TextureProperty, value);
            }
            get
            {
                return (TextureModel)GetValue(TextureProperty);
            }
        }
        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new EnvironmentMapNode();
        }
        /// <summary>
        /// Assigns the default values to scene node.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            base.AssignDefaultValuesToSceneNode(core);
            (SceneNode as EnvironmentMapNode).Texture = Texture;
        }
    }
}
