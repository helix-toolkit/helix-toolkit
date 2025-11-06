using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Shaders;
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

public class LineMaterial : Material
{
    #region Dependency Properties        
    /// <summary>
    /// The color property
    /// </summary>
    public static readonly DependencyProperty ColorProperty =
        HelixProperty.Register<LineMaterial, UIColor>("Color",
            UIColors.Black, (d, e) =>
            {
                if (d is LineMaterial { Core: LineMaterialCore core })
                {
                    core.LineColor = ((UIColor)e.NewValue!).ToColor4();
                }
            });

    /// <summary>
    /// The thickness property
    /// </summary>
    public static readonly DependencyProperty ThicknessProperty =
        HelixProperty.Register<LineMaterial, double>("Thickness",
            1.0, (d, e) =>
        {
            if (d is LineMaterial { Core: LineMaterialCore core })
            {
                core.Thickness = (float)(double)e.NewValue!;
            }
        });

    /// <summary>
    /// The smoothness property
    /// </summary>
    public static readonly DependencyProperty SmoothnessProperty =
        HelixProperty.Register<LineMaterial, double>("Smoothness",
            0.0,
        (d, e) =>
        {
            if (d is LineMaterial { Core: LineMaterialCore core })
            {
                core.Smoothness = (float)(double)e.NewValue!;
            }
        });

    public static readonly DependencyProperty EnableDistanceFadingProperty =
        HelixProperty.Register<LineMaterial, bool>("EnableDistanceFading",
            true,
            (d, e) =>
            {
                if (d is LineMaterial { Core: LineMaterialCore core })
                {
                    core.EnableDistanceFading = (bool)e.NewValue!;
                }
            });

    public static readonly DependencyProperty FadingNearDistanceProperty =
        HelixProperty.Register<LineMaterial, double>("FadingNearDistance",
            0.0,
            (d, e) =>
            {
                if (d is LineMaterial { Core: LineMaterialCore core })
                {
                    core.FadingNearDistance = (float)(double)e.NewValue!;
                }
            });

    public static readonly DependencyProperty FadingFarDistanceProperty =
        HelixProperty.Register<LineMaterial, double>("FadingFarDistance",
            100.0,
            (d, e) =>
            {
                if (d is LineMaterial { Core: LineMaterialCore core })
                {
                    core.FadingFarDistance = (float)(double)e.NewValue!;
                }
            });

    public static readonly DependencyProperty TextureProperty =
        HelixProperty.Register<LineMaterial, TextureModel?>("Texture",
            null,
            (d, e) =>
            {
                if (d is LineMaterial { Core: LineMaterialCore core })
                {
                    core.Texture = e.NewValue == null ? null : (TextureModel)e.NewValue;
                }
            });


    public static readonly DependencyProperty TextureScaleProperty =
        HelixProperty.Register<LineMaterial, double>("TextureScale",
            1.0, (d, e) =>
        {
            if (d is LineMaterial { Core: LineMaterialCore core })
            {
                core.TextureScale = (float)(double)e.NewValue!;
            }
        });

    public static readonly DependencyProperty SamplerDescriptionProperty =
        HelixProperty.Register<LineMaterial, SamplerStateDescription>("SamplerDescription",
            DefaultSamplers.LineSamplerUWrapVClamp,
            (d, e) =>
            {
                if (d is LineMaterial { Core: LineMaterialCore core })
                {
                    core.SamplerDescription = (SamplerStateDescription)e.NewValue!;
                }
            });

    public static readonly DependencyProperty FixedSizeProperty =
        HelixProperty.Register<LineMaterial, bool>("FixedSize",
            true, (d, e) =>
        {
            if (d is LineMaterial { Core: LineMaterialCore core })
            {
                core.FixedSize = (bool)e.NewValue!;
            }
        });

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public UIColor Color
    {
        get
        {
            return (UIColor)this.GetValue(ColorProperty)!;
        }
        set
        {
            this.SetValue(ColorProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the thickness.
    /// </summary>
    /// <value>
    /// The thickness.
    /// </value>
    public double Thickness
    {
        get
        {
            return (double)this.GetValue(ThicknessProperty)!;
        }
        set
        {
            this.SetValue(ThicknessProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the smoothness.
    /// </summary>
    /// <value>
    /// The smoothness.
    /// </value>
    public double Smoothness
    {
        get
        {
            return (double)this.GetValue(SmoothnessProperty)!;
        }
        set
        {
            this.SetValue(SmoothnessProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [enable distance fading].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable distance fading]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableDistanceFading
    {
        set
        {
            SetValue(EnableDistanceFadingProperty, value);
        }
        get
        {
            return (bool)GetValue(EnableDistanceFadingProperty)!;
        }
    }
    /// <summary>
    /// Gets or sets the fading near distance.
    /// </summary>
    /// <value>
    /// The fading near distance.
    /// </value>
    public double FadingNearDistance
    {
        get
        {
            return (double)this.GetValue(FadingNearDistanceProperty)!;
        }
        set
        {
            this.SetValue(FadingNearDistanceProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the fading far distance.
    /// </summary>
    /// <value>
    /// The fading far distance.
    /// </value>
    public double FadingFarDistance
    {
        get
        {
            return (double)this.GetValue(FadingFarDistanceProperty)!;
        }
        set
        {
            this.SetValue(FadingFarDistanceProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the texture.
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

    /// <summary>
    /// Gets or sets the texture scale. Scale on texture X length
    /// </summary>
    /// <value>
    /// The texture scale.
    /// </value>
    public double TextureScale
    {
        get
        {
            return (double)GetValue(TextureScaleProperty)!;
        }
        set
        {
            SetValue(TextureScaleProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the sampler.
    /// </summary>
    /// <value>
    /// The sampler.
    /// </value>
    public SamplerStateDescription SamplerDescription
    {
        get
        {
            return (SamplerStateDescription)GetValue(SamplerDescriptionProperty)!;
        }
        set
        {
            SetValue(SamplerDescriptionProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [fixed size].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [fixed size]; otherwise, <c>false</c>.
    /// </value>
    public bool FixedSize
    {
        get
        {
            return (bool)GetValue(FixedSizeProperty)!;
        }
        set
        {
            SetValue(FixedSizeProperty, value);
        }
    }
    #endregion

    public LineMaterial()
    {
    }

    public LineMaterial(LineMaterialCore core) : base(core)
    {
        Name = core.Name;
        Color = core.LineColor.ToColor();
        Smoothness = core.Smoothness;
        Thickness = core.Thickness;
        EnableDistanceFading = core.EnableDistanceFading;
        FadingNearDistance = core.FadingNearDistance;
        FadingFarDistance = core.FadingFarDistance;
        Texture = core.Texture;
        TextureScale = core.TextureScale;
        SamplerDescription = core.SamplerDescription;
        FixedSize = core.FixedSize;
    }

    protected override MaterialCore OnCreateCore()
    {
        return new LineMaterialCore()
        {
            Name = Name,
            LineColor = Color.ToColor4(),
            Smoothness = (float)Smoothness,
            Thickness = (float)Thickness,
            EnableDistanceFading = EnableDistanceFading,
            FadingNearDistance = (float)FadingNearDistance,
            FadingFarDistance = (float)FadingFarDistance,
            Texture = Texture,
            TextureScale = (float)TextureScale,
            SamplerDescription = SamplerDescription,
            FixedSize = FixedSize
        };
    }

#if false
#elif WINUI
#elif WPF
    protected override Freezable CreateInstanceCore()
    {
        return new LineMaterial()
        {
            Name = Name,
            Color = Color,
            Smoothness = Smoothness,
            Thickness = Thickness,
            EnableDistanceFading = EnableDistanceFading,
            FadingNearDistance = FadingNearDistance,
            FadingFarDistance = FadingFarDistance,
            Texture = Texture,
            TextureScale = TextureScale,
            SamplerDescription = SamplerDescription,
            FixedSize = FixedSize
        };
    }
#elif AVALONIA
#else
#error Unknown framework
#endif
}
