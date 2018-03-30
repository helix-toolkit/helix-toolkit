// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaterialGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using Core;
    using System.Windows;
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.Wpf.SharpDX.GeometryModel3D" />
    public abstract class MaterialGeometryModel3D : GeometryModel3D
    {
        #region Dependency Properties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderDiffuseMapProperty =
            DependencyProperty.Register("RenderDiffuseMap", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    if ((d as IRenderable).RenderCore is IMaterialRenderParams m)
                    {
                        m.RenderDiffuseMap = (bool)e.NewValue;
                    }
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderDiffuseAlphaMapProperty =
            DependencyProperty.Register("RenderDiffuseAlphaMap", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    if ((d as IRenderable).RenderCore is IMaterialRenderParams m)
                    {
                        m.RenderDiffuseAlphaMap = (bool)e.NewValue;
                    }
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderNormalMapProperty =
            DependencyProperty.Register("RenderNormalMap", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    if ((d as IRenderable).RenderCore is IMaterialRenderParams m)
                    {
                        m.RenderNormalMap = (bool)e.NewValue;
                    }
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderDisplacementMapProperty =
            DependencyProperty.Register("RenderDisplacementMap", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    if ((d as IRenderable).RenderCore is IMaterialRenderParams m)
                    {
                        m.RenderDisplacementMap = (bool)e.NewValue;
                    }
                }));

        /// <summary>
        /// Render shadow on this mesh if has shadow map
        /// </summary>
        public static readonly DependencyProperty RenderShadowMapProperty =
            DependencyProperty.Register("RenderShadowMap", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(false,
                (d, e) =>
                {
                    if ((d as IRenderable).RenderCore is IMaterialRenderParams m)
                    {
                        m.RenderShadowMap = (bool)e.NewValue;
                    }
                }));

        /// <summary>
        /// Render environment reflection map on this mesh if has environment map
        /// </summary>
        public static readonly DependencyProperty RenderEnvironmentMapProperty =
            DependencyProperty.Register("RenderEnvironmentMap", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(false,
                (d, e) =>
                {
                    if ((d as IRenderable).RenderCore is IMaterialRenderParams m)
                    {
                        m.RenderEnvironmentMap = (bool)e.NewValue;
                    }
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(MaterialGeometryModel3D), new PropertyMetadata(null, MaterialChanged));

        /// <summary>
        /// Specifiy if model material is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public static readonly DependencyProperty IsTransparentProperty =
            DependencyProperty.Register("IsTransparent", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(false, (d, e) =>
            {
                var model = d as Element3DCore;
                if (model.RenderCore.RenderType == RenderType.Opaque || model.RenderCore.RenderType == RenderType.Transparent)
                {
                    model.RenderCore.RenderType = (bool)e.NewValue ? RenderType.Transparent : RenderType.Opaque;
                }
            }));


        /// <summary>
        /// 
        /// </summary>
        public bool RenderDiffuseMap
        {
            get { return (bool)this.GetValue(RenderDiffuseMapProperty); }
            set { this.SetValue(RenderDiffuseMapProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RenderNormalMap
        {
            get { return (bool)this.GetValue(RenderNormalMapProperty); }
            set { this.SetValue(RenderNormalMapProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RenderDiffuseAlphaMap
        {
            get { return (bool)this.GetValue(RenderDiffuseAlphaMapProperty); }
            set { this.SetValue(RenderDiffuseAlphaMapProperty, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool RenderDisplacementMap
        {
            get { return (bool)this.GetValue(RenderDisplacementMapProperty); }
            set { this.SetValue(RenderDisplacementMapProperty, value); }
        }

        /// <summary>
        /// Render shadow on this mesh if has shadow map
        /// <para>Default: false</para>
        /// </summary>
        public bool RenderShadowMap
        {
            get { return (bool)this.GetValue(RenderShadowMapProperty); }
            set { this.SetValue(RenderShadowMapProperty, value); }
        }

        /// <summary>
        /// Render environment map on this mesh if has environment map
        /// <para>Default: false</para>
        /// </summary>
        public bool RenderEnvironmentMap
        {
            get { return (bool)this.GetValue(RenderEnvironmentMapProperty); }
            set { this.SetValue(RenderEnvironmentMapProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Material Material
        {
            get { return (Material)this.GetValue(MaterialProperty); }
            set { this.SetValue(MaterialProperty, value); }
        }

        /// <summary>
        /// Specifiy if model material is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public bool IsTransparent
        {
            get { return (bool)GetValue(IsTransparentProperty); }
            set { SetValue(IsTransparentProperty, value); }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// 
        /// </summary>
        protected static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is PhongMaterial material)
            {
                var model = d as MaterialGeometryModel3D;
                (model.RenderCore as IMaterialRenderParams).Material = material;
                if (model.RenderHost != null)
                {
                    if (model.IsAttached)
                    {
                        model.AttachMaterial();
                        model.InvalidateRender();
                    }
                    else
                    {
                        var host = model.RenderHost;
                        model.Detach();
                        model.Attach(host);
                    }
                }               
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected virtual void AttachMaterial()
        {
            var core = RenderCore as IMaterialRenderParams;
            core.Material = this.Material;
            core.RenderDiffuseMap = this.RenderDiffuseMap;
            core.RenderDiffuseAlphaMap = this.RenderDiffuseAlphaMap;
            core.RenderNormalMap = this.RenderNormalMap;
            core.RenderDisplacementMap = this.RenderDisplacementMap;
            core.RenderEnvironmentMap = this.RenderEnvironmentMap;
            core.RenderShadowMap = this.RenderShadowMap;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (!base.OnAttach(host))
            {
                return false;
            }
            // --- material 
            this.AttachMaterial();
            return true;
        }
    }
}
