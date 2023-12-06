using HelixToolkit.SharpDX.Model;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Used to use gradient data as Volume 3D texture. 
/// User must create their own data reader to read texture files as pixel byte[] and pass the necessary information as <see cref="VolumeTextureParams"/>
/// <para>
/// Pixel Byte[] is equal to Width * Height * Depth * BytesPerPixel.
/// </para>
/// </summary>
public sealed class VolumeTextureDiffuseMaterial : VolumeTextureMaterialBase
{
    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    /// <value>
    /// The texture.
    /// </value>
    public VolumeTextureGradientParams Texture
    {
        get
        {
            return (VolumeTextureGradientParams)GetValue(TextureProperty);
        }
        set
        {
            SetValue(TextureProperty, value);
        }
    }

    public static readonly DependencyProperty TextureProperty =
        DependencyProperty.Register("Texture", typeof(VolumeTextureGradientParams), typeof(VolumeTextureDiffuseMaterial),
            new PropertyMetadata(new VolumeTextureGradientParams(), (d, e) =>
            {
                if (d is VolumeTextureDiffuseMaterial { Core: VolumeTextureDiffuseMaterialCore core })
                {
                    core.VolumeTexture = (VolumeTextureGradientParams)e.NewValue;
                }
            }));

    public VolumeTextureDiffuseMaterial()
    {
    }

    public VolumeTextureDiffuseMaterial(VolumeTextureDiffuseMaterialCore core) : base(core)
    {
        Texture = core.VolumeTexture;
    }

    protected override MaterialCore OnCreateCore()
    {
        return new VolumeTextureDiffuseMaterialCore()
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

#if WPF
    protected override Freezable CreateInstanceCore()
    {
        return new VolumeTextureDiffuseMaterial()
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
#endif
}
