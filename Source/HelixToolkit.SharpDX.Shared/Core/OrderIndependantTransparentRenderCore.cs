/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#define MSAASEPARATE
using SharpDX.Direct3D11;
using SharpDX;
using SharpDX.DXGI;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX.Direct3D;
    using Render;
    using Shaders;
    using System.Runtime.CompilerServices;
    using Utilities;
    public class OrderIndependentTransparentRenderCore : RenderCoreBase<int>
    {
        private ShaderResourceViewProxy colorTarget;
        private ShaderResourceViewProxy alphaTarget;
        private ShaderResourceViewProxy colorTargetNoMSAA;
        private ShaderResourceViewProxy alphaTargetNoMSAA;

        private SampleDescription sampleDesc = new SampleDescription(1, 0);

        private Texture2DDescription colorDesc = new Texture2DDescription()
        {
            Format = Format.R16G16B16A16_Float,
            OptionFlags = ResourceOptionFlags.None,
            MipLevels = 1,
            ArraySize = 1,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
        };


        private Texture2DDescription alphaDesc = new Texture2DDescription()
        {
            Format = Format.A8_UNorm,
            OptionFlags = ResourceOptionFlags.None,
            MipLevels = 1,
            ArraySize = 1,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
        };

        private int width = 0;
        private int height = 0;
#if MSAASEPARATE
        private bool hasMSAA = false;
#endif

        private ShaderPass screenQuadPass = ShaderPass.NullPass;
        private int colorTexIndex, alphaTexIndex, samplerIndex;
        private SamplerStateProxy targetSampler;
        public int RenderCount { private set; get; } = 0;

        private RenderTargetView[] targets;
        public OrderIndependentTransparentRenderCore() : base(RenderType.Transparent)
        { }

        private bool CreateTextureResources(RenderContext context, DeviceContextProxy deviceContext)
        {
            var currSampleDesc = context.RenderHost.RenderBuffer.ColorBufferSampleDesc;
#if MSAASEPARATE
            hasMSAA = currSampleDesc.Count > 1 || currSampleDesc.Quality > 0;
#endif
            if (width != (int)context.ActualWidth || height != (int)context.ActualHeight
                || sampleDesc.Count != currSampleDesc.Count || sampleDesc.Quality != currSampleDesc.Quality)
            {
                RemoveAndDispose(ref colorTarget);
                RemoveAndDispose(ref alphaTarget);
                RemoveAndDispose(ref colorTargetNoMSAA);
                RemoveAndDispose(ref alphaTargetNoMSAA);
                sampleDesc = currSampleDesc;

                width = (int)context.ActualWidth;
                height = (int)context.ActualHeight;
                colorDesc.Width = alphaDesc.Width = width;
                colorDesc.Height = alphaDesc.Height = height;
                colorDesc.SampleDescription = alphaDesc.SampleDescription = sampleDesc;
#if MSAASEPARATE
                if (hasMSAA)
                {
                    colorDesc.BindFlags = alphaDesc.BindFlags = BindFlags.RenderTarget;
                }
                else
#endif
                {
                    colorDesc.BindFlags = alphaDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
                }

                colorTarget = Collect(new ShaderResourceViewProxy(Device, colorDesc));
                alphaTarget = Collect(new ShaderResourceViewProxy(Device, alphaDesc));


                colorTarget.CreateRenderTargetView();
                alphaTarget.CreateRenderTargetView();
#if MSAASEPARATE
                if (!hasMSAA)
#endif
                {
                    alphaTarget.CreateTextureView();
                    colorTarget.CreateTextureView();
                    colorTargetNoMSAA = colorTarget;
                    alphaTargetNoMSAA = alphaTarget;
                }
#if MSAASEPARATE
                else
                {
                    colorDesc.SampleDescription = alphaDesc.SampleDescription = new SampleDescription(1, 0);
                    colorDesc.BindFlags = alphaDesc.BindFlags = BindFlags.ShaderResource;
                    colorTargetNoMSAA = Collect(new ShaderResourceViewProxy(Device, colorDesc));
                    alphaTargetNoMSAA = Collect(new ShaderResourceViewProxy(Device, alphaDesc));
                    colorTargetNoMSAA.CreateTextureView();
                    alphaTargetNoMSAA.CreateTextureView();
                }
#endif
                InvalidateRenderer();
                return true; // Skip this frame if texture resized to reduce latency.
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Bind(RenderContext context, DeviceContextProxy deviceContext)
        {
            targets = deviceContext.DeviceContext.OutputMerger.GetRenderTargets(2);
            deviceContext.DeviceContext.ClearRenderTargetView(colorTarget, Color.Zero);
            deviceContext.DeviceContext.ClearRenderTargetView(alphaTarget, Color.White);       
            deviceContext.DeviceContext.OutputMerger.SetRenderTargets(context.RenderHost.DepthStencilBufferView, 
                new RenderTargetView[] { colorTarget, alphaTarget });  
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnBind(RenderContext context, DeviceContextProxy deviceContext)
        {
            deviceContext.DeviceContext.OutputMerger.SetRenderTargets(context.RenderHost.DepthStencilBufferView, targets);
            foreach(var target in targets)
            {
                target?.Dispose();
            }
#if MSAASEPARATE
            if (hasMSAA)
            {
                deviceContext.DeviceContext.ResolveSubresource(colorTarget.Resource, 0, colorTargetNoMSAA.Resource, 0, colorDesc.Format);
                deviceContext.DeviceContext.ResolveSubresource(alphaTarget.Resource, 0, alphaTargetNoMSAA.Resource, 0, alphaDesc.Format);
            }
#endif
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return null;
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                screenQuadPass = technique[DefaultPassNames.Default];
                colorTexIndex = screenQuadPass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.OITColorTB);
                alphaTexIndex = screenQuadPass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.OITAlphaTB);
                samplerIndex = screenQuadPass.GetShader(ShaderStage.Pixel).SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
                targetSampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerWrapAni2));
                RenderCount = 0;
                return true;
            }
            else
            { return false; }
        }

        protected override void OnDetach()
        {
            width = height = 0;
            colorTarget = null;
            alphaTarget = null;
            colorTargetNoMSAA = null;
            alphaTargetNoMSAA = null;
            base.OnDetach();
        }

        protected override bool CanRender(RenderContext context)
        {
            return base.CanRender(context) && context.RenderHost.PerFrameTransparentNodes.Count > 0;
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            RenderCount = 0;
            if(CreateTextureResources(context, deviceContext))
            {
                InvalidateRenderer();
                return; // Skip this frame if texture resized to reduce latency.
            }
            Bind(context, deviceContext);
            var frustum = context.BoundingFrustum;
            int count = context.RenderHost.PerFrameTransparentNodes.Count;
            context.IsOITPass = true;
            for (int i = 0; i < count; ++i)
            {
                var renderable = context.RenderHost.PerFrameTransparentNodes[i];
                if (context.EnableBoundingFrustum && !renderable.TestViewFrustum(ref frustum))
                {
                    continue;
                }
                renderable.RenderCore.Render(context, deviceContext);
                ++RenderCount;
            }
            context.IsOITPass = false;
            UnBind(context, deviceContext);
            screenQuadPass.BindShader(deviceContext);
            screenQuadPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
            screenQuadPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, colorTexIndex, colorTargetNoMSAA);
            screenQuadPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, alphaTexIndex, alphaTargetNoMSAA);
            screenQuadPass.GetShader(ShaderStage.Pixel).BindSampler(deviceContext, samplerIndex, targetSampler);
            deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            deviceContext.DeviceContext.Draw(4, 0);
        }

        protected override void OnUpdatePerModelStruct(ref int model, RenderContext context)
        {
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
        }
    }
}
