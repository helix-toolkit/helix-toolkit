using System;
using System.Collections.Generic;
using System.Text;
using SharpDX;

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
    namespace Core2D
    {
        /// <summary>
        /// 
        /// </summary>
        public sealed class EmptyRenderCore2D : RenderCore2DBase
        {
            /// <summary>
            /// Called when [render].
            /// </summary>
            /// <param name="matrices">The matrices.</param>
            protected override void OnRender(RenderContext2D matrices)
            {
            
            }
        }
    }

}
