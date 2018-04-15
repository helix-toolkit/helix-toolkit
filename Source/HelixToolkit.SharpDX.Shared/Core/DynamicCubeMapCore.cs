﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define TEST
using System;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Threading.Tasks;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Render;
    using Utilities;


    public class DynamicCubeMapCore : RenderCoreBase<GlobalTransformStruct>
    {
        public HashSet<Guid> IgnoredGuid { get; } = new HashSet<Guid>();
        private ShaderResourceViewProxy cubeMap;
        public ShaderResourceViewProxy CubeMap
        {
            get
            {
                return cubeMap;
            }
        }

        private ShaderResourceViewProxy cubeDSV;
        // The RTVs, one for each face of cubemap
        private RenderTargetView[] cubeRTVs = new RenderTargetView[6];
        // The DSVs, one for each face of cubemap
        private DepthStencilView[] cubeDSVs = new DepthStencilView[6];

        public int FaceSize
        {
            set; get;
        } = 256;

        private string defaultPassName = DefaultPassNames.Default;
        /// <summary>
        /// Name of the default pass inside a technique.
        /// <para>Default: <see cref="DefaultPassNames.Default"/></para>
        /// </summary>
        public string DefaultShaderPassName
        {
            set
            {
                if (Set(ref defaultPassName, value) && IsAttached)
                {
                    DefaultShaderPass = EffectTechnique[value];
                }
            }
            get
            {
                return defaultPassName;
            }
        }

        private IShaderPass defaultShaderPass = NullShaderPass.NullPass;
        /// <summary>
        /// 
        /// </summary>
        protected IShaderPass DefaultShaderPass
        {
            private set
            {
                if (Set(ref defaultShaderPass, value))
                {
                    cubeTextureSlot = value.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(ShaderCubeTextureName);
                    textureSamplerSlot = value.GetShader(ShaderStage.Pixel).SamplerMapping.TryGetBindSlot(ShaderCubeTextureSamplerName);
                    InvalidateRenderer();
                }
            }
            get
            {
                return defaultShaderPass;
            }
        }

        private SamplerStateDescription samplerDescription = DefaultSamplers.CubeSampler;
        /// <summary>
        /// Gets or sets the sampler description.
        /// </summary>
        /// <value>
        /// The sampler description.
        /// </value>
        public SamplerStateDescription SamplerDescription
        {
            set
            {
                if (SetAffectsRender(ref samplerDescription, value) && IsAttached)
                {
                    RemoveAndDispose(ref textureSampler);
                    textureSampler = Collect(EffectTechnique.EffectsManager.StateManager.Register(value));
                }
            }
            get
            {
                return samplerDescription;
            }
        }

        /// <summary>
        /// Gets or sets the name of the shader cube texture.
        /// </summary>
        /// <value>
        /// The name of the shader cube texture.
        /// </value>
        public string ShaderCubeTextureName { set; get; } = DefaultBufferNames.CubeMapTB;
        /// <summary>
        /// Gets or sets the name of the shader cube texture sampler.
        /// </summary>
        /// <value>
        /// The name of the shader cube texture sampler.
        /// </value>
        public string ShaderCubeTextureSamplerName { set; get; } = DefaultSamplerStateNames.CubeMapSampler;

        private int cubeTextureSlot;
        private int textureSamplerSlot;
        private SamplerState textureSampler;

        private DeviceContextPool contextPool;

        private CubeFaceCamerasStruct cubeFaceCameras = new CubeFaceCamerasStruct() { Cameras = new CubeFaceCamera[6] };

        private Vector3[] targets = new Vector3[6];
        private Vector3[] upVectors = new Vector3[6] { Vector3.UnitY, Vector3.UnitY, -Vector3.UnitZ, +Vector3.UnitZ, Vector3.UnitY, Vector3.UnitY };

        // Create the cube map TextureCube (array of 6 textures)
        private Texture2DDescription textureDesc = new Texture2DDescription()
        {
            Format = Format.R8G8B8A8_UNorm,
            ArraySize = 6, // 6-sides of the cube
            BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
            OptionFlags = ResourceOptionFlags.GenerateMipMaps | ResourceOptionFlags.TextureCube,
            SampleDescription = new SampleDescription(1, 0),
            MipLevels = 0,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
        };

        private Texture2DDescription dsvTextureDesc = new Texture2DDescription()
        {
            Format = Format.R32_Float,
            BindFlags = BindFlags.DepthStencil,
            Usage = ResourceUsage.Default,
            SampleDescription = new SampleDescription(1, 0),
            CpuAccessFlags = CpuAccessFlags.None,
            MipLevels = 1,
            OptionFlags = ResourceOptionFlags.TextureCube,
            ArraySize = 6
        };

        private Viewport viewport;


        public DynamicCubeMapCore() : base(RenderType.Opaque)
        {
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes);
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                DefaultShaderPass = technique[DefaultShaderPassName];
                textureDesc.Width = textureDesc.Height = dsvTextureDesc.Width = dsvTextureDesc.Height = FaceSize;
                cubeMap = Collect(new ShaderResourceViewProxy(Device, textureDesc));

                var srvDesc = new ShaderResourceViewDescription()
                {
                    Format = textureDesc.Format,
                    Dimension = global::SharpDX.Direct3D.ShaderResourceViewDimension.TextureCube,
                    TextureCube = new ShaderResourceViewDescription.TextureCubeResource() { MostDetailedMip = 0, MipLevels = -1 }
                };
                cubeMap.CreateView(srvDesc);

                var rtsDesc = new RenderTargetViewDescription()
                {
                    Format = textureDesc.Format,
                    Dimension = RenderTargetViewDimension.Texture2DArray,
                    Texture2DArray = new RenderTargetViewDescription.Texture2DArrayResource() { MipSlice = 0, FirstArraySlice = 0, ArraySize = 1 }
                };

                for (int i = 0; i < 6; ++i)
                {
                    rtsDesc.Texture2DArray.FirstArraySlice = i;
                    cubeRTVs[i] = Collect(new RenderTargetView(Device, CubeMap.Resource, rtsDesc));
                }

                cubeDSV = Collect(new ShaderResourceViewProxy(Device, dsvTextureDesc));
                var dsvDesc = new DepthStencilViewDescription()
                {
                    Format = dsvTextureDesc.Format,
                    Dimension = DepthStencilViewDimension.Texture2DArray,
                    Flags = DepthStencilViewFlags.None,
                    Texture2DArray = new DepthStencilViewDescription.Texture2DArrayResource() { MipSlice = 0, FirstArraySlice = 0, ArraySize = 1 }
                };

                for (int i = 0; i < 6; ++i)
                {
                    dsvDesc.Texture2DArray.FirstArraySlice = i;
                    cubeDSVs[i] = Collect(new DepthStencilView(Device, cubeDSV.Resource, dsvDesc));
                }

                viewport = new Viewport(0, 0, FaceSize, FaceSize);

                contextPool = Collect(new DeviceContextPool(Device));
                textureSampler = Collect(technique.EffectsManager.StateManager.Register(SamplerDescription));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            contextPool = null;
            cubeMap = null;
            cubeDSV = null;
            for (int i = 0; i < 6; ++i)
            {
                cubeRTVs[i] = null;
                cubeDSVs[i] = null;
            }           
            base.OnDetach();
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            var commands = new CommandList[6];
            Parallel.For(0, 6, (index) => 
            {
                var ctx = contextPool.Get();
                ctx.DeviceContext.ClearRenderTargetView(cubeRTVs[index], Color.Black);
                ctx.DeviceContext.ClearDepthStencilView(cubeDSVs[index], DepthStencilClearFlags.Depth, 1, 0);
                ctx.DeviceContext.OutputMerger.SetRenderTargets(cubeDSVs[index], cubeRTVs[index]);
                ctx.DeviceContext.Rasterizer.SetViewport(viewport);
                ctx.DeviceContext.Rasterizer.SetScissorRectangle(0, 0, FaceSize, FaceSize);
                var transforms = new GlobalTransformStruct();
                transforms.Projection = cubeFaceCameras.Cameras[index].Projection;
                transforms.View = cubeFaceCameras.Cameras[index].View;
                transforms.Viewport = new Vector4(0, 0, FaceSize, FaceSize);
                transforms.ViewProjection = transforms.View * transforms.ViewProjection;
                ModelConstBuffer.UploadDataToBuffer(ctx, ref transforms);

                for(int i =0; i< context.RenderHost.PerFrameGeneralNodes.Count; ++i)
                {
                    if (!IgnoredGuid.Contains(context.RenderHost.PerFrameGeneralNodes[i].GUID))
                    {
                        context.RenderHost.PerFrameGeneralNodes[i].Render(context, ctx);
                    }
                }
                commands[index] = ctx.DeviceContext.FinishCommandList(true);
                ctx.DeviceContext.OutputMerger.ResetTargets();
                contextPool.Put(ctx);
            });
           
            for(int i=0; i < commands.Length; ++i)
            {
                Device.ImmediateContext.ExecuteCommandList(commands[i], true);
                commands[i].Dispose();
            }
            deviceContext.DeviceContext.GenerateMips(CubeMap);
            deviceContext.SetRenderTargets(context.RenderHost.RenderBuffer);
            deviceContext.DeviceContext.PixelShader.SetShaderResource(cubeTextureSlot, CubeMap);
            deviceContext.DeviceContext.PixelShader.SetSampler(textureSamplerSlot, textureSampler);
        }

        protected override void OnUpdatePerModelStruct(ref GlobalTransformStruct model, IRenderContext context)
        {
            var camPos = context.Camera.Position;
            targets[0] = camPos + Vector3.UnitX;
            targets[1] = camPos - Vector3.UnitX;
            targets[2] = camPos + Vector3.UnitY;
            targets[3] = camPos - Vector3.UnitY;
            targets[4] = camPos + Vector3.UnitZ;
            targets[5] = camPos - Vector3.UnitZ;

            for (int i = 0; i < 6; ++i)
            {
                cubeFaceCameras.Cameras[i].View = Matrix.LookAtRH(camPos, targets[i], upVectors[i]);
                cubeFaceCameras.Cameras[i].Projection = Matrix.PerspectiveFovRH((float)Math.PI * 0.5f, 1, 0.1f, 100f);
            }
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
            //ModelConstBuffer.UploadDataToBuffer(context, modelStruct.Cameras, modelStruct.Cameras.Length);
        }
    }
}