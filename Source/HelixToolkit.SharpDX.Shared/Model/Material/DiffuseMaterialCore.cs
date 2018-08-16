/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Numerics;
using HelixToolkit.Mathematics;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Core;
    using Render;
    using ShaderManager;
    using Shaders;
    using Utilities;

    public sealed class DiffuseMaterialCore : PhongMaterialCore
    {
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new DiffuseMaterialVariables(DefaultPassNames.Diffuse, manager, technique, this);
        }
    }

    public sealed class ViewCubeMaterialCore : PhongMaterialCore
    {
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new DiffuseMaterialVariables(DefaultPassNames.ViewCube, manager, technique, this);
        }
    }

    public sealed class DiffuseMaterialVariables : MaterialVariable
    {
        private const int NUMTEXTURES = 1;
        private const int NUMSAMPLERS = 1;
        private const int DiffuseIdx = 0;

        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private readonly ShaderResourceViewProxy[] TextureResources = new ShaderResourceViewProxy[NUMTEXTURES];
        private readonly SamplerStateProxy[] SamplerResources = new SamplerStateProxy[NUMSAMPLERS];

        private int texDiffuseSlot;
        private int samplerDiffuseSlot, samplerShadowSlot;
        private uint textureIndex = 0;

        private bool HasTextures
        {
            get
            {
                return textureIndex != 0;
            }
        }

        public ShaderPass MaterialPass { get; private set; } = ShaderPass.NullPass;
        public ShaderPass TransparentPass { private set; get; } = ShaderPass.NullPass;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderDiffuseTexName { set; get; } = DefaultBufferNames.DiffuseMapTB;

        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerDiffuseTexName { set; get; } = DefaultSamplerStateNames.DiffuseMapSampler;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderSamplerShadowMapName { set; get; } = DefaultSamplerStateNames.ShadowMapSampler;

        private string defaultShaderPassName = DefaultPassNames.Default;
        public override string DefaultShaderPassName
        {
            set
            {
                if (!fixedPassName && SetAffectsRender(ref defaultShaderPassName, value))
                {
                    MaterialPass = Technique[value];
                    UpdateMappings(MaterialPass);
                }
            }
            get
            {
                return defaultShaderPassName;
            }
        }

        private string transparentPassName = DefaultPassNames.DiffuseOIT;
        /// <summary>
        /// Gets or sets the name of the mesh transparent pass.
        /// </summary>
        /// <value>
        /// The name of the transparent pass.
        /// </value>
        public string TransparentPassName
        {
            set
            {
                if (!fixedPassName && Set(ref transparentPassName, value))
                {
                    TransparentPass = Technique[value];
                }
            }
            get
            {
                return transparentPassName;
            }
        }
        private readonly PhongMaterialCore material;
        private readonly bool fixedPassName = false;
        private PhongMaterialStruct materialStruct = new PhongMaterialStruct() { UVTransformR1 = new Vector4(1, 0, 0, 0), UVTransformR2 = new Vector4(0, 1, 0, 0) };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="technique"></param>
        /// <param name="material"></param>
        private DiffuseMaterialVariables(IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore material)
            : base(manager, technique, DefaultMeshConstantBufferDesc)
        {
            this.material = material;
            material.PropertyChanged += Material_OnMaterialPropertyChanged;
            texDiffuseSlot = -1;
            samplerDiffuseSlot = samplerShadowSlot = -1;
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            MaterialPass = technique[DefaultShaderPassName];
            TransparentPass = technique[TransparentPassName];
            UpdateMappings(MaterialPass);
            CreateTextureViews();
            CreateSamplers();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureSharedPhongMaterialVariables"/> class. This construct will be using the PassName pass into constructor only.
        /// </summary>
        /// <param name="passName">Name of the pass.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="technique"></param>
        /// <param name="material">The material.</param>
        public DiffuseMaterialVariables(string passName, IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore material)
            : this(manager, technique, material)
        {
            DefaultShaderPassName = passName;
            fixedPassName = true;
        }

        private void Material_OnMaterialPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }
            if (e.PropertyName.Equals(nameof(PhongMaterialCore.DiffuseMap)))
            {
                CreateTextureView((sender as PhongMaterialCore).DiffuseMap, DiffuseIdx);
            }
            else if (e.PropertyName.Equals(nameof(PhongMaterialCore.DiffuseMapSampler)))
            {
                RemoveAndDispose(ref SamplerResources[DiffuseIdx]);
                SamplerResources[DiffuseIdx] = Collect(statePoolManager.Register((sender as PhongMaterialCore).DiffuseMapSampler));
            }
            NotifyUpdateNeeded();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateTextureView(System.IO.Stream stream, int index)
        {
            RemoveAndDispose(ref TextureResources[index]);
            TextureResources[index] = stream == null ? null : Collect(textureManager.Register(stream));
            if (TextureResources[index] != null)
            {
                textureIndex |= 1u << index;
            }
            else
            {
                textureIndex &= ~(1u << index);
            }
        }

        private void CreateTextureViews()
        {
            if (material != null)
            {
                CreateTextureView(material.DiffuseMap, DiffuseIdx);
            }
            else
            {
                for (int i = 0; i < NUMTEXTURES; ++i)
                {
                    RemoveAndDispose(ref TextureResources[i]);
                }
                textureIndex = 0;
            }
        }

        private void CreateSamplers()
        {
            RemoveAndDispose(ref SamplerResources[DiffuseIdx]);
            if (material != null)
            {
                SamplerResources[DiffuseIdx] = Collect(statePoolManager.Register(material.DiffuseMapSampler));
            }
        }

        protected override void UpdateInternalVariables(DeviceContextProxy context)
        {
            if (NeedUpdate)
            {
                materialStruct = new PhongMaterialStruct
                {
                    Ambient = material.AmbientColor,
                    Diffuse = material.DiffuseColor,
                    Emissive = material.EmissiveColor,
                    Reflect = material.ReflectiveColor,
                    Specular = material.SpecularColor,
                    Shininess = material.SpecularShininess,
                    HasDiffuseMap = material.RenderDiffuseMap && TextureResources[DiffuseIdx] != null ? 1 : 0,
                    HasDiffuseAlphaMap = 0,
                    HasNormalMap = 0,
                    HasDisplacementMap = 0,
                    DisplacementMapScaleMask = material.DisplacementMapScaleMask,
                    RenderShadowMap = 0,
                    HasCubeMap = 0,
                    UVTransformR1 = material.UVTransform.Column1(),
                    UVTransformR2 = material.UVTransform.Column2()
                };
                NeedUpdate = false;
            }
        }

        protected override void WriteMaterialDataToConstantBuffer(global::SharpDX.DataStream cbStream)
        {
            cbStream.Write(materialStruct);
        }

        protected override bool OnBindMaterialTextures(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            if (HasTextures)
            {
                OnBindMaterialTextures(deviceContext, shaderPass.PixelShader);
            }
            return true;
        }

        /// <summary>
        /// Actual bindings
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shader"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnBindMaterialTextures(DeviceContextProxy context, PixelShader shader)
        {
            if (shader.IsNULL)
            {
                return;
            }
            int idx = shader.ShaderStageIndex;
            shader.BindTexture(context, texDiffuseSlot, TextureResources[DiffuseIdx]);
            shader.BindSampler(context, samplerDiffuseSlot, SamplerResources[DiffuseIdx]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateMappings(ShaderPass shaderPass)
        {
            texDiffuseSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
            samplerDiffuseSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerDiffuseTexName);
            samplerShadowSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderSamplerShadowMapName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                material.PropertyChanged -= Material_OnMaterialPropertyChanged;
                for (int i = 0; i < NUMTEXTURES; ++i)
                {
                    TextureResources[i] = null;
                }
                for (int i = 0; i < NUMSAMPLERS; ++i)
                {
                    SamplerResources[i] = null;
                }
            }

            base.OnDispose(disposeManagedResources);
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return renderType == RenderType.Transparent && context.IsOITPass ? TransparentPass : MaterialPass;
        }
    }
}
