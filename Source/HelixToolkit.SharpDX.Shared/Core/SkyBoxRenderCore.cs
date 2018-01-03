/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using System.IO;
    using Utilities;

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

        private Stream cubeTexture = null;

        public Stream CubeTexture
        {
            set
            {
                if (cubeTexture == value) { return; }
                cubeTexture = value;
                if (IsAttached)
                {
                    cubeTextureRes.CreateView(value);
                }
            }
            get
            {
                return cubeTexture;
            }
        }

        public virtual string ShaderCubeTextureName { get { return DefaultBufferNames.CubeMapTB; } }

        private ShaderResouceViewProxy cubeTextureRes;
        private SamplerState sampler;

        public SkyBoxRenderCore()
        {
            RasterDescription = DefaultRasterDescriptions.RSSkybox;
        }

        private Vector3[] CreateVertexArray(PointGeometry3D geometry)
        {
            return geometry.Positions.ToArray();
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                var buffer = Collect(new PointGeometryBufferModel<Vector3>(Vector3.SizeInBytes));
                buffer.Geometry = new PointGeometry3D() { Positions = BoxPositions };
                buffer.OnBuildVertexArray = CreateVertexArray;
                buffer.Topology = PrimitiveTopology.TriangleList;
                GeometryBuffer = buffer;
                cubeTextureRes = Collect(new ShaderResouceViewProxy(Device));
                if (cubeTexture != null)
                {
                    cubeTextureRes.CreateView(cubeTexture);
                }
                sampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.CubeSampler));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return null;
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContext context)
        {

        }

        protected override void OnRender(IRenderContext context)
        {
            DefaultShaderPass.BindShader(context.DeviceContext);
            DefaultShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            DefaultShaderPass.GetShader(ShaderStage.Pixel).BindTexture(context.DeviceContext, ShaderCubeTextureName, cubeTextureRes);
            DefaultShaderPass.GetShader(ShaderStage.Pixel).BindSampler(context.DeviceContext, DefaultSamplerStateNames.CubeMapSampler, sampler);
            context.DeviceContext.Draw(GeometryBuffer.VertexBuffer.Count, 0);
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {

        }
    }
}
