using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;

#if fals
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
/// <seealso cref="Element3D" />
public class PostEffectMeshXRay : Element3D
{
    #region Dependency Properties
    /// <summary>
    /// The effect name property
    /// </summary>
    public static readonly DependencyProperty EffectNameProperty =
        DependencyProperty.Register("EffectName", typeof(string), typeof(PostEffectMeshXRay), new PropertyMetadata(DefaultRenderTechniqueNames.PostEffectMeshXRay, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: NodePostEffectXRay core })
            {
                core.EffectName = (string)e.NewValue;
            }
        }));

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
            return (string)GetValue(EffectNameProperty);
        }
        set
        {
            SetValue(EffectNameProperty, value);
        }
    }


    /// <summary>
    /// The outline color property
    /// </summary>
    public static readonly DependencyProperty OutlineColorProperty = DependencyProperty.Register("OutlineColor", typeof(UIColor), typeof(PostEffectMeshXRay),
        new PropertyMetadata(UIColors.Blue,
        (d, e) =>
        {
            if (d is Element3DCore { SceneNode: NodePostEffectXRay core })
            {
                core.Color = ((UIColor)e.NewValue).ToColor4();
            }
        }));

    /// <summary>
    /// Gets or sets the color of the outline.
    /// </summary>
    /// <value>
    /// The color of the outline.
    /// </value>
    public UIColor OutlineColor
    {
        set
        {
            SetValue(OutlineColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(OutlineColorProperty);
        }
    }

    /// <summary>
    /// The outline fading factor property
    /// </summary>
    public static readonly DependencyProperty OutlineFadingFactorProperty = DependencyProperty.Register("OutlineFadingFactor", typeof(double), typeof(PostEffectMeshXRay),
        new PropertyMetadata(1.5, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: NodePostEffectXRay core })
            {
                core.OutlineFadingFactor = (float)(double)e.NewValue;
            }
        }));

    /// <summary>
    /// Gets or sets the outline fading factor.
    /// </summary>
    /// <value>
    /// The outline fading factor.
    /// </value>
    public double OutlineFadingFactor
    {
        set
        {
            SetValue(OutlineFadingFactorProperty, value);
        }
        get
        {
            return (double)GetValue(OutlineFadingFactorProperty);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [double pass]. Double pass uses stencil buffer to reduce overlapping artifacts
    /// </summary>
    public static readonly DependencyProperty EnableDoublePassProperty =
        DependencyProperty.Register("EnableDoublePass", typeof(bool), typeof(PostEffectMeshXRay), new PropertyMetadata(false, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: NodePostEffectXRay core })
            {
                core.EnableDoublePass = (bool)e.NewValue;
            }
        }));


    /// <summary>
    /// Gets or sets a value indicating whether [double pass]. Double pass uses stencil buffer to reduce overlapping artifacts
    /// </summary>
    public bool EnableDoublePass
    {
        get
        {
            return (bool)GetValue(EnableDoublePassProperty);
        }
        set
        {
            SetValue(EnableDoublePassProperty, value);
        }
    }
    #endregion

    protected override SceneNode OnCreateSceneNode()
    {
        return new NodePostEffectXRay();
    }

    /// <summary>
    /// Assigns the default values to core.
    /// </summary>
    /// <param name="core">The core.</param>
    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        base.AssignDefaultValuesToSceneNode(core);
        if (core is NodePostEffectXRay c)
        {
            c.EffectName = EffectName;
            c.Color = OutlineColor.ToColor4();
            c.OutlineFadingFactor = (float)OutlineFadingFactor;
            c.EnableDoublePass = EnableDoublePass;
        }
    }
}
