﻿using HelixToolkit.SharpDX.Core.Components;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public class ShadowMapCore : RenderCore, IShadowMapRenderParams
{
    public sealed class UpdateLightSourceEventArgs : EventArgs
    {
        public RenderContext Context
        {
            private set; get;
        }
        public UpdateLightSourceEventArgs(RenderContext context)
        {
            Context = context;
        }
    }
    public event EventHandler<UpdateLightSourceEventArgs>? OnUpdateLightSource;
    #region Variables
    private ShaderResourceViewProxy? viewResource;
    private int currentFrame = 0;
    private bool resolutionChanged = true;
    private ShadowMapParamStruct modelStruct;
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

    private readonly ConstantBufferComponent modelCB;
    #endregion
    #region Properties
    /// <summary>
    /// 
    /// </summary>
    public int Width
    {
        set
        {
            if (SetAffectsRender(ref modelStruct.ShadowMapSize.X, value))
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
        get
        {
            return modelStruct.ShadowMapInfo.X;
        }
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
        get
        {
            return modelStruct.ShadowMapInfo.Z;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public Matrix LightView
    {
        set
        {
            SetAffectsRender(ref modelStruct.LightView, value);
        }
        get
        {
            return modelStruct.LightView;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public Matrix LightProjection
    {
        set
        {
            SetAffectsRender(ref modelStruct.LightProjection, value);
        }
        get
        {
            return modelStruct.LightProjection;
        }
    }
    /// <summary>
    /// Set to true if found the light source, otherwise false.
    /// </summary>
    public bool FoundLightSource { set; get; } = false;
    /// <summary>
    /// Update shadow map every N frames
    /// </summary>
    public int UpdateFrequency { set; get; } = 1;

    public bool NeedRender { set; get; } = true;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public ShadowMapCore() : base(RenderType.PreProc)
    {
        modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.ShadowParamCB, ShadowMapParamStruct.SizeInBytes)));
        Bias = 0.0015f;
        Intensity = 0.5f;
        Width = Height = 1024;
    }

    public override void Render(RenderContext context, DeviceContextProxy deviceContext)
    {
        if (!NeedRender)
        {
            modelStruct.HasShadowMap = 0;
            modelCB.Upload(deviceContext, ref modelStruct);
            return;
        }
        OnUpdateLightSource?.Invoke(this, new UpdateLightSourceEventArgs(context));
        ++currentFrame;
        currentFrame %= Math.Max(1, UpdateFrequency);
        if (!FoundLightSource || currentFrame != 0)
        {
            return;
        }
        if (resolutionChanged)
        {
            RemoveAndDispose(ref viewResource);
            if (Device is not null)
            {
                viewResource = new ShaderResourceViewProxy(Device, ShadowMapTextureDesc);
                viewResource.CreateView(DepthStencilViewDesc);
                viewResource.CreateView(ShaderResourceViewDesc);
            }
            resolutionChanged = false;
        }

        deviceContext.ClearDepthStencilView(viewResource, DepthStencilClearFlags.Depth, 1.0f, 0);
        var orgFrustum = context.BoundingFrustum;
        var frustum = new BoundingFrustum(LightView * LightProjection);
        context.BoundingFrustum = frustum;
#if !TEST
        deviceContext.SetViewport(0, 0, Width, Height);

        deviceContext.SetDepthStencil(viewResource?.DepthStencilView);
        modelStruct.HasShadowMap = context.RenderHost.IsShadowMapEnabled ? 1 : 0;
        modelCB.Upload(deviceContext, ref modelStruct);
        for (var i = 0; i < context.RenderHost.PerFrameOpaqueNodes.Count; ++i)
        {
            //Only support opaque object for throwing shadows.
            var core = context.RenderHost.PerFrameOpaqueNodes[i];
            if (core.RenderCore.IsThrowingShadow && core.TestViewFrustum(ref frustum))
            {
                core.RenderShadow(context, deviceContext);
            }
        }
        context.BoundingFrustum = orgFrustum;
        context.RenderHost.SetDefaultRenderTargets(false);
        if (context.SharedResource is not null)
        {
            context.SharedResource.ShadowView = viewResource;
        }
#endif
    }

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        return true;
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref viewResource);
        resolutionChanged = true;
    }
}
