using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
using SharpDX;

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
