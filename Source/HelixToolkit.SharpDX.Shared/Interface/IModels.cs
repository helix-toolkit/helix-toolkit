/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.IO;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core;
    using global::SharpDX.Direct3D11;
    using System;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public interface IBillboardText
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        BillboardType Type { get; }
        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        Stream Texture { get; }

        /// <summary>
        /// Draws the texture.
        /// </summary>
        /// <param name="devices">The devices.</param>
        void DrawTexture(IDeviceResources devices);
        /// <summary>
        /// Gets the billboard vertices.
        /// </summary>
        /// <value>
        /// The billboard vertices.
        /// </value>
        IList<BillboardVertex> BillboardVertices { get; }
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        float Width { get; }
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        float Height { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum BillboardType
    {
        SingleText = 1, MultipleText = 2, SingleImage = 4
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILightsBufferProxy<T> where T : struct
    {
        /// <summary>
        /// Gets the size of the buffer.
        /// </summary>
        /// <value>
        /// The size of the buffer.
        /// </value>
        int BufferSize { get; }
        /// <summary>
        /// Gets the light array
        /// </summary>
        /// <value>
        /// The lights.
        /// </value>
        T[] Lights { get; }
        /// <summary>
        /// Gets or sets the ambient light.
        /// </summary>
        /// <value>
        /// The ambient light.
        /// </value>
        Color4 AmbientLight { set; get; }
        /// <summary>
        /// Gets the light count.
        /// </summary>
        /// <value>
        /// The light count.
        /// </value>
        int LightCount { get; }
        /// <summary>
        /// Resets the light count. Must call before calling light render
        /// </summary>
        void ResetLightCount();
        /// <summary>
        /// Increments the light count. Increment during each light render (except Ambient light). <see cref="LightCoreBase.Render(IRenderContext, Render.DeviceContextProxy)"/>
        /// </summary>
        void IncrementLightCount();
        /// <summary>
        /// Upload light models to constant buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="context">The context.</param>
        void UploadToBuffer(IBufferProxy buffer, DeviceContext context);
    }
}
