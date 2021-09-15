// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayExceptionEventArgs.cs" company="Helix Toolkit">
//   Copyright (c) 2015 Helix Toolkit contributors
// </copyright>
// <summary>
//   Extended <see cref="EventArgs"/> to relay an <see cref="Exception"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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
    namespace Utilities
    {
        /// <summary>
        /// Extended <see cref="EventArgs"/> to relay an <see cref="Exception"/>.
        /// </summary>
        public class RelayExceptionEventArgs : EventArgs
        {
            /// <summary>
            /// The <see cref="Exception"/> to be relayed.
            /// </summary>
            public Exception Exception { get; private set; }

            /// <summary>
            ///  Gets or sets a value indicating whether the <see cref="Exception"/> is handled.
            /// </summary>
            public bool Handled { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="RelayExceptionEventArgs"/> class.
            /// </summary>
            /// <param name="exception">The <see cref="Exception"/> to be relayed.</param>
            public RelayExceptionEventArgs(Exception exception)
            {
                this.Exception = exception;
                this.Handled = false;
            }
        }
    }
    


}
