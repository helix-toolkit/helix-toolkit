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

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Utilities;
    using Render;

    /// <summary>
    /// 
    /// </summary>
    public class ShadowMapCore : RenderCoreBase<ShadowMapParamStruct>, IShadowMapRenderParams
    {
        public sealed class UpdateLightSourceEventArgs : EventArgs
        {
            public IRenderContext Context { private set; get; }
            public UpdateLightSourceEventArgs(IRenderContext context)
            {
                Context = context;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected ShaderResourceViewProxy viewResource;

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
        public float Intensity
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
        public float Bias
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
        /// Set to true if found the light source, otherwise false.
        /// </summary>
        public bool FoundLightSource { set; get; } = false;
        /// <summary>
        /// Update shadow map every N frames
        /// </summary>
        public int UpdateFrequency { set; get; } = 1;

        private int currentFrame = 0;

        public event EventHandler<UpdateLightSourceEventArgs> OnUpdateLightSource;
        /// <summary>
        /// 
        /// </summary>
        public ShadowMapCore() : base(RenderType.PreProc)
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
                OnUpdateLightSource?.Invoke(this, new UpdateLightSourceEventArgs(context));
                ++currentFrame;
                currentFrame %= Math.Max(1, UpdateFrequency);
                return FoundLightSource && currentFrame == 0;
            }
            else
            {
                return false;
            }
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            if (resolutionChanged)
            {
                RemoveAndDispose(ref viewResource);
                viewResource = Collect(new ShaderResourceViewProxy(Device, ShadowMapTextureDesc));
                viewResource.CreateView(DepthStencilViewDesc);
                viewResource.CreateView(ShaderResourceViewDesc);
                resolutionChanged = false;
            }

            deviceContext.DeviceContext.ClearDepthStencilView(viewResource, DepthStencilClearFlags.Depth, 1.0f, 0);
            context.IsShadowPass = true;
            var orgFrustum = context.BoundingFrustum;
            context.BoundingFrustum = new BoundingFrustum(LightViewProjectMatrix);
#if !TEST            
            deviceContext.DeviceContext.Rasterizer.SetViewport(0, 0, Width, Height);

            deviceContext.DeviceContext.OutputMerger.SetTargets(viewResource.DepthStencilView, new RenderTargetView[0]);
            for (int i = 0; i < context.RenderHost.PerFrameGeneralRenderCores.Count; ++i)
            {
                var core = context.RenderHost.PerFrameGeneralRenderCores[i];
                if (core.IsThrowingShadow && core.RenderType == RenderType.Opaque)
                {
                    core.Render(context, deviceContext);
                }
            }

            context.IsShadowPass = false;
            context.BoundingFrustum = orgFrustum;
            context.RenderHost.SetDefaultRenderTargets(false);
            context.SharedResource.ShadowView = viewResource.TextureView;
#endif
        }

        protected override void OnUpdatePerModelStruct(ref ShadowMapParamStruct model, IRenderContext context)
        {
            model.HasShadowMap = context.RenderHost.IsShadowMapEnabled ? 1 : 0;
        }
    }
}
