﻿using HelixToolkit.SharpDX.Model.Scene;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;

namespace HelixToolkit.SharpDX.Render;

/// <summary>
/// 
/// </summary>
public class DeferredContextRenderer : ImmediateContextRenderer
{
    private IDeviceContextPool? deferredContextPool;
    private readonly IRenderTaskScheduler scheduler;
    private readonly List<KeyValuePair<int, CommandList>> commandList = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DeferredContextRenderer"/> class.
    /// </summary>
    /// <param name="deviceResources">The deviceResources.</param>
    /// <param name="scheduler"></param>
    public DeferredContextRenderer(IDevice3DResources deviceResources, IRenderTaskScheduler scheduler) : base(deviceResources)
    {
        deferredContextPool = deviceResources.DeviceContextPool;
        this.scheduler = scheduler;
    }

    /// <summary>
    /// Renders the scene.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="renderables">The renderables.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="testFrustum"></param>
    /// <returns>Number of node has been rendered</returns>
    public override int RenderOpaque(RenderContext context, FastList<SceneNode> renderables,
        ref RenderParameter parameter, bool testFrustum)
    {
        if (ImmediateContext is null || deferredContextPool is null)
        {
            return 0;
        }

        if (scheduler.ScheduleAndRun(renderables, deferredContextPool, context, parameter,
            testFrustum, commandList, out var counter))
        {
            var param = parameter;

            foreach (var command in commandList.OrderBy(x => x.Key))
            {
                ImmediateContext.ExecuteCommandList(command.Value, true);
                command.Value.Dispose();
            }
            commandList.Clear();
            return counter;
        }
        else
        {
            return base.RenderOpaque(context, renderables, ref parameter, testFrustum);
        }
    }


    private void SetRenderTargets(DeviceContext context, ref RenderParameter parameter)
    {
        context.OutputMerger.SetTargets(parameter.DepthStencilView, parameter.RenderTargetView);
        context.Rasterizer.SetViewport(parameter.ViewportRegion.ToStruct<ViewportF, RawViewportF>());
        context.Rasterizer.SetScissorRectangle(parameter.ScissorRegion.Left, parameter.ScissorRegion.Top,
            parameter.ScissorRegion.Right, parameter.ScissorRegion.Bottom);
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        commandList.Clear();
        deferredContextPool = null;
        base.OnDispose(disposeManagedResources);
    }
}
