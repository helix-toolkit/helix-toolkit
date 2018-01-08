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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="clear"></param>
        public void SetRenderTargets(IDX11RenderBufferProxy buffer, bool clear)
        {
            buffer.SetDefaultRenderTargets(deviceContext, clear);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearRenderTagets()
        {
            deviceContext.OutputMerger.ResetTargets();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void Dispose(bool disposeManagedResources)
        {
            ClearRenderTagets();
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
