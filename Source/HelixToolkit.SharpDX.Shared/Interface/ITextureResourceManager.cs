using System;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using System.IO;
    using Utilities;

    public interface ITextureResourceManager
    {
        int Count { get; }
        /// <summary>
        /// Registers the specified texture stream. This creates mipmaps automatically
        /// </summary>
        /// <param name="textureStream">The texture stream.</param>
        /// <returns></returns>
        ShaderResourceViewProxy Register(Stream textureStream);
        /// <summary>
        /// Registers the specified texture stream.
        /// </summary>
        /// <param name="textureStream">The texture stream.</param>
        /// <param name="disableAutoGenMipMap">if set to <c>true</c> [disable automatic gen mip map].</param>
        /// <returns></returns>
        ShaderResourceViewProxy Register(Stream textureStream, bool disableAutoGenMipMap);
    }
}
