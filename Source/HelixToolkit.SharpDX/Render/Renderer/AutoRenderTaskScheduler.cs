using HelixToolkit.SharpDX.Model.Scene;
using SharpDX.Direct3D11;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Render;

/// <summary>
/// 
/// </summary>
public class AutoRenderTaskScheduler : IRenderTaskScheduler
{
    private readonly AutoTaskSchedulerParameter schedulerParams;
    /// <summary>
    /// Values the tuple.
    /// </summary>
    /// <returns></returns>
    public AutoRenderTaskScheduler()
    {
        schedulerParams = new AutoTaskSchedulerParameter();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoRenderTaskScheduler"/> class.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    public AutoRenderTaskScheduler(AutoTaskSchedulerParameter parameter)
    {
        this.schedulerParams = parameter;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="items"></param>
    /// <param name="pool"></param>
    /// <param name="context"></param>
    /// <param name="parameter"></param>
    /// <param name="outputCommands"></param>
    /// <param name="numRendered"></param>
    /// <param name="testFrustum"></param>
    /// <returns></returns>
    public bool ScheduleAndRun(FastList<SceneNode> items, IDeviceContextPool pool,
        RenderContext context, RenderParameter parameter, bool testFrustum,
        List<KeyValuePair<int, CommandList>> outputCommands, out int numRendered)
    {
        outputCommands.Clear();
        var totalCount = 0;
        numRendered = 0;
        Exception? exception = null;
        if (items.Count > schedulerParams.MinimumDrawCalls)
        {
            var frustum = context.BoundingFrustum;
            var partitionParams = Partitioner.Create(0, items.Count, items.Count / schedulerParams.MaxNumberOfTasks + 1);
            Parallel.ForEach(partitionParams, (range, state) =>
            {
                try
                {
                    var counter = 0;
                    var deferred = pool.Get();
                    SetRenderTargets(deferred, ref parameter);
                    if (!testFrustum)
                    {
                        for (var i = range.Item1; i < range.Item2; ++i)
                        {
                            items[i].Render(context, deferred);
                            ++counter;
                        }
                    }
                    else
                    {
                        for (var i = range.Item1; i < range.Item2; ++i)
                        {
                            if (context.EnableBoundingFrustum && !items[i].TestViewFrustum(ref frustum))
                            {
                                continue;
                            }
                            items[i].Render(context, deferred);
                            ++counter;
                        }
                    }

                    var command = deferred.FinishCommandList(true);
                    pool.Put(deferred);
                    lock (outputCommands)
                    {
                        outputCommands.Add(new KeyValuePair<int, CommandList>(range.Item1, command!));
                        totalCount += counter;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            numRendered = totalCount;
            if (exception != null)
            {
                throw exception;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Sets the render targets.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="parameter">The parameter.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetRenderTargets(DeviceContextProxy context, ref RenderParameter parameter)
    {
        context.SetRenderTargets(parameter.DepthStencilView, parameter.RenderTargetView);
        context.SetViewport(ref parameter.ViewportRegion);
        context.SetScissorRectangle(parameter.ScissorRegion.Left, parameter.ScissorRegion.Top,
            parameter.ScissorRegion.Right, parameter.ScissorRegion.Bottom);
    }
}
