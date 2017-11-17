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

        public PhongMaterial Material { set; get; } = PhongMaterials.Black;
        public MeshGeometry3D Geometry { set; get; }
        public bool HasShadowMap { set; get; } = false;

        protected override bool OnAttach(IRenderHost host, RenderTechnique technique)
        {
            if(base.OnAttach(host, technique))
            {
                RemoveAndDispose(ref materialVariables);
                materialVariables = Collect(new EffectMaterialVariables(Effect, Material));
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

        public class EffectMaterialVariables : DisposeObject
        {
            public event System.EventHandler OnInvalidateRenderer;
            private readonly PhongMaterial material;
            private Device device;
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
                RemoveAndDispose(ref textureView);
                if (stream != null && device != null)
                {
                    Collect(textureView = TextureLoader.FromMemoryAsShaderResourceView(device, stream));
                }
            }

            public void CreateTextureViews(Device device, MaterialGeometryModel3D model)
            {
                this.device = device;
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
                this.material.OnMaterialPropertyChanged -= Material_OnMaterialPropertyChanged;
                OnInvalidateRenderer = null;
                base.Dispose(disposeManagedResources);
            }
        }
    }
}
