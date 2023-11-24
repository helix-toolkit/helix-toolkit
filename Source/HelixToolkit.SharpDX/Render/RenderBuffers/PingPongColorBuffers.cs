using HelixToolkit.SharpDX.Utilities;
using Microsoft.Extensions.Logging;
using SharpDX.Direct3D11;
using Format = SharpDX.DXGI.Format;

namespace HelixToolkit.SharpDX.Render;

/// <summary>
/// 
/// </summary>
public sealed class PingPongColorBuffers : DisposeObject
{
    static readonly ILogger logger = Logger.LogManager.Create<PingPongColorBuffers>();
    /// <summary>
    /// Gets the current ShaderResourceViewProxy.
    /// </summary>
    /// <value>
    /// The current SRV.
    /// </value>
    public ShaderResourceViewProxy? CurrentSRV
    {
        get
        {
            return textures[0];
        }
    }

    /// <summary>
    /// Gets the next SRV.
    /// </summary>
    /// <value>
    /// The next SRV.
    /// </value>
    public ShaderResourceViewProxy? NextSRV
    {
        get
        {
            return textures[1];
        }
    }

    public int Width
    {
        get
        {
            return texture2DDesc.Width;
        }
    }

    public int Height
    {
        get
        {
            return texture2DDesc.Height;
        }
    }

    /// <summary>
    /// Gets the current RenderTargetView.
    /// </summary>
    /// <value>
    /// The current RTV.
    /// </value>
    public ShaderResourceViewProxy? CurrentRTV
    {
        get
        {
            return textures[0];
        }
    }

    /// <summary>
    /// Gets the next RTV.
    /// </summary>
    /// <value>
    /// The next RTV.
    /// </value>
    public ShaderResourceViewProxy? NextRTV
    {
        get
        {
            return textures[1];
        }
    }

    public Resource? CurrentTexture
    {
        get
        {
            return textures[0]?.Resource;
        }
    }
    #region Texture Resources

    private const int NumPingPongBlurBuffer = 2;

    private readonly ShaderResourceViewProxy?[] textures = new ShaderResourceViewProxy?[NumPingPongBlurBuffer];

    private Texture2DDescription texture2DDesc = new()
    {
        BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
        CpuAccessFlags = CpuAccessFlags.None,
        Usage = ResourceUsage.Default,
        ArraySize = 1,
        MipLevels = 1,
        OptionFlags = ResourceOptionFlags.None,
        SampleDescription = new global::SharpDX.DXGI.SampleDescription(1, 0)
    };

    #endregion Texture Resources
    private readonly IDevice3DResources deviceResources;
    public bool Initialized { private set; get; } = false;
    private readonly object lockObj = new();
    /// <summary>
    /// Initializes a new instance of the <see cref="PingPongColorBuffers"/> class.
    /// </summary>
    /// <param name="textureFormat">The texture format.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="deviceRes">The device resource.</param>
    public PingPongColorBuffers(global::SharpDX.DXGI.Format textureFormat, int width, int height, IDevice3DResources deviceRes)
    {
        texture2DDesc.Format = textureFormat;
        deviceResources = deviceRes;
        texture2DDesc.Width = width;
        texture2DDesc.Height = height;
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Initialize()
    {
        if (deviceResources.Device is null)
        {
            return;
        }

        lock (lockObj)
        {
            if (Initialized)
            {
                return;
            }
            for (var i = 0; i < NumPingPongBlurBuffer; ++i)
            {
                textures[i] = new ShaderResourceViewProxy(deviceResources.Device, texture2DDesc);
                textures[i]!.CreateRenderTargetView();
                textures[i]!.CreateTextureView();
            }
            Initialized = true;
        }
    }

    /// <summary>
    /// Swaps the targets.
    /// </summary>
    public void SwapTargets()
    {
        lock (lockObj)
        {
            //swap buffer
            var current = textures[0];
            textures[0] = textures[1];
            textures[1] = current;
        }
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        for (var i = 0; i < NumPingPongBlurBuffer; ++i)
        {
            RemoveAndDispose(ref textures[i]);
        }
        base.OnDispose(disposeManagedResources);
    }
}
