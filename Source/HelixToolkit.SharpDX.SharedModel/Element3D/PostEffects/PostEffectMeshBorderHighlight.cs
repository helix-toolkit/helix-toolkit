#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.SharpDX.Core.Wpf
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
#endif
{
#if !COREWPF
    using Model.Scene;
#endif
    /// <summary>
    /// Highlight the border of meshes
    /// </summary>
    public class PostEffectMeshBorderHighlight : PostEffectMeshOutlineBlur
    {
        /// <summary>
        /// Gets or sets the draw mode.
        /// </summary>
        /// <value>
        /// The draw mode.
        /// </value>
        public OutlineMode DrawMode
        {
            get { return (OutlineMode)GetValue(DrawModeProperty); }
            set { SetValue(DrawModeProperty, value); }
        }

        /// <summary>
        /// The draw mode property
        /// </summary>
        public static readonly DependencyProperty DrawModeProperty =
            DependencyProperty.Register("DrawMode", typeof(OutlineMode), typeof(PostEffectMeshBorderHighlight), new PropertyMetadata(OutlineMode.Merged, 
                (d,e)=>
                {
                    ((d as Element3D).SceneNode as NodePostEffectBorderHighlight).DrawMode = (OutlineMode)e.NewValue;
                }));


        protected override SceneNode OnCreateSceneNode()
        {
            return new NodePostEffectBorderHighlight();
        }
    }
}
