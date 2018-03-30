
using SharpDX;
using System.Collections.Generic;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    using Model;
    using Model.Scene;

    public class BoneSkinMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static DependencyProperty VertexBoneIdsProperty = DependencyProperty.Register("VertexBoneIds", typeof(IList<BoneIds>), typeof(BoneSkinMeshGeometryModel3D), 
            new PropertyMetadata(null, (d,e)=>
            {
                ((d as Element3DCore).SceneNode as NodeBoneSkinMesh).VertexBoneIds = e.NewValue as IList<BoneIds>;
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
                    ((d as Element3DCore).SceneNode as NodeBoneSkinMesh).BoneMatrices = (BoneMatricesStruct)e.NewValue;
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
            return new NodeBoneSkinMesh();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            if(core is NodeBoneSkinMesh n)
            {
                n.BoneMatrices = BoneMatrices;
            }
            base.AssignDefaultValuesToSceneNode(core);
        }
    }
}
