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
        public static DependencyProperty GridColorProperty = DependencyProperty.Register("GridColor", typeof(Color), typeof(PostEffectMeshXRayGrid),
            new PropertyMetadata(Colors.DarkBlue,
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
        public Color GridColor
        {
            set
            {
                SetValue(GridColorProperty, value);
            }
            get
            {
                return (Color)GetValue(GridColorProperty);
            }
        }

        /// <summary>
        /// Gets or sets the grid density.
        /// </summary>
        /// <value>
        /// The grid density.
        /// </value>
        public int GridDensity
        {
            get
            {
                return (int)GetValue(GridDensityProperty);
            }
            set
            {
                SetValue(GridDensityProperty, value);
            }
        }
        /// <summary>
        /// The grid density property
        /// </summary>
        public static readonly DependencyProperty GridDensityProperty =
            DependencyProperty.Register("GridDensity", typeof(int), typeof(PostEffectMeshXRayGrid), new PropertyMetadata(8,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as NodePostEffectXRayGrid).GridDensity = (int)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the dimming factor.
        /// </summary>
        /// <value>
        /// The dimming factor.
        /// </value>
        public double DimmingFactor
        {
            get
            {
                return (double)GetValue(DimmingFactorProperty);
            }
            set
            {
                SetValue(DimmingFactorProperty, value);
            }
        }

        /// <summary>
        /// The dimming factor property
        /// </summary>
        public static readonly DependencyProperty DimmingFactorProperty =
            DependencyProperty.Register("DimmingFactor", typeof(double), typeof(PostEffectMeshXRayGrid), new PropertyMetadata(0.8,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as NodePostEffectXRayGrid).DimmingFactor = (float)(double)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the blending factor for grid and original mesh color blending
        /// </summary>
        /// <value>
        /// The blending factor.
        /// </value>
        public double BlendingFactor
        {
            get
            {
                return (double)GetValue(BlendingFactorProperty);
            }
            set
            {
                SetValue(BlendingFactorProperty, value);
            }
        }

        /// <summary>
        /// The blending factor property
        /// </summary>
        public static readonly DependencyProperty BlendingFactorProperty =
            DependencyProperty.Register("BlendingFactor", typeof(double), typeof(PostEffectMeshXRayGrid), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as NodePostEffectXRayGrid).BlendingFactor = (float)(double)e.NewValue;
                }));


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
                c.Color = GridColor.ToColor4();
                c.GridDensity = GridDensity;
                c.DimmingFactor = (float)DimmingFactor;
                c.BlendingFactor = (float)BlendingFactor;
            }
        }
    }
}
