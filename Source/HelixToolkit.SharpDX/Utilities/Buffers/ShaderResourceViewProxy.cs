﻿using Microsoft.Extensions.Logging;
using SharpDX.Direct3D11;
using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// A proxy container to handle view resources
/// </summary>
public class ShaderResourceViewProxy : DisposeObject
{
    static readonly ILogger logger = Logger.LogManager.Create<ShaderResourceViewProxy>();
    public Guid Guid { set; get; } = Guid.NewGuid();
    public static ShaderResourceViewProxy Empty { get; } = new ShaderResourceViewProxy();
    /// <summary>
    /// Gets the texture view.
    /// </summary>
    /// <value>
    /// The texture view.
    /// </value>
    public ShaderResourceView? TextureView
    {
        get
        {
            return textureView;
        }
    }
    private ShaderResourceView? textureView;
    /// <summary>
    /// Gets the depth stencil view.
    /// </summary>
    /// <value>
    /// The depth stencil view.
    /// </value>
    public DepthStencilView? DepthStencilView
    {
        get
        {
            return depthStencilView;
        }
    }
    private DepthStencilView? depthStencilView;
    /// <summary>
    /// Gets the render target view.
    /// </summary>
    /// <value>
    /// The render target view.
    /// </value>
    public RenderTargetView? RenderTargetView
    {
        get
        {
            return renderTargetView;
        }
    }
    private RenderTargetView? renderTargetView;
    /// <summary>
    /// Gets the resource.
    /// </summary>
    /// <value>
    /// The resource.
    /// </value>
    public Resource? Resource
    {
        get
        {
            return resource;
        }
    }
    private Resource? resource;

    private readonly Device? device;

    public global::SharpDX.DXGI.Format TextureFormat
    {
        private set; get;
    }

    private ShaderResourceViewProxy()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    public ShaderResourceViewProxy(Device device)
    {
        this.device = device;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="textureDesc">The texture desc.</param>
    public ShaderResourceViewProxy(Device device, Texture1DDescription textureDesc) : this(device)
    {
        resource = new Texture1D(device, textureDesc);
        TextureFormat = textureDesc.Format;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="textureDesc">The texture desc.</param>
    public ShaderResourceViewProxy(Device device, Texture2DDescription textureDesc) : this(device)
    {
        resource = new Texture2D(device, textureDesc);
        TextureFormat = textureDesc.Format;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="textureDesc">The texture desc.</param>
    public ShaderResourceViewProxy(Device device, Texture3DDescription textureDesc) : this(device)
    {
        resource = new Texture3D(device, textureDesc);
        TextureFormat = textureDesc.Format;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="textureDesc">The texture desc.</param>
    public ShaderResourceViewProxy(Device device, ref Texture1DDescription textureDesc) : this(device)
    {
        resource = new Texture1D(device, textureDesc);
        TextureFormat = textureDesc.Format;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="textureDesc">The texture desc.</param>
    public ShaderResourceViewProxy(Device device, ref Texture2DDescription textureDesc) : this(device)
    {
        resource = new Texture2D(device, textureDesc);
        TextureFormat = textureDesc.Format;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="textureDesc">The texture desc.</param>
    public ShaderResourceViewProxy(Device device, ref Texture3DDescription textureDesc) : this(device)
    {
        resource = new Texture3D(device, textureDesc);
        TextureFormat = textureDesc.Format;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="device"></param>
    /// <param name="resource"></param>
    public ShaderResourceViewProxy(Device device, Resource resource) : this(device)
    {
        this.resource = resource;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderResourceViewProxy"/> class.
    /// </summary>
    /// <param name="view">The view.</param>
    public ShaderResourceViewProxy(ShaderResourceView view) : this(view.Device)
    {
        textureView = view;
        TextureFormat = view.Description.Format;
    }

    private bool HandleTextureContent(TextureInfo content, bool createSRV, bool enableAutoGenMipMap)
    {
        if (content == TextureInfo.Null)
        {
            return false;
        }
        if (content.IsCompressed)
        {
            var stream = content.Texture;
            if (stream == null || !stream.CanRead)
            {
                logger.LogWarning("Stream is null or unreadable.");
                return false;
            }

            if (device is null)
            {
                logger.LogWarning("Device is null.");
                return false;
            }

            resource = TextureLoader.FromMemoryAsShaderResource(device, stream, !enableAutoGenMipMap);
            if (createSRV)
            {
                textureView = new ShaderResourceView(device, resource);
            }
            TextureFormat = textureView?.Description.Format ?? global::SharpDX.DXGI.Format.Unknown;
        }
        else
        {
            var handle = new GCHandle();
            var pixelPtr = IntPtr.Zero;
            try
            {
                switch (content.DataType)
                {
                    case TextureDataType.ByteArray:
                        handle = GCHandle.Alloc(content.TextureRaw, GCHandleType.Pinned);
                        pixelPtr = handle.AddrOfPinnedObject();
                        break;
                    case TextureDataType.Color4:
                        handle = GCHandle.Alloc(content.Color4Array, GCHandleType.Pinned);
                        pixelPtr = handle.AddrOfPinnedObject();
                        break;
                    case TextureDataType.Stream:
                        if (content.Texture == null || !content.Texture.CanRead)
                        {
                            logger.LogWarning("Data is null or unreadable.");
                            return false;
                        }
                        var temp = new byte[content.Texture.Length];
                        using (var pixelStream = new MemoryStream(temp))
                        {
                            lock (content.Texture)
                            {
                                content.Texture.Position = 0;
                                content.Texture.CopyTo(pixelStream);
                            }
                        }
                        handle = GCHandle.Alloc(temp, GCHandleType.Pinned);
                        pixelPtr = handle.AddrOfPinnedObject();
                        break;
                    case TextureDataType.RawPointer:
                        pixelPtr = content.RawPointer;
                        break;
                }
                if (pixelPtr != IntPtr.Zero)
                {
                    switch (content.Dimension)
                    {
                        case 1:
                            CreateView(pixelPtr, content.Width, content.PixelFormat, createSRV, enableAutoGenMipMap);
                            break;
                        case 2:
                            CreateView(pixelPtr, content.Width, content.Height, content.PixelFormat, createSRV, enableAutoGenMipMap);
                            break;
                        case 3:
                            CreateView(pixelPtr, content.Width, content.Height, content.Depth, content.PixelFormat, createSRV, enableAutoGenMipMap);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }
        return true;
    }

    private void DisposeAll()
    {
        RemoveAndDispose(ref textureView);
        RemoveAndDispose(ref depthStencilView);
        RemoveAndDispose(ref renderTargetView);
        RemoveAndDispose(ref resource);
    }

    /// <summary>
    /// Creates the view from texture model.
    /// </summary>
    /// <param name="texture">The stream.</param>
    /// <param name="createSRV">Create ShaderResourceView</param>
    /// <param name="enableAutoGenMipMap">Enable auto mipmaps generation</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public void CreateView(TextureModel? texture, bool createSRV = true, bool enableAutoGenMipMap = true)
    {
        DisposeAll();
        if (texture != null && device != null && texture.TextureInfoLoader != null)
        {
            var content = texture.TextureInfoLoader.Load(texture.Guid);
            var succ = HandleTextureContent(content, createSRV, content.GenerateMipMaps && enableAutoGenMipMap);
            texture.TextureInfoLoader.Complete(texture.Guid, content, succ);
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
        textureView = new ShaderResourceView(device, resource, desc);
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
        depthStencilView = new DepthStencilView(device, resource, desc);
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
        renderTargetView = new RenderTargetView(device, resource, desc);
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
        textureView = new ShaderResourceView(device, resource);
    }
    /// <summary>
    /// Creates the view.
    /// </summary>
    /// <param name="desc"></param>
    public void CreateTextureView(ShaderResourceViewDescription desc)
    {
        CreateTextureView(ref desc);
    }
    /// <summary>
    /// Creates the view.
    /// </summary>
    public void CreateTextureView(ref ShaderResourceViewDescription desc)
    {
        RemoveAndDispose(ref textureView);
        if (resource == null)
        {
            return;
        }
        textureView = new ShaderResourceView(device, resource, desc);
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
        renderTargetView = new RenderTargetView(device, resource);
    }

    public void CreateDepthStencilView()
    {
        RemoveAndDispose(ref depthStencilView);
        if (resource == null)
        {
            return;
        }
        depthStencilView = new DepthStencilView(device, resource);
    }

    public void CreateDepthStencilView(DepthStencilViewDescription desc)
    {
        CreateDepthStencilView(ref desc);
    }

    public void CreateDepthStencilView(ref DepthStencilViewDescription desc)
    {
        RemoveAndDispose(ref depthStencilView);
        if (resource == null)
        {
            return;
        }
        depthStencilView = new DepthStencilView(device, resource, desc);
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
        bool createSRV = true, bool generateMipMaps = true) where T : unmanaged
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
        bool createSRV = true, bool generateMipMaps = true) where T : unmanaged
    {
        DisposeAll();

        if (device is null)
        {
            return;
        }

        var texture = global::SharpDX.Toolkit.Graphics.Texture1D.New(device, Math.Min(array.Length, length), format, array);
        TextureFormat = format;
        if (texture.Description.MipLevels == 1 && generateMipMaps)
        {
            if (TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
            {
                resource = mipmapTexture;
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
            textureView = new ShaderResourceView(device, resource);
        }
    }

    /// <summary>
    /// Creates the shader resource view from data ptr.
    /// </summary>
    /// <param name="dataPtr">The data PTR.</param>
    /// <param name="width">The width.</param>
    /// <param name="format">The format.</param>
    /// <param name="createSRV">if set to <c>true</c> [create SRV].</param>
    /// <param name="generateMipMaps"></param>
    public unsafe void CreateView(IntPtr dataPtr, int width,
        global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true)
    {
        DisposeAll();

        if (device is null)
        {
            return;
        }

        var ptr = (IntPtr)dataPtr;

        global::SharpDX.Toolkit.Graphics.Image
            .ComputePitch(format, width, 1,
            out var rowPitch, out var slicePitch, out _, out _);

        var databox = new DataBox(ptr, rowPitch, slicePitch);

        var texture = global::SharpDX.Toolkit.Graphics.Texture1D.New(device, width, format,
            new[] { databox });
        TextureFormat = format;
        if (texture.Description.MipLevels == 1 && generateMipMaps)
        {
            if (TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
            {
                resource = mipmapTexture;
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
            textureView = new ShaderResourceView(device, resource);
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
        bool createSRV = true, bool generateMipMaps = true) where T : unmanaged
    {
        DisposeAll();

        if (device is null)
        {
            return;
        }

        if (width * height > array.Length)
        {
            throw new ArgumentOutOfRangeException($"Width*Height = {width * height} is larger than array size {array.Length}.");
        }

        var texture = global::SharpDX.Toolkit.Graphics.Texture2D.New(device, width, height,
            format, array);
        TextureFormat = format;
        if (texture.Description.MipLevels == 1 && generateMipMaps)
        {
            if (TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
            {
                resource = mipmapTexture;
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
            textureView = new ShaderResourceView(device, resource);
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
        DisposeAll();

        if (device is null)
        {
            return;
        }

        var ptr = (IntPtr)dataPtr;
        global::SharpDX.Toolkit.Graphics.Image
            .ComputePitch(format, width, height,
            out var rowPitch, out var slicePitch, out var widthCount, out var heightCount);

        var databox = new DataBox(ptr, rowPitch, slicePitch);

        var texture = global::SharpDX.Toolkit.Graphics.Texture2D.New(device, width, height, 1, format,
            new[] { databox });
        TextureFormat = format;
        if (texture.Description.MipLevels == 1 && generateMipMaps)
        {
            if (TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
            {
                resource = mipmapTexture;
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
            textureView = new ShaderResourceView(device, resource);
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
        global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true) where T : unmanaged
    {
        DisposeAll();

        if (device is null)
        {
            return;
        }

        if (width * height * depth > pixels.Length)
        {
            throw new ArgumentOutOfRangeException($"Width*Height*Depth = {width * height * depth} is larger than array size {pixels.Length}.");
        }
        var texture = global::SharpDX.Toolkit.Graphics.Texture3D.New(device, width, height, depth,
            format, pixels);
        TextureFormat = format;
        if (texture.Description.MipLevels == 1 && generateMipMaps)
        {
            if (TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
            {
                resource = mipmapTexture;
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
            textureView = new ShaderResourceView(device, resource);
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
        DisposeAll();

        if (device is null)
        {
            return;
        }

        var ptr = (IntPtr)dataPtr;
        var img = global::SharpDX.Toolkit.Graphics.Image.New3D(width, height, depth, global::SharpDX.Toolkit.Graphics.MipMapCount.Auto, format, dataPtr);
        var databox = img.ToDataBox();
        var texture = global::SharpDX.Toolkit.Graphics.Texture3D.New(device, width, height, depth, format,
            databox);
        TextureFormat = format;
        if (texture.Description.MipLevels == 1 && generateMipMaps)
        {
            if (TextureLoader.GenerateMipMaps(device, texture, out var mipmapTexture))
            {
                resource = mipmapTexture;
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
            textureView = new ShaderResourceView(device, resource);
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

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref renderTargetView);
        RemoveAndDispose(ref depthStencilView);
        RemoveAndDispose(ref textureView);
        RemoveAndDispose(ref resource);
        base.OnDispose(disposeManagedResources);
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
        global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true) where T : unmanaged
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
        global::SharpDX.DXGI.Format format, bool createSRV = true, bool generateMipMaps = true) where T : unmanaged
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
    public static implicit operator ShaderResourceView?(ShaderResourceViewProxy? proxy)
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
    public static implicit operator DepthStencilView?(ShaderResourceViewProxy? proxy)
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
    public static implicit operator RenderTargetView?(ShaderResourceViewProxy? proxy)
    {
        return proxy?.renderTargetView;
    }
}
