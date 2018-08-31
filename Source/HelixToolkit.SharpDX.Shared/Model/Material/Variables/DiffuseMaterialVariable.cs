/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Runtime.CompilerServices;
using System.ComponentModel;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Render;
    using ShaderManager;
    using Shaders;   
    using Utilities;

    /// <summary>
    /// 
    /// </summary>
    public sealed class DiffuseMaterialVariables : MaterialVariable
    {
        private const int NUMTEXTURES = 1;
        private const int NUMSAMPLERS = 1;
        private const int DiffuseIdx = 0;

        private readonly ITextureResourceManager textureManager;
        private readonly IStatePoolManager statePoolManager;
        private ShaderResourceViewProxy TextureResource;
        private SamplerStateProxy SamplerResource;

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
        public ShaderPass ShadowPass { private set; get; } = ShaderPass.NullPass;
        public ShaderPass WireframePass { private set; get; } = ShaderPass.NullPass;
        public ShaderPass WireframeOITPass { private set; get; } = ShaderPass.NullPass;
        /// <summary>
        /// 
        /// </summary>
        public string ShaderDiffuseTexName { get; } = DefaultBufferNames.DiffuseMapTB;

        /// <summary>
        /// 
        /// </summary>
        public string SamplerDiffuseTexName { get; } = DefaultSamplerStateNames.SurfaceSampler;
        /// <summary>
        /// 
        /// </summary>
        public string SamplerShadowMapName {  get; } = DefaultSamplerStateNames.ShadowMapSampler;


        public string DefaultShaderPassName
        {
            get;
        } = DefaultPassNames.Diffuse;


        public string ShadowPassName
        {
            get;
        } = DefaultPassNames.ShadowPass;

        public string WireframePassName
        {
            get;
        } = DefaultPassNames.Wireframe;

        public string TransparentPassName
        {
            get;
        } = DefaultPassNames.DiffuseOIT;

        public string WireframeOITPassName
        {
            get;
        } = DefaultPassNames.WireframeOITPass;

        private readonly PhongMaterialCore material;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="technique"></param>
        /// <param name="materialCore"></param>
        private DiffuseMaterialVariables(IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore materialCore)
            : base(manager, technique, DefaultMeshConstantBufferDesc, materialCore)
        {
            this.material = materialCore;
            texDiffuseSlot = -1;
            samplerDiffuseSlot = samplerShadowSlot = -1;
            textureManager = manager.MaterialTextureManager;
            statePoolManager = manager.StateManager;
            MaterialPass = technique[DefaultShaderPassName];
            TransparentPass = technique[TransparentPassName];
            ShadowPass = technique[ShadowPassName];
            WireframePass = technique[WireframePassName];
            WireframeOITPass = technique[WireframeOITPassName];
            UpdateMappings(MaterialPass);
            CreateTextureViews();
            CreateSamplers();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PhongMaterialVariables"/> class. This construct will be using the PassName pass into constructor only.
        /// </summary>
        /// <param name="passName">Name of the pass.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="technique"></param>
        /// <param name="material">The material.</param>
        public DiffuseMaterialVariables(string passName, IEffectsManager manager, IRenderTechnique technique, PhongMaterialCore material)
            : this(manager, technique, material)
        {
            DefaultShaderPassName = passName;
            MaterialPass = technique[DefaultShaderPassName];
        }

        protected override void OnInitialPropertyBindings()
        {
            base.OnInitialPropertyBindings();
            AddPropertyBinding(nameof(PhongMaterialCore.DiffuseColor), () => { WriteValue(PhongMaterialStruct.DiffuseStr, material.DiffuseColor); });
            AddPropertyBinding(nameof(PhongMaterialCore.UVTransform), () => 
            {
                WriteValue(PhongMaterialStruct.UVTransformR1Str, material.UVTransform.Column1);
                WriteValue(PhongMaterialStruct.UVTransformR2Str, material.UVTransform.Column2);
            });
            AddPropertyBinding(nameof(PhongMaterialCore.DiffuseMap), () =>
            {
                CreateTextureView(material.DiffuseMap, DiffuseIdx);
                WriteValue(PhongMaterialStruct.HasDiffuseMapStr, material.RenderDiffuseMap && TextureResource != null ? 1 : 0);
            });
            AddPropertyBinding(nameof(PhongMaterialCore.DiffuseMapSampler), () =>
            {
                RemoveAndDispose(ref SamplerResource);
                SamplerResource = Collect(statePoolManager.Register(material.DiffuseMapSampler));
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateTextureView(System.IO.Stream stream, int index)
        {
            RemoveAndDispose(ref TextureResource);
            TextureResource = stream == null ? null : Collect(textureManager.Register(stream));
            if (TextureResource != null)
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
                RemoveAndDispose(ref TextureResource);
                textureIndex = 0;
            }
        }

        private void CreateSamplers()
        {
            RemoveAndDispose(ref SamplerResource);
            if (material != null)
            {
                SamplerResource = Collect(statePoolManager.Register(material.DiffuseMapSampler));
            }
        }

        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
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
            shader.BindTexture(context, texDiffuseSlot, TextureResource);
            shader.BindSampler(context, samplerDiffuseSlot, SamplerResource);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateMappings(ShaderPass shaderPass)
        {
            texDiffuseSlot = shaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderDiffuseTexName);
            samplerDiffuseSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(SamplerDiffuseTexName);
            samplerShadowSlot = shaderPass.PixelShader.SamplerMapping.TryGetBindSlot(SamplerShadowMapName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                TextureResource = null;
                SamplerResource = null;
            }

            base.OnDispose(disposeManagedResources);
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return renderType == RenderType.Transparent && context.IsOITPass ? TransparentPass : MaterialPass;
        }

        public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
        {
            return ShadowPass;
        }

        public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
        {
            return renderType == RenderType.Transparent && context.IsOITPass ? WireframeOITPass : WireframePass;
        }

        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
            DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
        }
    }
}
