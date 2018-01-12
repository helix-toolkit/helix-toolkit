/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define TEST
using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using System.Linq;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using System.Collections.Generic;
    using Utilities;
    using Render;

    /// <summary>
    /// 
    /// </summary>
    public class ShadowMapCore : RenderCoreBase<ShadowMapParamStruct>, IShadowMapRenderParams
    {
        /// <summary>
        /// 
        /// </summary>
        protected ShaderResouceViewProxy viewResource;

        private bool resolutionChanged = true;
        /// <summary>
        /// 
        /// </summary>
        public int Width
        {
            set
            {
                if(SetAffectsRender(ref modelStruct.ShadowMapSize.X, value))
                {
                    resolutionChanged = true;
                }
            }
            get
            {
                return (int)modelStruct.ShadowMapSize.X;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Height
        {
            set
            {
                if (SetAffectsRender(ref modelStruct.ShadowMapSize.Y, value))
                {
                    resolutionChanged = true;
                }
            }
            get
            {
                return (int)modelStruct.ShadowMapSize.Y;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual float Intensity
        {
            set
            {
                SetAffectsRender(ref modelStruct.ShadowMapInfo.X, value);
            }
            get { return modelStruct.ShadowMapInfo.X; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual float Bias
        {
            set
            {
                SetAffectsRender(ref modelStruct.ShadowMapInfo.Z, value);
            }
            get { return modelStruct.ShadowMapInfo.Z; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Matrix LightViewProjectMatrix
        {
            set
            {
                SetAffectsRender(ref modelStruct.LightViewProjection, value);
            }
            get { return modelStruct.LightViewProjection; }
        }
        /// <summary>
        /// Update shadow map every N frames
        /// </summary>
        public int UpdateFrequency { set; get; } = 1;

        private int currentFrame = 0;
        /// <summary>
        /// 
        /// </summary>
        public ShadowMapCore()
        {
            Bias = 0.0015f;
            Intensity = 0.5f;
            Width = Height = 1024;
        }
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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

        private readonly List<IRenderCore> pendingRenders = new List<IRenderCore>(100);
        private readonly Stack<IEnumerator<IRenderable>> stackCache = new Stack<IEnumerator<IRenderable>>(20);

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
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
            deviceContext.DeviceContext.Rasterizer.SetViewport(0, 0, Width, Height);
            DepthStencilView orgDSV;
            var orgRT = deviceContext.DeviceContext.OutputMerger.GetRenderTargets(1, out orgDSV);
            deviceContext.DeviceContext.ClearDepthStencilView(viewResource, DepthStencilClearFlags.Depth, 1.0f, 0);
            deviceContext.DeviceContext.OutputMerger.SetTargets(viewResource.DepthStencilView, new RenderTargetView[0]);
            pendingRenders.Clear();
            try
            {
                pendingRenders.AddRange(context.RenderHost.Viewport.Renderables
                    .PreorderDFTGetCores(x => x.IsRenderable && !(x is ILight3D) && x.RenderCore.IsThrowingShadow, stackCache));
                foreach (var item in pendingRenders)
                {
                    item.Render(context, deviceContext);
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
                deviceContext.DeviceContext.OutputMerger.SetRenderTargets(orgDSV, orgRT);
                orgDSV?.Dispose();
                foreach (var rt in orgRT)
                {
                    rt?.Dispose();
                }
                deviceContext.DeviceContext.Rasterizer.SetViewport(0, 0, (float)context.ActualWidth, (float)context.ActualHeight);
                context.SharedResource.ShadowView = viewResource.TextureView;
            }
#endif
        }

        protected override void OnUpdatePerModelStruct(ref ShadowMapParamStruct model, IRenderContext context)
        {
            model.HasShadowMap = context.RenderHost.IsShadowMapEnabled ? 1 : 0;
        }
    }
}
