// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextureLoader.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using SharpDX.Direct3D11;
using System.IO;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// Utilities to load textures.
    /// </summary>
    public static class TextureLoader
    {
        /// <summary>
        /// Loads a texture from a file as a resource.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns></returns>
        public static Resource FromFileAsResource(Device device, string fileName)
        {
            return global::SharpDX.Toolkit.Graphics.Texture.Load(device, fileName);
        }

        /// <summary>
        /// Loads a texture from a file as a shader resource view.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns></returns>
        public static ShaderResourceView FromFileAsShaderResourceView(Device device, string fileName)
        {
            using (var texture = global::SharpDX.Toolkit.Graphics.Texture.Load(device, fileName))
            {
                return new ShaderResourceView(device, texture);
            }
        }

        /// <summary>
        /// Loads a texture from a memory buffer as a shader resource view.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="memory">The memory buffer.</param>
        /// <returns></returns>
        public static ShaderResourceView FromMemoryAsShaderResourceView(Device device, byte[] memory)
        {
            using (var stream = new MemoryStream(memory))
            using (var texture = global::SharpDX.Toolkit.Graphics.Texture.Load(device, stream))
            {
                return new ShaderResourceView(device, texture);
            }
        }
    }
}
