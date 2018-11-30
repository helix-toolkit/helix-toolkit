/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using System.IO;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core
    {
        using Shaders;   
        using Utilities;
        using Render;
        /// <summary>
        /// 
        /// </summary>
        public class SkyBoxRenderCore : GeometryRenderCore, ISkyboxRenderParams
        {
            #region Default Mesh
            private static readonly Vector3Collection BoxPositions = new Vector3Collection()
            {
                new Vector3(-10.0f,  10.0f, -10.0f),
                new Vector3( -10.0f, -10.0f, -10.0f),
                new Vector3( 10.0f, -10.0f, -10.0f),
                new Vector3(  10.0f, -10.0f, -10.0f),
                new Vector3(  10.0f,  10.0f, -10.0f),
                new Vector3( -10.0f,  10.0f, -10.0f),

                new Vector3( -10.0f, -10.0f,  10.0f),
                new Vector3(-10.0f, -10.0f, -10.0f),
                new Vector3(  -10.0f,  10.0f, -10.0f),
                new Vector3(  -10.0f,  10.0f, -10.0f),
                new Vector3(  -10.0f,  10.0f,  10.0f),
                new Vector3(  -10.0f, -10.0f,  10.0f),

                new Vector3(   10.0f, -10.0f, -10.0f),
                new Vector3(   10.0f, -10.0f,  10.0f),
                new Vector3(   10.0f,  10.0f,  10.0f),
                new Vector3(   10.0f,  10.0f,  10.0f),
                new Vector3(   10.0f,  10.0f, -10.0f),
                new Vector3(   10.0f, -10.0f, -10.0f),

                new Vector3(  -10.0f, -10.0f,  10.0f),
                new Vector3(  -10.0f,  10.0f,  10.0f),
                new Vector3(   10.0f,  10.0f,  10.0f),
                new Vector3(   10.0f,  10.0f,  10.0f),
                new Vector3(   10.0f, -10.0f,  10.0f),
                new Vector3(  -10.0f, -10.0f,  10.0f),

                new Vector3(  -10.0f,  10.0f, -10.0f),
                new Vector3(   10.0f,  10.0f, -10.0f),
                new Vector3(   10.0f,  10.0f,  10.0f),
                new Vector3(   10.0f,  10.0f,  10.0f),
                new Vector3(  -10.0f,  10.0f,  10.0f),
                new Vector3(  -10.0f,  10.0f, -10.0f),

                new Vector3(  -10.0f, -10.0f, -10.0f),
                new Vector3(  -10.0f, -10.0f,  10.0f),
                new Vector3(   10.0f, -10.0f, -10.0f),
                new Vector3(   10.0f, -10.0f, -10.0f),
                new Vector3( -10.0f, -10.0f,  10.0f),
                new Vector3(   10.0f, -10.0f,  10.0f)
            };
            #endregion

            #region Variables
            private ShaderResourceViewProxy cubeTextureRes;
            private int cubeTextureSlot;
            private SamplerStateProxy textureSampler;
            private int textureSamplerSlot;
            private ShaderPass DefaultShaderPass;
            #endregion

            #region Properties
            private Stream cubeTexture = null;
            /// <summary>
            /// Gets or sets the cube texture.
            /// </summary>
            /// <value>
            /// The cube texture.
            /// </value>
            public Stream CubeTexture
            {
                set
                {
                    if(SetAffectsRender(ref cubeTexture, value) && IsAttached)
                    {
                        UpdateTexture();
                    }
                }
                get
                {
                    return cubeTexture;
                }
            }
            /// <summary>
            /// Gets the mip map levels for current cube texture.
            /// </summary>
            /// <value>
            /// The mip map levels.
            /// </value>
            public int MipMapLevels { private set; get; } = 0;

            private SamplerStateDescription samplerDescription = DefaultSamplers.EnvironmentSampler;
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
                    if(SetAffectsRender(ref samplerDescription, value) && IsAttached)
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
            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="SkyBoxRenderCore"/> class.
            /// </summary>
            public SkyBoxRenderCore()
            {
                RasterDescription = DefaultRasterDescriptions.RSSkybox;
            }
            /// <summary>
            /// Called when [attach].
            /// </summary>
            /// <param name="technique">The technique.</param>
            /// <returns></returns>
            protected override bool OnAttach(IRenderTechnique technique)
            {
                if (base.OnAttach(technique))
                {
                    DefaultShaderPass = technique[DefaultPassNames.Default];
                    OnDefaultPassChanged(DefaultShaderPass);
                    var buffer = Collect(new SkyBoxBufferModel());
                    buffer.Geometry = new PointGeometry3D() { Positions = BoxPositions };
                    buffer.Topology = PrimitiveTopology.TriangleList;
                    GeometryBuffer = buffer;
                    cubeTextureRes = Collect(new ShaderResourceViewProxy(Device));
                    UpdateTexture();
                    textureSampler = Collect(technique.EffectsManager.StateManager.Register(SamplerDescription));
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private void UpdateTexture()
            {
                MipMapLevels = 0;
                if (cubeTexture != null)
                {
                    cubeTextureRes.CreateView(cubeTexture);
                    if(cubeTextureRes.TextureView != null && cubeTextureRes.TextureView.Description.Dimension == ShaderResourceViewDimension.TextureCube)
                    {
                        MipMapLevels = cubeTextureRes.TextureView.Description.TextureCube.MipLevels;
                    }
                }
                else
                {
                    cubeTextureRes.DisposeAndClear();
                }
            }

            protected override void OnDetach()
            {
                MipMapLevels = 0;
                textureSampler = null;
                cubeTextureRes = null;
                base.OnDetach();
            }

            /// <summary>
            /// Called when [default pass changed].
            /// </summary>
            /// <param name="pass">The pass.</param>
            protected void OnDefaultPassChanged(ShaderPass pass)
            {
                cubeTextureSlot = pass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderCubeTextureName);
                textureSamplerSlot = pass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderCubeTextureSamplerName);
            }

            /// <summary>
            /// Called when [render].
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="deviceContext">The device context.</param>
            protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
            {
                if (context.Camera.CreateLeftHandSystem && RasterDescription.IsFrontCounterClockwise)
                {
                    var desc = RasterDescription;
                    desc.IsFrontCounterClockwise = false;
                    RasterDescription = desc;
                    RaiseInvalidateRender();
                    return;
                }
                context.SharedResource.EnvironementMap = cubeTextureRes;
                context.SharedResource.EnvironmentMapMipLevels = MipMapLevels;
                DefaultShaderPass.BindShader(deviceContext);
                DefaultShaderPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                DefaultShaderPass.PixelShader.BindTexture(deviceContext, cubeTextureSlot, cubeTextureRes);
                DefaultShaderPass.PixelShader.BindSampler(deviceContext, textureSamplerSlot, textureSampler);
                deviceContext.Draw(GeometryBuffer.VertexBuffer[0].ElementCount, 0);
            }

            /// <summary>
            /// Called when [render shadow].
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="deviceContext">The device context.</param>
            protected sealed override void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext)
            {
            
            }

            protected sealed override void OnRenderCustom(RenderContext context, DeviceContextProxy deviceContext)
            {
            
            }
            protected sealed override void OnRenderDepth(RenderContext context, DeviceContextProxy deviceContext, Shaders.ShaderPass customPass)
            {
            }
            /// <summary>
            /// 
            /// </summary>
            private sealed class SkyBoxBufferModel : PointGeometryBufferModel<Vector3>
            {
                public SkyBoxBufferModel() : base(Vector3.SizeInBytes)
                {
                    Topology = PrimitiveTopology.TriangleList;
                }

                protected override void OnCreateVertexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D geometry, IDeviceResources deviceResources)
                {
                    // -- set geometry if given
                    if (geometry != null && geometry.Positions != null && geometry.Positions.Count > 0)
                    {

                        buffer.UploadDataToBuffer(context, geometry.Positions, geometry.Positions.Count);
                    }
                    else
                    {
                        buffer.UploadDataToBuffer(context, emptyVerts, 0);
                    }
                }
            }
        }
    }

}
