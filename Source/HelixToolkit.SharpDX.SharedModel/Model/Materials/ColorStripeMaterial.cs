using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Shaders;
using SharpDX;
using SharpDX.Direct3D11;
using System.ComponentModel;
using System.Runtime.Serialization;

#if false
#elif WINUI
#elif WPF
using HelixToolkit.Wpf.SharpDX.Utilities;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Utilities;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
s#error Unknown framework
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
        HelixProperty.Register<ColorStripeMaterial, Color4>("DiffuseColor",
            (Color4)Color.White,
            (d, e) =>
            {
                if (d is Material { Core: ColorStripeMaterialCore core })
                {
                    core.DiffuseColor = (Color4)e.NewValue!;
                }
            });

    /// <summary>
    /// Gets or sets the diffuse color for the material.
    /// </summary>
#if false
#elif WINUI
#elif WPF
    [TypeConverter(typeof(Color4Converter))]
#elif AVALONIA
    [TypeConverter(typeof(Color4Converter))]
#else
#error Unknown framework
#endif
    public Color4 DiffuseColor
    {
        get
        {
            return (Color4)this.GetValue(DiffuseColorProperty)!;
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
        HelixProperty.Register<ColorStripeMaterial, IList<Color4>?>("ColorStripeX",
            null, (d, e) =>
        {
            if (d is Material { Core: ColorStripeMaterialCore core })
            {
                core.ColorStripeX = (IList<Color4>?)e.NewValue;
            }
        });

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
        HelixProperty.Register<ColorStripeMaterial, IList<Color4>?>("ColorStripeY",
            null, (d, e) =>
        {
            if (d is Material { Core: ColorStripeMaterialCore core })
            {
                core.ColorStripeY = (IList<Color4>?)e.NewValue;
            }
        });

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
        HelixProperty.Register<ColorStripeMaterial, SamplerStateDescription>("ColorStripeSampler",
            DefaultSamplers.LinearSamplerClampAni1,
            (d, e) =>
            {
                if (d is Material { Core: ColorStripeMaterialCore core })
                {
                    core.ColorStripeSampler = (SamplerStateDescription)e.NewValue!;
                }
            });

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
            return (bool)GetValue(ColorStripeXEnabledProperty)!;
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
        HelixProperty.Register<ColorStripeMaterial, bool>("ColorStripeXEnabled",
            true,
            (d, e) =>
            {
                if (d is Material { Core: ColorStripeMaterialCore core })
                {
                    core.ColorStripeXEnabled = (bool)e.NewValue!;
                }
            });

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
            return (bool)GetValue(ColorStripeYEnabledProperty)!;
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
        HelixProperty.Register<ColorStripeMaterial, bool>("ColorStripeYEnabled",
            true,
            (d, e) =>
            {
                if (d is Material { Core: ColorStripeMaterialCore core })
                {
                    core.ColorStripeYEnabled = (bool)e.NewValue!;
                }
            });

    /// <summary>
    /// 
    /// </summary>
    public SamplerStateDescription ColorStripeSampler
    {
        get
        {
            return (SamplerStateDescription)this.GetValue(ColorStripeSamplerProperty)!;
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

#if false
#elif WINUI
#elif WPF
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
#elif AVALONIA
#else
#error Unknown framework
#endif
}
