using SharpDX.Direct3D11;


#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{

    public abstract class MaterialGeometryRenderCore : GeometryRenderCore
    {
        private EffectMaterialVariables materialVariables;
        /// <summary>
        /// Used to wrap all material resources
        /// </summary>
        public EffectMaterialVariables MaterialVariables { get { return materialVariables; } }
        private PhongMaterial material = PhongMaterials.Black;
        public PhongMaterial Material
        {
            set
            {
                if (material != value)
                {
                    material = value;
                    if (materialVariables != null)
                    {
                        materialVariables.Material = value;
                    }
                }
            }
            get
            {
                return material;
            }
        }

        public bool HasShadowMap { set; get; } = false;

        protected override bool OnAttach(IRenderHost host, RenderTechnique technique)
        {
            if(base.OnAttach(host, technique))
            {
                if (materialVariables != null)
                {
                    materialVariables.OnInvalidateRenderer -= InvalidateRenderer;
                    RemoveAndDispose(ref materialVariables);
                }
                materialVariables = Collect(new EffectMaterialVariables(Effect, Material));
                materialVariables.OnInvalidateRenderer += InvalidateRenderer;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void SetMaterialVariables(MeshGeometry3D model)
        {
            if (model == null) { return; }
            materialVariables?.bHasShadowMapVariable?.Set(HasShadowMap);
            materialVariables?.AttachMaterial(model);
        }        

        public sealed class EffectMaterialVariables : DisposeObject
        {
            public event System.EventHandler<bool> OnInvalidateRenderer;
            private PhongMaterial material;
            private readonly Effect effect;
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

            public PhongMaterial Material
            {
                set
                {
                    if (material != value)
                    {
                        if (material != null)
                        {
                            material.OnMaterialPropertyChanged -= Material_OnMaterialPropertyChanged;
                        }
                        material = value;
                        if (material != null)
                        {
                            material.OnMaterialPropertyChanged += Material_OnMaterialPropertyChanged;                           
                        }
                        CreateTextureViews();
                    }
                }
            }

            public EffectMaterialVariables(Effect effect, PhongMaterial material)
            {
                this.effect = effect;
                this.material = material;
                this.material.OnMaterialPropertyChanged += Material_OnMaterialPropertyChanged;
                Collect(this.vMaterialAmbientVariable = effect.GetVariableByName("vMaterialAmbient").AsVector());
                Collect(this.vMaterialDiffuseVariable = effect.GetVariableByName("vMaterialDiffuse").AsVector());
                Collect(this.vMaterialEmissiveVariable = effect.GetVariableByName("vMaterialEmissive").AsVector());
                Collect(this.vMaterialSpecularVariable = effect.GetVariableByName("vMaterialSpecular").AsVector());
                Collect(this.vMaterialReflectVariable = effect.GetVariableByName("vMaterialReflect").AsVector());
                Collect(this.sMaterialShininessVariable = effect.GetVariableByName("sMaterialShininess").AsScalar());
                Collect(this.bHasDiffuseMapVariable = effect.GetVariableByName("bHasDiffuseMap").AsScalar());
                Collect(this.bHasDiffuseAlphaMapVariable = effect.GetVariableByName("bHasAlphaMap").AsScalar());
                Collect(this.bHasNormalMapVariable = effect.GetVariableByName("bHasNormalMap").AsScalar());
                Collect(this.bHasDisplacementMapVariable = effect.GetVariableByName("bHasDisplacementMap").AsScalar());
                Collect(this.bHasShadowMapVariable = effect.GetVariableByName("bHasShadowMap").AsScalar());
                Collect(this.texDiffuseMapVariable = effect.GetVariableByName("texDiffuseMap").AsShaderResource());
                Collect(this.texNormalMapVariable = effect.GetVariableByName("texNormalMap").AsShaderResource());
                Collect(this.texDisplacementMapVariable = effect.GetVariableByName("texDisplacementMap").AsShaderResource());
                Collect(this.texShadowMapVariable = effect.GetVariableByName("texShadowMap").AsShaderResource());
                Collect(this.texDiffuseAlphaMapVariable = effect.GetVariableByName("texAlphaMap").AsShaderResource());
                CreateTextureViews();
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
                OnInvalidateRenderer?.Invoke(this, true);
            }

            private void CreateTextureView(System.IO.Stream stream, ref ShaderResourceView textureView)
            {
                RemoveAndDispose(ref textureView);
                if (stream != null && effect != null && !effect.IsDisposed)
                {
                    Collect(textureView = TextureLoader.FromMemoryAsShaderResourceView(effect.Device, stream));
                }
            }

            private void CreateTextureViews()
            {
                if (material != null)
                {
                    CreateTextureView(material.DiffuseMap, ref this.texDiffuseMapView);
                    CreateTextureView(material.NormalMap, ref this.texNormalMapView);
                    CreateTextureView(material.DisplacementMap, ref this.texDisplacementMapView);
                    CreateTextureView(material.DiffuseAlphaMap, ref this.texDiffuseAlphaMapView);
                }
                else
                {
                    RemoveAndDispose(ref this.texDiffuseMapView);
                    RemoveAndDispose(ref this.texNormalMapView);
                    RemoveAndDispose(ref this.texDisplacementMapView);
                    RemoveAndDispose(ref this.texDiffuseAlphaMapView);
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

            protected override void Dispose(bool disposeManagedResources)
            {
                this.material = null;
                OnInvalidateRenderer = null;
                base.Dispose(disposeManagedResources);
            }
        }
    }
}
