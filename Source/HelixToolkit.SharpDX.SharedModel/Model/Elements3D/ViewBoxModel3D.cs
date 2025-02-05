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

/// <summary>
/// <para>Viewbox replacement for Viewport using swapchain rendering.</para>
/// <para>To replace box texture (such as text, colors), bind to custom material with different diffuseMap. </para>
/// <para>Create a image with 1 row and 6 evenly distributed columns. Each column occupies one box face. The face order is Front, Back, Down, Up, Left, Right</para>
/// </summary>
public class ViewBoxModel3D : ScreenSpacedElement3D
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register("UpDirection", typeof(Vector3D), typeof(ViewBoxModel3D),
        new PropertyMetadata(new Vector3D(0, 1, 0),
        (d, e) =>
        {
            if (d is Element3DCore { SceneNode: ViewBoxNode node })
            {
                node.UpDirection = ((Vector3D)e.NewValue).ToVector3();
            }
        }));


    /// <summary>
    /// Gets or sets up direction.
    /// </summary>
    /// <value>
    /// Up direction.
    /// </value>
    public Vector3D UpDirection
    {
        set
        {
            SetValue(UpDirectionProperty, value);
        }
        get
        {
            return (Vector3D)GetValue(UpDirectionProperty);
        }
    }


    public static readonly DependencyProperty ViewBoxTextureProperty = DependencyProperty.Register("ViewBoxTexture", typeof(TextureModel), typeof(ViewBoxModel3D),
        new PropertyMetadata(null, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: ViewBoxNode node })
            {
                node.ViewBoxTexture = (TextureModel?)e.NewValue;
            }
        }));

    public TextureModel? ViewBoxTexture
    {
        set
        {
            SetValue(ViewBoxTextureProperty, value);
        }
        get
        {
            return (TextureModel?)GetValue(ViewBoxTextureProperty);
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether [enable edge click].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable edge click]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableEdgeClick
    {
        get
        {
            return (bool)GetValue(EnableEdgeClickProperty);
        }
        set
        {
            SetValue(EnableEdgeClickProperty, value);
        }
    }

    /// <summary>
    /// The enable edge click property
    /// </summary>
    public static readonly DependencyProperty EnableEdgeClickProperty =
        DependencyProperty.Register("EnableEdgeClick", typeof(bool), typeof(ViewBoxModel3D), new PropertyMetadata(false, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: ViewBoxNode node })
            {
                node.EnableEdgeClick = (bool)e.NewValue;
            }
        }));

    protected override SceneNode OnCreateSceneNode()
    {
        var node = new ViewBoxNode();
        return node;
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode node)
    {
        if (node is ViewBoxNode view)
        {
            view.UpDirection = this.UpDirection.ToVector3();
        }

        base.AssignDefaultValuesToSceneNode(node);
    }
}
