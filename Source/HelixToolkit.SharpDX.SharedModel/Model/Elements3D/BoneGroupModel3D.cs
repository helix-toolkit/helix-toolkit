using HelixToolkit.SharpDX.Model.Scene;
using SharpDX;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Used to share bone matrices for multiple <see cref="BoneSkinMeshGeometryModel3D"/>
/// </summary>
public sealed class BoneGroupModel3D : GroupModel3D
{
    /// <summary>
    /// Gets or sets the bone matrices.
    /// </summary>
    /// <value>
    /// The bone matrices.
    /// </value>
    public Matrix[] BoneMatrices
    {
        get
        {
            return (Matrix[])GetValue(BoneMatricesProperty);
        }
        set
        {
            SetValue(BoneMatricesProperty, value);
        }
    }

    /// <summary>
    /// The bone matrices property
    /// </summary>
    public static readonly DependencyProperty BoneMatricesProperty =
        DependencyProperty.Register("BoneMatrices", typeof(Matrix[]), typeof(BoneGroupModel3D), new PropertyMetadata(null, (d, e) =>
        {
            if (d is Element3D { SceneNode: BoneGroupNode node })
            {
                node.BoneMatrices = (Matrix[])e.NewValue;
            }
        }));

    protected override SceneNode OnCreateSceneNode()
    {
        return new BoneGroupNode();
    }
}
