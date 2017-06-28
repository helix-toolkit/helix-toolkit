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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    using global::SharpDX;
    using global::SharpDX.Direct3D11;

    using Utilities;

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
                    if (model.effectMaterial != null)
                    {
                        model.effectMaterial.RenderDiffuseMap = (bool)e.NewValue;
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
                    if (model.effectMaterial != null)
                    {
                        model.effectMaterial.RenderDiffuseAlphaMap = (bool)e.NewValue;
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
                    if (model.effectMaterial != null)
                    {
                        model.effectMaterial.RenderNormalMap = (bool)e.NewValue;
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
                    if (model.effectMaterial != null)
                    {
                        model.effectMaterial.RenderDisplacementMap = (bool)e.NewValue;
                    }
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(MaterialGeometryModel3D), new AffectsRenderPropertyMetadata(null, MaterialChanged));


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
        public Material Material
        {
            get { return (Material)this.GetValue(MaterialProperty); }
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
            if (e.NewValue is PhongMaterial)
            {
                var model = ((MaterialGeometryModel3D)d);
                model.materialInternal = e.NewValue as PhongMaterial;
                if (model.renderHost != null)
                {
                    if (model.IsAttached)
                    {
                        model.AttachMaterial();
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

        #region Variables
        protected bool hasShadowMap = false;
        protected InputLayout vertexLayout;
        protected EffectTechnique effectTechnique;
        protected EffectTransformVariables effectTransforms;
        protected EffectMaterialVariables effectMaterial;
        #endregion
        #region Properties
        protected PhongMaterial materialInternal { private set; get; }
        /// <summary>
        /// For subclass override
        /// </summary>
        public abstract IBufferProxy VertexBuffer { get; }
        /// <summary>
        /// For subclass override
        /// </summary>
        public abstract IBufferProxy IndexBuffer { get; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected virtual void AttachMaterial()
        {
            Disposer.RemoveAndDispose(ref this.effectMaterial);
            if (materialInternal != null)
            {               
                this.effectMaterial = new EffectMaterialVariables(this.effect, materialInternal);
                this.effectMaterial.CreateTextureViews(Device, this);
                this.effectMaterial.RenderDiffuseMap = this.RenderDiffuseMap;
                this.effectMaterial.RenderDiffuseAlphaMap = this.RenderDiffuseAlphaMap;
                this.effectMaterial.RenderNormalMap = this.RenderNormalMap;
                this.effectMaterial.RenderDisplacementMap = this.RenderDisplacementMap;
                this.effectMaterial.OnInvalidateRenderer += (s,e) => { InvalidateRender(); };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref this.effectMaterial);
            Disposer.RemoveAndDispose(ref this.effectTransforms);

            this.effectTechnique = null;
            this.vertexLayout = null;

            base.OnDetach();
        }


        public class EffectMaterialVariables : System.IDisposable
        {
            public event System.EventHandler OnInvalidateRenderer;
            private readonly PhongMaterial material;
            private readonly Device device;
            private ShaderResourceView texDiffuseAlphaMapView;
            private ShaderResourceView texDiffuseMapView;
            private ShaderResourceView texNormalMapView;
            private ShaderResourceView texDisplacementMapView;
            private EffectVectorVariable vMaterialAmbientVariable, vMaterialDiffuseVariable, vMaterialEmissiveVariable, vMaterialSpecularVariable, vMaterialReflectVariable;
            private EffectScalarVariable sMaterialShininessVariable;
            private EffectScalarVariable bHasDiffuseMapVariable, bHasNormalMapVariable, bHasDisplacementMapVariable, bHasDiffuseAlphaMapVariable;
            private EffectShaderResourceVariable texDiffuseMapVariable, texNormalMapVariable, texDisplacementMapVariable, texShadowMapVariable, texDiffuseAlphaMapVariable;
            public EffectScalarVariable bHasShadowMapVariable;

            public bool RenderDiffuseMap { set; get; } = true;
            public bool RenderDiffuseAlphaMap { set; get; } = true;
            public bool RenderNormalMap { set; get; } = true;
            public bool RenderDisplacementMap { set; get; } = true;

            public EffectMaterialVariables(Effect effect, PhongMaterial material)
            {
                this.material = material;
                device = effect.Device;
                this.material.OnMaterialPropertyChanged += Material_OnMaterialPropertyChanged;
                this.vMaterialAmbientVariable = effect.GetVariableByName("vMaterialAmbient").AsVector();
                this.vMaterialDiffuseVariable = effect.GetVariableByName("vMaterialDiffuse").AsVector();
                this.vMaterialEmissiveVariable = effect.GetVariableByName("vMaterialEmissive").AsVector();
                this.vMaterialSpecularVariable = effect.GetVariableByName("vMaterialSpecular").AsVector();
                this.vMaterialReflectVariable = effect.GetVariableByName("vMaterialReflect").AsVector();
                this.sMaterialShininessVariable = effect.GetVariableByName("sMaterialShininess").AsScalar();
                this.bHasDiffuseMapVariable = effect.GetVariableByName("bHasDiffuseMap").AsScalar();
                this.bHasDiffuseAlphaMapVariable = effect.GetVariableByName("bHasAlphaMap").AsScalar();
                this.bHasNormalMapVariable = effect.GetVariableByName("bHasNormalMap").AsScalar();
                this.bHasDisplacementMapVariable = effect.GetVariableByName("bHasDisplacementMap").AsScalar();
                this.bHasShadowMapVariable = effect.GetVariableByName("bHasShadowMap").AsScalar();
                this.texDiffuseMapVariable = effect.GetVariableByName("texDiffuseMap").AsShaderResource();
                this.texNormalMapVariable = effect.GetVariableByName("texNormalMap").AsShaderResource();
                this.texDisplacementMapVariable = effect.GetVariableByName("texDisplacementMap").AsShaderResource();
                this.texShadowMapVariable = effect.GetVariableByName("texShadowMap").AsShaderResource();
                this.texDiffuseAlphaMapVariable = effect.GetVariableByName("texAlphaMap").AsShaderResource();
            }

            private void Material_OnMaterialPropertyChanged(object sender, MaterialPropertyChanged e)
            {
                if (e.PropertyName.Equals(nameof(material.DiffuseMap)))
                {
                    CreateTextureView(material.DiffuseMap, ref this.texDiffuseMapView);
                }
                else if (e.PropertyName.Equals(nameof(material.NormalMap)))
                {
                    CreateTextureView(material.NormalMap, ref this.texNormalMapView);
                }
                else if (e.PropertyName.Equals(nameof(material.DisplacementMap)))
                {
                    CreateTextureView(material.DisplacementMap, ref this.texDisplacementMapView);
                }
                else if (e.PropertyName.Equals(nameof(material.DiffuseAlphaMap)))
                {
                    CreateTextureView(material.DiffuseAlphaMap, ref this.texDiffuseAlphaMapView);
                }
                OnInvalidateRenderer?.Invoke(this, null);
            }

            private void CreateTextureView(System.IO.Stream stream, ref ShaderResourceView textureView)
            {
                Disposer.RemoveAndDispose(ref textureView);
                if (stream != null)
                {
                    textureView = TextureLoader.FromMemoryAsShaderResourceView(device, stream);
                }
            }

            public void CreateTextureViews(Device device, MaterialGeometryModel3D model)
            {
                if (material != null)
                {
                    CreateTextureView(material.DiffuseMap, ref this.texDiffuseMapView);
                    CreateTextureView(material.NormalMap, ref this.texNormalMapView);
                    CreateTextureView(material.DisplacementMap, ref this.texDisplacementMapView);
                    CreateTextureView(material.DiffuseAlphaMap, ref this.texDiffuseAlphaMapView);
                }
            }

            public bool AttachMaterial(MeshGeometry3D model)
            {
                if (material == null || model == null)
                {
                    return false;
                }
                this.vMaterialDiffuseVariable.Set(material.DiffuseColorInternal);
                this.vMaterialAmbientVariable.Set(material.AmbientColorInternal);
                this.vMaterialEmissiveVariable.Set(material.EmissiveColorInternal);
                this.vMaterialSpecularVariable.Set(material.SpecularColorInternal);
                this.vMaterialReflectVariable.Set(material.ReflectiveColorInternal);
                this.sMaterialShininessVariable.Set(material.SpecularShininessInternal);

                // --- has samples              
                bool hasDiffuseMap = RenderDiffuseMap && this.texDiffuseMapView != null;
                this.bHasDiffuseMapVariable.Set(hasDiffuseMap);
                if (hasDiffuseMap)
                { this.texDiffuseMapVariable.SetResource(this.texDiffuseMapView); }

                bool hasDiffuseAlphaMap = RenderDiffuseAlphaMap && this.texDiffuseAlphaMapView != null;
                this.bHasDiffuseAlphaMapVariable.Set(hasDiffuseAlphaMap);
                if (hasDiffuseAlphaMap)
                {
                    this.texDiffuseAlphaMapVariable.SetResource(this.texDiffuseAlphaMapView);
                }

                bool hasNormalMap = RenderNormalMap && this.texNormalMapView != null && model.Tangents != null;
                this.bHasNormalMapVariable.Set(hasNormalMap);
                if (hasNormalMap)
                {
                    this.texNormalMapVariable.SetResource(this.texNormalMapView);
                }

                bool hasDisplacementMap = RenderDisplacementMap && this.texDisplacementMapView != null && model.BiTangents != null;
                this.bHasDisplacementMapVariable.Set(hasDisplacementMap);
                if (hasDisplacementMap)
                {
                    this.texDisplacementMapVariable.SetResource(this.texDisplacementMapView);
                }
                return true;
            }

            public void Dispose()
            {
                this.material.OnMaterialPropertyChanged -= Material_OnMaterialPropertyChanged;
                OnInvalidateRenderer = null;
                Disposer.RemoveAndDispose(ref this.vMaterialAmbientVariable);
                Disposer.RemoveAndDispose(ref this.vMaterialDiffuseVariable);
                Disposer.RemoveAndDispose(ref this.vMaterialEmissiveVariable);
                Disposer.RemoveAndDispose(ref this.vMaterialSpecularVariable);
                Disposer.RemoveAndDispose(ref this.sMaterialShininessVariable);
                Disposer.RemoveAndDispose(ref this.vMaterialReflectVariable);
                Disposer.RemoveAndDispose(ref this.bHasDiffuseMapVariable);
                Disposer.RemoveAndDispose(ref this.bHasNormalMapVariable);
                Disposer.RemoveAndDispose(ref this.bHasDisplacementMapVariable);
                Disposer.RemoveAndDispose(ref this.bHasShadowMapVariable);
                Disposer.RemoveAndDispose(ref this.bHasDiffuseAlphaMapVariable);
                Disposer.RemoveAndDispose(ref this.texDiffuseMapVariable);
                Disposer.RemoveAndDispose(ref this.texNormalMapVariable);
                Disposer.RemoveAndDispose(ref this.texDisplacementMapVariable);
                Disposer.RemoveAndDispose(ref this.texShadowMapVariable);
                Disposer.RemoveAndDispose(ref this.texDiffuseAlphaMapVariable);
                Disposer.RemoveAndDispose(ref this.texDiffuseMapView);
                Disposer.RemoveAndDispose(ref this.texNormalMapView);
                Disposer.RemoveAndDispose(ref this.texDisplacementMapView);
                Disposer.RemoveAndDispose(ref this.texDiffuseAlphaMapView);
            }
        }

    }
}
