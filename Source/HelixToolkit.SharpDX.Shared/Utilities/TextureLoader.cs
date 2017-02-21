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
        /// <param name="disableAutoGenMipMap"></param>
        /// <returns></returns>
        public static ShaderResourceView FromFileAsShaderResourceView(Device device, string fileName, bool disableAutoGenMipMap = false)
        {
            using (var texture = global::SharpDX.Toolkit.Graphics.Texture.Load(device, fileName))
            {
                if (!disableAutoGenMipMap && texture.Description.MipLevels == 1)// Check if it already has mipmaps or not, if loaded DDS file, it may already has precompiled mipmaps, don't need to generate again
                {
                    using (var textureMipmap = GenerateMipMaps(device, texture))
                    {
                        return new ShaderResourceView(device, textureMipmap);
                    }
                }
                else
                {
                    return new ShaderResourceView(device, texture);
                }
            }
        }

        /// <summary>
        /// Loads a texture from a memory buffer as a shader resource view.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="memory">The memory buffer.</param>
        /// <param name="disableAutoGenMipMap"></param>
        /// <returns></returns>
        public static ShaderResourceView FromMemoryAsShaderResourceView(Device device, byte[] memory, bool disableAutoGenMipMap = false)
        {
            using(var memStream = new MemoryStream(memory))
            {
                return FromMemoryAsShaderResourceView(device, memStream, disableAutoGenMipMap);
            }
        }

        /// <summary>
        /// Loads a texture from a memory buffer as a shader resource view.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="memory">The memory stream.</param>
        /// <param name="disableAutoGenMipMap"></param>
        /// <returns></returns>
        public static ShaderResourceView FromMemoryAsShaderResourceView(Device device, Stream memory, bool disableAutoGenMipMap = false)
        {
            using (var texture = global::SharpDX.Toolkit.Graphics.Texture.Load(device, memory))
            {
                if (!disableAutoGenMipMap && texture.Description.MipLevels == 1)// Check if it already has mipmaps or not, if loaded DDS file, it may already has precompiled mipmaps, don't need to generate again
                {
                    using (var textureMipmap = GenerateMipMaps(device, texture))
                    {
                        return new ShaderResourceView(device, textureMipmap);
                    }
                }
                else
                {
                    return new ShaderResourceView(device, texture);
                }
            }
        }

        public static Texture2D GenerateMipMaps(Device device, global::SharpDX.Toolkit.Graphics.Texture texture)
        {
            var desc = new Texture2DDescription()
            {
                Width = texture.Description.Width,
                Height = texture.Description.Height,
                MipLevels = 0,
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Usage = ResourceUsage.Default,
                SampleDescription = texture.Description.SampleDescription,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                Format = texture.Description.Format
            };

            var textMip = new Texture2D(device, desc);

            using (var shaderRes = new ShaderResourceView(device, textMip))
            {
                device.ImmediateContext.CopySubresourceRegion(texture, 0, null, textMip, 0);
                device.ImmediateContext.GenerateMips(shaderRes);
            }
            return textMip;
        }
    }
}
