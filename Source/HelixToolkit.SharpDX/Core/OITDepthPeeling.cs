using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Diagnostics;

namespace HelixToolkit.SharpDX.Core;

public sealed class OITDepthPeeling : RenderCore
{
    private int currWidth = 0, currHeight = 0;
    private ShaderResourceViewProxy? minMaxZTarget0, minMaxZTarget1, frontBlendingTarget, backBlendingTarget;
    private readonly ShaderResourceViewProxy?[] minMaxZTargets = new ShaderResourceViewProxy?[2];
    private readonly RenderTargetView?[] targets = new RenderTargetView?[3];
    private readonly ShaderResourceView?[] finalSRVs = new ShaderResourceView?[3];
    private ShaderPass? finalPass = ShaderPass.NullPass;
    public RenderParameter ExternRenderParameter
    {
        set; get;
    }
    public int RenderCount { private set; get; }

    public int PeelingIteration { set; get; } = 4;

    public OITDepthPeeling() : base(RenderType.Transparent) { }

    private bool CreateRenderTargets(int width, int height)
    {
        if (currWidth == width && currHeight == height)
        {
            return false;
        }
        DisposeAllTargets();
        currWidth = width;
        currHeight = height;
        var tex2DDesc = new Texture2DDescription
        {
            Width = width,
            Height = height,
            ArraySize = 1,
            MipLevels = 1,
            SampleDescription = new SampleDescription(1, 0),
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None
        };

        if (Device is not null)
        {
            tex2DDesc.Format = Format.R32G32_Float;
            minMaxZTarget0 = new ShaderResourceViewProxy(Device, tex2DDesc);
            minMaxZTarget0.CreateRenderTargetView();
            minMaxZTarget0.CreateTextureView();
            minMaxZTarget1 = new ShaderResourceViewProxy(Device, tex2DDesc);
            minMaxZTarget1.CreateRenderTargetView();
            minMaxZTarget1.CreateTextureView();

            minMaxZTargets[0] = minMaxZTarget0;
            minMaxZTargets[1] = minMaxZTarget1;
            tex2DDesc.Format = Format.B8G8R8A8_UNorm;

            frontBlendingTarget = new ShaderResourceViewProxy(Device, tex2DDesc);
            frontBlendingTarget.CreateRenderTargetView();
            frontBlendingTarget.CreateTextureView();
            backBlendingTarget = new ShaderResourceViewProxy(Device, tex2DDesc);
            backBlendingTarget.CreateRenderTargetView();
            backBlendingTarget.CreateTextureView();
        }

        return true;
    }

    private void DisposeAllTargets()
    {
        RemoveAndDispose(ref minMaxZTarget0);
        RemoveAndDispose(ref minMaxZTarget1);
        RemoveAndDispose(ref frontBlendingTarget);
        RemoveAndDispose(ref backBlendingTarget);
    }

    private void InitializeMinMaxRenderTarget(DeviceContextProxy deviceContext)
    {
        var color = new Color4(0, 0, 0, 1);
        deviceContext.ClearRenderTargetView(frontBlendingTarget, color);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        if (ExternRenderParameter.RenderTargetView is not null &&
            ExternRenderParameter.RenderTargetView.Length > 0 && 
            backBlendingTarget.Resource is not null)
        {
            if (ExternRenderParameter.IsMSAATexture)
            {

                deviceContext.ResolveSubresource(ExternRenderParameter.RenderTargetView[0].Resource, 0, backBlendingTarget.Resource, 0, Format.B8G8R8A8_UNorm);
            }
            else
            {

                deviceContext.CopyResource(ExternRenderParameter.RenderTargetView[0].Resource, backBlendingTarget.Resource);
            }
        }
        else
        {
            color = new Color4(0, 0, 0, 0);
            deviceContext.ClearRenderTargetView(backBlendingTarget, color);
        }
        color = new Color4(-1, -1, 0, 0);
        deviceContext.ClearRenderTargetView(minMaxZTargets[0], color);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    private void DrawMesh(RenderContext context, DeviceContextProxy deviceContext)
    {
        var parameter = ExternRenderParameter;
        if (!parameter.ScissorRegion.IsEmpty)
        {
            RenderCount = context.RenderHost.Renderer?.
                RenderOpaque(context, context.RenderHost.PerFrameTransparentNodes, ref parameter, context.EnableBoundingFrustum) ?? 0;
        }
        else
        {
            var count = context.RenderHost.PerFrameTransparentNodes.Count;
            for (var i = 0; i < count; ++i)
            {
                var renderable = context.RenderHost.PerFrameTransparentNodes[i];
                renderable.RenderCore.Render(context, deviceContext);
                ++RenderCount;
            }
        }
    }

    public override void Render(RenderContext context, DeviceContextProxy deviceContext)
    {
        if (CreateRenderTargets((int)context.ActualWidth, (int)context.ActualHeight))
        {
            RaiseInvalidateRender();
            return;
        }
        var buffer = context.RenderHost.RenderBuffer;
        if (buffer is null)
        {
            return;
        }
        var hasMSAA = buffer.ColorBufferSampleDesc.Count > 1;
        var nonMSAADepthBuffer = hasMSAA ? context.RenderHost.RenderBuffer?.DepthStencilBufferNoMSAA : null;
        var depthStencilView = hasMSAA ? nonMSAADepthBuffer : ExternRenderParameter.DepthStencilView;

        RenderCount = 0;
        InitializeMinMaxRenderTarget(deviceContext);
        context.OITRenderStage = OITRenderStage.DepthPeelingInitMinMaxZ;
        deviceContext.SetRenderTarget(depthStencilView, minMaxZTargets[0]);
        DrawMesh(context, deviceContext);

        context.OITRenderStage = OITRenderStage.DepthPeeling;
        var currId = 0;
        for (var layer = 1; layer < PeelingIteration; ++layer)
        {
            currId = layer % 2;
            var prevId = 1 - currId;
            var color = new Color4(-1, -1, 0, 0);
            deviceContext.ClearRenderTargetView(minMaxZTargets[currId], color);
            targets[0] = minMaxZTargets[currId];
            targets[1] = frontBlendingTarget;
            targets[2] = backBlendingTarget;
            deviceContext.SetRenderTargets(depthStencilView, targets);
            deviceContext.SetShaderResource(new PixelShaderType(), 100, minMaxZTargets[prevId]);
            DrawMesh(context, deviceContext);
            deviceContext.SetShaderResource(new PixelShaderType(), 100, null);
        }
        context.OITRenderStage = OITRenderStage.None;
        finalSRVs[0] = minMaxZTargets[currId];
        finalSRVs[1] = frontBlendingTarget;
        finalSRVs[2] = backBlendingTarget;
        finalPass?.BindShader(deviceContext);
        finalPass?.BindStates(deviceContext, StateType.All);
        deviceContext.SetRenderTargets(null, ExternRenderParameter.RenderTargetView);
        deviceContext.SetShaderResources(new PixelShaderType(), 100, finalSRVs);
        deviceContext.Draw(4, 0);
    }

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        finalPass = technique?[DefaultPassNames.OITDepthPeelingFinal];
        Debug.Assert(finalPass is not null && !finalPass.IsNULL);
        return true;
    }

    protected override void OnDetach()
    {
        DisposeAllTargets();
        currWidth = currHeight = 0;
        finalPass = ShaderPass.NullPass;
    }
}
