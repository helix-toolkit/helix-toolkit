/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    public class OrderIndependantTransparentRenderCore : RenderCoreBase<int>
    {
        private ShaderResourceViewProxy colorTarget;
        private ShaderResourceViewProxy alphaTarget;
        private Texture2DDescription colorDesc = new Texture2DDescription()
        {
            Format = Format.R16G16B16A16_Float,
            BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
            OptionFlags = ResourceOptionFlags.None,
            SampleDescription = new SampleDescription(1, 0),
            MipLevels = 1,
            ArraySize = 1,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
        };


        private Texture2DDescription alphaDesc = new Texture2DDescription()
        {
            Format = Format.R8_UNorm,
            BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
            OptionFlags = ResourceOptionFlags.None,
            SampleDescription = new SampleDescription(1, 0),
            MipLevels = 1,
            ArraySize = 1,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
        };

        private int width = 0;
        private int height = 0;

        private IShaderPass screenQuadPass = NullShaderPass.NullPass;
        private int colorTexIndex, alphaTexIndex, samplerIndex;
        private SamplerStateProxy targetSampler;
        public int RenderCount { private set; get; } = 0;
        public OrderIndependantTransparentRenderCore() : base(RenderType.Transparent)
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Bind(IRenderContext context, DeviceContextProxy deviceContext)
        {
            if (width != (int)context.ActualWidth && height != (int)context.ActualHeight)
            {
                RemoveAndDispose(ref colorTarget);
                RemoveAndDispose(ref alphaTarget);
                width = (int)context.ActualWidth;
                height = (int)context.ActualHeight;
                colorDesc.Width = alphaDesc.Width = width;
                colorDesc.Height = alphaDesc.Height = height;
                colorTarget = Collect(new ShaderResourceViewProxy(deviceContext.DeviceContext.Device, colorDesc));
                alphaTarget = Collect(new ShaderResourceViewProxy(deviceContext.DeviceContext.Device, alphaDesc));

                var srvDesc = new ShaderResourceViewDescription()
                {
                    Dimension = global::SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource() { MipLevels = 1, MostDetailedMip = 0 }
                };

                var rtsDesc = new RenderTargetViewDescription()
                {
                    Dimension = RenderTargetViewDimension.Texture2D,
                    Texture2D = new RenderTargetViewDescription.Texture2DResource() { MipSlice = 0 }
                };

                srvDesc.Format = rtsDesc.Format = colorDesc.Format;
                colorTarget.CreateView(srvDesc);
                colorTarget.CreateView(rtsDesc);

                srvDesc.Format = rtsDesc.Format = alphaDesc.Format;
                alphaTarget.CreateView(srvDesc);
                alphaTarget.CreateView(rtsDesc);
            }

            deviceContext.DeviceContext.ClearRenderTargetView(colorTarget, Color.Zero);
            deviceContext.DeviceContext.ClearRenderTargetView(alphaTarget, Color.White);
            deviceContext.DeviceContext.OutputMerger.SetRenderTargets(context.RenderHost.DepthStencilBufferView, new RenderTargetView[] { colorTarget, alphaTarget });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnBind(IRenderContext context, DeviceContextProxy deviceContext)
        {
            deviceContext.SetRenderTargets(context.RenderHost.RenderBuffer);
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

        protected override bool CanRender(IRenderContext context)
        {
            return base.CanRender(context) && context.RenderHost.PerFrameTransparentNodes.Count > 0;
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            RenderCount = 0;
            Bind(context, deviceContext);
            var frustum = context.BoundingFrustum;
            int count = context.RenderHost.PerFrameTransparentNodes.Count;
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
            UnBind(context, deviceContext);
            screenQuadPass.BindShader(deviceContext);
            screenQuadPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
            screenQuadPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, colorTexIndex, colorTarget);
            screenQuadPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, alphaTexIndex, alphaTarget);
            screenQuadPass.GetShader(ShaderStage.Pixel).BindSampler(deviceContext, samplerIndex, targetSampler);
            deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            deviceContext.DeviceContext.Draw(4, 0);
        }

        protected override void OnUpdatePerModelStruct(ref int model, IRenderContext context)
        {
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
        }
    }
}
