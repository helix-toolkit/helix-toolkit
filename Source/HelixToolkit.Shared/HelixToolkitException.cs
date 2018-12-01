// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelixToolkitException.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents errors that occurs in the Helix 3D Toolkit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
#if SHARPDX
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
#else
namespace HelixToolkit.Wpf
#endif
{
    using System;

#pragma warning disable 0436
    /// <summary>
    /// Represents errors that occurs in the Helix 3D Toolkit.
    /// </summary>
#if !NETFX_CORE
    [Serializable]
#endif
    public class HelixToolkitException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelixToolkitException"/> class.
        /// </summary>
        /// <param name="formatString">
        /// The format string.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public HelixToolkitException(string formatString, params object[] args)
            : base(string.Format(formatString, args))
        {
        }
    }
#pragma warning restore 0436
}