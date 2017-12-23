/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    using System.Collections.Generic;
    using Utilities;

    /// <summary>
    /// Default PhongMaterial Variables
    /// </summary>
    public class PhongMaterialVariables : DisposeObject, IEffectMaterialVariables
    {
        public event System.EventHandler<bool> OnInvalidateRenderer;
        private IPhongMaterial material;
        public Device Device { private set; get; }

        public ShaderResourceView TextureAlphaView { get { return texAlphaMapView; } }
        private ShaderResourceView texAlphaMapView;
        public virtual string TextureAlphaName { get { return DefaultBufferNames.AlphaMapTB; } }
        public ShaderResourceView TextureDiffuseView { get { return texDiffuseMapView; } }
        private ShaderResourceView texDiffuseMapView;
        public virtual string TextureDiffuseName { get { return DefaultBufferNames.DiffuseMapTB; } }
        public ShaderResourceView TextureNormalView { get { return texNormalMapView; } }
        private ShaderResourceView texNormalMapView;
        public virtual string TextureNormalName { get { return DefaultBufferNames.NormalMapTB; } }
        public ShaderResourceView TextureDisplacementView { get { return texDisplacementMapView; } }
        private ShaderResourceView texDisplacementMapView;
        public virtual string TextureDisplacementName { get { return DefaultBufferNames.DisplacementMapTB; } }

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
        private readonly IBufferProxy materialBuffer;

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

        public PhongMaterialVariables(IConstantBufferPool cbpool)
        {
            materialBuffer = cbpool.Register(new ConstantBufferDescription(DefaultBufferNames.MaterialCB, MaterialStruct.SizeInBytes));
            Device = cbpool.Device;
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
                CreateTextureView((sender as IPhongMaterial).DiffuseAlphaMap, ref this.texAlphaMapView);
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
                CreateTextureView(material.DiffuseAlphaMap, ref this.texAlphaMapView);
            }
            else
            {
                RemoveAndDispose(ref this.texDiffuseMapView);
                RemoveAndDispose(ref this.texNormalMapView);
                RemoveAndDispose(ref this.texDisplacementMapView);
                RemoveAndDispose(ref this.texAlphaMapView);
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
                HasDiffuseMap = RenderDiffuseMap && texDiffuseMapView != null ? 1 : 0,
                HasDiffuseAlphaMap = RenderDiffuseAlphaMap && texAlphaMapView != null ? 1 : 0,
                HasNormalMap = RenderNormalMap && texNormalMapView != null ? 1 : 0,
                HasDisplacementMap = RenderDisplacementMap && texDisplacementMapView != null ? 1 : 0,
                HasShadowMap = HasShadowMap ? 1 : 0,
                DisplacementMapScaleMask = material.DisplacementMapScaleMask,
                HasCubeMap = 0               
            };
        }
        /// <summary>
        /// <see cref="IEffectMaterialVariables.UpdateMaterialConstantBuffer(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool UpdateMaterialConstantBuffer(DeviceContext context)
        {
            if (material == null)
            {
                return false;
            }
            OnUpdateMaterialConstantBuffer(context);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnUpdateMaterialConstantBuffer(DeviceContext context)
        {
            if (needUpdate)
            {
                AssignVariables();                
                needUpdate = false;
            }
            materialBuffer.UploadDataToBuffer(context, ref materialStruct);
        }
        /// <summary>
        /// <see cref="IEffectMaterialVariables.BindMaterialTextures(DeviceContext, IShader)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shader"></param>
        /// <returns></returns>
        public bool BindMaterialTextures(DeviceContext context, IShader shader)
        {
            if (material == null)
            {
                return false;
            }
            OnBindMaterialTextures(context, shader);
            return true;
        }
        /// <summary>
        /// <see cref="IEffectMaterialVariables.BindMaterialTextures(DeviceContext, IEnumerable{IShader})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shader"></param>
        /// <returns></returns>
        public bool BindMaterialTextures(DeviceContext context, IEnumerable<IShader> shader)
        {
            if (material == null)
            {
                return false;
            }
            foreach (var s in shader)
            {
                OnBindMaterialTextures(context, s);
            }
            return true;
        }
        /// <summary>
        /// Actual bindings
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shader"></param>
        protected virtual void OnBindMaterialTextures(DeviceContext context, IShader shader)
        {
            context.AttachShaderResources(shader.ShaderType, shader.TryGetTextureIndex(TextureDiffuseName), texDiffuseMapView);
            context.AttachShaderResources(shader.ShaderType, shader.TryGetTextureIndex(TextureAlphaName), texAlphaMapView);
            context.AttachShaderResources(shader.ShaderType, shader.TryGetTextureIndex(TextureNormalName), texNormalMapView);
            context.AttachShaderResources(shader.ShaderType, shader.TryGetTextureIndex(TextureDisplacementName), texDisplacementMapView);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void Dispose(bool disposeManagedResources)
        {
            this.material = null;
            OnInvalidateRenderer = null;
            base.Dispose(disposeManagedResources);
        }
    }
}
