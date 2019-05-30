/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using System.Threading.Tasks;
using global::SharpDX.Direct3D11;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Render
    {
        using Core;           
        using Model.Scene;
    

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
            /// <param name="outputCommands"></param>
            /// <param name="numRendered"></param>
            /// <param name="testFrustum"></param>
            /// <returns></returns>
            public bool ScheduleAndRun(FastList<SceneNode> items, IDeviceContextPool pool,
                RenderContext context, RenderParameter parameter, bool testFrustum,
                List<KeyValuePair<int, CommandList>> outputCommands, out int numRendered)
            {
                outputCommands.Clear();
                int totalCount = 0;
                numRendered = 0;
                Exception exception = null;
                if (items.Count > schedulerParams.MinimumDrawCalls)
                {
                    var frustum = context.BoundingFrustum;
                    var partitionParams = Partitioner.Create(0, items.Count, items.Count/schedulerParams.MaxNumberOfTasks+1);
                    Parallel.ForEach(partitionParams, (range, state) =>
                    {
                        try
                        {
                            int counter = 0;                    
                            var deferred = pool.Get();
                            SetRenderTargets(deferred, ref parameter);
                            if (!testFrustum)
                            {
                                for (int i = range.Item1; i < range.Item2; ++i)
                                {
                                    items[i].RenderCore.Render(context, deferred);
                                    ++counter;
                                }
                            }
                            else
                            {
                                for (int i = range.Item1; i < range.Item2; ++i)
                                {
                                    if (context.EnableBoundingFrustum && !items[i].TestViewFrustum(ref frustum))
                                    {
                                        continue;
                                    }
                                    items[i].RenderCore.Render(context, deferred);
                                    ++counter;
                                }
                            }

                            var command = deferred.FinishCommandList(true);
                            pool.Put(deferred);
                            lock (outputCommands)
                            {
                                outputCommands.Add(new KeyValuePair<int, CommandList>(range.Item1, command));
                                totalCount += counter;
                            }
                        }
                        catch(Exception ex)
                        {
                            exception = ex;
                        }
                    });
                    numRendered = totalCount;
                    if(exception != null)
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
    }

}
