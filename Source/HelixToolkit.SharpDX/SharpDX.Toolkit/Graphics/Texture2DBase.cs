using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics;

/// <summary>
/// Abstract class front end to <see cref="SharpDX.Direct3D11.Texture2D"/>.
/// </summary>
public abstract class Texture2DBase : Texture
{
    /// <summary>
    /// 
    /// </summary>
    protected readonly new Direct3D11.Texture2D Resource;
    private DXGI.Surface? dxgiSurface;

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DBase" /> class.
    /// </summary>
    /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
    /// <param name="description2D">The description.</param>
    /// <msdn-id>ff476521</msdn-id>	
    /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>	
    /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>	
    protected internal Texture2DBase(Direct3D11.Device device, Texture2DDescription description2D)
        : base(device, description2D)
    {
        Resource = new Direct3D11.Texture2D(device, description2D);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DBase" /> class.
    /// </summary>
    /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
    /// <param name="description2D">The description.</param>
    /// <param name="dataBoxes">A variable-length parameters list containing data rectangles.</param>
    /// <msdn-id>ff476521</msdn-id>	
    /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>	
    /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>	
    protected internal Texture2DBase(Direct3D11.Device device, Texture2DDescription description2D, DataBox[] dataBoxes)
        : base(device, description2D)
    {
        Resource = new Direct3D11.Texture2D(device, description2D, dataBoxes);
    }

    /// <summary>
    /// Specialised constructor for use only by derived classes.
    /// </summary>
    /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
    /// <param name="texture">The texture.</param>
    /// <msdn-id>ff476521</msdn-id>	
    /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>	
    /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>	
    protected internal Texture2DBase(Direct3D11.Device device, Direct3D11.Texture2D texture)
        : base(device, texture.Description)
    {
        Resource = texture;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual DXGI.Format GetDefaultViewFormat()
    {
        return this.Description.Format;
    }

    /// <summary>
    /// <see cref="SharpDX.DXGI.Surface"/> casting operator.
    /// </summary>
    /// <param name="from">From the Texture1D.</param>
    public static implicit operator SharpDX.DXGI.Surface?(Texture2DBase from)
    {
        // Don't bother with multithreading here
        return from == null ? null : (from.dxgiSurface ??= from.ToDispose(from.Resource.QueryInterface<DXGI.Surface>()));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="format"></param>
    /// <param name="textureFlags"></param>
    /// <param name="mipCount"></param>
    /// <param name="arraySize"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    protected static Texture2DDescription NewDescription(int width, int height, PixelFormat format, TextureFlags textureFlags, int mipCount, int arraySize, ResourceUsage usage)
    {
        if ((textureFlags & TextureFlags.UnorderedAccess) != 0)
            usage = ResourceUsage.Default;

        var desc = new Texture2DDescription()
        {
            Width = width,
            Height = height,
            ArraySize = arraySize,
            SampleDescription = new DXGI.SampleDescription(1, 0),
            BindFlags = GetBindFlagsFromTextureFlags(textureFlags),
            Format = format,
            MipLevels = CalculateMipMapCount(mipCount, width, height),
            Usage = usage,
            CpuAccessFlags = GetCpuAccessFlagsFromUsage(usage),
            OptionFlags = ResourceOptionFlags.None
        };


        // If the texture is a RenderTarget + ShaderResource + MipLevels > 1, then allow for GenerateMipMaps method
        if ((desc.BindFlags & BindFlags.RenderTarget) != 0 && (desc.BindFlags & BindFlags.ShaderResource) != 0 && desc.MipLevels > 1)
        {
            desc.OptionFlags |= ResourceOptionFlags.GenerateMipMaps;
        }

        return desc;
    }

    protected override void Dispose(bool disposeManagedResources)
    {
        Resource?.Dispose();
        base.Dispose(disposeManagedResources);
    }
}