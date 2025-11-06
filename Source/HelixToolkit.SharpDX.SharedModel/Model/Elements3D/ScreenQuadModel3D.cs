using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.SharpDX.Shaders;
using SharpDX.Direct3D11;

#if false
#elif WINUI
using System.Runtime.Versioning;
#elif WPF
#elif AVALONIA
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
#if false
#elif WINUI
[SupportedOSPlatform("windows")]
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
public class ScreenQuadModel3D : Element3D
{
    public TextureModel? Texture
    {
        get
        {
            return (TextureModel?)GetValue(TextureProperty);
        }
        set
        {
            SetValue(TextureProperty, value);
        }
    }

    public static readonly DependencyProperty TextureProperty =
        HelixProperty.Register<ScreenQuadModel3D, TextureModel?>("Texture",
            null, (d, e) =>
        {
            if (d is ScreenQuadModel3D { SceneNode: ScreenQuadNode node })
            {
                node.Texture = (TextureModel?)e.NewValue;
            }
        });

    public SamplerStateDescription SamplerDescription
    {
        get
        {
            return (SamplerStateDescription)GetValue(SamplerDescriptionProperty)!;
        }
        set
        {
            SetValue(SamplerDescriptionProperty, value);
        }
    }


    public static readonly DependencyProperty SamplerDescriptionProperty =
        HelixProperty.Register<ScreenQuadModel3D, SamplerStateDescription>("SamplerDescription",
            DefaultSamplers.LinearSamplerClampAni1, (d, e) =>
            {
                if (d is ScreenQuadModel3D { SceneNode: ScreenQuadNode node })
                {
                    node.Sampler = (SamplerStateDescription)e.NewValue!;
                }
            });

    /// <summary>
    /// Gets or sets the depth of the quad, range from 0 ~ 1.
    /// </summary>
    /// <value>
    /// The depth.
    /// </value>
    public double Depth
    {
        get
        {
            return (double)GetValue(DepthProperty)!;
        }
        set
        {
            SetValue(DepthProperty, value);
        }
    }

    public static readonly DependencyProperty DepthProperty =
        HelixProperty.Register<ScreenQuadModel3D, double>("Depth",
            1.0,
            (d, e) =>
            {
                if (d is ScreenQuadModel3D { SceneNode: ScreenQuadNode node })
                {
                    node.Depth = (float)Math.Max(0, Math.Min(1, (double)e.NewValue!));
                }
            });

    protected override SceneNode OnCreateSceneNode()
    {
        return new ScreenQuadNode();
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode node)
    {
        base.AssignDefaultValuesToSceneNode(node);
        if (node is ScreenQuadNode n)
        {
            n.Texture = Texture;
            n.Sampler = SamplerDescription;
            n.Depth = (float)Math.Max(0, Math.Min(1, Depth));
        }
    }
}
