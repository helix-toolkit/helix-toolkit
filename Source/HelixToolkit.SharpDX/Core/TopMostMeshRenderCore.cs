using HelixToolkit.SharpDX.Render;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// Clears the depth buffer and reset global transform.
/// </summary>
public class TopMostMeshRenderCore : RenderCore
{
    public TopMostMeshRenderCore() : base(RenderType.ScreenSpaced)
    {
    }

    public override void Render(RenderContext context, DeviceContextProxy deviceContext)
    {
        if (RenderType != RenderType.ScreenSpaced)
        {
            return;
        }
        deviceContext.GetDepthStencilView(out var dsView);
        if (dsView == null)
        {
            return;
        }

        deviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Depth, 1f, 0);
        dsView.Dispose();
        context.RestoreGlobalTransform();
        context.UpdatePerFrameData(true, false, deviceContext);
        deviceContext.SetViewport(context.Viewport.X, context.Viewport.Y, context.Viewport.Width, context.Viewport.Height);
        deviceContext.SetScissorRectangle((int)context.Viewport.X, (int)context.Viewport.Y,
            (int)context.Viewport.Width, (int)context.Viewport.Height);
    }

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        return true;
    }

    protected override void OnDetach()
    {
    }
}
