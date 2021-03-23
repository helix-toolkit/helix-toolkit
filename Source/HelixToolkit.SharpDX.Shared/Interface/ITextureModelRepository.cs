/*
The MIT License (MIT)
Copyright (c) 2021 Helix Toolkit contributors
*/
using System;
using System.IO;
using SharpDX;
using SharpDX.DXGI;
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
    /// Used to cache texture models. Reuse existing texture model to avoid duplicate texture loading.
    /// </summary>
    public interface ITextureModelRepository
    {
        /// <summary>
        /// Creates texture model from a specified stream such as memory stream or file stream.
        /// <para>This is used for implicit conversion from a Stream to TextureModel</para>
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        TextureModel Create(Stream stream);
        /// <summary>
        /// Creates texture model from a specified texture path
        /// </summary>
        /// <param name="texturePath">The texture path.</param>
        /// <returns></returns>
        TextureModel Create(string texturePath);
    }

    /// <summary>
    /// Loads texture info and uploads texture to GPU on demand.
    /// </summary>
    public interface ITextureInfoLoader
    {
        /// <summary>
        /// Called before GPU texture resource creation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        TextureInfo Load(Guid id);
        /// <summary>
        /// Called after GPU texture resource creation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="info">The information.</param>
        /// <param name="succeeded">if set to <c>true</c> [succeeded].</param>
        void Complete(Guid id, TextureInfo info, bool succeeded);
    }
}
