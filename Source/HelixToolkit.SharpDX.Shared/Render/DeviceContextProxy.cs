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
    /// <summary>
    /// 
    /// </summary>
    public class DeviceContextProxy : DisposeObject
    {
        private DeviceContext deviceContext;
        /// <summary>
        /// 
        /// </summary>
        public DeviceContext DeviceContext { get { return deviceContext; } }

        public DeviceContextProxy(Device device)
        {
            deviceContext = Collect(new DeviceContext(device));
        }

        public DeviceContextProxy(DeviceContext context)
        {
            this.deviceContext = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="clear"></param>
        public void SetRenderTargets(IDX11RenderBufferProxy buffer)
        {
            buffer.SetDefaultRenderTargets(deviceContext);
        }

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
