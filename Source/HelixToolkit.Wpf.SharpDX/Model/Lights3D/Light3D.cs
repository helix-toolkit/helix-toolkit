// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Light3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Direction of the light.
//   It applies to Directional Light and to Spot Light,
//   for all other lights it is ignored.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using global::SharpDX;
    using HelixToolkit.Wpf.SharpDX.Core;
    using System.Windows;
    using System.Windows.Media.Media3D;
    using Media = System.Windows.Media;

    public abstract class Light3D : Element3DCore, ILight3D, ITransformable
    {
        /// <summary>
        /// Indicates, if this element should be rendered,
        /// default is true
        /// </summary>
        public static readonly DependencyProperty IsRenderingProperty =
            DependencyProperty.Register("IsRendering", typeof(bool), typeof(Light3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    (d as Element3DCore).Visible = (bool)e.NewValue;
                }));

        /// <summary>
        /// Indicates, if this element should be rendered.
        /// Use this also to make the model visible/unvisible
        /// default is true
        /// </summary>
        public bool IsRendering
        {
            get { return (bool)GetValue(IsRenderingProperty); }
            set { SetValue(IsRenderingProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Media.Color), typeof(Light3D), new PropertyMetadata(Media.Colors.Gray, (d,e)=>
            {
                (((IRenderable)d).RenderCore as LightCoreBase).Color = ((Media.Color)e.NewValue).ToColor4();
            }));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform3D), typeof(Light3D), new PropertyMetadata(Transform3D.Identity, (d, e) =>
            {
                ((IRenderable)d).ModelMatrix = e.NewValue != null ? ((Transform3D)e.NewValue).Value.ToMatrix() : Matrix.Identity;
            }));
        /// <summary>
        /// 
        /// </summary>
        public Transform3D Transform
        {
            get { return (Transform3D)this.GetValue(TransformProperty); }
            set { this.SetValue(TransformProperty, value); }
        }
        /// <summary>
        /// Color of the light.
        /// For simplicity, this color applies to the diffuse and specular properties of the light.
        /// </summary>
        public Media.Color Color
        {
            get { return (Media.Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public LightType LightType
        {
            get { return (RenderCore as ILight3D).LightType; }
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            (core as LightCoreBase).Color = Color.ToColor4();
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref System.Collections.Generic.List<HitTestResult> hits)
        {
            return false;
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }
    }
}
