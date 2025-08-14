using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Shaders;
using SharpDX;
using SharpDX.Direct3D11;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
#error Unknown framework
#endif

public abstract class VolumeTextureMaterialBase : Material, IVolumeTextureMaterial
{
    public SamplerStateDescription Sampler
    {
        get
        {
            return (SamplerStateDescription)GetValue(SamplerProperty)!;
        }
        set
        {
            SetValue(SamplerProperty, value);
        }
    }


    public static readonly DependencyProperty SamplerProperty =
        HelixProperty.Register<VolumeTextureMaterialBase, SamplerStateDescription>("Sampler",
            DefaultSamplers.VolumeSampler, (d, e) =>
            {
                if (d is VolumeTextureMaterialBase { Core: VolumeTextureDDS3DMaterialCore core })
                {
                    core.Sampler = (SamplerStateDescription)e.NewValue!;
                }
            });

    /// <summary>
    /// Gets or sets the sample distance. Default = 1.0
    /// </summary>
    /// <value>
    /// The size of the step.
    /// </value>
    public double SampleDistance
    {
        get
        {
            return (double)GetValue(SampleDistanceProperty)!;
        }
        set
        {
            SetValue(SampleDistanceProperty, value);
        }
    }

    public static readonly DependencyProperty SampleDistanceProperty =
        HelixProperty.Register<VolumeTextureMaterialBase, double>("SampleDistance",
            1.0, (d, e) =>
            {
                if (d is VolumeTextureMaterialBase { Core: IVolumeTextureMaterial core })
                {
                    core.SampleDistance = (double)e.NewValue!;
                }
            });


    /// <summary>
    /// Gets or sets the max iterations. Usually equal to the Texture Depth. Default = INT.MAX for automatic iteration
    /// </summary>
    /// <value>
    /// The iterations.
    /// </value>
    public int MaxIterations
    {
        get
        {
            return (int)GetValue(MaxIterationsProperty)!;
        }
        set
        {
            SetValue(MaxIterationsProperty, value);
        }
    }

    public static readonly DependencyProperty MaxIterationsProperty =
        HelixProperty.Register<VolumeTextureMaterialBase, int>("MaxIterations",
            int.MaxValue, (d, e) =>
            {
                if (d is VolumeTextureMaterialBase { Core: IVolumeTextureMaterial core })
                {
                    core.MaxIterations = (int)e.NewValue!;
                }
            });


    /// <summary>
    /// Gets or sets the iteration offset. This can be used to achieve cross section
    /// </summary>
    /// <value>
    /// The iteration offset.
    /// </value>
    public int IterationOffset
    {
        get
        {
            return (int)GetValue(IterationOffsetProperty)!;
        }
        set
        {
            SetValue(IterationOffsetProperty, value);
        }
    }


    public static readonly DependencyProperty IterationOffsetProperty =
        HelixProperty.Register<VolumeTextureMaterialBase, int>("IterationOffset",
            0, (d, e) =>
            {
                if (d is VolumeTextureMaterialBase { Core: IVolumeTextureMaterial core })
                {
                    core.IterationOffset = (int)e.NewValue!;
                }
            });

    /// <summary>
    /// Gets or sets the iso value. Only data with isovalue > sepecified iso value will be displayed
    /// Value must be normalized to 0~1. Default = 1, show all data.
    /// </summary>
    /// <value>
    /// The iso value.
    /// </value>
    public double IsoValue
    {
        get
        {
            return (double)GetValue(IsoValueProperty)!;
        }
        set
        {
            SetValue(IsoValueProperty, value);
        }
    }

    public static readonly DependencyProperty IsoValueProperty =
        HelixProperty.Register<VolumeTextureMaterialBase, double>("IsoValue",
            0.0, (d, e) =>
            {
                if (d is VolumeTextureMaterialBase { Core: IVolumeTextureMaterial core })
                {
                    core.IsoValue = (double)e.NewValue!;
                }
            });

    /// <summary>
    /// Gets or sets the color. It can also used to adjust opacity
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public Color4 Color
    {
        get
        {
            return (Color4)GetValue(ColorProperty)!;
        }
        set
        {
            SetValue(ColorProperty, value);
        }
    }


    public static readonly DependencyProperty ColorProperty =
        HelixProperty.Register<VolumeTextureMaterialBase, Color4>("Color4",
            new Color4(1, 1, 1, 1),
            (d, e) =>
            {
                if (d is VolumeTextureMaterialBase { Core: IVolumeTextureMaterial core })
                {
                    core.Color = (Color4)e.NewValue!;
                }
            });


    /// <summary>
    /// Gets or sets the Color Transfer Map.
    /// </summary>
    /// <value>
    /// The gradient map.
    /// </value>
    public Color4[]? TransferMap
    {
        get
        {
            return (Color4[]?)GetValue(TransferMapProperty);
        }
        set
        {
            SetValue(TransferMapProperty, value);
        }
    }

    public static readonly DependencyProperty TransferMapProperty =
        HelixProperty.Register<VolumeTextureMaterialBase, Color4[]?>("TransferMap",
            null,
            (d, e) =>
            {
                if (d is VolumeTextureMaterialBase { Core: IVolumeTextureMaterial core })
                {
                    core.TransferMap = (Color4[])e.NewValue!;
                }
            });

    public bool EnablePlaneAlignment
    {
        get
        {
            return (bool)GetValue(EnablePlaneAlignmentProperty)!;
        }
        set
        {
            SetValue(EnablePlaneAlignmentProperty, value);
        }
    }

    // Using a DependencyProperty as the backing store for EnablePlaneAlignment.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty EnablePlaneAlignmentProperty =
        HelixProperty.Register<VolumeTextureMaterialBase, bool>("EnablePlaneAlignment",
            true, (d, e) =>
        {
            if (d is VolumeTextureMaterialBase { Core: IVolumeTextureMaterial core })
            {
                core.EnablePlaneAlignment = (bool)e.NewValue!;
            }
        });

    public VolumeTextureMaterialBase()
    {
    }

    public VolumeTextureMaterialBase(IVolumeTextureMaterial core) : base(core as MaterialCore)
    {
        SampleDistance = core.SampleDistance;
        MaxIterations = core.MaxIterations;
        Sampler = core.Sampler;
        Color = core.Color;
        TransferMap = core.TransferMap;
        IsoValue = core.IsoValue;
        IterationOffset = core.IterationOffset;
        EnablePlaneAlignment = core.EnablePlaneAlignment;
    }
}
