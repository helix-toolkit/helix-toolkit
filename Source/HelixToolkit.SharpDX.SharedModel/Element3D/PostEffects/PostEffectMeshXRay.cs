#if NETFX_CORE
using  Windows.Foundation;
using Windows.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Windows.UI.Colors;
using Media = Windows.UI;

namespace HelixToolkit.UWP
#elif WINUI 
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Microsoft.UI.Colors;
using Media = Windows.UI;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media = System.Windows.Media;
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
    /// <seealso cref="Element3D" />
    public class PostEffectMeshXRay : Element3D
    {
        #region Dependency Properties
        /// <summary>
        /// The effect name property
        /// </summary>
        public static readonly DependencyProperty EffectNameProperty =
            DependencyProperty.Register("EffectName", typeof(string), typeof(PostEffectMeshXRay), new PropertyMetadata(DefaultRenderTechniqueNames.PostEffectMeshXRay, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectXRay).EffectName = (string)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the name of the effect.
        /// </summary>
        /// <value>
        /// The name of the effect.
        /// </value>
        public string EffectName
        {
            get
            {
                return (string)GetValue(EffectNameProperty);
            }
            set
            {
                SetValue(EffectNameProperty, value);
            }
        }


        /// <summary>
        /// The outline color property
        /// </summary>
        public static DependencyProperty OutlineColorProperty = DependencyProperty.Register("OutlineColor", typeof(Color), typeof(PostEffectMeshXRay),
            new PropertyMetadata(Colors.Blue,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectXRay).Color = ((Color)e.NewValue).ToColor4();
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

        /// <summary>
        /// The outline fading factor property
        /// </summary>
        public static DependencyProperty OutlineFadingFactorProperty = DependencyProperty.Register("OutlineFadingFactor", typeof(double), typeof(PostEffectMeshXRay),
            new PropertyMetadata(1.5, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectXRay).OutlineFadingFactor = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the outline fading factor.
        /// </summary>
        /// <value>
        /// The outline fading factor.
        /// </value>
        public double OutlineFadingFactor
        {
            set
            {
                SetValue(OutlineFadingFactorProperty, value);
            }
            get
            {
                return (double)GetValue(OutlineFadingFactorProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [double pass]. Double pass uses stencil buffer to reduce overlapping artifacts
        /// </summary>
        public static readonly DependencyProperty EnableDoublePassProperty =
            DependencyProperty.Register("EnableDoublePass", typeof(bool), typeof(PostEffectMeshXRay), new PropertyMetadata(false, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectXRay).EnableDoublePass = (bool)e.NewValue;
            }));


        /// <summary>
        /// Gets or sets a value indicating whether [double pass]. Double pass uses stencil buffer to reduce overlapping artifacts
        /// </summary>
        public bool EnableDoublePass
        {
            get
            {
                return (bool)GetValue(EnableDoublePassProperty);
            }
            set
            {
                SetValue(EnableDoublePassProperty, value);
            }
        }
        #endregion

        protected override SceneNode OnCreateSceneNode()
        {
            return new NodePostEffectXRay();
        }

        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            base.AssignDefaultValuesToSceneNode(core);
            if (core is NodePostEffectXRay c)
            {
                c.EffectName = EffectName;
                c.Color = OutlineColor.ToColor4();
                c.OutlineFadingFactor = (float)OutlineFadingFactor;
                c.EnableDoublePass = EnableDoublePass;
            }
        }
    }
}
