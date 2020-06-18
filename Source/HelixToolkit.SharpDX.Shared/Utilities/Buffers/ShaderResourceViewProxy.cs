using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Threading;

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
    using Model;
    namespace Utilities
    {
        /// <summary>
        /// A proxy container to handle view resources
        /// </summary>
        public class ShaderResourceViewProxy : ReferenceCountDisposeObject
        {
            public static ShaderResourceViewProxy Empty { get; } = new ShaderResourceViewProxy();
            /// <summary>
            /// Gets the texture view.
            /// </summary>
            /// <value>
            /// The texture view.
            /// </value>
            public ShaderResourceView TextureView { get { return textureView; } }
            private ShaderResourceView textureView;
            /// <summary>
            /// Gets the depth stencil view.
            /// </summary>
            /// <value>
            /// The depth stencil view.
            /// </value>
            public DepthStencilView DepthStencilView { get { return depthStencilView; } }
            private DepthStencilView depthStencilView;
            /// <summary>
            /// Gets the render target view.
            /// </summary>
            /// <value>
            /// The render target view.
            /// </value>
            public RenderTargetView RenderTargetView { get { return renderTargetView; } }
            private RenderTargetView renderTargetView;
            /// <summary>
            /// Gets the resource.
            /// </summary>
            /// <value>
            /// The resource.
            /// </value>
            public Resource Resource { get { return resource; } }
            private Resource resource;

            private readonly Device device;

            public global::SharpDX.DXGI.Format TextureFormat { private set; get; }

            private ShaderResourceViewProxy() { }
            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public ShaderResourceViewProxy(Device device) { this.device = device; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="textureDesc">The texture desc.</param>
            public ShaderResourceViewProxy(Device device, Texture1DDescription textureDesc) : this(device)
            {
                resource = Collect(new Texture1D(device, textureDesc));
                TextureFormat = textureDesc.Format;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="textureDesc">The texture desc.</param>
            public ShaderResourceViewProxy(Device device, Texture2DDescription textureDesc) : this(device)
            {
                resource = Collect(new Texture2D(device, textureDesc));
                TextureFormat = textureDesc.Format;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="textureDesc">The texture desc.</param>
            public ShaderResourceViewProxy(Device device, Texture3DDescription textureDesc) : this(device)
            {
                resource = Collect(new Texture3D(device, textureDesc));
                TextureFormat = textureDesc.Format;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device"></param>
            /// <param name="resource"></param>
            public ShaderResourceViewProxy(Device device, Resource resource) : this(device)
            {
                this.resource = Collect(resource);
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
            /// </summary>
            /// <param name="view">The view.</param>
            public ShaderResourceViewProxy(ShaderResourceView view) : this(view.Device)
            {
                textureView = Collect(view);
                TextureFormat = view.Description.Format;
            }
            /// <summary>
            /// Creates the view from texture model.
            /// </summary>
            /// <param name="texture">The stream.</param>
            /// <param name="createSRV">Create ShaderResourceView</param>
            /// <param name="enableAutoGenMipMap">Enable auto mipmaps generation</param>
            /// <exception cref="ArgumentOutOfRangeException"/>
            public void CreateView(TextureModel texture, bool createSRV = true, bool enableAutoGenMipMap = true)
            {
                this.DisposeAndClear();
                if (texture != null && device != null)
                {
                    if (texture.IsCompressed && texture.CompressedStream != null)
                    {
                        resource = Collect(TextureLoader.FromMemoryAsShaderResource(device, texture.CompressedStream, !enableAutoGenMipMap));
                        if (createSRV)
                        {
                            textureView = Collect(new ShaderResourceView(device, resource));
                        }
                        TextureFormat = textureView.Description.Format;
                    }
                    else if (texture.NonCompressedData != null && texture.NonCompressedData.Length > 0)
                    {
                        if (texture.Width * texture.Height > texture.NonCompressedData.Length)
                        {
                            throw new ArgumentOutOfRangeException($"Texture width * height = {texture.Width * texture.Height} is larger than texture data length {texture.NonCompressedData.Length}.");
                        }
                        else if (texture.Height <= 1)
                        {
                            if (texture.Width == 0)
                            {
                                CreateView(texture.NonCompressedData, texture.UncompressedFormat, createSRV, enableAutoGenMipMap);
                            }
                            else
                            {
                                CreateView(texture.NonCompressedData, texture.Width, texture.UncompressedFormat, createSRV, enableAutoGenMipMap);
                            }
                        }
                        else
                        {                          
                            CreateView(texture.NonCompressedData, texture.Width, texture.Height, texture.UncompressedFormat, createSRV, enableAutoGenMipMap);
                        }
                    }
                }
            }
            /// <summary>
            /// Creates the view.
            /// </summary>
            /// <param name="desc">The desc.</param>
            public void CreateView(ShaderResourceViewDescription desc)
            {
                RemoveAndDispose(ref textureView);
                if (resource == null)
                {
                    return;
                }
                textureView = Collect(new ShaderResourceView(device, resource, desc));
            }
            /// <summary>
            /// Creates the view.
            /// </summary>
            /// <param name="desc">The desc.</param>
            public void CreateView(DepthStencilViewDescription desc)
            {
                RemoveAndDispose(ref depthStencilView);
                if (resource == null)
                {
                    return;
                }
                depthStencilView = Collect(new DepthStencilView(device, resource, desc));
            }
            /// <summary>
            /// Creates the view.
            /// </summary>
            /// <param name="desc">The desc.</param>
            public void CreateView(RenderTargetViewDescription desc)
            {
                RemoveAndDispose(ref renderTargetView);
                if (resource == null)
                {
                    return;
                }
                renderTargetView = Collect(new RenderTargetView(device, resource, desc));
            }
            /// <summary>
            /// Creates the view.
            /// </summary>
            public void CreateTextureView()
            {
                RemoveAndDispose(ref textureView);
                if (resource == null)
                {
                    return;
                }
                textureView = Collect(new ShaderResourceView(device, resource));
            }
            /// <summary>
            /// Creates the render target.
            /// </summary>
            public void CreateRenderTargetView()
            {
                RemoveAndDispose(ref renderTargetView);
                if (resource == null)
                {
                    return;
                }
                renderTargetView = Collect(new RenderTargetView(device, resource));
            }

            public void CreateDepthStencilView()
            {
                RemoveAndDispose(ref depthStencilView);
                if (resource == null)
                {
                    return;
                }
                depthStencilView = Collect(new DepthStencilView(device, resource));
            }

            /// <summary>
            /// Creates the 1D texture view from data array.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array">The array.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
            public void CreateView<T>(T[] array, global::SharpDX.DXGI.Format format,
                bool createSRV = true, bool generateMipMaps = true) where T : struct
            {
                CreateView(array, array.Length, format, createSRV, generateMipMaps);
            }
            /// <summary>
            /// Creates the 1D texture view from data array.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array">The array.</param>
            /// <param name="length">data length</param>
            /// <param name="format">The pixel format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps"></param>
            public void CreateView<T>(T[] array, int length, global::SharpDX.DXGI.Format format,
                bool createSRV = true, bool generateMipMaps = true) where T : struct
            {
                this.DisposeAndClear();
                var texture = Collect(global::SharpDX.Toolkit.Graphics.Texture1D.New(device, Math.Min(array.Length, length), format, array));
                TextureFormat = format;
                if (texture.Description.MipLevels == 1 && generateMipMaps)
                {
                    if(TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
                    {
                        resource = Collect(mipmapTexture);
                        RemoveAndDispose(ref texture);
                    }
                    else
                    {
                        resource = texture;
                    }
                }
                else
                {
                    resource = texture;
                }
                if (createSRV)
                {
                    textureView = Collect(new ShaderResourceView(device, resource));
                }
            }
            /// <summary>
            /// Creates the 2D texture view from data array.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="array">The array.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps"></param>
            /// <exception cref="ArgumentOutOfRangeException"/>
            public void CreateView<T>(T[] array, int width, int height, global::SharpDX.DXGI.Format format,
                bool createSRV = true, bool generateMipMaps = true) where T : struct
            {
                this.DisposeAndClear();
                if(width * height > array.Length)
                {
                    throw new ArgumentOutOfRangeException($"Width*Height = {width * height} is larger than array size {array.Length}.");
                }
                var texture = Collect(global::SharpDX.Toolkit.Graphics.Texture2D.New(device, width, height, 
                    format, array));
                TextureFormat = format;
                if (texture.Description.MipLevels == 1 && generateMipMaps)
                {
                    if (TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
                    {
                        resource = Collect(mipmapTexture);
                        RemoveAndDispose(ref texture);
                    }
                    else
                    {
                        resource = texture;
                    }
                }
                else
                {
                    resource = texture;
                }
                if (createSRV)
                {
                    textureView = Collect(new ShaderResourceView(device, resource));
                }
            }
            /// <summary>
            /// Creates the shader resource view from data ptr.
            /// </summary>
            /// <param name="dataPtr">The data PTR.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps"></param>
            public unsafe void CreateView(IntPtr dataPtr, int width, int height,
                global::SharpDX.DXGI.Format format,
                bool createSRV = true, bool generateMipMaps = true)
            {
                this.DisposeAndClear();
                var ptr = (IntPtr)dataPtr;
                global::SharpDX.Toolkit.Graphics.Image
                    .ComputePitch(format, width, height, 
                    out var rowPitch, out var slicePitch, out var widthCount, out var heightCount);
                
                var databox = new DataBox(ptr, rowPitch, slicePitch);

                var texture = Collect(global::SharpDX.Toolkit.Graphics.Texture2D.New(device, width, height, 1, format, 
                    new[] { databox }));
                TextureFormat = format;
                if (texture.Description.MipLevels == 1 && generateMipMaps)
                {
                    if (TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
                    {
                        resource = Collect(mipmapTexture);
                        RemoveAndDispose(ref texture);
                    }
                    else
                    {
                        resource = texture;
                    }
                }
                else
                {
                    resource = texture;
                }
                if (createSRV)
                {
                    textureView = Collect(new ShaderResourceView(device, resource));
                }
            }

            /// <summary>
            /// Creates the view from 3D texture byte array.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="pixels">The pixels.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="depth">The depth.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps"></param>
            /// <exception cref="ArgumentOutOfRangeException"/>
            public void CreateView<T>(T[] pixels, int width, int height, int depth,
                global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true) where T : struct
            {
                this.DisposeAndClear();
                if (width * height * depth > pixels.Length)
                {
                    throw new ArgumentOutOfRangeException($"Width*Height*Depth = {width * height * depth} is larger than array size {pixels.Length}.");
                }
                var texture = Collect(global::SharpDX.Toolkit.Graphics.Texture3D.New(device, width, height, depth,
                    format, pixels));
                TextureFormat = format;
                if (texture.Description.MipLevels == 1 && generateMipMaps)
                {
                    if (TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
                    {
                        resource = Collect(mipmapTexture);
                        RemoveAndDispose(ref texture);
                    }
                    else
                    {
                        resource = texture;
                    }
                }
                else
                {
                    resource = texture;
                }
                if (createSRV)
                {
                    textureView = Collect(new ShaderResourceView(device, resource));
                }
            }

            /// <summary>
            /// Creates the view.
            /// </summary>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="depth">The depth.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="dataPtr"></param>
            /// <param name="generateMipMaps"></param>
            public unsafe void CreateView(IntPtr dataPtr, int width, int height, int depth,
                global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true)
            {
                this.DisposeAndClear();
                var ptr = (IntPtr)dataPtr;
                var img = global::SharpDX.Toolkit.Graphics.Image.New3D(width, height, depth, global::SharpDX.Toolkit.Graphics.MipMapCount.Auto, format, dataPtr);
                var databox = img.ToDataBox();
                var texture = Collect(global::SharpDX.Toolkit.Graphics.Texture3D.New(device, width, height, depth, format,
                    databox));
                TextureFormat = format;
                if (texture.Description.MipLevels == 1 && generateMipMaps)
                {
                    if (TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
                    {
                        resource = Collect(mipmapTexture);
                        RemoveAndDispose(ref texture);
                    }
                    else
                    {
                        resource = texture;
                    }
                }
                else
                {
                    resource = texture;
                }
                if (createSRV)
                {
                    textureView = Collect(new ShaderResourceView(device, resource));
                }
            }
            /// <summary>
            /// Creates the 1D texture view from color array.
            /// </summary>
            /// <param name="array">The array.</param>
            public void CreateViewFromColorArray(Color4[] array)
            {
                CreateView(array, global::SharpDX.DXGI.Format.R32G32B32A32_Float);
            }
            /// <summary>
            /// Creates the 2D texture view from color array.
            /// </summary>
            /// <param name="array">The array.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="createSRV"></param>
            /// <param name="generateMipMaps"></param>
            public void CreateViewFromColorArray(Color4[] array, int width, int height, 
                bool createSRV = true, bool generateMipMaps = true)
            {
                CreateView(array, width, height, global::SharpDX.DXGI.Format.R32G32B32A32_Float, createSRV, generateMipMaps);
            }

            #region Static Creator        
            /// <summary>
            /// Creates ShaderResourceViewProxy from 2D texture array
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="device">The device.</param>
            /// <param name="array">The array.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps"></param>
            /// <returns></returns>
            public static ShaderResourceViewProxy CreateView<T>(Device device, T[] array, 
                global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true) where T : struct
            {
                var proxy = new ShaderResourceViewProxy(device);
                proxy.CreateView(array, format, createSRV, generateMipMaps);
                return proxy;
            }

            /// <summary>
            /// Creates ShaderResourceViewProxy from common file formats such as Jpg, Bmp, DDS, Png, etc
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="texture">The texture.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps"></param>
            /// <returns></returns>
            public static ShaderResourceViewProxy CreateView(Device device, System.IO.Stream texture, bool createSRV = true, bool generateMipMaps = true)
            {
                var proxy = new ShaderResourceViewProxy(device);
                proxy.CreateView(texture, createSRV, generateMipMaps);
                return proxy;
            }
            /// <summary>
            /// Creates the 2D texture view from data array
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="device">The device.</param>
            /// <param name="array">The array.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps"></param>
            /// <returns></returns>
            public static ShaderResourceViewProxy CreateView<T>(Device device, T[] array, int width, int height, 
                global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true) where T : struct
            {
                var proxy = new ShaderResourceViewProxy(device);
                proxy.CreateView(array, width, height, format, createSRV, generateMipMaps);
                return proxy;
            }

            /// <summary>
            /// Creates the 2D texture view from raw pixel byte array
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="dataPtr">The data PTR.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps"></param>
            /// <returns></returns>
            public unsafe static ShaderResourceViewProxy CreateView(Device device, IntPtr dataPtr, int width, int height,
                global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true)
            {
                var proxy = new ShaderResourceViewProxy(device);
                proxy.CreateView(dataPtr, width, height, format, createSRV, generateMipMaps);
                return proxy;
            }

            /// <summary>
            /// Creates the 1D texture view from color array.
            /// </summary>
            /// <param name="device"></param>
            /// <param name="array">The array.</param>
            public static ShaderResourceViewProxy CreateViewFromColorArray(Device device, Color4[] array)
            {
                var proxy = new ShaderResourceViewProxy(device);
                proxy.CreateViewFromColorArray(array);
                return proxy;
            }
            /// <summary>
            /// Creates the 2D texture view from color array.
            /// </summary>
            /// <param name="device"></param>
            /// <param name="array">The array.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="createSRV"></param>
            /// <param name="generateMipMaps"></param>
            public static ShaderResourceViewProxy CreateViewFromColorArray(Device device, Color4[] array, 
                int width, int height, bool createSRV = true, bool generateMipMaps = true)
            {
                var proxy = new ShaderResourceViewProxy(device);
                proxy.CreateViewFromColorArray(array, width, height, createSRV, generateMipMaps);
                return proxy;
            }

            /// <summary>
            /// Creates the 3D texture view from raw pixel array
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="pixels">The pixels.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="depth">The depth.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps"></param>
            /// <returns></returns>
            public static ShaderResourceViewProxy CreateViewFromPixelData(Device device, byte[] pixels, 
                int width, int height, int depth,
                global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true)
            {
                var proxy = new ShaderResourceViewProxy(device);
                proxy.CreateView(pixels, width, height, depth, format, createSRV, generateMipMaps);
                return proxy;
            }
            /// <summary>
            /// Creates the view from pixel data.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="pixels">The pixels.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="depth">The depth.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
            /// <returns></returns>
            public static ShaderResourceViewProxy CreateViewFromPixelData(Device device, Half4[] pixels,
                int width, int height, int depth,
                global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true)
            {
                var proxy = new ShaderResourceViewProxy(device);
                proxy.CreateView(pixels, width, height, depth, format, createSRV, generateMipMaps);
                return proxy;
            }
            /// <summary>
            /// Creates the view from pixel data.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="pixels">The pixels.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="depth">The depth.</param>
            /// <param name="format">The format.</param>
            /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
            /// <param name="generateMipMaps"></param>
            /// <returns></returns>
            public unsafe static ShaderResourceViewProxy CreateViewFromPixelData(Device device, IntPtr pixels, 
                int width, int height, int depth,
                global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true)
            {
                var proxy = new ShaderResourceViewProxy(device);
                proxy.CreateView(pixels, width, height, depth, format, createSRV, generateMipMaps);
                return proxy;
            }
            #endregion
            /// <summary>
            /// Performs an implicit conversion from <see cref="ShaderResourceViewProxy"/> to <see cref="ShaderResourceView"/>.
            /// </summary>
            /// <param name="proxy">The proxy.</param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public static implicit operator ShaderResourceView(ShaderResourceViewProxy proxy)
            {
                return proxy?.textureView;
            }
            /// <summary>
            /// Performs an implicit conversion from <see cref="ShaderResourceViewProxy"/> to <see cref="DepthStencilView"/>.
            /// </summary>
            /// <param name="proxy">The proxy.</param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public static implicit operator DepthStencilView(ShaderResourceViewProxy proxy)
            {
                return proxy?.depthStencilView;
            }
            /// <summary>
            /// Performs an implicit conversion from <see cref="ShaderResourceViewProxy"/> to <see cref="RenderTargetView"/>.
            /// </summary>
            /// <param name="proxy">The proxy.</param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public static implicit operator RenderTargetView(ShaderResourceViewProxy proxy)
            {
                return proxy?.renderTargetView;
            }
        }
    }

}
