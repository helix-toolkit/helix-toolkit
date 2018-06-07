#if NETFX_CORE
using Windows.Foundation;
using Windows.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Windows.UI.Colors;
using Media = Windows.UI;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media = System.Windows.Media;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Model.Scene;
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Element3D" />
    public class PostEffectMeshXRayGrid : Element3D
    {
        #region Dependency Properties
        /// <summary>
        /// The effect name property
        /// </summary>
        public static readonly DependencyProperty EffectNameProperty =
            DependencyProperty.Register("EffectName", typeof(string), typeof(PostEffectMeshXRayGrid), new PropertyMetadata(DefaultRenderTechniqueNames.PostEffectMeshXRayGrid, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectXRayGrid).EffectName = (string)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the name of the effect.
        /// </summary>
        /// <value>
        /// The name of the effect.
        /// </value>
        public string EffectName
        {
            get { return (string)GetValue(EffectNameProperty); }
            set { SetValue(EffectNameProperty, value); }
        }


        /// <summary>
        /// The outline color property
        /// </summary>
        public static DependencyProperty OutlineColorProperty = DependencyProperty.Register("OutlineColor", typeof(Color), typeof(PostEffectMeshXRayGrid),
            new PropertyMetadata(Colors.Gray,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectXRayGrid).Color = ((Color)e.NewValue).ToColor4();
            }));

        /// <summary>
        /// Gets or sets the color of the outline.
        /// </summary>
        /// <value>
        /// The color of the outline.
        /// </value>
        public Color OutlineColor
        {
            set
            {
                SetValue(OutlineColorProperty, value);
            }
            get
            {
                return (Color)GetValue(OutlineColorProperty);
            }
        }
        #endregion

        protected override SceneNode OnCreateSceneNode()
        {
            return new NodePostEffectXRayGrid();
        }

        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            base.AssignDefaultValuesToSceneNode(core);
            if (core is NodePostEffectXRayGrid c)
            {
                c.EffectName = EffectName;
                c.Color = OutlineColor.ToColor4();
            }
        }
    }
}
