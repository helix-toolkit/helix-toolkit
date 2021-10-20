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
    public sealed class BoolEventArgs : EventArgs
    {
        /// <summary>
        /// The true arguments
        /// </summary>
        public static readonly BoolEventArgs TrueArgs = new BoolEventArgs(true);
        /// <summary>
        /// The false arguments
        /// </summary>
        public static readonly BoolEventArgs FalseArgs = new BoolEventArgs(false);
        /// <summary>
        /// The value
        /// </summary>
        public bool Value
        {
            get;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolEventArgs"/> class.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public BoolEventArgs(bool value)
        {
            Value = value;
        }
    }
}
