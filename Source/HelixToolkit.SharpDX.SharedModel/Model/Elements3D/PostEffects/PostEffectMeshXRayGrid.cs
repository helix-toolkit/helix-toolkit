using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;

#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Model;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Model;
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

/// <summary>
/// 
/// </summary>
/// <seealso cref="Element3D" />
public class PostEffectMeshXRayGrid : Element3D
{
    #region Dependency Properties
    /// <summary>
    /// The effect name property
    /// </summary>
    public static readonly DependencyProperty EffectNameProperty =
        HelixProperty.Register<PostEffectMeshXRayGrid, string>("EffectName",
            DefaultRenderTechniqueNames.PostEffectMeshXRayGrid, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: NodePostEffectXRayGrid core })
            {
                core.EffectName = (string)e.NewValue!;
            }
        });

    /// <summary>
    /// Gets or sets the name of the effect.
    /// </summary>
    /// <value>
    /// The name of the effect.
    /// </value>
    public string EffectName
    {
        get
        {
            return (string)GetValue(EffectNameProperty)!;
        }
        set
        {
            SetValue(EffectNameProperty, value);
        }
    }

    /// <summary>
    /// The outline color property
    /// </summary>
    public static readonly DependencyProperty GridColorProperty =
        HelixProperty.Register<PostEffectMeshXRayGrid, UIColor>("GridColor",
            UIColors.DarkBlue,
        (d, e) =>
        {
            if (d is Element3DCore { SceneNode: NodePostEffectXRayGrid core })
            {
                core.Color = ((UIColor)e.NewValue!).ToColor4();
            }
        });

    /// <summary>
    /// Gets or sets the color of the outline.
    /// </summary>
    /// <value>
    /// The color of the outline.
    /// </value>
    public UIColor GridColor
    {
        set
        {
            SetValue(GridColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(GridColorProperty)!;
        }
    }

    /// <summary>
    /// Gets or sets the grid density.
    /// </summary>
    /// <value>
    /// The grid density.
    /// </value>
    public int GridDensity
    {
        get
        {
            return (int)GetValue(GridDensityProperty)!;
        }
        set
        {
            SetValue(GridDensityProperty, value);
        }
    }
    /// <summary>
    /// The grid density property
    /// </summary>
    public static readonly DependencyProperty GridDensityProperty =
        HelixProperty.Register<PostEffectMeshXRayGrid, int>("GridDensity",
            8,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: NodePostEffectXRayGrid core })
                {
                    core.GridDensity = (int)e.NewValue!;
                }
            });

    /// <summary>
    /// Gets or sets the dimming factor.
    /// </summary>
    /// <value>
    /// The dimming factor.
    /// </value>
    public double DimmingFactor
    {
        get
        {
            return (double)GetValue(DimmingFactorProperty)!;
        }
        set
        {
            SetValue(DimmingFactorProperty, value);
        }
    }

    /// <summary>
    /// The dimming factor property
    /// </summary>
    public static readonly DependencyProperty DimmingFactorProperty =
        HelixProperty.Register<PostEffectMeshXRayGrid, double>("DimmingFactor",
            0.8,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: NodePostEffectXRayGrid core })
                {
                    core.DimmingFactor = (float)(double)e.NewValue!;
                }
            });

    /// <summary>
    /// Gets or sets the blending factor for grid and original mesh color blending
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

    /// <summary>
    /// The blending factor property
    /// </summary>
    public static readonly DependencyProperty BlendingFactorProperty =
        HelixProperty.Register<PostEffectMeshXRayGrid, double>("BlendingFactor",
            1.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: NodePostEffectXRayGrid core })
                {
                    core.BlendingFactor = (float)(double)e.NewValue!;
                }
            });

    #endregion

    protected override SceneNode OnCreateSceneNode()
    {
        return new NodePostEffectXRayGrid();
    }

    /// <summary>
    /// Assigns the default values to core.
    /// </summary>
    /// <param name="core">The core.</param>
    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        base.AssignDefaultValuesToSceneNode(core);
        if (core is NodePostEffectXRayGrid c)
        {
            c.EffectName = EffectName;
            c.Color = GridColor.ToColor4();
            c.GridDensity = GridDensity;
            c.DimmingFactor = (float)DimmingFactor;
            c.BlendingFactor = (float)BlendingFactor;
        }
    }
}
