using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Shaders;
    /// <summary>
    /// 
    /// </summary>
    public class DeviceContextProxy : DisposeObject, IDeviceContext
    {
        private DeviceContext deviceContext;
        /// <summary>
        /// 
        /// </summary>
        public DeviceContext DeviceContext { get { return deviceContext; } }
        /// <summary>
        /// Gets or sets the last shader pass.
        /// </summary>
        /// <value>
        /// The last shader pass.
        /// </value>
        public IShaderPass LastShaderPass { set; get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContextProxy"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DeviceContextProxy(Device device)
        {
            deviceContext = Collect(new DeviceContext(device));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContextProxy"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public DeviceContextProxy(DeviceContext context)
        {
            this.deviceContext = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        public void SetRenderTargets(IDX11RenderBufferProxy buffer)
        {
            buffer.SetDefaultRenderTargets(deviceContext);
        }
        /// <summary>
        /// Clears the render targets.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="color">The color.</param>
        public void ClearRenderTargets(IDX11RenderBufferProxy buffer, Color4 color)
        {
            buffer.ClearRenderTarget(deviceContext, color);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearRenderTagetBindings()
        {
            deviceContext.OutputMerger.ResetTargets();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void Dispose(bool disposeManagedResources)
        {
            ClearRenderTagetBindings();
            base.Dispose(disposeManagedResources);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        public static implicit operator DeviceContext(DeviceContextProxy proxy)
        {
            return proxy.DeviceContext;
        }
    }
}
