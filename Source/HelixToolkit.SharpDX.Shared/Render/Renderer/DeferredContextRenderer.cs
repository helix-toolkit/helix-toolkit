/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;
#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core;
    /// <summary>
    /// 
    /// </summary>
    public class DeferredContextRenderer : ImmediateContextRenderer
    {
        private IDeviceContextPool deferredContextPool;
        private readonly IRenderTaskScheduler scheduler;
        private readonly List<KeyValuePair<int, CommandList>> commandList = new List<KeyValuePair<int, CommandList>>();
        /// <summary>
        /// Initializes a new instance of the <see cref="DeferredContextRenderer"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="scheduler"></param>
        public DeferredContextRenderer(Device device, IRenderTaskScheduler scheduler) : base(device)
        {
            deferredContextPool = Collect(new DeviceContextPool(device));
            this.scheduler = scheduler;
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="renderables">The renderables.</param>
        /// <param name="parameter">The parameter.</param>
        public override void RenderScene(IRenderContext context, List<IRenderCore> renderables, ref RenderParameter parameter)
        {
            if(scheduler.ScheduleAndRun(renderables, deferredContextPool, context, parameter, commandList))
            {
                foreach(var command in commandList.OrderBy(x=>x.Key))
                {
                    ImmediateContext.DeviceContext.ExecuteCommandList(command.Value, false);
                    command.Value.Dispose();
                }
                commandList.Clear();
            }
            else
            {
                base.RenderScene(context, renderables, ref parameter);
            }
        }

        public override void RenderPreProc(IRenderContext context, List<IRenderCore> renderables, ref RenderParameter parameter)
        {
            base.RenderScene(context, renderables, ref parameter);
        }

        public override void RenderPostProc(IRenderContext context, List<IRenderCore> renderables, ref RenderParameter parameter)
        {
            base.RenderScene(context, renderables, ref parameter);
        }
    }
}
