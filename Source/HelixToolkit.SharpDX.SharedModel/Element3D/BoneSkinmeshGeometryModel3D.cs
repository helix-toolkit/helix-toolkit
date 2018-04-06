
using SharpDX;
using System.Collections.Generic;

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
        public static DependencyProperty VertexBoneIdsProperty = DependencyProperty.Register("VertexBoneIds", typeof(IList<BoneIds>), typeof(BoneSkinMeshGeometryModel3D),
            new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BoneSkinMeshNode).VertexBoneIds = e.NewValue as IList<BoneIds>;
            }));

        public IList<BoneIds> VertexBoneIds
        {
            set
            {
                SetValue(VertexBoneIdsProperty, value);
            }
            get
            {
                return (IList<BoneIds>)GetValue(VertexBoneIdsProperty);
            }
        }

        public static DependencyProperty BoneMatricesProperty = DependencyProperty.Register("BoneMatrices", typeof(BoneMatricesStruct), typeof(BoneSkinMeshGeometryModel3D),
            new PropertyMetadata(new BoneMatricesStruct() { Bones = new Matrix[BoneMatricesStruct.NumberOfBones] },
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as BoneSkinMeshNode).BoneMatrices = (BoneMatricesStruct)e.NewValue;
                }));

        public BoneMatricesStruct BoneMatrices
        {
            set
            {
                SetValue(BoneMatricesProperty, value);
            }
            get
            {
                return (BoneMatricesStruct)GetValue(BoneMatricesProperty);
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new BoneSkinMeshNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            if (core is BoneSkinMeshNode n)
            {
                n.BoneMatrices = BoneMatrices;
            }
            base.AssignDefaultValuesToSceneNode(core);
        }
    }
}
