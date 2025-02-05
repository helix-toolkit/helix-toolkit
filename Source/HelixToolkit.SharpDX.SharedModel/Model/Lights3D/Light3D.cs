using HelixToolkit.SharpDX;
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

public abstract class Light3D : Element3D
{
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(UIColor), typeof(Light3D), new PropertyMetadata(UIColors.Gray, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: LightNode node })
            {
                node.Color = ((UIColor)e.NewValue).ToColor4();
            }
        }));

    /// <summary>
    /// Color of the light.
    /// For simplicity, this color applies to the diffuse and specular properties of the light.
    /// </summary>
    public UIColor Color
    {
        get
        {
            return (UIColor)GetValue(ColorProperty);
        }
        set
        {
            SetValue(ColorProperty, value);
        }
    }

    public LightType LightType
    {
        get
        {
            if (SceneNode is LightNode node)
            {
                return node.LightType;
            }

            return LightType.None;
        }
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        if (core is LightNode node)
        {
            node.Color = Color.ToColor4();
        }

        base.AssignDefaultValuesToSceneNode(core);
    }
}
