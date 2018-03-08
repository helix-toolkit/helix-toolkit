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
    using Utilities;
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
        /// <summary>
        /// 
        /// </summary>
        RasterizerStateProxy LastRasterState { get; }
        /// <summary>
        /// 
        /// </summary>
        DepthStencilStateProxy LastDepthStencilState { get; }
        /// <summary>
        /// 
        /// </summary>
        BlendStateProxy LastBlendState { get; }
        /// <summary>
        /// Sets the state of the raster.
        /// </summary>
        /// <param name="rasterState">State of the raster.</param>
        void SetRasterState(RasterizerStateProxy rasterState);
        /// <summary>
        /// Sets the state of the depth stencil.
        /// </summary>
        /// <param name="depthStencilState">State of the depth stencil.</param>
        void SetDepthStencilState(DepthStencilStateProxy depthStencilState);
        /// <summary>
        /// Sets the state of the blend.
        /// </summary>
        /// <param name="blendState">State of the blend.</param>
        void SetBlendState(BlendStateProxy blendState);
        /// <summary>
        /// 
        /// </summary>
        void ResetLastHistory();
    }
}
