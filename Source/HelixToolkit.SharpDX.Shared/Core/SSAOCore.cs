/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System;    
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Components;
    using Utilities;

    public sealed class SSAOCore : RenderCore
    {
        private Texture2DDescription ssaoTextureDesc = new Texture2DDescription()
        {
            CpuAccessFlags = CpuAccessFlags.None,
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            Format = global::SharpDX.DXGI.Format.R16_Float,
            SampleDescription = new global::SharpDX.DXGI.SampleDescription(1, 0),
            OptionFlags = ResourceOptionFlags.None,
            Usage = ResourceUsage.Default,
            ArraySize = 1,
            MipLevels = 1,
        };

        private ShaderResourceViewProxy ssaoView, ssaoNoise;
        private int width, height;
        private int ssaoTexSlot, noiseTexSlot, surfaceSampleSlot, noiseSamplerSlot;
        private ShaderPass ssaoPass;
        private SamplerStateProxy surfaceSampler, noiseSampler;
        private SSAOParamStruct ssaoParam;
        private readonly ConstantBufferComponent ssaoCB;
        private const int KernalSize = 32;
        private readonly Vector4[] kernels = new Vector4[KernalSize];
        private readonly Vector3[] frustumCorners = new Vector3[8];
        private readonly Vector4[] fpCorners = new Vector4[4];
        public SSAOCore():base(RenderType.PreProc)
        {
            ssaoCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.SSAOCB, SSAOParamStruct.SizeInBytes)));
        }

        public override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
            EnsureTextureResources((int)context.ActualWidth, (int)context.ActualHeight, deviceContext);
            var ds = context.RenderHost.RenderBuffer.FullResDepthStencilPool.Get(global::SharpDX.DXGI.Format.D32_Float);
            var rt0 = context.RenderHost.RenderBuffer.FullResRenderTargetPool.Get(global::SharpDX.DXGI.Format.R16G16B16A16_Float);
            deviceContext.ClearDepthStencilView(ds, DepthStencilClearFlags.Depth, 1, 0);
            deviceContext.ClearRenderTargetView(rt0, new Color4(0,0,0,0));
            deviceContext.SetRenderTarget(ds, rt0);
            IRenderTechnique currTechnique = null;
            ShaderPass ssaoPass1 = ShaderPass.NullPass;
            for(int i = 0; i < context.RenderHost.PerFrameOpaqueNodes.Count; ++i)
            {
                var node = context.RenderHost.PerFrameOpaqueNodes[i];
                if(currTechnique != node.EffectTechnique)
                {
                    currTechnique = node.EffectTechnique;
                    ssaoPass1 = currTechnique[DefaultPassNames.MeshSSAOPass];
                }
                if (ssaoPass1.IsNULL)
                {
                    continue;
                }
                node.RenderCore.RenderDepth(context, deviceContext, ssaoPass1);
            }
            context.BoundingFrustum.GetCorners(frustumCorners);
            fpCorners[0] = Vector3.Transform(frustumCorners[5], context.ViewMatrix);
            fpCorners[1] = Vector3.Transform(frustumCorners[6], context.ViewMatrix);
            fpCorners[2] = Vector3.Transform(frustumCorners[4], context.ViewMatrix);
            fpCorners[3] = Vector3.Transform(frustumCorners[7], context.ViewMatrix);
            ssaoParam.NoiseScale = new Vector2(context.ActualWidth/4f, context.ActualHeight/4f);
            ssaoCB.ModelConstBuffer.UploadDataToBuffer(deviceContext, (stream) =>
            {
                stream.WriteRange(kernels);
                stream.WriteRange(fpCorners);
                stream.Write(ssaoParam);
            });
            deviceContext.SetRenderTarget(null, ssaoView);
            ssaoPass.BindShader(deviceContext);
            ssaoPass.BindStates(deviceContext, StateType.All);
            ssaoPass.PixelShader.BindTexture(deviceContext, ssaoTexSlot, rt0);
            ssaoPass.PixelShader.BindTexture(deviceContext, noiseTexSlot, ssaoNoise);
            ssaoPass.PixelShader.BindSampler(deviceContext, surfaceSampleSlot, surfaceSampler);
            ssaoPass.PixelShader.BindSampler(deviceContext, noiseSamplerSlot, noiseSampler);
            deviceContext.Draw(4, 0);

            context.RenderHost.SetDefaultRenderTargets(false);

            context.RenderHost.RenderBuffer.FullResDepthStencilPool.Put(global::SharpDX.DXGI.Format.D32_Float, ds);
            context.RenderHost.RenderBuffer.FullResRenderTargetPool.Put(global::SharpDX.DXGI.Format.R16G16B16A16_Float, rt0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureTextureResources(int w, int h, DeviceContextProxy deviceContext)
        {
            if(w != width || h != height)
            {
                RemoveAndDispose(ref ssaoView);
                width = w;
                height = h;
                if(width > 10 && height > 0)
                {
                    ssaoTextureDesc.Width = width;
                    ssaoTextureDesc.Height = height;
                    ssaoView = Collect(new ShaderResourceViewProxy(deviceContext, ssaoTextureDesc));
                    ssaoView.CreateTextureView();
                    ssaoView.CreateRenderTargetView();
                }
            }
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (technique.IsNull)
            {
                return false;
            }
            width = height = 0;
            ssaoPass = technique[DefaultPassNames.Default];
            if (ssaoPass.IsNULL)
            {
                return false;
            }
            ssaoTexSlot = ssaoPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SSAOMapTB);
            noiseTexSlot = ssaoPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SSAONoiseTB);
            surfaceSampleSlot = ssaoPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SurfaceSampler);
            noiseSamplerSlot = ssaoPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.NoiseSampler);
            surfaceSampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.PointSamplerWrap));
            noiseSampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.SSAONoise));

            InitialParameters();
            return true;
        }

        private void InitialParameters()
        {
            ssaoParam.KernelSize = KernalSize;
            ssaoParam.Radius = 0.5f;
            var rnd = new Random((int)Stopwatch.GetTimestamp());
            float scale = 0.99f;
            for(int i = 0; i < 32; ++i)
            {
                float x = (float)rnd.NextDouble(-1, 1);
                float y = (float)rnd.NextDouble(-1, 1);
                float z = (float)rnd.NextDouble(0, 1);
                var v = Vector3.Normalize(new Vector3(x, y, z));                
                v *= scale;
                scale = 0.1f + scale * scale * (0.9f);
                kernels[i] = new Vector4(v.X, v.Y, v.Z, 0);
            }

            Vector3[] noise = new Vector3[4 * 4];
            for(int i=0; i<16; ++i)
            {
                float x = (float)rnd.NextDouble(-1, 1);
                float y = (float)rnd.NextDouble(-1, 1);
                noise[i] = new Vector3(x, y, 0);
            }
            ssaoNoise = Collect(ShaderResourceViewProxy
                .CreateView(Device, noise, 4, 4, global::SharpDX.DXGI.Format.R32G32B32_Float, true, false));
        }
    }
}
