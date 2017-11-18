using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public interface IRenderCore : IGUID
    {
        event EventHandler<bool> OnInvalidateRenderer;
        /// <summary>
        /// If render core is attached or not
        /// </summary>
        bool IsAttached { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="technique"></param>
        void Attach(IRenderHost host, RenderTechnique technique);
        /// <summary>
        /// 
        /// </summary>
        void Detach();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Render(IRenderMatrices context);
        /// <summary>
        /// Unsubscribe all OnInvalidateRenderer event handler;
        /// </summary>
        void ResetInvalidateHandler();
    }
}
