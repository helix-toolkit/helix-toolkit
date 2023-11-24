using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Render;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// Use this node to keep update rendering in each frame.
/// <para>Default behavior for render host is lazy rendering. 
/// Only property changes trigger a render inside render loop. 
/// However, sometimes user want to keep updating rendering for each frame such as doing shader animation using TimeStamp.
/// Use this node to invalidate rendering and keep render host busy.</para>
/// </summary>
public sealed class ContinuousRenderNode : SceneNode
{
    protected override RenderCore OnCreateRenderCore()
    {
        return new InvalidRendererCore();
    }

    protected override bool CanHitTest(HitTestContext? context)
    {
        return false;
    }

    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        return false;
    }

    private sealed class InvalidRendererCore : RenderCore
    {
        public InvalidRendererCore() : base(RenderType.GlobalEffect) { }

        public override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
            RaiseInvalidateRender();
        }

        protected override bool OnAttach(IRenderTechnique? technique)
        {
            return true;
        }

        protected override void OnDetach()
        {
        }
    }
}
