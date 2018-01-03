/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define TEST
using System;
using SharpDX;
using SharpDX.Direct3D11;
using System.Linq;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX.Direct3D;
    using global::SharpDX.DXGI;
    using Shaders;
    using Utilities;

    /// <summary>
    /// 
    /// </summary>
    public class ShadowMapCore : RenderCoreBase<ShadowMapParamStruct>, IShadowMapRenderParams
    {
        protected ShaderResouceViewProxy viewResource;

        private bool resolutionChanged = true;
        private int width = 1024;
        public int Width
        {
            set
            {
                if (width == value) { return; }
                width = value;
                resolutionChanged = true;
            }
            get
            {
                return width;
            }
        }

        private int height = 1024;
        public int Height
        {
            set
            {
                if (height == value) { return; }
                height = value;
                resolutionChanged = true;
            }
            get
            {
                return height;
            }
        }

       // public float FactorPCF { set; get; } = 1.5f;
        public float Bias { set; get; } = 0.0015f;

        public float Intensity { set; get; } = 0.5f;
        public Matrix LightViewProjectMatrix { set; get; }
        /// <summary>
        /// Update shadow map every N frames
        /// </summary>
        public int UpdateFrequency { set; get; } = 2;

        private int currentFrame = 0;

        protected virtual Texture2DDescription ShadowMapTextureDesc
        {
            get
            {
                return new Texture2DDescription()
                {
                    Format = Format.R32_Typeless, //!!!! because of depth and shader resource
                                                  //Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = Width,
                    Height = Height,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource, //!!!!
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                };
            }
        }

        protected virtual DepthStencilViewDescription DepthStencilViewDesc
        {
            get
            {
                return new DepthStencilViewDescription()
                {
                    Format = Format.D32_Float,
                    Dimension = DepthStencilViewDimension.Texture2D,
                    Texture2D = new DepthStencilViewDescription.Texture2DResource()
                    {
                        MipSlice = 0
                    }
                };
            }
        }

        protected virtual ShaderResourceViewDescription ShaderResourceViewDesc
        {
            get
            {
                return new ShaderResourceViewDescription()
                {
                    Format = Format.R32_Float,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                    {
                        MipLevels = 1,
                        MostDetailedMip = 0,
                    }
                };
            }
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ShadowParamCB, ShadowMapParamStruct.SizeInBytes);
        }

        protected override bool CanRender(IRenderContext context)
        {
#if TEST
            if (base.CanRender(context))
#else
            if(base.CanRender(context) && !context.IsShadowPass)
#endif
            {
                ++currentFrame;
                currentFrame %= Math.Max(1, UpdateFrequency);
                return currentFrame == 0;
            }
            else
            {
                return false;
            }
        }

        protected override void OnRender(IRenderContext context)
        {
            context.IsShadowPass = true;
            var orgFrustum = context.BoundingFrustum;
            context.BoundingFrustum = new BoundingFrustum(LightViewProjectMatrix);
#if !TEST            
            if (resolutionChanged)
            {
                RemoveAndDispose(ref viewResource);
                viewResource = Collect(new ShaderResouceViewProxy(Device, ShadowMapTextureDesc));
                viewResource.CreateView(DepthStencilViewDesc);
                viewResource.CreateView(ShaderResourceViewDesc);
                resolutionChanged = false;
            }
            context.DeviceContext.Rasterizer.SetViewport(0, 0, Width, Height);
            DepthStencilView orgDSV;
            var orgRT = context.DeviceContext.OutputMerger.GetRenderTargets(1, out orgDSV);
            context.DeviceContext.ClearDepthStencilView(viewResource, DepthStencilClearFlags.Depth, 1.0f, 0);
            context.DeviceContext.OutputMerger.SetTargets(viewResource.DepthStencilView, new RenderTargetView[0]);
            try
            {
                foreach (var item in context.RenderHost.Renderable.Renderables.Where(x => x is IThrowingShadow && ((IThrowingShadow)x).IsThrowingShadow))
                {
                    item.Render(context);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                context.IsShadowPass = false;
                context.BoundingFrustum = orgFrustum;
                context.DeviceContext.OutputMerger.SetRenderTargets(orgDSV, orgRT);
                orgDSV?.Dispose();
                foreach (var rt in orgRT)
                {
                    rt?.Dispose();
                }
                context.DeviceContext.Rasterizer.SetViewport(0, 0, (float)context.ActualWidth, (float)context.ActualHeight);
                context.SharedResource.ShadowView = viewResource.TextureView;
            }
#endif
        }

        protected override void OnUpdatePerModelStruct(ref ShadowMapParamStruct model, IRenderContext context)
        {
            model.ShadowMapInfo = new Vector4(Intensity, 0, Bias, 0);
            model.ShadowMapSize = new Vector2(Width, Height);
            model.LightViewProjection = LightViewProjectMatrix;
            model.HasShadowMap = context.RenderHost.IsShadowMapEnabled ? 1 : 0;
        }
    }
}
