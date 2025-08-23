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
public class EnvironmentMap3D : Element3D
{
    /// <summary>
    /// The texture property
    /// </summary>
    public static readonly DependencyProperty TextureProperty =
        HelixProperty.Register<EnvironmentMap3D, TextureModel?>("Texture",
            null, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: EnvironmentMapNode node })
            {
                node.Texture = (TextureModel?)e.NewValue;
            }
        });

    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    /// <value>
    /// The texture.
    /// </value>
    public TextureModel? Texture
    {
        set
        {
            SetValue(TextureProperty, value);
        }
        get
        {
            return (TextureModel?)GetValue(TextureProperty);
        }
    }

    public static readonly DependencyProperty SkipRenderingProperty =
        HelixProperty.Register<EnvironmentMap3D, bool>("SkipRendering",
            false, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: EnvironmentMapNode node })
            {
                node.SkipRendering = (bool)e.NewValue!;
            }
        });

    /// <summary>
    /// Skip environment map rendering, but still keep it available for other object to use.
    /// </summary>
    public bool SkipRendering
    {
        set
        {
            SetValue(SkipRenderingProperty, value);
        }
        get
        {
            return (bool)GetValue(SkipRenderingProperty)!;
        }
    }

    /// <summary>
    /// Called when [create scene node].
    /// </summary>
    /// <returns></returns>
    protected override SceneNode OnCreateSceneNode()
    {
        return new EnvironmentMapNode();
    }

    /// <summary>
    /// Assigns the default values to scene node.
    /// </summary>
    /// <param name="core">The core.</param>
    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        base.AssignDefaultValuesToSceneNode(core);

        if (SceneNode is EnvironmentMapNode n)
        {
            n.Texture = Texture;
        }
    }
}
