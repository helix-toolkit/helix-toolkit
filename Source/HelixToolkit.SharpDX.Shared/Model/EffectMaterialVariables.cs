using SharpDX.Direct3D11;
using System.ComponentModel;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    public sealed class EffectMaterialVariables : DisposeObject, IEffectMaterialVariables
    {
        public event System.EventHandler<bool> OnInvalidateRenderer;
        private IPhongMaterial material;
        private readonly Effect effect;
        private ShaderResourceView texDiffuseAlphaMapView;
        private ShaderResourceView texDiffuseMapView;
        private ShaderResourceView texNormalMapView;
        private ShaderResourceView texDisplacementMapView;
        private EffectVectorVariable vMaterialAmbientVariable, vMaterialDiffuseVariable, vMaterialEmissiveVariable, vMaterialSpecularVariable, vMaterialReflectVariable;
        private EffectScalarVariable sMaterialShininessVariable;
        private EffectScalarVariable bHasDiffuseMapVariable, bHasNormalMapVariable, bHasDisplacementMapVariable, bHasDiffuseAlphaMapVariable;
        private EffectShaderResourceVariable texDiffuseMapVariable, texNormalMapVariable, texDisplacementMapVariable, texShadowMapVariable, texDiffuseAlphaMapVariable;
        private EffectScalarVariable bHasShadowMapVariable;
        public bool RenderDiffuseMap { set; get; } = true;
        public bool RenderDiffuseAlphaMap { set; get; } = true;
        public bool RenderNormalMap { set; get; } = true;
        public bool RenderDisplacementMap { set; get; } = true;
        public bool HasShadowMap { set; get; } = false;

        public IMaterial Material
        {
            set
            {
                if (material != value)
                {
                    if (material != null)
                    {
                        material.PropertyChanged -= Material_OnMaterialPropertyChanged;
                    }
                    material = value as IPhongMaterial;
                    if (material != null)
                    {
                        material.PropertyChanged += Material_OnMaterialPropertyChanged;
                    }
                    CreateTextureViews();
                }
            }
            get
            {
                return material;
            }
        }

        public EffectMaterialVariables(Effect effect)
        {
            this.effect = effect;
            Collect(this.vMaterialAmbientVariable = effect.GetVariableByName(ShaderVariableNames.MaterialAmbientVariable).AsVector());
            Collect(this.vMaterialDiffuseVariable = effect.GetVariableByName(ShaderVariableNames.MaterialDiffuseVariable).AsVector());
            Collect(this.vMaterialEmissiveVariable = effect.GetVariableByName(ShaderVariableNames.MaterialEmissiveVariable).AsVector());
            Collect(this.vMaterialSpecularVariable = effect.GetVariableByName(ShaderVariableNames.MaterialSpecularVariable).AsVector());
            Collect(this.vMaterialReflectVariable = effect.GetVariableByName(ShaderVariableNames.MaterialReflectVariable).AsVector());
            Collect(this.sMaterialShininessVariable = effect.GetVariableByName(ShaderVariableNames.MaterialShininessVariable).AsScalar());
            Collect(this.bHasDiffuseMapVariable = effect.GetVariableByName(ShaderVariableNames.HasDiffuseMapVariable).AsScalar());
            Collect(this.bHasDiffuseAlphaMapVariable = effect.GetVariableByName(ShaderVariableNames.HasDiffuseAlphaMapVariable).AsScalar());
            Collect(this.bHasNormalMapVariable = effect.GetVariableByName(ShaderVariableNames.HasNormalMapVariable).AsScalar());
            Collect(this.bHasDisplacementMapVariable = effect.GetVariableByName(ShaderVariableNames.HasDisplacementMapVariable).AsScalar());
            Collect(this.bHasShadowMapVariable = effect.GetVariableByName(ShaderVariableNames.HasShadowMapVariable).AsScalar());
            Collect(this.texDiffuseMapVariable = effect.GetVariableByName(ShaderVariableNames.TextureDiffuseMapVariable).AsShaderResource());
            Collect(this.texNormalMapVariable = effect.GetVariableByName(ShaderVariableNames.TextureNormalMapVariable).AsShaderResource());
            Collect(this.texDisplacementMapVariable = effect.GetVariableByName(ShaderVariableNames.TextureDisplacementMapVariable).AsShaderResource());
            Collect(this.texShadowMapVariable = effect.GetVariableByName(ShaderVariableNames.TextureShadowMapVariable).AsShaderResource());
            Collect(this.texDiffuseAlphaMapVariable = effect.GetVariableByName(ShaderVariableNames.TextureDiffuseAlphaMapVariable).AsShaderResource());
            CreateTextureViews();
        }

        private void Material_OnMaterialPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IPhongMaterial.DiffuseMap)))
            {
                CreateTextureView((sender as IPhongMaterial).DiffuseMap, ref this.texDiffuseMapView);
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.NormalMap)))
            {
                CreateTextureView((sender as IPhongMaterial).NormalMap, ref this.texNormalMapView);
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DisplacementMap)))
            {
                CreateTextureView((sender as IPhongMaterial).DisplacementMap, ref this.texDisplacementMapView);
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DiffuseAlphaMap)))
            {
                CreateTextureView((sender as IPhongMaterial).DiffuseAlphaMap, ref this.texDiffuseAlphaMapView);
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

        public bool AttachMaterial(Geometry3D model)
        {
            var mesh = model as MeshGeometry3D;
            if (material == null || mesh == null)
            {
                return false;
            }          
            this.vMaterialDiffuseVariable.Set(material.DiffuseColor);
            this.vMaterialAmbientVariable.Set(material.AmbientColor);
            this.vMaterialEmissiveVariable.Set(material.EmissiveColor);
            this.vMaterialSpecularVariable.Set(material.SpecularColor);
            this.vMaterialReflectVariable.Set(material.ReflectiveColor);
            this.sMaterialShininessVariable.Set(material.SpecularShininess);

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

            bool hasNormalMap = RenderNormalMap && this.texNormalMapView != null && mesh.Tangents != null;
            this.bHasNormalMapVariable.Set(hasNormalMap);
            if (hasNormalMap)
            {
                this.texNormalMapVariable.SetResource(this.texNormalMapView);
            }

            bool hasDisplacementMap = RenderDisplacementMap && this.texDisplacementMapView != null && mesh.BiTangents != null;
            this.bHasDisplacementMapVariable.Set(hasDisplacementMap);
            if (hasDisplacementMap)
            {
                this.texDisplacementMapVariable.SetResource(this.texDisplacementMapView);
            }
            this.bHasShadowMapVariable.Set(HasShadowMap);
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
