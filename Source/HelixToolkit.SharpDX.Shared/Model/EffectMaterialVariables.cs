using SharpDX;
using SharpDX.Direct3D11;
using System.ComponentModel;
using System.Runtime.InteropServices;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using ShaderManager;
    using Shaders;
    using Utilities;

    /// <summary>
    /// Default PhongMaterial Variables
    /// </summary>
    public class EffectMaterialVariables : DisposeObject, IEffectMaterialVariables
    {
        public event System.EventHandler<bool> OnInvalidateRenderer;
        public int MaterialIndex = 2;
        public int DiffuseMapIndex = 0;
        public int DiffuseAlphaMapIndex = 1;
        public int NormalMapIndex = 2;
        public int DisplacementMapIndex = 3;

        private IPhongMaterial material;
        public Device Device { private set; get; }
        //private readonly Effect effect;
        private ShaderResourceView texDiffuseAlphaMapView;
        private ShaderResourceView texDiffuseMapView;
        private ShaderResourceView texNormalMapView;
        private ShaderResourceView texDisplacementMapView;

        private bool renderDiffuseMap = true;
        public bool RenderDiffuseMap
        {
            set
            {
                if(renderDiffuseMap == value)
                {
                    return;
                }
                renderDiffuseMap = value;
                needUpdate = true;
            }
            get
            {
                return renderDiffuseMap;
            }
        }
        private bool renderDiffuseAlphaMap = true;
        public bool RenderDiffuseAlphaMap
        {
            set
            {
                if (renderDiffuseAlphaMap == value)
                {
                    return;
                }
                renderDiffuseAlphaMap = value;
                needUpdate = true;
            }
            get
            {
                return renderDiffuseAlphaMap;
            }
        }
        private bool renderNormalMap = true;
        public bool RenderNormalMap
        {
            set
            {
                if (renderNormalMap == value) { return; }
                renderNormalMap = value;
                needUpdate = true;
            }
            get
            {
                return renderNormalMap;
            }
        }
        private bool renderDisplacementMap = true;
        public bool RenderDisplacementMap
        {
            set
            {
                if (renderDisplacementMap == value)
                { return; }
                renderDisplacementMap = value;
                needUpdate = true;
            }
            get
            {
                return renderDisplacementMap;
            }
        }
        private bool hasShadowMap = false;
        public bool HasShadowMap
        {
            set
            {
                if (hasShadowMap == value) { return; }
                hasShadowMap = value;
                needUpdate = true;
            }
            get
            {
                return hasShadowMap;
            }
        }

        private bool needUpdate = true;
        private MaterialStruct materialStruct;
        private readonly IBufferProxy<MaterialStruct> materialBuffer;

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

        public EffectMaterialVariables(IConstantBufferPool cbPool)
        {
            materialBuffer = cbPool.Get(DefaultConstantBufferDescriptions.MaterialCB) as IBufferProxy<MaterialStruct>;
            CreateTextureViews();
        }        

        

        private void Material_OnMaterialPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            needUpdate = true;
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
            if (stream != null && Device != null)
            {
                textureView = Collect(TextureLoader.FromMemoryAsShaderResourceView(Device, stream));
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

        private void AssignVariables()
        {
            materialStruct = new MaterialStruct
            {
                Ambient = material.AmbientColor,
                Diffuse = material.DiffuseColor,
                Emissive = material.EmissiveColor,
                Reflect = material.ReflectiveColor,
                Specular = material.SpecularColor,
                Shininess = material.SpecularShininess,
                HasDiffuseMap = RenderDiffuseMap && texDiffuseMapView != null ? 1u : 0,
                HasDiffuseAlphaMap = RenderDiffuseAlphaMap && texDiffuseAlphaMapView != null ? 1u : 0,
                HasNormalMap = RenderNormalMap && texNormalMapView != null ? 1u : 0,
                HasDisplacementMap = RenderDisplacementMap && texDisplacementMapView != null ? 1u : 0,
                HasShadowMap = HasShadowMap ? 1u : 0
            };
        }

        public bool AttachMaterial(DeviceContext context)
        {
            if (material == null)
            {
                return false;
            }
            if (needUpdate)
            {
                AssignVariables();                
                needUpdate = false;
            }
            materialBuffer.UploadDataToBuffer(context, ref materialStruct);
            return true;
        }

        protected virtual void OnAttachMaterial(DeviceContext context)
        {
            //context.AttachConstantBuffer(ShaderStage.Vertex, MaterialIndex, materialBuffer.Buffer);
            //context.AttachConstantBuffer(ShaderStage.Pixel, MaterialIndex, materialBuffer.Buffer);
            context.AttachShaderResources(ShaderStage.Pixel, DiffuseMapIndex, texDiffuseMapView);
            context.AttachShaderResources(ShaderStage.Pixel, DiffuseAlphaMapIndex, texDiffuseAlphaMapView);
            context.AttachShaderResources(ShaderStage.Pixel, NormalMapIndex, texNormalMapView);
            context.AttachShaderResources(ShaderStage.Pixel, DisplacementMapIndex, texDisplacementMapView);
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            this.material = null;
            OnInvalidateRenderer = null;
            base.Dispose(disposeManagedResources);
        }
    }
}
