/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
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
    namespace Utilities
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
                    if(texture == null)
                    {
                        return null;
                    }
                    if (!disableAutoGenMipMap && texture.Description.MipLevels == 1)// Check if it already has mipmaps or not, if loaded DDS file, it may already has precompiled mipmaps, don't need to generate again
                    {
                        if (GenerateMipMaps(device, texture, out var textureMipmap))
                        {
                            using (textureMipmap)
                            {
                                return new ShaderResourceView(device, textureMipmap);
                            }
                        }
                    }
                    return new ShaderResourceView(device, texture);
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
                    if(texture == null)
                    {
                        return null;
                    }
                    if (!disableAutoGenMipMap && texture.Description.MipLevels == 1)// Check if it already has mipmaps or not, if loaded DDS file, it may already has precompiled mipmaps, don't need to generate again
                    {
                        if(GenerateMipMaps(device, texture, out var textureMipmap))
                        {
                            using (textureMipmap)
                            {
                                return new ShaderResourceView(device, textureMipmap);
                            }
                        }
                    }
                    return new ShaderResourceView(device, texture);
                }
            }
            /// <summary>
            /// Froms the memory as shader resource.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="memory">The memory.</param>
            /// <param name="disableAutoGenMipMap">if set to <c>true</c> [disable automatic gen mip map].</param>
            /// <returns></returns>
            public static Resource FromMemoryAsShaderResource(Device device, Stream memory, bool disableAutoGenMipMap = false)
            {
                var texture = global::SharpDX.Toolkit.Graphics.Texture.Load(device, memory);
                if(texture == null)
                {
                    return null;
                }
                if (!disableAutoGenMipMap && texture.Description.MipLevels == 1)// Check if it already has mipmaps or not, if loaded DDS file, it may already has precompiled mipmaps, don't need to generate again
                {
                    try
                    {
                        if(GenerateMipMaps(device, texture, out var textureMipmap))
                        {
                            texture.Dispose();
                        }
                        return textureMipmap;
                    }
                    catch(System.Exception ex)
                    {
                        texture.Dispose();
                        throw new System.Exception(ex.Message);
                    }
                }
                return texture;
            }
            /// <summary>
            /// Generates the mip maps.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="texture">The texture.</param>
            /// <param name="textMip">Returns a new texture with mipmaps if succeeded. Otherwise returns the input texture</param>
            /// <returns>True succeed. False: Format not supported.</returns>
            /// <exception cref="InvalidDataException">Input texture is invalid.</exception>
            public static bool GenerateMipMaps(Device device, global::SharpDX.Toolkit.Graphics.Texture texture, out Resource textMip)
            {
                textMip = null;
                //Check texture format support: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476426(v=vs.85).aspx
                switch (texture.Description.Format)
                {
                    case global::SharpDX.DXGI.Format.R8G8B8A8_UNorm:
                    case global::SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb:
                    case global::SharpDX.DXGI.Format.B5G6R5_UNorm:
                    case global::SharpDX.DXGI.Format.B8G8R8A8_UNorm:
                    case global::SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb:
                    case global::SharpDX.DXGI.Format.B8G8R8X8_UNorm:
                    case global::SharpDX.DXGI.Format.B8G8R8X8_UNorm_SRgb:
                    case global::SharpDX.DXGI.Format.R16G16B16A16_Float:
                    case global::SharpDX.DXGI.Format.R16G16B16A16_UNorm:
                    case global::SharpDX.DXGI.Format.R16G16_Float:
                    case global::SharpDX.DXGI.Format.R16G16_UNorm:
                    case global::SharpDX.DXGI.Format.R32_Float:
                    case global::SharpDX.DXGI.Format.R32G32B32A32_Float:
                    case global::SharpDX.DXGI.Format.B4G4R4A4_UNorm:
                    case global::SharpDX.DXGI.Format.R32G32B32_Float:
                    case global::SharpDX.DXGI.Format.R16G16B16A16_SNorm:
                    case global::SharpDX.DXGI.Format.R32G32_Float:
                    case global::SharpDX.DXGI.Format.R10G10B10A2_UNorm:
                    case global::SharpDX.DXGI.Format.R11G11B10_Float:
                    case global::SharpDX.DXGI.Format.R8G8B8A8_SNorm:
                    case global::SharpDX.DXGI.Format.R16G16_SNorm:
                    case global::SharpDX.DXGI.Format.R8G8_UNorm:
                    case global::SharpDX.DXGI.Format.R8G8_SNorm:
                    case global::SharpDX.DXGI.Format.R16_Float:
                    case global::SharpDX.DXGI.Format.R16_UNorm:
                    case global::SharpDX.DXGI.Format.R16_SNorm:
                    case global::SharpDX.DXGI.Format.R8_UNorm:
                    case global::SharpDX.DXGI.Format.R8_SNorm:
                    case global::SharpDX.DXGI.Format.A8_UNorm:
                    case global::SharpDX.DXGI.Format.B5G5R5A1_UNorm:
                        break;
                    default:
                        textMip = texture;//Format not support, return the original texture.
                        return false;
                }
                switch (texture.Description.Dimension)
                {
                    case global::SharpDX.Toolkit.Graphics.TextureDimension.Texture1D:
                        var desc1D = new Texture1DDescription()
                        {
                            Width = texture.Description.Width,
                            MipLevels = 0,
                            ArraySize = 1,
                            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                            CpuAccessFlags = CpuAccessFlags.None,
                            Usage = ResourceUsage.Default,
                            OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                            Format = texture.Description.Format
                        };
                        textMip = new Texture1D(device, desc1D);
                        break;
                    case global::SharpDX.Toolkit.Graphics.TextureDimension.Texture2D:
                        var desc2D = new Texture2DDescription()
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
                        textMip = new Texture2D(device, desc2D);
                        break;
                    case global::SharpDX.Toolkit.Graphics.TextureDimension.Texture3D:
                        var desc3D = new Texture3DDescription()
                        {
                            Width = texture.Description.Width,
                            Height = texture.Description.Height,
                            Depth = texture.Description.Depth,
                            MipLevels = 0,
                            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                            CpuAccessFlags = CpuAccessFlags.None,
                            Usage = ResourceUsage.Default,
                            OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                            Format = texture.Description.Format
                        };
                        textMip = new Texture3D(device, desc3D);
                        break;
                    case global::SharpDX.Toolkit.Graphics.TextureDimension.TextureCube:
                        var descCube = new Texture3DDescription()
                        {
                            Width = texture.Description.Width,
                            Height = texture.Description.Height,
                            Depth = texture.Description.ArraySize,
                            MipLevels = 0,
                            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                            CpuAccessFlags = CpuAccessFlags.None,
                            Usage = ResourceUsage.Default,
                            OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                            Format = texture.Description.Format
                        };
                        textMip = new Texture3D(device, descCube);
                        break;
                    default:
                        throw new InvalidDataException("Input texture is invalid.");
                }

                using (var shaderRes = new ShaderResourceView(device, textMip))
                {
                    device.ImmediateContext.CopySubresourceRegion(texture, 0, null, textMip, 0);
                    device.ImmediateContext.GenerateMips(shaderRes);
                }
                return true;
            }
        }
    }

}
