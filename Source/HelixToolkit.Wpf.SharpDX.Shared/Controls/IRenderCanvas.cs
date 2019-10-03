#if COREWPF
using HelixToolkit.SharpDX.Core;
using RelayExceptionEventArgs = HelixToolkit.SharpDX.Core.Utilities.RelayExceptionEventArgs;
#endif
namespace HelixToolkit.Wpf.SharpDX
{
    namespace Controls
    {
        using System;
        using Utilities;

        /// <summary>
        /// Canvas holds the RenderHost. Provide entry point or render surface for RenderHost to render to.
        /// </summary>
        public interface IRenderCanvas
        {
            /// <summary>
            /// Gets the render host.
            /// </summary>
            /// <value>
            /// The render host.
            /// </value>
            IRenderHost RenderHost { get; }

            /// <summary>
            /// Fired whenever an exception occurred on this object.
            /// </summary>
            event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;
        }
    }

}
