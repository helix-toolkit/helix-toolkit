#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model.Scene;
    
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
