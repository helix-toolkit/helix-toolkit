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

        private readonly Dictionary<string, ShaderResouceViewProxy> ShaderResourceDict = new Dictionary<string, ShaderResouceViewProxy>();
        private readonly Dictionary<string, SamplerProxy> SamplerDict = new Dictionary<string, SamplerProxy>();

        public string ShaderAlphaTexName { set; get; } = DefaultBufferNames.AlphaMapTB;
        public string ShaderDiffuseTexName { set; get; } = DefaultBufferNames.DiffuseMapTB;
        public string ShaderNormalTexName { set; get; } = DefaultBufferNames.NormalMapTB;
        public string ShaderDisplaceTexName { set; get; } = DefaultBufferNames.DisplacementMapTB;
        public string ShaderSamplerAlphaTexName { set; get; } = DefaultSamplerStateNames.AlphaMapSampler;
        public string ShaderSamplerDiffuseTexName { set; get; } = DefaultSamplerStateNames.DiffuseMapSampler;
        public string ShaderSamplerNormalTexName { set; get; } = DefaultSamplerStateNames.NormalMapSampler;
        public string ShaderSamplerDisplaceTexName { set; get; } = DefaultSamplerStateNames.DisplacementMapSampler;
        public string ShaderSamplerShadowMapName { set; get; } = DefaultSamplerStateNames.ShadowMapSampler;

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
                    CreateSamplers();
                }
            }
            get
            {
                return material;
            }
        }

        public PhongMaterialVariables(IEffectsManager manager)
        {
            materialBuffer = manager.ConstantBufferPool.Register(new ConstantBufferDescription(DefaultBufferNames.MaterialCB, MaterialStruct.SizeInBytes));
            Device = manager.Device;
            ShaderResourceDict.Add(ShaderDiffuseTexName, Collect(new ShaderResouceViewProxy(Device)));
            ShaderResourceDict.Add(ShaderNormalTexName, Collect(new ShaderResouceViewProxy(Device)));
            ShaderResourceDict.Add(ShaderDisplaceTexName, Collect(new ShaderResouceViewProxy(Device)));
            ShaderResourceDict.Add(ShaderAlphaTexName, Collect(new ShaderResouceViewProxy(Device)));
            SamplerDict.Add(ShaderSamplerDiffuseTexName, new SamplerProxy(manager));
            SamplerDict.Add(ShaderSamplerNormalTexName, new SamplerProxy(manager));
            SamplerDict.Add(ShaderSamplerDisplaceTexName, new SamplerProxy(manager));
            SamplerDict.Add(ShaderSamplerAlphaTexName, new SamplerProxy(manager));
            SamplerDict.Add(ShaderSamplerShadowMapName, new SamplerProxy(manager));
            CreateTextureViews();
            CreateSamplers();
        }               

        private void Material_OnMaterialPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            needUpdate = true;
            if (e.PropertyName.Equals(nameof(IPhongMaterial.DiffuseMap)))
            {
                CreateTextureView((sender as IPhongMaterial).DiffuseMap, ShaderResourceDict[ShaderDiffuseTexName]);
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.NormalMap)))
            {
                CreateTextureView((sender as IPhongMaterial).NormalMap, ShaderResourceDict[ShaderNormalTexName]);
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DisplacementMap)))
            {
                CreateTextureView((sender as IPhongMaterial).DisplacementMap, ShaderResourceDict[ShaderDisplaceTexName]);
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DiffuseAlphaMap)))
            {
                CreateTextureView((sender as IPhongMaterial).DiffuseAlphaMap, ShaderResourceDict[ShaderAlphaTexName]);
            }           
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DiffuseMapSampler)))
            {
                SamplerDict[ShaderSamplerDiffuseTexName].Description = (sender as IPhongMaterial).DiffuseMapSampler;
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DiffuseAlphaMapSampler)))
            {
                SamplerDict[ShaderSamplerAlphaTexName].Description = (sender as IPhongMaterial).DiffuseAlphaMapSampler;
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.DisplacementMapSampler)))
            {
                SamplerDict[ShaderSamplerDisplaceTexName].Description = (sender as IPhongMaterial).DisplacementMapSampler;
            }
            else if (e.PropertyName.Equals(nameof(IPhongMaterial.NormalMapSampler)))
            {
                SamplerDict[ShaderSamplerNormalTexName].Description = (sender as IPhongMaterial).NormalMapSampler;
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
                CreateTextureView(material.DiffuseMap, ShaderResourceDict[ShaderDiffuseTexName]);
                CreateTextureView(material.NormalMap, ShaderResourceDict[ShaderNormalTexName]);
                CreateTextureView(material.DisplacementMap, ShaderResourceDict[ShaderDisplaceTexName]);
                CreateTextureView(material.DiffuseAlphaMap, ShaderResourceDict[ShaderAlphaTexName]);
            }
            else
            {
                foreach(var item in ShaderResourceDict.Values)
                {
                    item.CreateView(null);
                }
            }
        }

        private void CreateSamplers()
        {
            if (material != null)
            {
                SamplerDict[ShaderSamplerDiffuseTexName].Description = material.DiffuseMapSampler;
                SamplerDict[ShaderSamplerNormalTexName].Description = material.NormalMapSampler;
                SamplerDict[ShaderSamplerAlphaTexName].Description = material.DiffuseAlphaMapSampler;
                SamplerDict[ShaderSamplerDisplaceTexName].Description = material.DisplacementMapSampler;
                SamplerDict[ShaderSamplerShadowMapName].Description = DefaultSamplers.ShadowSampler;
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
                HasDiffuseMap = RenderDiffuseMap && ShaderResourceDict[ShaderDiffuseTexName].TextureView != null ? 1 : 0,
                HasDiffuseAlphaMap = RenderDiffuseAlphaMap && ShaderResourceDict[ShaderAlphaTexName].TextureView != null ? 1 : 0,
                HasNormalMap = RenderNormalMap && ShaderResourceDict[ShaderNormalTexName].TextureView != null ? 1 : 0,
                HasDisplacementMap = RenderDisplacementMap && ShaderResourceDict[ShaderDisplaceTexName].TextureView != null ? 1 : 0,
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
            var flag = ShaderStage.Vertex | ShaderStage.Pixel | ShaderStage.Domain;
            foreach (var s in shader.Where(x=> !x.IsNULL && flag.HasFlag(x.ShaderType)))
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
            if(shader.IsNULL)
            {
                return;
            }
            foreach(var item in ShaderResourceDict)
            {
                shader.BindTexture(context, item.Key, item.Value);
            }
            foreach (var item in SamplerDict)
            {
                shader.BindSampler(context, item.Key, item.Value);
            }
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
