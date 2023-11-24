using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.WinUI.SharpDX.Model;
using Color = Windows.UI.Color;
using Colors = Microsoft.UI.Colors;

namespace HelixToolkit.WinUI.SharpDX;

public abstract class Light3D : Element3D
{
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Color), typeof(Light3D), new PropertyMetadata(Colors.Gray, (d, e) =>
        {
            if (d is Element3DCore element && element.SceneNode is LightNode node)
            {
                node.Color = ((Color)e.NewValue).ToColor4();
            }
        }));

    /// <summary>
    /// Color of the light.
    /// For simplicity, this color applies to the diffuse and specular properties of the light.
    /// </summary>
    public Color Color
    {
        get { return (Color)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    public LightType LightType => (SceneNode as LightNode)?.LightType ?? LightType.None;

    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        if (core is LightNode lightNode)
        {
            lightNode.Color = Color.ToColor4();
        }

        base.AssignDefaultValuesToSceneNode(core);
    }
}
