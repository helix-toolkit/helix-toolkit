using System;
using System.Collections.Generic;
using System.Text;

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
    /// <summary>
    /// 
    /// </summary>
    public static class IViewportExtensions
    {
        /// <summary>
        /// Forces to update transform and bounds.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        public static void ForceUpdateTransformsAndBounds(this IViewport3DX viewport)
        {
            viewport.Renderables.ForceUpdateTransformsAndBounds();
        }
    }
}
