﻿using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Core;

public sealed class OrderIndependentTransparentRenderCore : RenderCore
{
    #region Variables
    private ShaderResourceViewProxy? colorTarget;
    private ShaderResourceViewProxy? alphaTarget;
    private ShaderResourceViewProxy? colorTargetNoMSAA;
    private ShaderResourceViewProxy? alphaTargetNoMSAA;
    private SamplerStateProxy? targetSampler;

    private SampleDescription sampleDesc = new(1, 0);
    private Texture2DDescription colorDesc = new()
    {
        Format = Format.R16G16B16A16_Float,
        OptionFlags = ResourceOptionFlags.None,
        MipLevels = 1,
        ArraySize = 1,
        Usage = ResourceUsage.Default,
        CpuAccessFlags = CpuAccessFlags.None,
    };
    private Texture2DDescription alphaDesc = new()
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
    private ShaderPass? screenQuadPass = ShaderPass.NullPass;
    private int colorTexIndex, alphaTexIndex, samplerIndex;
    private RenderTargetView?[]? targets;
    #endregion

    #region Properties
    public int RenderCount { private set; get; } = 0;
    public RenderParameter ExternRenderParameter
    {
        set; get;
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderIndependentTransparentRenderCore"/> class.
    /// </summary>
    public OrderIndependentTransparentRenderCore() : base(RenderType.Transparent)
    {
    }

    private bool CreateTextureResources(RenderContext context, DeviceContextProxy deviceContext)
    {
        if (Device is null || context.RenderHost.RenderBuffer is null)
        {
            return false;
        }

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

            colorTarget = new ShaderResourceViewProxy(Device, colorDesc);
            alphaTarget = new ShaderResourceViewProxy(Device, alphaDesc);


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
                        colorTargetNoMSAA = new ShaderResourceViewProxy(Device, colorDesc);
                        alphaTargetNoMSAA = new ShaderResourceViewProxy(Device, alphaDesc);
                        colorTargetNoMSAA.CreateTextureView();
                        alphaTargetNoMSAA.CreateTextureView();
                    }
#endif
            RaiseInvalidateRender();
            return true; // Skip this frame if texture resized to reduce latency.
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Bind(RenderContext context, DeviceContextProxy deviceContext)
    {
        targets = deviceContext.GetRenderTargets(2);
        deviceContext.ClearRenderTargetView(colorTarget, Color.Zero);
        deviceContext.ClearRenderTargetView(alphaTarget, Color.White);
        deviceContext.SetRenderTargets(context.RenderHost.DepthStencilBufferView,
            new RenderTargetView?[] { colorTarget, alphaTarget });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UnBind(RenderContext context, DeviceContextProxy deviceContext)
    {
        deviceContext.SetRenderTargets(context.RenderHost.DepthStencilBufferView, targets);

        if (targets is null)
        {
            return;
        }

        for (var i = 0; i < targets.Length; ++i)
        {
            targets[i]?.Dispose();
            targets[i] = null;
        }
#if MSAASEPARATE
                if (hasMSAA)
                {
                    deviceContext.ResolveSubresource(colorTarget.Resource, 0, colorTargetNoMSAA.Resource, 0, colorDesc.Format);
                    deviceContext.ResolveSubresource(alphaTarget.Resource, 0, alphaTargetNoMSAA.Resource, 0, alphaDesc.Format);
                }
#endif
    }

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        screenQuadPass = technique?[DefaultPassNames.Default];
        colorTexIndex = screenQuadPass?.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.OITColorTB) ?? 0;
        alphaTexIndex = screenQuadPass?.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.OITAlphaTB) ?? 0;
        samplerIndex = screenQuadPass?.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SurfaceSampler) ?? 0;
        targetSampler = technique?.EffectsManager?.StateManager?.Register(DefaultSamplers.LinearSamplerWrapAni1);
        RenderCount = 0;
        return true;
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref targetSampler);
        width = height = 0;
        RemoveAndDispose(ref colorTarget);
        RemoveAndDispose(ref alphaTarget);
        RemoveAndDispose(ref colorTargetNoMSAA);
        RemoveAndDispose(ref alphaTargetNoMSAA);
    }

    public override void Render(RenderContext context, DeviceContextProxy deviceContext)
    {
        RenderCount = 0;
        if (context.RenderHost.PerFrameTransparentNodes.Count == 0)
        {
            return;
        }
        else if (CreateTextureResources(context, deviceContext))
        {
            RaiseInvalidateRender();
            return; // Skip this frame if texture resized to reduce latency.
        }
        Bind(context, deviceContext);

        context.OITRenderStage = OITRenderStage.SinglePassWeighted;
        var parameter = ExternRenderParameter;
        if (!parameter.ScissorRegion.IsEmpty)
        {
            parameter.RenderTargetView = new RenderTargetView?[] { colorTarget, alphaTarget };
            RenderCount = context.RenderHost.Renderer?.
                RenderOpaque(context, context.RenderHost.PerFrameTransparentNodes, ref parameter, context.EnableBoundingFrustum) ?? 0;
        }
        else
        {
            var frustum = context.BoundingFrustum;
            var count = context.RenderHost.PerFrameTransparentNodes.Count;
            for (var i = 0; i < count; ++i)
            {
                var renderable = context.RenderHost.PerFrameTransparentNodes[i];
                renderable.RenderCore.Render(context, deviceContext);
                ++RenderCount;
            }
        }
        context.OITRenderStage = OITRenderStage.None;
        UnBind(context, deviceContext);

        if (screenQuadPass is null)
        {
            return;
        }

        screenQuadPass.BindShader(deviceContext);
        screenQuadPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
        screenQuadPass.PixelShader.BindTexture(deviceContext, colorTexIndex, colorTargetNoMSAA);
        screenQuadPass.PixelShader.BindTexture(deviceContext, alphaTexIndex, alphaTargetNoMSAA);
        screenQuadPass.PixelShader.BindSampler(deviceContext, samplerIndex, targetSampler);
        deviceContext.Draw(4, 0);
    }
}
