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

    public abstract class MaterialGeometryModel3D : GeometryModel3D
    {
        protected InputLayout vertexLayout;
        protected Buffer vertexBuffer;
        protected Buffer indexBuffer;
        protected Buffer instanceBuffer;
        protected EffectTechnique effectTechnique;
        protected EffectTransformVariables effectTransforms;
        protected EffectMaterialVariables effectMaterial;
        protected PhongMaterial phongMaterial;
        protected ShaderResourceView texDiffuseAlphaMapView;
        protected ShaderResourceView texDiffuseMapView;
        protected ShaderResourceView texNormalMapView;
        protected ShaderResourceView texDisplacementMapView;
        protected EffectScalarVariable bHasInstances;
        protected bool isInstanceChanged = true;
        protected bool hasInstances = false;
        protected bool hasShadowMap = false;
        public bool HasInstancing { get { return hasInstances; } }
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
            DependencyProperty.Register("RenderDiffuseMap", typeof(bool), typeof(MaterialGeometryModel3D), new UIPropertyMetadata(true));

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
            DependencyProperty.Register("RenderAlphaDiffuseMap", typeof(bool), typeof(MaterialGeometryModel3D), new UIPropertyMetadata(true));

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
            DependencyProperty.Register("RenderNormalMap", typeof(bool), typeof(MaterialGeometryModel3D), new UIPropertyMetadata(true));

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
            DependencyProperty.Register("RenderDisplacementMap", typeof(bool), typeof(MaterialGeometryModel3D), new UIPropertyMetadata(true));

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
            DependencyProperty.Register("Material", typeof(Material), typeof(MaterialGeometryModel3D), new UIPropertyMetadata(MaterialChanged));

        /// <summary>
        /// 
        /// </summary>
        protected static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is PhongMaterial)
            {
                var model = ((MaterialGeometryModel3D)d);
                model.phongMaterial = e.NewValue as PhongMaterial;
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
        public Matrix[] Instances
        {
            get { return (Matrix[])this.GetValue(InstancesProperty); }
            set { this.SetValue(InstancesProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty InstancesProperty =
            DependencyProperty.Register("Instances", typeof(Matrix[]), typeof(MaterialGeometryModel3D), new UIPropertyMetadata(null, InstancesChanged));

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
            DependencyProperty.Register("TextureCoodScale", typeof(Vector2), typeof(MaterialGeometryModel3D), new UIPropertyMetadata(new Vector2(1, 1)));


        /// <summary>
        /// 
        /// </summary>
        private static void InstancesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (MaterialGeometryModel3D)d;
            model.InstancesChanged();
        }

        protected virtual void InstancesChanged()
        {
            this.hasInstances = (this.Instances != null) && (this.Instances.Any());
            UpdateInstancesBounds();
            isInstanceChanged = true;
        }

        protected BoundingBox instancesBound;
        public BoundingBox InstancesBound
        {
            protected set
            {
                instancesBound = value;
            }
            get
            {
                return instancesBound;
            }
        }

        protected virtual void UpdateInstancesBounds()
        {
            if(!hasInstances)
            {
                InstancesBound = this.BoundsWithTransform;
            }
            else
            {
                var bound = BoundingBox.FromPoints(this.BoundsWithTransform.GetCorners().Select(x => Vector3.TransformCoordinate(x, Instances[0])).ToArray());
                foreach(var instance in Instances)
                {
                    var b = BoundingBox.FromPoints(this.BoundsWithTransform.GetCorners().Select(x => Vector3.TransformCoordinate(x, instance)).ToArray());
                    BoundingBox.Merge(ref bound, ref b, out bound);
                }
                InstancesBound = bound;
            }
        }

        protected override bool CheckBoundingFrustum(ref BoundingFrustum boundingFrustum)
        {
            return !hasInstances && base.CheckBoundingFrustum(ref boundingFrustum) || boundingFrustum.Intersects(ref instancesBound);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void AttachMaterial()
        {
            this.phongMaterial = Material as PhongMaterial;
            if (phongMaterial != null)
            {
                this.effectMaterial = new EffectMaterialVariables(this.effect);

                /// --- has texture
                if (phongMaterial.DiffuseMap != null && RenderDiffuseMap)
                {
                    this.texDiffuseMapView = TextureLoader.FromMemoryAsShaderResourceView(Device, phongMaterial.DiffuseMap);
                    this.effectMaterial.texDiffuseMapVariable.SetResource(this.texDiffuseMapView);
                    this.effectMaterial.bHasDiffuseMapVariable.Set(true);
                }
                else
                {
                    this.effectMaterial.bHasDiffuseMapVariable.Set(false);
                }

                if (phongMaterial.DiffuseAlphaMap != null && RenderAlphaDiffuseMap)
                {
                    this.texDiffuseAlphaMapView = TextureLoader.FromMemoryAsShaderResourceView(Device, phongMaterial.DiffuseAlphaMap);
                    this.effectMaterial.texDiffuseAlphaMapVariable.SetResource(this.texDiffuseAlphaMapView);
                    this.effectMaterial.bHasDiffuseAlphaMapVariable.Set(true);
                }
                else
                {
                    this.effectMaterial.bHasDiffuseAlphaMapVariable.Set(false);
                }

                // --- has bumpmap
                if (phongMaterial.NormalMap != null && RenderNormalMap)
                {
                    var geometry = this.Geometry as MeshGeometry3D;
                    if (geometry != null)
                    {
                        if (geometry.Tangents == null)
                        {
                            //System.Windows.MessageBox.Show(string.Format("No Tangent-Space found. NormalMap will be omitted."), "Warrning", MessageBoxButton.OK);
                            phongMaterial.NormalMap = null;
                        }
                        else
                        {
                            this.texNormalMapView = TextureLoader.FromMemoryAsShaderResourceView(Device, phongMaterial.NormalMap);
                            this.effectMaterial.texNormalMapVariable.SetResource(this.texNormalMapView);
                            this.effectMaterial.bHasNormalMapVariable.Set(true);
                        }
                    }
                }
                else
                {
                    this.effectMaterial.bHasNormalMapVariable.Set(false);
                }

                // --- has displacement map
                if (phongMaterial.DisplacementMap != null && RenderDisplacementMap)
                {
                    this.texDisplacementMapView = TextureLoader.FromMemoryAsShaderResourceView(Device, phongMaterial.DisplacementMap);
                    this.effectMaterial.texDisplacementMapVariable.SetResource(this.texDisplacementMapView);
                    this.effectMaterial.bHasDisplacementMapVariable.Set(true);
                }
                else
                {
                    this.effectMaterial.bHasDisplacementMapVariable.Set(false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>        
        public override bool HitTest(Ray rayWS, ref List<HitTestResult> hits)
        {
            if ((this.Instances != null) && (this.Instances.Any()))
            {
                bool hit = false;
                foreach (var modelMatrix in Instances)
                {
                    var b = this.Bounds;
                    this.PushMatrix(modelMatrix);
                    this.Bounds = BoundingBox.FromPoints(this.Geometry.Positions.Select(x => Vector3.TransformCoordinate(x, this.modelMatrix)).ToArray());
                    if (base.HitTest(rayWS, ref hits))
                    {
                        hit = true;
                        var lastHit = hits[hits.Count - 1];
                        lastHit.Tag = modelMatrix;
                        hits[hits.Count - 1] = lastHit;
                    }
                    this.PopMatrix();
                    this.Bounds = b;
                }

                return hit;
            }
            else
            {
                return base.HitTest(rayWS, ref hits);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            this.Detach();
        }

        public class EffectMaterialVariables : System.IDisposable
        {
            public EffectMaterialVariables(Effect effect)
            {
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
            public EffectVectorVariable vMaterialAmbientVariable, vMaterialDiffuseVariable, vMaterialEmissiveVariable, vMaterialSpecularVariable, vMaterialReflectVariable;
            public EffectScalarVariable sMaterialShininessVariable;
            public EffectScalarVariable bHasDiffuseMapVariable, bHasNormalMapVariable, bHasDisplacementMapVariable, bHasShadowMapVariable, bHasDiffuseAlphaMapVariable;
            public EffectShaderResourceVariable texDiffuseMapVariable, texNormalMapVariable, texDisplacementMapVariable, texShadowMapVariable, texDiffuseAlphaMapVariable;

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
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref this.vertexBuffer);
            Disposer.RemoveAndDispose(ref this.indexBuffer);
            Disposer.RemoveAndDispose(ref this.instanceBuffer);

            Disposer.RemoveAndDispose(ref this.effectMaterial);
            Disposer.RemoveAndDispose(ref this.effectTransforms);
            Disposer.RemoveAndDispose(ref this.texDiffuseMapView);
            Disposer.RemoveAndDispose(ref this.texNormalMapView);
            Disposer.RemoveAndDispose(ref this.texDisplacementMapView);
            Disposer.RemoveAndDispose(ref this.texDiffuseAlphaMapView);
            Disposer.RemoveAndDispose(ref this.bHasInstances);

            this.phongMaterial = null;
            this.effectTechnique = null;
            this.vertexLayout = null;

            base.OnDetach();
        }

    }
}
