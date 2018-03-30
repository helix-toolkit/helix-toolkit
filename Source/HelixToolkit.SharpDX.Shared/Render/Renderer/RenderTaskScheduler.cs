/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using System.Threading.Tasks;
#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core;
    using global::SharpDX.Direct3D11;
    using System;
    using System.Collections.Concurrent;

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
        /// <param name="filterType"></param>
        /// <returns></returns>
        bool ScheduleAndRun(List<RenderCore> items, IDeviceContextPool pool,
            IRenderContext context, RenderParameter parameter, RenderType filterType, List<KeyValuePair<int, CommandList>> outputCommands);
    }
    /// <summary>
    /// 
    /// </summary>
    public class AutoTaskSchedulerParameter
    {
        /// <summary>
        /// Gets or sets the number processor.
        /// </summary>
        /// <value>
        /// The number processor.
        /// </value>
        public int NumProcessor { private set; get; }

        /// <summary>
        /// Gets or sets the minimum item to start multi-threading
        /// <para>https://docs.nvidia.com/gameworks/content/gameworkslibrary/graphicssamples/d3d_samples/d3d11deferredcontextssample.htm</para>
        /// <para>Note: Only if draw calls > 3000 to be benefit according to the online performance test.</para>
        /// </summary>
        /// <value>
        /// The minimum item per task.
        /// </value>
        public int MinimumDrawCalls { set; get; } = 600;

        /// <summary>
        /// Gets or sets the maximum number of tasks.
        /// </summary>
        /// <value>
        /// The maximum number of tasks.
        /// </value>
        public int MaxNumberOfTasks { set; get; } = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTaskSchedulerParameter"/> class.
        /// </summary>
        public AutoTaskSchedulerParameter()
        {
            NumProcessor = Environment.ProcessorCount;
        }
    }

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
        /// <param name="filterType"></param>
        /// <param name="outputCommands"></param>
        /// <returns></returns>
        public bool ScheduleAndRun(List<RenderCore> items, IDeviceContextPool pool,
            IRenderContext context, RenderParameter parameter, RenderType filterType, List<KeyValuePair<int, CommandList>> outputCommands)
        {
            outputCommands.Clear();
            if(items.Count > schedulerParams.MinimumDrawCalls)
            {
                var partitionParams = Partitioner.Create(0, items.Count, items.Count/schedulerParams.MaxNumberOfTasks+1);
                Parallel.ForEach(partitionParams, (range, state) =>
                {
                    var deferred = pool.Get();
                    SetRenderTargets(deferred, ref parameter);
                    for(int i=range.Item1; i<range.Item2; ++i)
                    {
                        if (items[i].RenderType == filterType)
                        {
                            items[i].Render(context, deferred);
                        }
                    }
                    var command = deferred.DeviceContext.FinishCommandList(true);
                    pool.Put(deferred);
                    lock (outputCommands)
                    {
                        outputCommands.Add(new KeyValuePair<int, CommandList>(range.Item1, command));
                    }
                });
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Sets the render targets.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parameter">The parameter.</param>
        private void SetRenderTargets(DeviceContext context, ref RenderParameter parameter)
        {
            context.OutputMerger.SetTargets(parameter.DepthStencilView, parameter.RenderTargetView);
            context.Rasterizer.SetViewport(parameter.ViewportRegion);
            context.Rasterizer.SetScissorRectangle(parameter.ScissorRegion.Left, parameter.ScissorRegion.Top,
                parameter.ScissorRegion.Right, parameter.ScissorRegion.Bottom);
        }
    }
}
