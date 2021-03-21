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

    public struct TextureFileStream
    {
        /// <summary>
        /// Gets or sets the texture stream.
        /// </summary>
        /// <value>
        /// The texture stream.
        /// </value>
        public Stream Stream
        {
            set; get;
        }
        /// <summary>
        /// Gets or sets a value indicating whether [automatic close after loading texture into GPU].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic close after loading texture into GPU]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoCloseAfterLoading
        {
            set; get;
        }
    }

    public interface ITextureFileLoader
    {
        TextureFileStream Load(string texturePath);
    }

    public interface ITextureModelRepository
    {
        /// <summary>
        /// Loads texture from a specified texture file path.
        /// </summary>
        /// <param name="texturePath">The texture file path.</param>
        /// <returns></returns>
        TextureFileStream Load(string texturePath);

        /// <summary>
        /// Loads texture from a specified stream such as memory stream or file stream
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        TextureModel Create(Stream stream);
    }
}
