using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
using SharpDX;

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

/// <summary>
/// 
/// </summary>
public class BoneSkinMeshGeometryModel3D : MeshGeometryModel3D
{
    public static readonly DependencyProperty BoneMatricesProperty = DependencyProperty.Register("BoneMatrices", typeof(Matrix[]), typeof(BoneSkinMeshGeometryModel3D),
        new PropertyMetadata(BoneMatricesStruct.DefaultBones,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: BoneSkinMeshNode node })
                {
                    node.BoneMatrices = (Matrix[])e.NewValue;
                }
            }));

    public Matrix[] BoneMatrices
    {
        set
        {
            SetValue(BoneMatricesProperty, value);
        }
        get
        {
            return (Matrix[])GetValue(BoneMatricesProperty);
        }
    }

    protected override SceneNode OnCreateSceneNode()
    {
        return new BoneSkinMeshNode();
    }
}
