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
    using System.IO;
    using Utilities;
    using Model;
    public interface ITextureResourceManager
    {
        int Count { get; }
        /// <summary>
        /// Registers the specified texture stream. This creates mipmaps automatically
        /// </summary>
        /// <param name="textureStream">The texture stream.</param>
        /// <returns></returns>
        ShaderResourceViewProxy Register(TextureModel textureStream);
        /// <summary>
        /// Registers the specified texture stream.
        /// </summary>
        /// <param name="textureStream">The texture stream.</param>
        /// <param name="enableAutoGenMipMap">if set to <c>false</c> [disable automatic gen mip map].</param>
        /// <returns></returns>
        ShaderResourceViewProxy Register(TextureModel textureStream, bool enableAutoGenMipMap);
    }
}
