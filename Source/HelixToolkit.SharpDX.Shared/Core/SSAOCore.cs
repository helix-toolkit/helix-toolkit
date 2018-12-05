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
        using Render;
        using Shaders;
        using Components;
        using Utilities;

        public sealed class SSAOCore : RenderCore
        {
            private float radius = 0.5f;
            public float Radius
            {
                set { SetAffectsRender(ref radius, value); }
                get { return radius; }
            }

            private SSAOQuality quality = SSAOQuality.Low;
            public SSAOQuality Quality
            {
                set
                {
                    if(SetAffectsRender(ref quality, value))
                    {
                        offScreenTextureSize = value == SSAOQuality.High ? OffScreenTextureSize.Full : OffScreenTextureSize.Half;
                    }
                }
                get { return quality; }
            }

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
            private int ssaoTexSlot, noiseTexSlot, surfaceSampleSlot, noiseSamplerSlot, depthSlot;
            private ShaderPass ssaoPass, ssaoBlur;
            private SamplerStateProxy surfaceSampler, noiseSampler, blurSampler;
            private SSAOParamStruct ssaoParam;
            private readonly ConstantBufferComponent ssaoCB;
            private const int KernalSize = 32;
            private readonly Vector4[] kernels = new Vector4[KernalSize];
            private const global::SharpDX.DXGI.Format DEPTHFORMAT = global::SharpDX.DXGI.Format.R32_Typeless;
            private const global::SharpDX.DXGI.Format RENDERTARGETFORMAT = global::SharpDX.DXGI.Format.R16G16B16A16_Float;
            private const global::SharpDX.DXGI.Format SSAOTARGETFORMAT = global::SharpDX.DXGI.Format.R16_Float;

            private OffScreenTextureSize offScreenTextureSize = OffScreenTextureSize.Half;

            public SSAOCore() : base(RenderType.PreProc)
            {
                ssaoCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.SSAOCB, SSAOParamStruct.SizeInBytes)));
            }

            public override void Render(RenderContext context, DeviceContextProxy deviceContext)
            {
                EnsureTextureResources((int)context.ActualWidth, (int)context.ActualHeight, deviceContext);
                int texScale = (int)offScreenTextureSize;
                using (var ds = context.GetOffScreenDS(offScreenTextureSize, DEPTHFORMAT))
                {
                    using (var rt0 = context.GetOffScreenRT(offScreenTextureSize, RENDERTARGETFORMAT))
                    {
                        using (var rt1 = context.GetOffScreenRT(offScreenTextureSize, SSAOTARGETFORMAT))
                        {
                            int w = (int)(context.ActualWidth / texScale);// Make sure to set correct viewport width/height by quality
                            int h = (int)(context.ActualHeight / texScale);
                            deviceContext.SetRenderTarget(ds, rt0, w, h, true, new Color4(0, 0, 0, 1), true, DepthStencilClearFlags.Depth);
                            IRenderTechnique currTechnique = null;
                            ShaderPass ssaoPass1 = ShaderPass.NullPass;
                            var frustum = context.BoundingFrustum;
                            for (int i = 0; i < context.RenderHost.PerFrameOpaqueNodesInFrustum.Count; ++i)
                            {
                                var node = context.RenderHost.PerFrameOpaqueNodesInFrustum[i];
                                if (currTechnique != node.EffectTechnique)
                                {
                                    currTechnique = node.EffectTechnique;
                                    ssaoPass1 = currTechnique[DefaultPassNames.MeshSSAOPass];
                                }
                                if (ssaoPass1.IsNULL)
                                {
                                    continue;
                                }
                                node.RenderDepth(context, deviceContext, ssaoPass1);
                            }

                            var invProjection = context.ProjectionMatrix.Inverted();
                            ssaoParam.InvProjection = invProjection;
                            ssaoParam.NoiseScale = new Vector2(w / 4f, h / 4f);
                            ssaoParam.Radius = radius;
                            ssaoParam.TextureScale = texScale;
                            ssaoCB.ModelConstBuffer.UploadDataToBuffer(deviceContext, (stream) =>
                            {
                                stream.WriteRange(kernels);
                                stream.Write(ssaoParam);
                            });
                            deviceContext.SetRenderTargetOnly(rt1);
                            ssaoPass.BindShader(deviceContext);
                            ssaoPass.BindStates(deviceContext, StateType.All);
                            ssaoPass.PixelShader.BindTexture(deviceContext, ssaoTexSlot, rt0);
                            ssaoPass.PixelShader.BindTexture(deviceContext, noiseTexSlot, ssaoNoise);
                            ssaoPass.PixelShader.BindTexture(deviceContext, depthSlot, ds);
                            ssaoPass.PixelShader.BindSampler(deviceContext, surfaceSampleSlot, surfaceSampler);
                            ssaoPass.PixelShader.BindSampler(deviceContext, noiseSamplerSlot, noiseSampler);
                            deviceContext.Draw(4, 0);

                            ssaoPass.PixelShader.BindTexture(deviceContext, depthSlot, null);

                            deviceContext.SetRenderTarget(ssaoView, width, height);
                            ssaoBlur.BindShader(deviceContext);
                            ssaoBlur.BindStates(deviceContext, StateType.All);
                            ssaoBlur.PixelShader.BindTexture(deviceContext, ssaoTexSlot, rt1);
                            ssaoBlur.PixelShader.BindSampler(deviceContext, surfaceSampleSlot, blurSampler);
                            deviceContext.Draw(4, 0);
                            context.SharedResource.SSAOMap = ssaoView;

                            context.RenderHost.SetDefaultRenderTargets(false);
                            deviceContext.SetShaderResource(PixelShader.Type, ssaoTexSlot, ssaoView);
                        }
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void EnsureTextureResources(int w, int h, DeviceContextProxy deviceContext)
            {
                if (w != width || h != height)
                {
                    RemoveAndDispose(ref ssaoView);
                    width = w;
                    height = h;
                    if (width > 10 && height > 10)
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
                ssaoBlur = technique[DefaultPassNames.EffectBlurHorizontal];
                if (ssaoPass.IsNULL || ssaoBlur.IsNULL)
                {
                    return false;
                }
                ssaoTexSlot = ssaoPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SSAOMapTB);
                noiseTexSlot = ssaoPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SSAONoiseTB);
                depthSlot = ssaoPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SSAODepthTB);
                surfaceSampleSlot = ssaoPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SurfaceSampler);
                noiseSamplerSlot = ssaoPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.NoiseSampler);           
                surfaceSampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.SSAOSamplerClamp));
                noiseSampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.SSAONoise));
                blurSampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni1));
                InitialParameters();
                return true;
            }

            private void InitialParameters()
            {
                ssaoParam.Radius = radius;
                var rnd = new Random((int)Stopwatch.GetTimestamp());
                double thres = Math.Cos(Math.PI / 2 - Math.PI / 12);
                for (int i = 0; i < 32; ++i)
                {
                    while (true)
                    {
                        float x = rnd.NextFloat(-1, 1);
                        float y = rnd.NextFloat(-1, 1);
                        float z = rnd.NextFloat(1e-3f, 1);
                        var v = Vector3.Normalize(new Vector3(x, y, z));
                        float angle = Vector3.Dot(v, Vector3.UnitZ);
                        if (Vector3.Dot(v, Vector3.UnitZ) < thres)
                        {
                            continue;
                        }
                        float scale = i / 32f;
                        scale = 0.1f + 0.9f * scale;
                        v *= scale;
                        kernels[i] = new Vector4(v.X, v.Y, v.Z, 0);
                        break;
                    }
                }

                Vector3[] noise = new Vector3[4 * 4];
                for (int i = 0; i < 16; ++i)
                {
                    float x = rnd.NextFloat(-1, 1);
                    float y = rnd.NextFloat(-1, 1);
                    noise[i] = Vector3.Normalize(new Vector3(x, y, 0));
                }
                ssaoNoise = Collect(ShaderResourceViewProxy
                    .CreateView(Device, noise, 4, 4, global::SharpDX.DXGI.Format.R32G32B32_Float, true, false));
            }
        }
    }

}
