using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Model.Scene;

#if WINUI
#else
using System.Windows;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// 
/// </summary>
public class PointGeometryModel3D : GeometryModel3D
{
    #region Dependency Properties
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(UIColor), typeof(PointGeometryModel3D),
            new PropertyMetadata(UIColors.Black, (d, e) =>
            {
                if (d is PointGeometryModel3D core)
                {
                    core.material.PointColor = ((UIColor)e.NewValue).ToColor4();
                }
            }));

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register("Size", typeof(Size), typeof(PointGeometryModel3D), new PropertyMetadata(new Size(1.0, 1.0),
            (d, e) =>
            {
                if (d is PointGeometryModel3D core)
                {
                    var size = (Size)e.NewValue;
                    core.material.Width = (float)size.Width;
                    core.material.Height = (float)size.Height;
                }
            }));

    public static readonly DependencyProperty FigureProperty =
        DependencyProperty.Register("Figure", typeof(PointFigure), typeof(PointGeometryModel3D), new PropertyMetadata(PointFigure.Rect,
            (d, e) =>
            {
                if (d is PointGeometryModel3D core)
                {
                    core.material.Figure = (PointFigure)e.NewValue;
                }
            }));

    public static readonly DependencyProperty FigureRatioProperty =
        DependencyProperty.Register("FigureRatio", typeof(double), typeof(PointGeometryModel3D), new PropertyMetadata(0.25,
            (d, e) =>
            {
                if (d is PointGeometryModel3D core)
                {
                    core.material.FigureRatio = (float)(double)e.NewValue;
                }
            }));

    public static readonly DependencyProperty HitTestThicknessProperty =
        DependencyProperty.Register("HitTestThickness", typeof(double), typeof(PointGeometryModel3D), new PropertyMetadata(4.0, (d, e) =>
        {
            if (d is PointGeometryModel3D { SceneNode: PointNode node })
            {
                node.HitTestThickness = (float)(double)e.NewValue;
            }
        }));

    /// <summary>
    /// Fixed sized. Default = true. 
    /// <para>When FixedSize = true, the render size will be scale to normalized device coordinates(screen) size</para>
    /// <para>When FixedSize = false, the render size will be actual size in 3D world space</para>
    /// </summary>
    public static readonly DependencyProperty FixedSizeProperty
        = DependencyProperty.Register("FixedSize", typeof(bool), typeof(PointGeometryModel3D),
        new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is PointGeometryModel3D core)
                {
                    core.material.FixedSize = (bool)e.NewValue;
                }
            }));

    // Using a DependencyProperty as the backing store for EnableColorBlending.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty EnableColorBlendingProperty =
        DependencyProperty.Register("EnableColorBlending", typeof(bool), typeof(PointGeometryModel3D), new PropertyMetadata(false,
            (d, e) =>
            {
                if (d is PointGeometryModel3D core)
                {
                    core.material.EnableColorBlending = (bool)e.NewValue;
                }
            }));

    // Using a DependencyProperty as the backing store for BlendingFactor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BlendingFactorProperty =
        DependencyProperty.Register("BlendingFactor", typeof(double), typeof(PointGeometryModel3D), new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is PointGeometryModel3D core)
                {
                    core.material.BlendingFactor = (float)(double)e.NewValue;
                }
            }));

    public UIColor Color
    {
        get
        {
            return (UIColor)this.GetValue(ColorProperty);
        }
        set
        {
            this.SetValue(ColorProperty, value);
        }
    }

    public Size Size
    {
        get
        {
            return (Size)this.GetValue(SizeProperty);
        }
        set
        {
            this.SetValue(SizeProperty, value);
        }
    }

    public PointFigure Figure
    {
        get
        {
            return (PointFigure)this.GetValue(FigureProperty);
        }
        set
        {
            this.SetValue(FigureProperty, value);
        }
    }

    public double FigureRatio
    {
        get
        {
            return (double)this.GetValue(FigureRatioProperty);
        }
        set
        {
            this.SetValue(FigureRatioProperty, value);
        }
    }

    /// <summary>
    /// Used only for point/line hit test
    /// </summary>
    public double HitTestThickness
    {
        get
        {
            return (double)this.GetValue(HitTestThicknessProperty);
        }
        set
        {
            this.SetValue(HitTestThicknessProperty, value);
        }
    }

    /// <summary>
    /// Fixed sized billboard. Default = true. 
    /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
    /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
    /// </summary>
    public bool FixedSize
    {
        set
        {
            SetValue(FixedSizeProperty, value);
        }
        get
        {
            return (bool)GetValue(FixedSizeProperty);
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
            return (bool)GetValue(EnableColorBlendingProperty);
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
            return (double)GetValue(BlendingFactorProperty);
        }
        set
        {
            SetValue(BlendingFactorProperty, value);
        }
    }
    #endregion

    protected readonly PointMaterialCore material = new();

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override SceneNode OnCreateSceneNode()
    {
        return new PointNode() { Material = material };
    }

    /// <summary>
    /// Assigns the default values to core.
    /// </summary>
    /// <param name="core">The core.</param>
    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        material.Width = (float)Size.Width;
        material.Height = (float)Size.Height;
        material.Figure = Figure;
        material.FigureRatio = (float)FigureRatio;
        material.PointColor = Color.ToColor4();
        material.FixedSize = FixedSize;
        base.AssignDefaultValuesToSceneNode(core);
    }
}
