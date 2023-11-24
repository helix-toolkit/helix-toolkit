using HelixToolkit.SharpDX.Core.Components;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using PixelShader = HelixToolkit.SharpDX.Shaders.PixelShader;

namespace HelixToolkit.SharpDX.Core;

public sealed class SSAOCore : RenderCore
{
    private float radius = 0.5f;
    public float Radius
    {
        set
        {
            SetAffectsRender(ref radius, value);
        }
        get
        {
            return radius;
        }
    }

    private SSAOQuality quality = SSAOQuality.Low;
    public SSAOQuality Quality
    {
        set
        {
            if (SetAffectsRender(ref quality, value))
            {
                offScreenTextureSize = value == SSAOQuality.High ? OffScreenTextureSize.Full : OffScreenTextureSize.Half;
            }
        }
        get
        {
            return quality;
        }
    }

    private Texture2DDescription ssaoTextureDesc = new()
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

    private ShaderResourceViewProxy? ssaoView, ssaoNoiseView;
    private int width, height;
    private int ssaoTexSlot, noiseTexSlot, surfaceSampleSlot, noiseSamplerSlot, depthSlot;
    private ShaderPass? ssaoPass, ssaoBlur;
    private SamplerStateProxy? surfaceSampler, noiseSampler, blurSampler;
    private SSAOParamStruct ssaoParam;
    private readonly ConstantBufferComponent ssaoCB;
    private const int KernalSize = 32;
    private readonly Vector4[] kernels = new Vector4[KernalSize];
    private const global::SharpDX.DXGI.Format DEPTHFORMAT = global::SharpDX.DXGI.Format.D32_Float;
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
        var viewport = context.Viewport;
        using var ds = context.GetOffScreenDS(offScreenTextureSize, DEPTHFORMAT);
        using var rt0 = context.GetOffScreenRT(offScreenTextureSize, RENDERTARGETFORMAT);
        using var rt1 = context.GetOffScreenRT(offScreenTextureSize, SSAOTARGETFORMAT);
        var w = (int)(context.ActualWidth / texScale);// Make sure to set correct viewport width/height by quality
        var h = (int)(context.ActualHeight / texScale);
        deviceContext.SetRenderTarget(ds, rt0, true, new Color4(0, 0, 0, 1), true, DepthStencilClearFlags.Depth);
        deviceContext.SetViewport(0, 0, w, h);
        deviceContext.SetScissorRectangle(0, 0, w, h);
        IRenderTechnique? currTechnique = null;
        ShaderPass? ssaoPass1 = ShaderPass.NullPass;
        var frustum = context.BoundingFrustum;
        for (var i = 0; i < context.RenderHost.PerFrameOpaqueNodesInFrustum.Count; ++i)
        {
            var node = context.RenderHost.PerFrameOpaqueNodesInFrustum[i];
            if (currTechnique != node.EffectTechnique)
            {
                currTechnique = node?.EffectTechnique;
                ssaoPass1 = currTechnique?[DefaultPassNames.MeshSSAOPass];
            }
            if (ssaoPass1 is null || ssaoPass1.IsNULL)
            {
                continue;
            }
            node?.RenderDepth(context, deviceContext, ssaoPass1);
        }

        var invProjection = context.ProjectionMatrix.Inverted();
        ssaoParam.InvProjection = invProjection;
        ssaoParam.NoiseScale = new Vector2(w / 4f, h / 4f);
        ssaoParam.Radius = radius;
        ssaoParam.TextureScale = texScale;
        ssaoCB.ModelConstBuffer?.UploadDataToBuffer(deviceContext, (dataBox) =>
        {
            Debug.Assert(UnsafeHelper.SizeOf(kernels)
                + UnsafeHelper.SizeOf(ref ssaoParam) <= ssaoCB.ModelConstBuffer.bufferDesc.SizeInBytes);
            var nextPtr = UnsafeHelper.Write(dataBox.DataPointer, kernels, 0, kernels.Length);
            UnsafeHelper.Write(nextPtr, ref ssaoParam);
        });
        deviceContext.SetRenderTarget(rt1);

        if (ssaoPass is null || ssaoBlur is null || context.SharedResource is null)
        {
            return;
        }

        ssaoPass.BindShader(deviceContext);
        ssaoPass.BindStates(deviceContext, StateType.All);
        ssaoPass.PixelShader.BindTexture(deviceContext, ssaoTexSlot, rt0);
        ssaoPass.PixelShader.BindTexture(deviceContext, noiseTexSlot, ssaoNoiseView);
        ssaoPass.PixelShader.BindTexture(deviceContext, depthSlot, ds);
        ssaoPass.PixelShader.BindSampler(deviceContext, surfaceSampleSlot, surfaceSampler);
        ssaoPass.PixelShader.BindSampler(deviceContext, noiseSamplerSlot, noiseSampler);
        deviceContext.Draw(4, 0);

        ssaoPass.PixelShader.BindTexture(deviceContext, depthSlot, null);

        deviceContext.SetRenderTarget(ssaoView);
        deviceContext.SetViewport(ref viewport);
        deviceContext.SetScissorRectangle(ref viewport);
        ssaoBlur.BindShader(deviceContext);
        ssaoBlur.BindStates(deviceContext, StateType.All);
        ssaoBlur.PixelShader.BindTexture(deviceContext, ssaoTexSlot, rt1);
        ssaoBlur.PixelShader.BindSampler(deviceContext, surfaceSampleSlot, blurSampler);
        deviceContext.Draw(4, 0);
        context.SharedResource.SSAOMap = ssaoView;

        context.RenderHost.SetDefaultRenderTargets(false);
        deviceContext.SetShaderResource(PixelShader.Type, ssaoTexSlot, ssaoView);
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
                ssaoView = new ShaderResourceViewProxy(deviceContext, ssaoTextureDesc);
                ssaoView.CreateTextureView();
                ssaoView.CreateRenderTargetView();
            }
        }
    }

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        if (technique is null || technique.IsNull)
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
        surfaceSampler = technique.EffectsManager?.StateManager?.Register(DefaultSamplers.SSAOSamplerClamp);
        noiseSampler = technique.EffectsManager?.StateManager?.Register(DefaultSamplers.SSAONoise);
        blurSampler = technique.EffectsManager?.StateManager?.Register(DefaultSamplers.LinearSamplerClampAni1);
        InitialParameters();
        return true;
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref surfaceSampler);
        RemoveAndDispose(ref noiseSampler);
        RemoveAndDispose(ref blurSampler);
        RemoveAndDispose(ref ssaoView);
        RemoveAndDispose(ref ssaoNoiseView);
    }

    private void InitialParameters()
    {
        ssaoParam.Radius = radius;
        var rnd = new Random((int)Stopwatch.GetTimestamp());
        var thres = Math.Cos(Math.PI / 2 - Math.PI / 12);
        for (var i = 0; i < 32; ++i)
        {
            while (true)
            {
                var x = rnd.NextFloat(-1, 1);
                var y = rnd.NextFloat(-1, 1);
                var z = rnd.NextFloat(1e-3f, 1);
                var v = Vector3.Normalize(new Vector3(x, y, z));
                var angle = Vector3.Dot(v, Vector3.UnitZ);
                if (Vector3.Dot(v, Vector3.UnitZ) < thres)
                {
                    continue;
                }
                var scale = i / 32f;
                scale = 0.1f + 0.9f * scale;
                v *= scale;
                kernels[i] = new Vector4(v.X, v.Y, v.Z, 0);
                break;
            }
        }

        var noise = new Vector3[4 * 4];
        for (var i = 0; i < 16; ++i)
        {
            var x = rnd.NextFloat(-1, 1);
            var y = rnd.NextFloat(-1, 1);
            noise[i] = Vector3.Normalize(new Vector3(x, y, 0));
        }
        ssaoNoiseView = Device is null ? null : ShaderResourceViewProxy
            .CreateView(Device, noise, 4, 4, global::SharpDX.DXGI.Format.R32G32B32_Float, true, false);
    }
}
