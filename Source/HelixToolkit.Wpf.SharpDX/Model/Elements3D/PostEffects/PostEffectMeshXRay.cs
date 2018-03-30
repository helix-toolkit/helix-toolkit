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
    public class PostEffectMeshXRay : Element3D
    {
        #region Dependency Properties
        /// <summary>
        /// The effect name property
        /// </summary>
        public static readonly DependencyProperty EffectNameProperty =
            DependencyProperty.Register("EffectName", typeof(string), typeof(PostEffectMeshXRay), new PropertyMetadata(DefaultRenderTechniqueNames.PostEffectMeshXRay, (d, e) =>
            {
                ((d as IRenderable).RenderCore as IPostEffect).EffectName = (string)e.NewValue;
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
        public static DependencyProperty OutlineColorProperty = DependencyProperty.Register("OutlineColor", typeof(Color), typeof(PostEffectMeshXRay),
            new PropertyMetadata(Colors.Blue,
            (d, e) =>
            {
                ((d as IRenderable).RenderCore as IPostEffectMeshXRay).Color = ((Color)e.NewValue).ToColor4();
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
            new PropertyMetadata(1.5, (d, e) => {
                ((d as IRenderable).RenderCore as IPostEffectMeshXRay).OutlineFadingFactor = (float)(double)e.NewValue;
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
        public bool EnableDoublePass
        {
            get { return (bool)GetValue(EnableDoublePassProperty); }
            set { SetValue(EnableDoublePassProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [double pass]. Double pass uses stencil buffer to reduce overlapping artifacts
        /// </summary>
        public static readonly DependencyProperty EnableDoublePassProperty =
            DependencyProperty.Register("EnableDoublePass", typeof(bool), typeof(PostEffectMeshXRay), new PropertyMetadata(false, (d,e)=>
            {
                ((d as IRenderable).RenderCore as IPostEffectMeshXRay).DoublePass = (bool)e.NewValue;
            }));
        #endregion

        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new PostEffectMeshXRayCore();
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToCore(RenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            (core as IPostEffectMeshXRay).EffectName = EffectName;
            (core as IPostEffectMeshXRay).Color = OutlineColor.ToColor4();
            (core as IPostEffectMeshXRay).OutlineFadingFactor = (float)OutlineFadingFactor;
            (core as IPostEffectMeshXRay).DoublePass = EnableDoublePass;
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
