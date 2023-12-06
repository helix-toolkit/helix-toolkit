using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Shaders;
using SharpDX;
using SharpDX.Direct3D11;
using System.ComponentModel;
using System.Runtime.Serialization;

#if WINUI
#else
using HelixToolkit.Wpf.SharpDX.Utilities;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// 
/// </summary>
[DataContract]
public class ColorStripeMaterial : Material
{
    /// <summary>
    /// The diffuse color property
    /// </summary>
    public static readonly DependencyProperty DiffuseColorProperty =
        DependencyProperty.Register("DiffuseColor", typeof(Color4), typeof(ColorStripeMaterial), new PropertyMetadata((Color4)Color.White,
            (d, e) =>
            {
                if (d is Material { Core: ColorStripeMaterialCore core })
                {
                    core.DiffuseColor = (Color4)e.NewValue;
                }
            }));

    /// <summary>
    /// Gets or sets the diffuse color for the material.
    /// </summary>
#if WPF
    [TypeConverter(typeof(Color4Converter))]
#endif
    public Color4 DiffuseColor
    {
        get
        {
            return (Color4)this.GetValue(DiffuseColorProperty);
        }
        set
        {
            this.SetValue(DiffuseColorProperty, value);
        }
    }

    /// <summary>
    /// The color stripe property
    /// </summary>
    public static readonly DependencyProperty ColorStripeXProperty =
        DependencyProperty.Register("ColorStripeX", typeof(IList<Color4>), typeof(ColorStripeMaterial), new PropertyMetadata(null, (d, e) =>
        {
            if (d is Material { Core: ColorStripeMaterialCore core })
            {
                core.ColorStripeX = (IList<Color4>?)e.NewValue;
            }
        }));

    /// <summary>
    /// Gets or sets the color stripe.
    /// </summary>
    /// <value>
    /// The color stripe.
    /// </value>
    public IList<Color4>? ColorStripeX
    {
        get
        {
            return (IList<Color4>?)GetValue(ColorStripeXProperty);
        }
        set
        {
            SetValue(ColorStripeXProperty, value);
        }
    }

    /// <summary>
    /// The color stripe property
    /// </summary>
    public static readonly DependencyProperty ColorStripeYProperty =
        DependencyProperty.Register("ColorStripeY", typeof(IList<Color4>), typeof(ColorStripeMaterial), new PropertyMetadata(null, (d, e) =>
        {
            if (d is Material { Core: ColorStripeMaterialCore core })
            {
                core.ColorStripeY = (IList<Color4>?)e.NewValue;
            }
        }));

    /// <summary>
    /// Gets or sets the color stripe.
    /// </summary>
    /// <value>
    /// The color stripe.
    /// </value>
    public IList<Color4>? ColorStripeY
    {
        get
        {
            return (IList<Color4>?)GetValue(ColorStripeYProperty);
        }
        set
        {
            SetValue(ColorStripeYProperty, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty ColorStripeSamplerProperty =
        DependencyProperty.Register("ColorStripeSampler", typeof(SamplerStateDescription), typeof(ColorStripeMaterial), new PropertyMetadata(DefaultSamplers.LinearSamplerClampAni1,
            (d, e) =>
            {
                if (d is Material { Core: ColorStripeMaterialCore core })
                {
                    core.ColorStripeSampler = (SamplerStateDescription)e.NewValue;
                }
            }));


    /// <summary>
    /// Gets or sets a value indicating whether [color stripe x enabled].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [color stripe x enabled]; otherwise, <c>false</c>.
    /// </value>
    public bool ColorStripeXEnabled
    {
        get
        {
            return (bool)GetValue(ColorStripeXEnabledProperty);
        }
        set
        {
            SetValue(ColorStripeXEnabledProperty, value);
        }
    }

    /// <summary>
    /// The color stripe x enabled property
    /// </summary>
    public static readonly DependencyProperty ColorStripeXEnabledProperty =
        DependencyProperty.Register("ColorStripeXEnabled", typeof(bool), typeof(ColorStripeMaterial), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Material { Core: ColorStripeMaterialCore core })
                {
                    core.ColorStripeXEnabled = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// Gets or sets a value indicating whether [color stripe y enabled].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [color stripe y enabled]; otherwise, <c>false</c>.
    /// </value>
    public bool ColorStripeYEnabled
    {
        get
        {
            return (bool)GetValue(ColorStripeYEnabledProperty);
        }
        set
        {
            SetValue(ColorStripeYEnabledProperty, value);
        }
    }

    /// <summary>
    /// The color stripe y enabled property
    /// </summary>
    public static readonly DependencyProperty ColorStripeYEnabledProperty =
        DependencyProperty.Register("ColorStripeYEnabled", typeof(bool), typeof(ColorStripeMaterial), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Material { Core: ColorStripeMaterialCore core })
                {
                    core.ColorStripeYEnabled = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public SamplerStateDescription ColorStripeSampler
    {
        get
        {
            return (SamplerStateDescription)this.GetValue(ColorStripeSamplerProperty);
        }
        set
        {
            this.SetValue(ColorStripeSamplerProperty, value);
        }
    }

    public ColorStripeMaterial()
    {
    }

    public ColorStripeMaterial(ColorStripeMaterialCore core) : base(core)
    {
        DiffuseColor = core.DiffuseColor;
        ColorStripeSampler = core.ColorStripeSampler;
        ColorStripeX = core.ColorStripeX;
        ColorStripeXEnabled = core.ColorStripeXEnabled;
        ColorStripeY = core.ColorStripeY;
        ColorStripeYEnabled = core.ColorStripeYEnabled;
    }

    protected override MaterialCore OnCreateCore()
    {
        return new ColorStripeMaterialCore()
        {
            DiffuseColor = DiffuseColor,
            ColorStripeSampler = ColorStripeSampler,
            ColorStripeX = ColorStripeX,
            ColorStripeXEnabled = ColorStripeXEnabled,
            ColorStripeY = ColorStripeY,
            ColorStripeYEnabled = ColorStripeYEnabled
        };
    }

#if WPF
    protected override Freezable CreateInstanceCore()
    {
        return new ColorStripeMaterial()
        {
            DiffuseColor = DiffuseColor,
            ColorStripeSampler = ColorStripeSampler,
            ColorStripeX = ColorStripeX,
            ColorStripeXEnabled = ColorStripeXEnabled,
            ColorStripeY = ColorStripeY,
            ColorStripeYEnabled = ColorStripeYEnabled,
            Name = Name
        };
    }
#endif
}
