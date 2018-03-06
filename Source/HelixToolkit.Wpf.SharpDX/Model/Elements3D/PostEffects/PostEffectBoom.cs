using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.Wpf.SharpDX.Element3D" />
    public class PostEffectBoom : Element3D
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
            get { return (string)GetValue(EffectNameProperty); }
            set { SetValue(EffectNameProperty, value); }
        }

        /// <summary>
        /// The effect name property
        /// </summary>
        public static readonly DependencyProperty EffectNameProperty =
            DependencyProperty.Register("EffectName", typeof(string), typeof(PostEffectBoom),
                new PropertyMetadata(DefaultRenderTechniqueNames.PostEffectBloom, (d, e) =>
                {
                    ((d as IRenderable).RenderCore as IPostEffectBloom).EffectName = (string)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the threshold color.
        /// </summary>
        /// <value>
        /// The threshold color.
        /// </value>
        public Color ThresholdColor
        {
            get { return (Color)GetValue(ThresholdColorProperty); }
            set { SetValue(ThresholdColorProperty, value); }
        }

        /// <summary>
        /// The threshold color property
        /// </summary>
        public static readonly DependencyProperty ThresholdColorProperty =
            DependencyProperty.Register("ThresholdColor", typeof(Color), typeof(PostEffectBoom), new PropertyMetadata(Color.FromArgb(0, 200, 200, 200), 
                (d, e) =>
                {
                    ((d as IRenderable).RenderCore as IPostEffectBloom).ThresholdColor = ((Color)e.NewValue).ToColor4();
                }));

        /// <summary>
        /// Gets or sets the number of blur pass.
        /// </summary>
        /// <value>
        /// The number of blur pass.
        /// </value>
        public int NumberOfBlurPass
        {
            get { return (int)GetValue(NumberOfBlurPassProperty); }
            set { SetValue(NumberOfBlurPassProperty, value); }
        }

        /// <summary>
        /// The number of blur pass property
        /// </summary>
        public static readonly DependencyProperty NumberOfBlurPassProperty =
            DependencyProperty.Register("NumberOfBlurPass", typeof(int), typeof(PostEffectBoom), new PropertyMetadata(2, (d, e) =>
            {
                ((d as IRenderable).RenderCore as IPostEffectBloom).NumberOfBlurPass = (int)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the maximum down sampling step.
        /// </summary>
        /// <value>
        /// The maximum down sampling step.
        /// </value>
        public int MaximumDownSamplingStep
        {
            get { return (int)GetValue(MaximumDownSamplingStepProperty); }
            set { SetValue(MaximumDownSamplingStepProperty, value); }
        }

        /// <summary>
        /// The maximum down sampling step property
        /// </summary>
        public static readonly DependencyProperty MaximumDownSamplingStepProperty =
            DependencyProperty.Register("MaximumDownSamplingStep", typeof(int), typeof(PostEffectBoom), new PropertyMetadata(3, (d, e) =>
            {
                ((d as IRenderable).RenderCore as IPostEffectBloom).MaximumDownSamplingStep = (int)e.NewValue;
            }));


        /// <summary>
        /// Gets or sets the bloom extract intensity.
        /// </summary>
        /// <value>
        /// The bloom extract intensity.
        /// </value>
        public double BloomExtractIntensity
        {
            get { return (double)GetValue(BloomExtractIntensityProperty); }
            set { SetValue(BloomExtractIntensityProperty, value); }
        }

        /// <summary>
        /// The bloom extract intensity property
        /// </summary>
        public static readonly DependencyProperty BloomExtractIntensityProperty =
            DependencyProperty.Register("BloomExtractIntensity", typeof(double), typeof(PostEffectBoom), new PropertyMetadata(1.0, (d,e)=> 
            {
                ((d as IRenderable).RenderCore as IPostEffectBloom).BloomExtractIntensity = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the bloom pass intensity.
        /// </summary>
        /// <value>
        /// The bloom pass intensity.
        /// </value>
        public double BloomPassIntensity
        {
            get { return (double)GetValue(BloomPassIntensityProperty); }
            set { SetValue(BloomPassIntensityProperty, value); }
        }

        /// <summary>
        /// The bloom pass intensity property
        /// </summary>
        public static readonly DependencyProperty BloomPassIntensityProperty =
            DependencyProperty.Register("BloomPassIntensity", typeof(double), typeof(PostEffectBoom), new PropertyMetadata(0.95, (d, e) =>
            {
                ((d as IRenderable).RenderCore as IPostEffectBloom).BloomPassIntensity = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the bloom combine intensity.
        /// </summary>
        /// <value>
        /// The bloom combine intensity.
        /// </value>
        public double BloomCombineIntensity
        {
            get { return (double)GetValue(BloomCombineIntensityProperty); }
            set { SetValue(BloomCombineIntensityProperty, value); }
        }

        /// <summary>
        /// The bloom combine intensity property
        /// </summary>
        public static readonly DependencyProperty BloomCombineIntensityProperty =
            DependencyProperty.Register("BloomCombineIntensity", typeof(double), typeof(PostEffectBoom), new PropertyMetadata(0.7, (d, e) =>
            {
                ((d as IRenderable).RenderCore as IPostEffectBloom).BloomCombineIntensity = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the bloom combine saturation.
        /// </summary>
        /// <value>
        /// The bloom combine saturation.
        /// </value>
        public double BloomCombineSaturation
        {
            get { return (double)GetValue(BloomCombineSaturationProperty); }
            set { SetValue(BloomCombineSaturationProperty, value); }
        }

        /// <summary>
        /// The bloom combine saturation property
        /// </summary>
        public static readonly DependencyProperty BloomCombineSaturationProperty =
            DependencyProperty.Register("BloomCombineSaturation", typeof(double), typeof(PostEffectBoom), new PropertyMetadata(0.7, (d, e) =>
            {
                ((d as IRenderable).RenderCore as IPostEffectBloom).BloomCombineSaturation = (float)(double)e.NewValue;
            }));
        #endregion

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override IRenderCore OnCreateRenderCore()
        {
            return new PostEffectBloomCore();
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            var c = core as IPostEffectBloom;
            c.EffectName = EffectName;
            c.BloomCombineIntensity = (float)BloomCombineIntensity;
            c.BloomCombineSaturation = (float)BloomCombineSaturation;
            c.BloomExtractIntensity = (float)BloomExtractIntensity;
            c.BloomPassIntensity = (float)BloomPassIntensity;
            c.MaximumDownSamplingStep = MaximumDownSamplingStep;
            c.NumberOfBlurPass = NumberOfBlurPass;
            c.ThresholdColor = ThresholdColor.ToColor4();
        }
        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="Element3DCore.OnSetRenderTechnique" /> is set, then <see cref="Element3DCore.OnSetRenderTechnique" /> instead of <see cref="Element3DCore.OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.PostEffectBloom];
        }

        /// <summary>
        /// Determines whether this instance [can hit test] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }
        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
