/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX.Direct3D11;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Shaders;
    using System;
    /// <summary>
    /// 
    /// </summary>
    public interface IDeviceContext : IDisposable
    {
        /// <summary>
        /// Gets the device context.
        /// </summary>
        /// <value>
        /// The device context.
        /// </value>
        DeviceContext DeviceContext { get; }
        /// <summary>
        /// Gets or sets the last shader pass.
        /// </summary>
        /// <value>
        /// The last shader pass.
        /// </value>
        IShaderPass LastShaderPass { set; get; }
    }
}
