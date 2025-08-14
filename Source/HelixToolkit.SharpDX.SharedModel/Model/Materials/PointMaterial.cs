using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model;

#if false
#elif WINUI
#elif WPF
using System.Windows;
#elif AVALONIA
using Avalonia;
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
#error Unknown framework
#endif

public class PointMaterial : Material
{
    #region Dependency Properties
    public static readonly DependencyProperty ColorProperty =
        HelixProperty.Register<PointMaterial, UIColor>("Color",
            UIColors.Black, (d, e) =>
            {
                if (d is PointMaterial { Core: PointMaterialCore core })
                {
                    core.PointColor = ((UIColor)e.NewValue!).ToColor4();
                }
            });

    public static readonly DependencyProperty SizeProperty =
        HelixProperty.Register<PointMaterial, Size>("Size", new Size(1.0, 1.0),
            (d, e) =>
            {
                if (d is PointMaterial { Core: PointMaterialCore core })
                {
                    var size = (Size)e.NewValue!;
                    core.Width = (float)size.Width;
                    core.Height = (float)size.Height;
                }
            });

    public static readonly DependencyProperty FigureProperty =
        HelixProperty.Register<PointMaterial, PointFigure>("Figure", PointFigure.Rect,
            (d, e) =>
            {
                if (d is PointMaterial { Core: PointMaterialCore core })
                {
                    core.Figure = (PointFigure)e.NewValue!;
                }
            });

    public static readonly DependencyProperty FigureRatioProperty =
        HelixProperty.Register<PointMaterial, double>("FigureRatio", 0.25,
            (d, e) =>
            {
                if (d is PointMaterial { Core: PointMaterialCore core })
                {
                    core.FigureRatio = (float)(double)e.NewValue!;
                }
            });

    public static readonly DependencyProperty EnableDistanceFadingProperty =
        HelixProperty.Register<PointMaterial, bool>("EnableDistanceFading", false,
            (d, e) =>
            {
                if (d is PointMaterial { Core: PointMaterialCore core })
                {
                    core.EnableDistanceFading = (bool)e.NewValue!;
                }
            });

    public static readonly DependencyProperty FadingNearDistanceProperty =
        HelixProperty.Register<PointMaterial, double>("FadingNearDistance", 0.0,
            (d, e) =>
            {
                if (d is PointMaterial { Core: PointMaterialCore core })
                {
                    core.FadingNearDistance = (float)(double)e.NewValue!;
                }
            });

    public static readonly DependencyProperty FadingFarDistanceProperty =
        HelixProperty.Register<PointMaterial, double>("FadingFarDistance", 100.0,
            (d, e) =>
            {
                if (d is PointMaterial { Core: PointMaterialCore core })
                {
                    core.FadingFarDistance = (float)(double)e.NewValue!;
                }
            });

    // Using a DependencyProperty as the backing store for EnableColorBlending.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty EnableColorBlendingProperty =
        HelixProperty.Register<PointMaterial, bool>("EnableColorBlending", false,
            (d, e) =>
            {
                if (d is PointMaterial { Core: PointMaterialCore core })
                {
                    core.EnableColorBlending = (bool)e.NewValue!;
                }
            });

    // Using a DependencyProperty as the backing store for BlendingFactor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BlendingFactorProperty =
        HelixProperty.Register<PointMaterial, double>("BlendingFactor", 0.0,
            (d, e) =>
            {
                if (d is PointMaterial { Core: PointMaterialCore core })
                {
                    core.BlendingFactor = (float)(double)e.NewValue!;
                }
            });

    /// <summary>
    /// Gets or sets the point color.
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
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public Size Size
    {
        get
        {
            return (Size)this.GetValue(SizeProperty)!;
        }
        set
        {
            this.SetValue(SizeProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the figure.
    /// </summary>
    /// <value>
    /// The figure.
    /// </value>
    public PointFigure Figure
    {
        get
        {
            return (PointFigure)this.GetValue(FigureProperty)!;
        }
        set
        {
            this.SetValue(FigureProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the figure ratio.
    /// </summary>
    /// <value>
    /// The figure ratio.
    /// </value>
    public double FigureRatio
    {
        get
        {
            return (double)this.GetValue(FigureRatioProperty)!;
        }
        set
        {
            this.SetValue(FigureRatioProperty, value);
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
    /// Gets or sets a value indicating whether [enable color blending].
    /// <para>Once enabled, final color 
    /// = <see cref="BlendingFactor"/> * <see cref="Color"/> + (1 - <see cref="BlendingFactor"/>) * Vertex Color.</para>
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable color blending]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableColorBlending
    {
        get
        {
            return (bool)GetValue(EnableColorBlendingProperty)!;
        }
        set
        {
            SetValue(EnableColorBlendingProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the blending factor.
    /// <para>Used when <see cref="EnableColorBlending"/> = true.</para>
    /// </summary>
    /// <value>
    /// The blending factor.
    /// </value>
    public double BlendingFactor
    {
        get
        {
            return (double)GetValue(BlendingFactorProperty)!;
        }
        set
        {
            SetValue(BlendingFactorProperty, value);
        }
    }
    #endregion

    public PointMaterial()
    {
    }

    public PointMaterial(PointMaterialCore core) : base(core)
    {
        Color = core.PointColor.ToColor();
        Size = new Size(core.Width, core.Height);
        Figure = core.Figure;
        FigureRatio = core.FigureRatio;
        Name = core.Name;
        EnableDistanceFading = core.EnableDistanceFading;
        FadingNearDistance = core.FadingNearDistance;
        FadingFarDistance = core.FadingFarDistance;
    }
    protected override MaterialCore OnCreateCore()
    {
        return new PointMaterialCore()
        {
            PointColor = Color.ToColor4(),
            Width = (float)Size.Width,
            Height = (float)Size.Height,
            Figure = Figure,
            FigureRatio = (float)FigureRatio,
            Name = Name,
            EnableDistanceFading = EnableDistanceFading,
            FadingNearDistance = (float)FadingNearDistance,
            FadingFarDistance = (float)FadingFarDistance
        };
    }

#if false
#elif WINUI
#elif WPF
    protected override Freezable CreateInstanceCore()
    {
        return new PointMaterial()
        {
            Name = Name
        };
    }
#elif AVALONIA
#else
#error Unknown framework
#endif
}
