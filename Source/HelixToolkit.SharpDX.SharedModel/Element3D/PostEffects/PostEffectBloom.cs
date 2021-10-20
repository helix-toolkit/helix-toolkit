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
    public class PostEffectBloom : Element3D
    {
        #region Dependency Properties
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
        /// The effect name property
        /// </summary>
        public static readonly DependencyProperty EffectNameProperty =
            DependencyProperty.Register("EffectName", typeof(string), typeof(PostEffectBloom),
                new PropertyMetadata(DefaultRenderTechniqueNames.PostEffectBloom, (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as NodePostEffectBloom).EffectName = (string)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the threshold color.
        /// </summary>
        /// <value>
        /// The threshold color.
        /// </value>
        public Color ThresholdColor
        {
            get
            {
                return (Color)GetValue(ThresholdColorProperty);
            }
            set
            {
                SetValue(ThresholdColorProperty, value);
            }
        }

        /// <summary>
        /// The threshold color property
        /// </summary>
        public static readonly DependencyProperty ThresholdColorProperty =
            DependencyProperty.Register("ThresholdColor", typeof(Color), typeof(PostEffectBloom), new PropertyMetadata(Color.FromArgb(0, 200, 200, 200),
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as NodePostEffectBloom).ThresholdColor = ((Color)e.NewValue).ToColor4();
                }));

        /// <summary>
        /// Gets or sets the number of blur pass.
        /// </summary>
        /// <value>
        /// The number of blur pass.
        /// </value>
        public int NumberOfBlurPass
        {
            get
            {
                return (int)GetValue(NumberOfBlurPassProperty);
            }
            set
            {
                SetValue(NumberOfBlurPassProperty, value);
            }
        }

        /// <summary>
        /// The number of blur pass property
        /// </summary>
        public static readonly DependencyProperty NumberOfBlurPassProperty =
            DependencyProperty.Register("NumberOfBlurPass", typeof(int), typeof(PostEffectBloom), new PropertyMetadata(1, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectBloom).NumberOfBlurPass = (int)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the bloom extract intensity.
        /// </summary>
        /// <value>
        /// The bloom extract intensity.
        /// </value>
        public double BloomExtractIntensity
        {
            get
            {
                return (double)GetValue(BloomExtractIntensityProperty);
            }
            set
            {
                SetValue(BloomExtractIntensityProperty, value);
            }
        }

        /// <summary>
        /// The bloom extract intensity property
        /// </summary>
        public static readonly DependencyProperty BloomExtractIntensityProperty =
            DependencyProperty.Register("BloomExtractIntensity", typeof(double), typeof(PostEffectBloom), new PropertyMetadata(1.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectBloom).BloomExtractIntensity = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the bloom pass intensity.
        /// </summary>
        /// <value>
        /// The bloom pass intensity.
        /// </value>
        public double BloomPassIntensity
        {
            get
            {
                return (double)GetValue(BloomPassIntensityProperty);
            }
            set
            {
                SetValue(BloomPassIntensityProperty, value);
            }
        }

        /// <summary>
        /// The bloom pass intensity property
        /// </summary>
        public static readonly DependencyProperty BloomPassIntensityProperty =
            DependencyProperty.Register("BloomPassIntensity", typeof(double), typeof(PostEffectBloom), new PropertyMetadata(0.95, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectBloom).BloomPassIntensity = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the bloom combine intensity.
        /// </summary>
        /// <value>
        /// The bloom combine intensity.
        /// </value>
        public double BloomCombineIntensity
        {
            get
            {
                return (double)GetValue(BloomCombineIntensityProperty);
            }
            set
            {
                SetValue(BloomCombineIntensityProperty, value);
            }
        }

        /// <summary>
        /// The bloom combine intensity property
        /// </summary>
        public static readonly DependencyProperty BloomCombineIntensityProperty =
            DependencyProperty.Register("BloomCombineIntensity", typeof(double), typeof(PostEffectBloom), new PropertyMetadata(0.7, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectBloom).BloomCombineIntensity = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the bloom combine saturation.
        /// </summary>
        /// <value>
        /// The bloom combine saturation.
        /// </value>
        public double BloomCombineSaturation
        {
            get
            {
                return (double)GetValue(BloomCombineSaturationProperty);
            }
            set
            {
                SetValue(BloomCombineSaturationProperty, value);
            }
        }

        /// <summary>
        /// The bloom combine saturation property
        /// </summary>
        public static readonly DependencyProperty BloomCombineSaturationProperty =
            DependencyProperty.Register("BloomCombineSaturation", typeof(double), typeof(PostEffectBloom), new PropertyMetadata(0.7, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodePostEffectBloom).BloomCombineSaturation = (float)(double)e.NewValue;
            }));
        #endregion

        protected override SceneNode OnCreateSceneNode()
        {
            return new NodePostEffectBloom();
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="node">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            base.AssignDefaultValuesToSceneNode(node);
            if (node is NodePostEffectBloom c)
            {
                c.EffectName = EffectName;
                c.BloomCombineIntensity = (float)BloomCombineIntensity;
                c.BloomCombineSaturation = (float)BloomCombineSaturation;
                c.BloomExtractIntensity = (float)BloomExtractIntensity;
                c.BloomPassIntensity = (float)BloomPassIntensity;
                c.NumberOfBlurPass = NumberOfBlurPass;
                c.ThresholdColor = ThresholdColor.ToColor4();
            }
        }
    }
}
