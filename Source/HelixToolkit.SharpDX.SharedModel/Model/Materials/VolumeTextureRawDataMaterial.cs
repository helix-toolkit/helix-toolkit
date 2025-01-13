using HelixToolkit.SharpDX.Model;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Used to use raw data as Volume 3D texture. 
/// User must create their own data reader to read texture files as pixel byte[] and pass the necessary information as <see cref="VolumeTextureParams"/>
/// <para>
/// Pixel Byte[] is equal to Width * Height * Depth * BytesPerPixel.
/// </para>
/// </summary>
public sealed class VolumeTextureRawDataMaterial : VolumeTextureMaterialBase
{
    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    /// <value>
    /// The texture.
    /// </value>
    public VolumeTextureParams Texture
    {
        get
        {
            return (VolumeTextureParams)GetValue(TextureProperty);
        }
        set
        {
            SetValue(TextureProperty, value);
        }
    }


    public static readonly DependencyProperty TextureProperty =
        DependencyProperty.Register("Texture", typeof(VolumeTextureParams), typeof(VolumeTextureRawDataMaterial),
            new PropertyMetadata(new VolumeTextureParams(), (d, e) =>
            {
                if (d is VolumeTextureRawDataMaterial { Core: VolumeTextureRawDataMaterialCore core })
                {
                    core.VolumeTexture = (VolumeTextureParams)e.NewValue;
                }
            }));

    public VolumeTextureRawDataMaterial()
    {
    }

    public VolumeTextureRawDataMaterial(VolumeTextureRawDataMaterialCore core) : base(core)
    {
        Texture = core.VolumeTexture;
    }

    protected override MaterialCore OnCreateCore()
    {
        return new VolumeTextureRawDataMaterialCore()
        {
            Name = Name,
            VolumeTexture = Texture,
            SampleDistance = SampleDistance,
            MaxIterations = MaxIterations,
            Sampler = Sampler,
            Color = Color,
            TransferMap = TransferMap,
            IsoValue = IsoValue,
            IterationOffset = IterationOffset,
            EnablePlaneAlignment = EnablePlaneAlignment,
        };
    }

#if false
#elif WINUI
#elif WPF
    protected override Freezable CreateInstanceCore()
    {
        return new VolumeTextureRawDataMaterial()
        {
            Name = Name,
            Texture = Texture,
            SampleDistance = SampleDistance,
            MaxIterations = MaxIterations,
            Sampler = Sampler,
            Color = Color,
            TransferMap = TransferMap,
            IsoValue = IsoValue,
            IterationOffset = IterationOffset,
            EnablePlaneAlignment = EnablePlaneAlignment,
        };
    }
#else
#error Unknown framework
#endif
}
