﻿using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Model.Scene;

#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Model;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// 
/// </summary>
/// <seealso cref="GeometryModel3D" />
public class LineGeometryModel3D : GeometryModel3D
{
    #region Dependency Properties        
    /// <summary>
    /// The color property
    /// </summary>
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(UIColor), typeof(LineGeometryModel3D),
            new PropertyMetadata(UIColors.Black, (d, e) =>
            {
                if (d is LineGeometryModel3D node)
                {
                    node.material.LineColor = ((UIColor)e.NewValue).ToColor4();
                }
            }));

    /// <summary>
    /// The thickness property
    /// </summary>
    public static readonly DependencyProperty ThicknessProperty =
        DependencyProperty.Register("Thickness", typeof(double), typeof(LineGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
        {
            if (d is LineGeometryModel3D node)
            {
                node.material.Thickness = (float)(double)e.NewValue;
            }
        }));

    /// <summary>
    /// The smoothness property
    /// </summary>
    public static readonly DependencyProperty SmoothnessProperty =
        DependencyProperty.Register("Smoothness", typeof(double), typeof(LineGeometryModel3D), new PropertyMetadata(0.0,
        (d, e) =>
        {
            if (d is LineGeometryModel3D node)
            {
                node.material.Smoothness = (float)(double)e.NewValue;
            }
        }));

    /// <summary>
    /// The hit test thickness property
    /// </summary>
    public static readonly DependencyProperty HitTestThicknessProperty =
        DependencyProperty.Register("HitTestThickness", typeof(double), typeof(LineGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: LineNode node })
            {
                node.HitTestThickness = (double)e.NewValue;
            }
        }));


    /// <summary>
    /// Fixed sized billboard. Default = true. 
    /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
    /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
    /// </summary>
    public static readonly DependencyProperty FixedSizeProperty
        = DependencyProperty.Register("FixedSize", typeof(bool), typeof(LineGeometryModel3D),
        new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is LineGeometryModel3D node)
                {
                    node.material.FixedSize = (bool)e.NewValue;
                }
            }));

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
            return (UIColor)this.GetValue(ColorProperty);
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
            return (double)this.GetValue(ThicknessProperty);
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
            return (double)this.GetValue(SmoothnessProperty);
        }
        set
        {
            this.SetValue(SmoothnessProperty, value);
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
    #endregion

    protected readonly LineMaterialCore material = new();

    /// <summary>
    /// Called when [create scene node].
    /// </summary>
    /// <returns></returns>
    protected override SceneNode OnCreateSceneNode()
    {
        return new LineNode()
        {
            Material = material
        };
    }

    /// <summary>
    /// Assigns the default values to core.
    /// </summary>
    /// <param name="core">The core.</param>
    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        material.LineColor = Color.ToColor4();
        material.Thickness = (float)Thickness;
        material.Smoothness = (float)Smoothness;
        material.FixedSize = FixedSize;
        base.AssignDefaultValuesToSceneNode(core);
    }
}
