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
    using System.ComponentModel;
    using System.Windows;

    using global::SharpDX;

    using Utilities;
    using Core;

    public abstract class MaterialGeometryModel3D : InstanceGeometryModel3D
    {
        #region Dependency Properties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderDiffuseMapProperty =
            DependencyProperty.Register("RenderDiffuseMap", typeof(bool), typeof(MaterialGeometryModel3D), new AffectsRenderPropertyMetadata(true,
                (d, e) =>
                {
                    var model = d as MaterialGeometryModel3D;
                    if (model.RenderCore != null)
                    {
                        (model.RenderCore as IMaterialRenderParams).RenderDiffuseMap = (bool)e.NewValue;
                    }
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderDiffuseAlphaMapProperty =
            DependencyProperty.Register("RenderDiffuseAlphaMap", typeof(bool), typeof(MaterialGeometryModel3D), new AffectsRenderPropertyMetadata(true,
                (d, e) =>
                {
                    var model = d as MaterialGeometryModel3D;
                    if (model.RenderCore != null)
                    {
                        (model.RenderCore as IMaterialRenderParams).RenderDiffuseAlphaMap = (bool)e.NewValue;
                    }
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderNormalMapProperty =
            DependencyProperty.Register("RenderNormalMap", typeof(bool), typeof(MaterialGeometryModel3D), new AffectsRenderPropertyMetadata(true,
                (d, e) =>
                {
                    var model = d as MaterialGeometryModel3D;
                    if (model.RenderCore != null)
                    {
                        (model.RenderCore as IMaterialRenderParams).RenderNormalMap = (bool)e.NewValue;
                    }
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderDisplacementMapProperty =
            DependencyProperty.Register("RenderDisplacementMap", typeof(bool), typeof(MaterialGeometryModel3D), new AffectsRenderPropertyMetadata(true,
                (d, e) =>
                {
                    var model = d as MaterialGeometryModel3D;
                    if (model.RenderCore != null)
                    {
                        (model.RenderCore as IMaterialRenderParams).RenderDisplacementMap = (bool)e.NewValue;
                    }
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(IMaterial), typeof(MaterialGeometryModel3D), new AffectsRenderPropertyMetadata(null, MaterialChanged));


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextureCoodScaleProperty =
            DependencyProperty.Register("TextureCoodScale", typeof(Vector2), typeof(MaterialGeometryModel3D), new AffectsRenderPropertyMetadata(new Vector2(1, 1)));


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
        /// 
        /// </summary>
        public IMaterial Material
        {
            get { return (IMaterial)this.GetValue(MaterialProperty); }
            set { this.SetValue(MaterialProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [TypeConverter(typeof(Vector2Converter))]
        public Vector2 TextureCoodScale
        {
            get { return (Vector2)this.GetValue(TextureCoodScaleProperty); }
            set { this.SetValue(TextureCoodScaleProperty, value); }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// 
        /// </summary>
        protected static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IPhongMaterial)
            {
                var model = ((MaterialGeometryModel3D)d);
                (model.RenderCore as IMaterialRenderParams).Material = e.NewValue as IPhongMaterial;
                if (model.renderHost != null)
                {
                    if (model.IsAttached)
                    {
                        model.AttachMaterial();
                        model.InvalidateRender();
                    }
                    else
                    {
                        var host = model.renderHost;
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
