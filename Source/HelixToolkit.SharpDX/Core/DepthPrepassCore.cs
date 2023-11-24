using HelixToolkit.SharpDX.Render;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// Do a depth prepass before rendering.
/// <para>Must customize the DefaultEffectsManager and set DepthStencilState to DefaultDepthStencilDescriptions.DSSDepthEqualNoWrite in default ShaderPass from EffectsManager to achieve best performance.</para>
/// </summary>
public sealed class DepthPrepassCore : RenderCore
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DepthPrepassCore"/> class.
    /// </summary>
    public DepthPrepassCore() : base(RenderType.PreProc)
    {
    }

    /// <summary>
    /// Called when [render].
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="deviceContext">The device context.</param>
    public override void Render(RenderContext context, DeviceContextProxy deviceContext)
    {
        context.CustomPassName = DefaultPassNames.DepthPrepass;
        for (var i = 0; i < context.RenderHost.PerFrameOpaqueNodesInFrustum.Count; ++i)
        {
            context.RenderHost.PerFrameOpaqueNodesInFrustum[i].RenderDepth(context, deviceContext, null);
        }
    }

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        return true;
    }

    protected override void OnDetach()
    {
    }
}
