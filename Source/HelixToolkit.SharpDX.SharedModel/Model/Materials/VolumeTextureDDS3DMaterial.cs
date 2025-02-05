using HelixToolkit.SharpDX;
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
/// Default Volume Texture Material. Supports 3D DDS memory stream as <see cref="VolumeTextureMaterialCoreBase{T}.VolumeTexture"/>
/// </summary>
public sealed class VolumeTextureDDS3DMaterial : VolumeTextureMaterialBase
{
    /// <summary>
    /// Gets or sets the texture. Only supports 3D DDS texture stream
    /// </summary>
    /// <value>
    /// The texture.
    /// </value>
    public TextureModel? Texture
    {
        get
        {
            return (TextureModel?)GetValue(TextureProperty);
        }
        set
        {
            SetValue(TextureProperty, value);
        }
    }

    public static readonly DependencyProperty TextureProperty =
        DependencyProperty.Register("Texture", typeof(TextureModel), typeof(VolumeTextureDDS3DMaterial),
            new PropertyMetadata(null, (d, e) =>
            {
                if (d is VolumeTextureDDS3DMaterial { Core: VolumeTextureDDS3DMaterialCore core })
                {
                    core.VolumeTexture = (TextureModel?)e.NewValue;
                }
            }));

    public VolumeTextureDDS3DMaterial()
    {

    }

    public VolumeTextureDDS3DMaterial(VolumeTextureDDS3DMaterialCore core) : base(core)
    {
        Texture = core.VolumeTexture;
    }
    protected override MaterialCore OnCreateCore()
    {
        return new VolumeTextureDDS3DMaterialCore()
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
        return new VolumeTextureDDS3DMaterial()
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
