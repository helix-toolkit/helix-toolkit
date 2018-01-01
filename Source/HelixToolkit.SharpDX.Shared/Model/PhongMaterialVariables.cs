/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Linq;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Shaders;
    using System;
    using System.Collections.Generic;
    using Utilities;

    /// <summary>
    /// Default PhongMaterial Variables
    /// </summary>
    public sealed class PhongMaterialVariables : DisposeObject, IEffectMaterialVariables
    {
        /// <summary>
        /// <see cref="IEffectMaterialVariables.OnInvalidateRenderer"/> 
        /// </summary>
        public event System.EventHandler<bool> OnInvalidateRenderer;
        private IPhongMaterial material;
        /// <summary>
        ///
        /// </summary>
        public Device Device { private set; get; }

        private const int NUMTEXTURES = 4;
        private const int NUMSAMPLERS = 5;
        private const int DiffuseIdx = 0, AlphaIdx = 1, NormalIdx = 2, DisplaceIdx = 3,
            SamplerDiffuseIdx = 0, SamplerAlphaIdx = 1, SamplerNormalIdx = 2, SamplerDisplaceIdx = 3, SamplerShadowIdx = 4;

        private ShaderResouceViewProxy[] TextureResources = new ShaderResouceViewProxy[NUMTEXTURES];
        private int[,] TextureBindingMap = new int[Constants.NumShaderStages, NUMTEXTURES];

        private SamplerProxy[] SamplerResources = new SamplerProxy[NUMSAMPLERS];
        private int[,] SamplerBindingMap = new int[Constants.NumShaderStages, NUMSAMPLERS];

        private IShaderPass currentPass;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderAlphaTexName { set; get; } = DefaultBufferNames.AlphaMapTB;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderDiffuseTexName { set; get; } = DefaultBufferNames.DiffuseMapTB;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderNormalTexName { set; get; } = DefaultBufferNames.NormalMapTB;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderDisplaceTexName { set; get; } = DefaultBufferNames.DisplacementMapTB;

        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerAlphaTexName { set; get; } = DefaultSamplerStateNames.AlphaMapSampler;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerDiffuseTexName { set; get; } = DefaultSamplerStateNames.DiffuseMapSampler;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerNormalTexName { set; get; } = DefaultSamplerStateNames.NormalMapSampler;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerDisplaceTexName { set; get; } = DefaultSamplerStateNames.DisplacementMapSampler;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerShadowMapName { set; get; } = DefaultSamplerStateNames.ShadowMapSampler;

        private bool renderDiffuseMap = true;
        /// <summary>
        /// <see cref="IMaterialRenderCore.RenderDiffuseMap"/> 
        /// </summary>
        public bool RenderDiffuseMap
        {
            set
            {
                if (renderDiffuseMap == value)
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
        /// <summary>
        /// <see cref="IMaterialRenderCore.RenderDiffuseAlphaMap"/> 
        /// </summary>
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
        /// <summary>
        /// <see cref="IMaterialRenderCore.RenderNormalMap"/> 
        /// </summary>
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
        /// <summary>
        /// <see cref="IMaterialRenderCore.RenderDisplacementMap"/> 
        /// </summary>
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

        private bool renderShadowMap = false;
        /// <summary>
        /// <see cref="IMaterialRenderCore.RenderShadowMap"/> 
        /// </summary>
        public bool RenderShadowMap
        {
            set
            {
                if (renderShadowMap == value)
                { return; }
                renderShadowMap = value;
                needUpdate = true;
            }
            get
            {
                return renderDisplacementMap;
            }
        }

        private bool needUpdate = true;
        private MaterialStruct materialStruct;
        private readonly IConstantBufferProxy materialBuffer;
        /// <summary>
        /// <see cref="IMaterialRenderCore.Material"/> 
        /// </summary>
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
                    CreateSamplers();
                }
            }
            get
            {
                return material;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        public PhongMaterialVariables(IEffectsManager manager)
        {
            materialBuffer = manager.ConstantBufferPool.Register(new ConstantBufferDescription(DefaultBufferNames.MaterialCB, MaterialStruct.SizeInBytes));
            Device = manager.Device;
            TextureResources[DiffuseIdx] = Collect(new ShaderResouceViewProxy(Device));
            TextureResources[NormalIdx] = Collect(new ShaderResouceViewProxy(Device));
            TextureResources[DisplaceIdx] = Collect(new ShaderResouceViewProxy(Device));
            TextureResources[AlphaIdx] = Collect(new ShaderResouceViewProxy(Device));
            SamplerResources[SamplerDiffuseIdx] = Collect(new SamplerProxy(manager.StateManager));
            SamplerResources[SamplerNormalIdx] = Collect(new SamplerProxy(manager.StateManager));
            SamplerResources[SamplerDisplaceIdx] = Collect(new SamplerProxy(manager.StateManager));
            SamplerResources[SamplerAlphaIdx] = Collect(new SamplerProxy(manager.StateManager));
            SamplerResources[SamplerShadowIdx] = Collect(new SamplerProxy(manager.StateManager));
            CreateTextureViews();
            CreateSamplers();
        }

        private void Material_OnMaterialPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            needUpdate = true;
            if (e.PropertyName.Equals(nameof(IPhongMaterial.DiffuseMap)))
            {
                CreateTextureView((sender as IPhongMaterial).DiffuseMap, TextureResources[DiffuseIdx]);
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.NormalMap)))
            {
                CreateTextureView((sender as IPhongMaterial).NormalMap, TextureResources[NormalIdx]);
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DisplacementMap)))
            {
                CreateTextureView((sender as IPhongMaterial).DisplacementMap, TextureResources[DisplaceIdx]);
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DiffuseAlphaMap)))
            {
                CreateTextureView((sender as IPhongMaterial).DiffuseAlphaMap, TextureResources[AlphaIdx]);
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DiffuseMapSampler)))
            {
                SamplerResources[SamplerDiffuseIdx].Description = (sender as IPhongMaterial).DiffuseMapSampler;
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DiffuseAlphaMapSampler)))
            {
                SamplerResources[SamplerAlphaIdx].Description = (sender as IPhongMaterial).DiffuseAlphaMapSampler;
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DisplacementMapSampler)))
            {
                SamplerResources[SamplerDisplaceIdx].Description = (sender as IPhongMaterial).DisplacementMapSampler;
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.NormalMapSampler)))
            {
                SamplerResources[SamplerNormalIdx].Description = (sender as IPhongMaterial).NormalMapSampler;
            }
            OnInvalidateRenderer?.Invoke(this, true);
        }

        private void CreateTextureView(System.IO.Stream stream, ShaderResouceViewProxy proxy)
        {
            proxy.CreateView(stream);
        }

        private void CreateTextureViews()
        {
            if (material != null)
            {
                CreateTextureView(material.DiffuseMap, TextureResources[DiffuseIdx]);
                CreateTextureView(material.NormalMap, TextureResources[NormalIdx]);
                CreateTextureView(material.DisplacementMap, TextureResources[DisplaceIdx]);
                CreateTextureView(material.DiffuseAlphaMap, TextureResources[AlphaIdx]);
            }
            else
            {
                foreach (var item in TextureResources)
                {
                    item.CreateView(null);
                }
            }
        }

        private void CreateSamplers()
        {
            if (material != null)
            {
                SamplerResources[SamplerDiffuseIdx].Description = material.DiffuseMapSampler;
                SamplerResources[SamplerNormalIdx].Description = material.NormalMapSampler;
                SamplerResources[SamplerAlphaIdx].Description = material.DiffuseAlphaMapSampler;
                SamplerResources[SamplerDisplaceIdx].Description = material.DisplacementMapSampler;
                SamplerResources[SamplerShadowIdx].Description = DefaultSamplers.ShadowSampler;
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
                HasDiffuseMap = RenderDiffuseMap && TextureResources[DiffuseIdx].TextureView != null ? 1 : 0,
                HasDiffuseAlphaMap = RenderDiffuseAlphaMap && TextureResources[AlphaIdx].TextureView != null ? 1 : 0,
                HasNormalMap = RenderNormalMap && TextureResources[NormalIdx].TextureView != null ? 1 : 0,
                HasDisplacementMap = RenderDisplacementMap && TextureResources[DisplaceIdx].TextureView != null ? 1 : 0,
                DisplacementMapScaleMask = material.DisplacementMapScaleMask,
                RenderShadowMap = RenderShadowMap ? 1 : 0,
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
        private void OnUpdateMaterialConstantBuffer(DeviceContext context)
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
        /// <see cref="IEffectMaterialVariables.BindMaterialTextures(DeviceContext, IShaderPass)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shaderPass"></param>
        /// <returns></returns>
        public bool BindMaterialTextures(DeviceContext context, IShaderPass shaderPass)
        {
            if (material == null)
            {
                return false;
            }
            UpdateMappings(shaderPass);
            foreach (var s in shaderPass.Shaders.Where(x => !x.IsNULL && Constants.CanBindTextureStages.HasFlag(x.ShaderType)))
            {
                OnBindMaterialTextures(context, s);
            }
            return true;
        }

        private void UpdateMappings(IShaderPass shaderPass)
        {
            if (currentPass == shaderPass)
            {
                return;
            }
            currentPass = shaderPass;

            foreach (var shader in shaderPass.Shaders.Where(x => !x.IsNULL && Constants.CanBindTextureStages.HasFlag(x.ShaderType)))
            {
                int idx = Constants.Convert(shader.ShaderType);
                TextureBindingMap[idx, DiffuseIdx] = shader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
                TextureBindingMap[idx, AlphaIdx] = shader.ShaderResourceViewMapping.TryGetBindSlot(ShaderAlphaTexName);
                TextureBindingMap[idx, NormalIdx] = shader.ShaderResourceViewMapping.TryGetBindSlot(ShaderNormalTexName);
                TextureBindingMap[idx, DisplaceIdx] = shader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDisplaceTexName);

                SamplerBindingMap[idx, SamplerDiffuseIdx] = shader.SamplerMapping.TryGetBindSlot(ShaderSamplerDiffuseTexName);
                SamplerBindingMap[idx, SamplerAlphaIdx] = shader.SamplerMapping.TryGetBindSlot(ShaderSamplerAlphaTexName);
                SamplerBindingMap[idx, SamplerNormalIdx] = shader.SamplerMapping.TryGetBindSlot(ShaderSamplerNormalTexName);
                SamplerBindingMap[idx, SamplerDisplaceIdx] = shader.SamplerMapping.TryGetBindSlot(ShaderSamplerDisplaceTexName);
                SamplerBindingMap[idx, SamplerShadowIdx] = shader.SamplerMapping.TryGetBindSlot(ShaderSamplerShadowMapName);
            }
        }


        /// <summary>
        /// Actual bindings
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shader"></param>
        private void OnBindMaterialTextures(DeviceContext context, IShader shader)
        {
            if (shader.IsNULL)
            {
                return;
            }
            int idx = Constants.Convert(shader.ShaderType);
            for (int i = 0; i < NUMTEXTURES; ++i)
            {
                if (TextureResources[i].TextureView == null) { continue; }
                shader.BindTexture(context, TextureBindingMap[idx, i], TextureResources[i]);
            }
            for (int i = 0; i < NUMSAMPLERS; ++i)
            {
                shader.BindSampler(context, SamplerBindingMap[idx, i], SamplerResources[i]);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void Dispose(bool disposeManagedResources)
        {
            this.material = null;
            TextureResources = null;
            SamplerResources = null;
            TextureBindingMap = null;
            SamplerBindingMap = null;
            OnInvalidateRenderer = null;
            base.Dispose(disposeManagedResources);
        }
    }
}
