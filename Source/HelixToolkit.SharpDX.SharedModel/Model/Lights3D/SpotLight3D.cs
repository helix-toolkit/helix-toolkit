using HelixToolkit.SharpDX.Model.Scene;
#if WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#else
using HelixToolkit.Wpf.SharpDX.Model;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public sealed class SpotLight3D : PointLight3D
{
    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register("Direction", typeof(Vector3D), typeof(SpotLight3D), new PropertyMetadata(new Vector3D(),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: SpotLightNode node })
                {
                    node.Direction = ((Vector3D)e.NewValue).ToVector3();
                }
            }));

    public static readonly DependencyProperty FalloffProperty =
        DependencyProperty.Register("Falloff", typeof(double), typeof(SpotLight3D), new PropertyMetadata(1.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: SpotLightNode node })
                {
                    node.FallOff = (float)(double)e.NewValue;
                }
            }));

    public static readonly DependencyProperty InnerAngleProperty =
        DependencyProperty.Register("InnerAngle", typeof(double), typeof(SpotLight3D), new PropertyMetadata(5.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: SpotLightNode node })
                {
                    node.InnerAngle = (float)(double)e.NewValue;
                }
            }));

    public static readonly DependencyProperty OuterAngleProperty =
        DependencyProperty.Register("OuterAngle", typeof(double), typeof(SpotLight3D), new PropertyMetadata(45.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: SpotLightNode node })
                {
                    node.OuterAngle = (float)(double)e.NewValue;
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
    /// <summary>
    /// Decay Exponent of the spotlight.
    /// The falloff the spotlight between inner and outer angle
    /// depends on this value.
    /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
    /// </summary>
    public double Falloff
    {
        get
        {
            return (double)this.GetValue(FalloffProperty);
        }
        set
        {
            this.SetValue(FalloffProperty, value);
        }
    }

    /// <summary>
    /// Full outer angle of the spot (Phi) in degrees
    /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
    /// </summary>
    public double OuterAngle
    {
        get
        {
            return (double)this.GetValue(OuterAngleProperty);
        }
        set
        {
            this.SetValue(OuterAngleProperty, value);
        }
    }

    /// <summary>
    /// Full inner angle of the spot (Theta) in degrees. 
    /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
    /// </summary>
    public double InnerAngle
    {
        get
        {
            return (double)this.GetValue(InnerAngleProperty);
        }
        set
        {
            this.SetValue(InnerAngleProperty, value);
        }
    }

    protected override SceneNode OnCreateSceneNode()
    {
        return new SpotLightNode();
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        base.AssignDefaultValuesToSceneNode(core);
        if (core is SpotLightNode c)
        {
            c.Direction = Direction.ToVector3();
            c.InnerAngle = (float)InnerAngle;
            c.OuterAngle = (float)OuterAngle;
            c.FallOff = (float)Falloff;
        }
    }
}
