using HelixToolkit.SharpDX.Model.Scene;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Render;

/// <summary>
/// 
/// </summary>
public interface IRenderTaskScheduler
{
    /// <summary>
    /// Schedules render tasks and run. 
    /// <para>If return false, meaning the render items cannot be scheduled,
    /// may be the number of items is less than <see cref="AutoTaskSchedulerParameter.MinimumDrawCalls"></see>. 
    /// Use Immediate context to render to achieve better performance.</para>
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="pool">The pool.</param>
    /// <param name="context">The context.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="outputCommands">The output commands.</param>
    /// <param name="testFrustum"></param>
    /// <param name="numRendered"></param>
    /// <returns></returns>
    bool ScheduleAndRun(FastList<SceneNode> items, IDeviceContextPool pool,
        RenderContext context, RenderParameter parameter, bool testFrustum, List<KeyValuePair<int, CommandList>> outputCommands, out int numRendered);
}
