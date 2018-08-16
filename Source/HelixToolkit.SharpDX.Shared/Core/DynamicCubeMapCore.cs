/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

//#define TEST
using HelixToolkit.Mathematics;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if !NETFX_CORE

namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Utilities;
    using Components;
    /// <summary>
    ///
    /// </summary>
    public class DynamicCubeMapCore : RenderCoreBase<GlobalTransformStruct>, IDynamicReflector
    {
        #region
        private readonly Vector3[] targets = new Vector3[6];
        private readonly Vector3[] upVectors = new Vector3[6] { Vector3.UnitY, Vector3.UnitY, -Vector3.UnitZ, Vector3.UnitZ, Vector3.UnitY, Vector3.UnitY };
        private CubeFaceCamerasStruct cubeFaceCameras = new CubeFaceCamerasStruct() { Cameras = new CubeFaceCamera[6] };
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
            Format = Format.D16_UNorm,
            BindFlags = BindFlags.DepthStencil,
            Usage = ResourceUsage.Default,
            SampleDescription = new SampleDescription(1, 0),
            CpuAccessFlags = CpuAccessFlags.None,
            MipLevels = 1,
            OptionFlags = ResourceOptionFlags.TextureCube,
            ArraySize = 6
        };

        private Viewport viewport;
        private int cubeTextureSlot;
        private int textureSamplerSlot;
        private ShaderResourceViewProxy cubeDSV;
        // The RTVs, one for each face of cubemap
        private readonly RenderTargetView[] cubeRTVs = new RenderTargetView[6];
        // The DSVs, one for each face of cubemap
        private readonly DepthStencilView[] cubeDSVs = new DepthStencilView[6];
        private SamplerStateProxy textureSampler;
        private IDeviceContextPool contextPool;
        private readonly CommandList[] commands = new CommandList[6];
        private readonly ConstantBufferComponent modelCB;
        #endregion
        #region Properties

        public HashSet<Guid> IgnoredGuid { get; } = new HashSet<Guid>();
        private ShaderResourceViewProxy cubeMap;

        public ShaderResourceViewProxy CubeMap
        {
            get
            {
                return cubeMap;
            }
        }

        private bool enableReflector = true;
        /// <summary>
        /// Gets or sets a value indicating whether [enable reflector].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable reflector]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableReflector
        {
            set
            {
                SetAffectsCanRenderFlag(ref enableReflector, value);
            }
            get
            {
                return enableReflector;
            }
        }

        private int faceSize = 256;

        public int FaceSize
        {
            set
            {
                SetAffectsRender(ref faceSize, value);
            }
            get
            {
                return faceSize;
            }
        }

        private string defaultPassName = DefaultPassNames.Default;

        /// <summary>
        /// Name of the default pass inside a technique.
        /// <para>Default: <see cref="DefaultPassNames.Default"/></para>
        /// </summary>
        public string DefaultShaderPassName
        {
            set
            {
                if (SetAffectsRender(ref defaultPassName, value) && IsAttached)
                {
                    DefaultShaderPass = EffectTechnique[value];
                }
            }
            get
            {
                return defaultPassName;
            }
        }

        private ShaderPass defaultShaderPass = ShaderPass.NullPass;

        /// <summary>
        ///
        /// </summary>
        protected ShaderPass DefaultShaderPass
        {
            private set
            {
                if (SetAffectsRender(ref defaultShaderPass, value))
                {
                    cubeTextureSlot = value.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderCubeTextureName);
                    textureSamplerSlot = value.PixelShader.SamplerMapping.TryGetBindSlot(ShaderCubeTextureSamplerName);
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

        private bool isleftHanded = false;

        /// <summary>
        /// Gets or sets a value indicating whether this coordinate system is left handed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this coordinate system is left handed; otherwise, <c>false</c>.
        /// </value>
        public bool IsLeftHanded
        {
            set
            {
                SetAffectsRender(ref isleftHanded, value);
            }
            get
            {
                return isleftHanded;
            }
        }

        private float nearField = 0.1f;

        /// <summary>
        /// Gets or sets the near field of perspective.
        /// </summary>
        /// <value>
        /// The near field.
        /// </value>
        public float NearField
        {
            set
            {
                SetAffectsRender(ref nearField, value);
            }
            get
            {
                return nearField;
            }
        }

        private float farField = 100f;

        /// <summary>
        /// Gets or sets the far field of perspective.
        /// </summary>
        /// <value>
        /// The far field.
        /// </value>
        public float FarField
        {
            set
            {
                SetAffectsRender(ref farField, value);
            }
            get
            {
                return farField;
            }
        }

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        /// <value>
        /// The center.
        /// </value>
        public Vector3 Center
        {
            set; get;
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

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicCubeMapCore"/> class.
        /// </summary>
        public DynamicCubeMapCore() : base(RenderType.PreProc)
        {
            modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes)));
        }

        private bool CreateCubeMapResources()
        {
            if(textureDesc.Width == faceSize && cubeMap != null && !cubeMap.IsDisposed)
            {
                return false;
            }
            textureDesc.Width = textureDesc.Height = dsvTextureDesc.Width = dsvTextureDesc.Height = FaceSize;

            RemoveAndDispose(ref cubeMap);
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
                RemoveAndDispose(ref cubeRTVs[i]);
                rtsDesc.Texture2DArray.FirstArraySlice = i;
                cubeRTVs[i] = Collect(new RenderTargetView(Device, CubeMap.Resource, rtsDesc));
            }

            RemoveAndDispose(ref cubeDSV);
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
                RemoveAndDispose(ref cubeDSVs[i]);
                dsvDesc.Texture2DArray.FirstArraySlice = i;
                cubeDSVs[i] = Collect(new DepthStencilView(Device, cubeDSV.Resource, dsvDesc));
            }

            viewport = new Viewport(0, 0, FaceSize, FaceSize);
            return true;
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            DefaultShaderPass = technique[DefaultShaderPassName];
            contextPool = technique.EffectsManager.DeviceContextPool;
            textureSampler = Collect(technique.EffectsManager.StateManager.Register(SamplerDescription));
            CreateCubeMapResources();
            return true;
        }

        protected override void OnDetach()
        {
            textureSampler = null;
            contextPool = null;
            cubeMap = null;
            cubeDSV = null;
            textureDesc.Width = textureDesc.Height = dsvTextureDesc.Width = dsvTextureDesc.Height = 0;
            for (int i = 0; i < 6; ++i)
            {
                cubeRTVs[i] = null;
                cubeDSVs[i] = null;
            }
            base.OnDetach();
        }

        protected override bool OnUpdateCanRenderFlag()
        {
            return base.OnUpdateCanRenderFlag() && EnableReflector;
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (CreateCubeMapResources())
            {
                InvalidateRenderer();
                return; // Skip this frame if texture resized to reduce latency.
            }
            context.IsInvertCullMode = true;
#if TEST
            for (int index = 0; index < 6; ++index)
#else
            Parallel.For(0, 6, (index) =>
#endif
            {
                var ctx = contextPool.Get();
                ctx.ClearRenderTargetView(cubeRTVs[index], context.RenderHost.ClearColor);
                ctx.ClearDepthStencilView(cubeDSVs[index], DepthStencilClearFlags.Depth, 1, 0);
                ctx.SetRenderTarget(cubeDSVs[index], cubeRTVs[index]);
                ctx.SetViewport(ref viewport);
                ctx.SetScissorRectangle(0, 0, FaceSize, FaceSize);
                var transforms = new GlobalTransformStruct();
                transforms.Projection = cubeFaceCameras.Cameras[index].Projection;
                transforms.View = cubeFaceCameras.Cameras[index].View;
                transforms.Viewport = new Vector4(FaceSize, FaceSize, 1/FaceSize, 1/FaceSize);
                transforms.ViewProjection = transforms.View * transforms.Projection;

                modelCB.Upload(ctx, ref transforms);

                var frustum = new BoundingFrustum(transforms.ViewProjection);
                //Render opaque
                for (int i = 0; i < context.RenderHost.PerFrameOpaqueNodes.Count; ++i)
                {
                    var node = context.RenderHost.PerFrameOpaqueNodes[i];
                    if (node.GUID != this.GUID && !IgnoredGuid.Contains(node.GUID) && node.TestViewFrustum(ref frustum))
                    {
                        node.Render(context, ctx);
                    }
                }
                //Render particle
                for (int i = 0; i < context.RenderHost.PerFrameParticleNodes.Count; ++i)
                {
                    var node = context.RenderHost.PerFrameParticleNodes[i];
                    if (node.GUID != this.GUID && !IgnoredGuid.Contains(node.GUID) && node.TestViewFrustum(ref frustum))
                    {
                        node.Render(context, ctx);
                    }
                }
                commands[index] = ctx.FinishCommandList(true);
                contextPool.Put(ctx);
            }
#if !TEST
            );
#endif
            context.IsInvertCullMode = false;
            for (int i = 0; i < commands.Length; ++i)
            {
                Device.ImmediateContext.ExecuteCommandList(commands[i], true);
                commands[i].Dispose();
            }
            deviceContext.GenerateMips(CubeMap);
            context.UpdatePerFrameData(true, false, deviceContext);
        }

        protected override void OnUpdatePerModelStruct(ref GlobalTransformStruct model, RenderContext context)
        {
            var camPos = Center;
            targets[0] = camPos + Vector3.UnitX;
            targets[1] = camPos - Vector3.UnitX;
            targets[2] = camPos + Vector3.UnitY;
            targets[3] = camPos - Vector3.UnitY;
            targets[4] = camPos + Vector3.UnitZ;
            targets[5] = camPos - Vector3.UnitZ;

            for (int i = 0; i < 6; ++i)
            {
                cubeFaceCameras.Cameras[i].View = (IsLeftHanded ? MatrixHelper.LookAtLH(camPos, targets[i], upVectors[i]) : MatrixHelper.LookAtRH(camPos, targets[i], upVectors[i]))
                    * Matrix.CreateScale(-1, 1, 1);
                cubeFaceCameras.Cameras[i].Projection = IsLeftHanded ? MatrixHelper.PerspectiveFovLH((float)Math.PI * 0.5f, 1, NearField, FarField)
                    : MatrixHelper.PerspectiveFovRH((float)Math.PI * 0.5f, 1, NearField, FarField);
            }
        }

        public sealed override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public sealed override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }
        #region IReflector

        private SamplerState[] currSampler;
        private ShaderResourceView[] currRes;

        /// <summary>
        /// Binds the cube map.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        public void BindCubeMap(DeviceContextProxy deviceContext)
        {
            currSampler = deviceContext.GetSampler(PixelShader.Type, cubeTextureSlot, 1);
            currRes = deviceContext.GetShaderResources(PixelShader.Type, cubeTextureSlot, 1);
            if (EnableReflector)
            {
                deviceContext.SetShaderResource(PixelShader.Type, cubeTextureSlot, CubeMap);
                deviceContext.SetSampler(PixelShader.Type, textureSamplerSlot, textureSampler);
            }
        }

        /// <summary>
        /// Uns the bind cube map.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        public void UnBindCubeMap(DeviceContextProxy deviceContext)
        {
            deviceContext.SetShaderResources(PixelShader.Type, cubeTextureSlot, currRes);
            deviceContext.SetSamplers(PixelShader.Type, textureSamplerSlot, currSampler);
            for (int i = 0; i < currSampler.Length; ++i)
            {
                Disposer.RemoveAndDispose(ref currSampler[i]);
            }
            for (int i = 0; i < currRes.Length; ++i)
            {
                Disposer.RemoveAndDispose(ref currRes[i]);
            }
            currSampler = null;
            currRes = null;
        }

        #endregion IReflector
    }
}