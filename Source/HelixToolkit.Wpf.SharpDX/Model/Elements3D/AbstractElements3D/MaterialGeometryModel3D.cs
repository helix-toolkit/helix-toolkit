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

    using HelixToolkit.Wpf.SharpDX.Utilities;

    public abstract class MaterialGeometryModel3D : InstanceGeometryModel3D
    {
        protected InputLayout vertexLayout;
        protected EffectTechnique effectTechnique;
        protected EffectTransformVariables effectTransforms;
        protected EffectMaterialVariables effectMaterial;
        /// <summary>
        /// For subclass override
        /// </summary>
        public abstract IBufferProxy VertexBuffer { get; }
        /// <summary>
        /// For subclass override
        /// </summary>
        public abstract IBufferProxy IndexBuffer { get; }

        protected bool hasShadowMap = false;
        public MaterialGeometryModel3D()
        {
        }

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
        public static readonly DependencyProperty RenderDiffuseMapProperty =
            DependencyProperty.Register("RenderDiffuseMap", typeof(bool), typeof(MaterialGeometryModel3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

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
        public static readonly DependencyProperty RenderAlphaDiffuseMapProperty =
            DependencyProperty.Register("RenderAlphaDiffuseMap", typeof(bool), typeof(MaterialGeometryModel3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 
        /// </summary>
        public bool RenderAlphaDiffuseMap
        {
            get { return (bool)this.GetValue(RenderAlphaDiffuseMapProperty); }
            set { this.SetValue(RenderAlphaDiffuseMapProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderNormalMapProperty =
            DependencyProperty.Register("RenderNormalMap", typeof(bool), typeof(MaterialGeometryModel3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

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
        public static readonly DependencyProperty RenderDisplacementMapProperty =
            DependencyProperty.Register("RenderDisplacementMap", typeof(bool), typeof(MaterialGeometryModel3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

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
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(MaterialGeometryModel3D), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, MaterialChanged));

        /// <summary>
        /// 
        /// </summary>
        protected static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is PhongMaterial)
            {
                var model = ((MaterialGeometryModel3D)d);
                if (model.IsAttached)
                {
                    var host = model.renderHost;
                    model.Detach();
                    model.Attach(host);
                }
            }
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

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextureCoodScaleProperty =
            DependencyProperty.Register("TextureCoodScale", typeof(Vector2), typeof(MaterialGeometryModel3D), new FrameworkPropertyMetadata(new Vector2(1, 1), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 
        /// </summary>
        protected virtual void AttachMaterial()
        {
            var phongMaterial = Material as PhongMaterial;
            if (phongMaterial != null)
            {
                this.effectMaterial = new EffectMaterialVariables(this.effect, phongMaterial);
                this.effectMaterial.CreateTextureViews(Device, this);
            }
        }

        public class EffectMaterialVariables : System.IDisposable
        {
            private PhongMaterial material;
            private ShaderResourceView texDiffuseAlphaMapView;
            private ShaderResourceView texDiffuseMapView;
            private ShaderResourceView texNormalMapView;
            private ShaderResourceView texDisplacementMapView;
            private EffectVectorVariable vMaterialAmbientVariable, vMaterialDiffuseVariable, vMaterialEmissiveVariable, vMaterialSpecularVariable, vMaterialReflectVariable;
            private EffectScalarVariable sMaterialShininessVariable;
            private EffectScalarVariable bHasDiffuseMapVariable, bHasNormalMapVariable, bHasDisplacementMapVariable, bHasDiffuseAlphaMapVariable;
            private EffectShaderResourceVariable texDiffuseMapVariable, texNormalMapVariable, texDisplacementMapVariable, texShadowMapVariable, texDiffuseAlphaMapVariable;
            public EffectScalarVariable bHasShadowMapVariable;
            public EffectMaterialVariables(Effect effect, PhongMaterial material)
            {
                this.material = material;

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

            public void CreateTextureViews(Device device, MaterialGeometryModel3D model)
            {
                if (material != null)
                {
                    /// --- has texture
                    if (material.DiffuseMap != null && model.RenderDiffuseMap)
                    {
                        Disposer.RemoveAndDispose(ref this.texDiffuseMapView);
                        this.texDiffuseMapView = TextureLoader.FromMemoryAsShaderResourceView(device, material.DiffuseMap);
                    }

                    if (material.DiffuseAlphaMap != null)
                    {
                        Disposer.RemoveAndDispose(ref this.texDiffuseAlphaMapView);
                        this.texDiffuseAlphaMapView = TextureLoader.FromMemoryAsShaderResourceView(device, material.DiffuseAlphaMap);
                    }

                    // --- has bumpmap
                    if (material.NormalMap != null && model.RenderNormalMap)
                    {
                        var geometry = model.geometryInternal as MeshGeometry3D;
                        if (geometry != null)
                        {
                            if (geometry.Tangents == null)
                            {
                                //System.Windows.MessageBox.Show(string.Format("No Tangent-Space found. NormalMap will be omitted."), "Warrning", MessageBoxButton.OK);
                                material.NormalMap = null;
                            }
                            else
                            {
                                Disposer.RemoveAndDispose(ref this.texNormalMapView);
                                this.texNormalMapView = TextureLoader.FromMemoryAsShaderResourceView(device, material.NormalMap);
                            }
                        }
                    }

                    // --- has displacement map
                    if (material.DisplacementMap != null && model.RenderDisplacementMap)
                    {
                        Disposer.RemoveAndDispose(ref this.texDisplacementMapView);
                        this.texDisplacementMapView = TextureLoader.FromMemoryAsShaderResourceView(device, material.DisplacementMap);
                    }
                }
            }

            public bool AttachMaterial()
            {
                // --- has samples              
                this.bHasDiffuseMapVariable.Set(this.texDiffuseMapView != null);
                this.bHasDiffuseAlphaMapVariable.Set(this.texDiffuseAlphaMapView != null);
                this.bHasNormalMapVariable.Set(this.texNormalMapView != null);
                this.bHasDisplacementMapVariable.Set(this.texDisplacementMapView != null);

                if (material != null)
                {
                    this.vMaterialDiffuseVariable.Set(material.DiffuseColorInternal);
                    this.vMaterialAmbientVariable.Set(material.AmbientColorInternal);
                    this.vMaterialEmissiveVariable.Set(material.EmissiveColorInternal);
                    this.vMaterialSpecularVariable.Set(material.SpecularColorInternal);
                    this.vMaterialReflectVariable.Set(material.ReflectiveColorInternal);
                    this.sMaterialShininessVariable.Set(material.SpecularShininessInternal);
                    this.texDiffuseMapVariable.SetResource(this.texDiffuseMapView);
                    this.texNormalMapVariable.SetResource(this.texNormalMapView);
                    this.texDiffuseAlphaMapVariable.SetResource(this.texDiffuseAlphaMapView);
                    this.texDisplacementMapVariable.SetResource(this.texDisplacementMapView);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Dispose()
            {
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

    }
}
