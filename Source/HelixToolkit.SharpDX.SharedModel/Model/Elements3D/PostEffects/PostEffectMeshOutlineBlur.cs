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
/// Highlight the border of meshes
/// </summary>
/// <seealso cref="Element3D" />
public class PostEffectMeshOutlineBlur : Element3D
{
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
    /// The effect name property
    /// </summary>
    public static readonly DependencyProperty EffectNameProperty =
        HelixProperty.Register<PostEffectMeshOutlineBlur, string>("EffectName",
            DefaultRenderTechniqueNames.PostEffectMeshOutlineBlur, (d, e) =>
            {
                if (d is Element3DCore { SceneNode: NodePostEffectMeshOutlineBlur core })
                {
                    core.EffectName = (string)e.NewValue!;
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
            return (UIColor)GetValue(ColorProperty)!;
        }
        set
        {
            SetValue(ColorProperty, value);
        }
    }

    /// <summary>
    /// The color property
    /// </summary>
    public static readonly DependencyProperty ColorProperty =
        HelixProperty.Register<PostEffectMeshOutlineBlur, UIColor>("Color",
            UIColors.Red, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: NodePostEffectMeshOutlineBlur core })
            {
                core.Color = ((UIColor)e.NewValue!).ToColor4();
            }
        });


    /// <summary>
    /// Gets or sets the scale x.
    /// </summary>
    /// <value>
    /// The scale x.
    /// </value>
    public double ScaleX
    {
        get
        {
            return (double)GetValue(ScaleXProperty)!;
        }
        set
        {
            SetValue(ScaleXProperty, value);
        }
    }

    /// <summary>
    /// The scale x property
    /// </summary>
    public static readonly DependencyProperty ScaleXProperty =
        HelixProperty.Register<PostEffectMeshOutlineBlur, double>("ScaleX",
            1.0, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: NodePostEffectMeshOutlineBlur core })
            {
                core.ScaleX = (float)(double)e.NewValue!;
            }
        });

    /// <summary>
    /// Gets or sets the scale y.
    /// </summary>
    /// <value>
    /// The scale y.
    /// </value>
    public double ScaleY
    {
        get
        {
            return (double)GetValue(ScaleYProperty)!;
        }
        set
        {
            SetValue(ScaleYProperty, value);
        }
    }

    /// <summary>
    /// The scale y property
    /// </summary>
    public static readonly DependencyProperty ScaleYProperty =
        HelixProperty.Register<PostEffectMeshOutlineBlur, double>("ScaleY",
            1.0, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: NodePostEffectMeshOutlineBlur core })
            {
                core.ScaleY = (float)(double)e.NewValue!;
            }
        });

    /// <summary>
    /// Gets or sets the number of blur pass.
    /// </summary>
    /// <value>
    /// The number of blur pass.
    /// </value>
    public int NumberOfBlurPass
    {
        get
        {
            return (int)GetValue(NumberOfBlurPassProperty)!;
        }
        set
        {
            SetValue(NumberOfBlurPassProperty, value);
        }
    }

    /// <summary>
    /// The number of blur pass property
    /// </summary>
    public static readonly DependencyProperty NumberOfBlurPassProperty =
        HelixProperty.Register<PostEffectMeshOutlineBlur, int>("NumberOfBlurPass",
            1, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: NodePostEffectMeshOutlineBlur core })
            {
                core.NumberOfBlurPass = (int)e.NewValue!;
            }
        });

    protected override SceneNode OnCreateSceneNode()
    {
        return new NodePostEffectMeshOutlineBlur();
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        base.AssignDefaultValuesToSceneNode(core);
        if (core is NodePostEffectMeshOutlineBlur c)
        {
            c.EffectName = EffectName;
            c.Color = Color.ToColor4();
            c.ScaleX = (float)ScaleX;
            c.ScaleY = (float)ScaleY;
            c.NumberOfBlurPass = (int)NumberOfBlurPass;
        }
    }
}
