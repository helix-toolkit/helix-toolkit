using System.Collections.Generic;
using Matrix = System.Numerics.Matrix4x4;
#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Model.Scene;
    /// <summary>
    /// 
    /// </summary>
    public class BoneSkinMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static DependencyProperty BoneMatricesProperty = DependencyProperty.Register("BoneMatrices", typeof(Matrix[]), typeof(BoneSkinMeshGeometryModel3D),
            new PropertyMetadata(BoneMatricesStruct.DefaultBones,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as BoneSkinMeshNode).BoneMatrices = (Matrix[])e.NewValue;
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
}
