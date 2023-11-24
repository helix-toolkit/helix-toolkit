using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics;

/// <summary>
/// Abstract class front end to <see cref="SharpDX.Direct3D11.Texture3D"/>.
/// </summary>
public abstract class Texture3DBase : Texture
{
    /// <summary>
    /// 
    /// </summary>
    protected readonly new Direct3D11.Texture3D Resource;

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture3DBase" /> class.
    /// </summary>
    /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
    /// <param name="description3D">The description.</param>
    /// <msdn-id>ff476522</msdn-id>	
    /// <unmanaged>HRESULT ID3D11Device::CreateTexture3D([In] const D3D11_TEXTURE3D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture3D** ppTexture3D)</unmanaged>	
    /// <unmanaged-short>ID3D11Device::CreateTexture3D</unmanaged-short>	
    protected internal Texture3DBase(Direct3D11.Device device, Texture3DDescription description3D)
        : base(device, description3D)
    {
        Resource = new Direct3D11.Texture3D(device, description3D);
        Initialize(Resource);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture3DBase" /> class.
    /// </summary>
    /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
    /// <param name="description3D">The description.</param>
    /// <param name="dataRectangles">A variable-length parameters list containing data rectangles.</param>
    /// <msdn-id>ff476522</msdn-id>	
    /// <unmanaged>HRESULT ID3D11Device::CreateTexture3D([In] const D3D11_TEXTURE3D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture3D** ppTexture3D)</unmanaged>	
    /// <unmanaged-short>ID3D11Device::CreateTexture3D</unmanaged-short>	
    protected internal Texture3DBase(Direct3D11.Device device, Texture3DDescription description3D, DataBox[] dataRectangles)
        : base(device, description3D)
    {
        Resource = new Direct3D11.Texture3D(device, description3D, dataRectangles);
        Initialize(Resource);
    }

    /// <summary>
    /// Specialised constructor for use only by derived classes.
    /// </summary>
    /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
    /// <param name="texture">The texture.</param>
    /// <msdn-id>ff476522</msdn-id>	
    /// <unmanaged>HRESULT ID3D11Device::CreateTexture3D([In] const D3D11_TEXTURE3D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture3D** ppTexture3D)</unmanaged>	
    /// <unmanaged-short>ID3D11Device::CreateTexture3D</unmanaged-short>	
    protected internal Texture3DBase(Direct3D11.Device device, Direct3D11.Texture3D texture)
        : base(device, texture.Description)
    {
        Resource = texture;
        Initialize(Resource);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="depth"></param>
    /// <param name="format"></param>
    /// <param name="textureFlags"></param>
    /// <param name="mipCount"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    protected static Texture3DDescription NewDescription(int width, int height, int depth, PixelFormat format, TextureFlags textureFlags, int mipCount, ResourceUsage usage)
    {
        if ((textureFlags & TextureFlags.UnorderedAccess) != 0)
            usage = ResourceUsage.Default;

        var desc = new Texture3DDescription()
        {
            Width = width,
            Height = height,
            Depth = depth,
            BindFlags = GetBindFlagsFromTextureFlags(textureFlags),
            Format = format,
            MipLevels = CalculateMipMapCount(mipCount, width, height, depth),
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