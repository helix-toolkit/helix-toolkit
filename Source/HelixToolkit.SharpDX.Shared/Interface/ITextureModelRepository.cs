/*
The MIT License (MIT)
Copyright (c) 2021 Helix Toolkit contributors
*/
using System.IO;
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
    public interface ITextureFileLoader
    {
        Stream Load(string texturePath);
    }

    public interface ITextureModelRepository
    {
        /// <summary>
        /// Loads texture from a specified texture file path.
        /// </summary>
        /// <param name="texturePath">The texture file path.</param>
        /// <returns></returns>
        Stream Load(string texturePath);

        /// <summary>
        /// Loads texture from a specified stream such as memory stream or file stream
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        TextureModel Create(Stream stream);
    }
}
