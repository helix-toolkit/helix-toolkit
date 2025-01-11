using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Highlight the border of meshes
/// </summary>
public class PostEffectMeshBorderHighlight : PostEffectMeshOutlineBlur
{
    /// <summary>
    /// Gets or sets the draw mode.
    /// </summary>
    /// <value>
    /// The draw mode.
    /// </value>
    public OutlineMode DrawMode
    {
        get
        {
            return (OutlineMode)GetValue(DrawModeProperty);
        }
        set
        {
            SetValue(DrawModeProperty, value);
        }
    }

    /// <summary>
    /// The draw mode property
    /// </summary>
    public static readonly DependencyProperty DrawModeProperty =
        DependencyProperty.Register("DrawMode", typeof(OutlineMode), typeof(PostEffectMeshBorderHighlight), new PropertyMetadata(OutlineMode.Merged,
            (d, e) =>
            {
                if (d is Element3D { SceneNode: NodePostEffectBorderHighlight core })
                {
                    core.DrawMode = (OutlineMode)e.NewValue;
                }
            }));


    protected override SceneNode OnCreateSceneNode()
    {
        return new NodePostEffectBorderHighlight();
    }
}
