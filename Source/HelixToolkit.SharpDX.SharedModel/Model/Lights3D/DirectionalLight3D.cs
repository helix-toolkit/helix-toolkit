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

public sealed class DirectionalLight3D : Light3D
{
    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register("Direction", typeof(Vector3D), typeof(Light3D), new PropertyMetadata(new Vector3D(),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: DirectionalLightNode node })
                {
                    node.Direction = ((Vector3D)e.NewValue).ToVector3();
                }
            }));

    /// <summary>
    /// Direction of the light.
    /// It applies to Directional Light and to Spot Light,
    /// for all other lights it is ignored.
    /// </summary>
    public Vector3D Direction
    {
        get
        {
            return (Vector3D)this.GetValue(DirectionProperty);
        }
        set
        {
            this.SetValue(DirectionProperty, value);
        }
    }

    protected override SceneNode OnCreateSceneNode()
    {
        return new DirectionalLightNode();
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        base.AssignDefaultValuesToSceneNode(core);

        if (core is DirectionalLightNode node)
        {
            node.Direction = Direction.ToVector3();
        }
    }
}
