// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Media3D = System.Windows.Media.Media3D;
using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX.Animations;
using SharpDX;

namespace MorphTargetAnimationDemo
{
    public class MainViewModel : BaseViewModel
    {
        public SceneNodeGroupModel3D ModelGroup { get; private set; }

        private BoneSkinMeshNode skinnedMeshNode;

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            ModelGroup = new SceneNodeGroupModel3D();

            //Setup cube mesh
            MeshBuilder mb = new MeshBuilder();
            mb.AddCube();

            //Setup bone skinned geometry
            BoneSkinnedMeshGeometry3D skinnedGeometry = new BoneSkinnedMeshGeometry3D(mb.ToMesh());

            //Setup skinned mesh node
            skinnedMeshNode = new BoneSkinMeshNode();
            skinnedMeshNode.Geometry = skinnedGeometry;
            skinnedMeshNode.Material = new PhongMaterial();

            //Add skinned mesh node to model group
            ModelGroup.AddNode(skinnedMeshNode);

            //Create single morph target test
            MorphTargetVertex[] mtv = new MorphTargetVertex[mb.ToMesh().Positions.Count];
            for (int i = 0; i < mtv.Length; i++)
            {
                mtv[i].deltaPosition = new Vector3(0, 0, 0);
                mtv[i].deltaNormal = new Vector3(0, 0, 0);
                mtv[i].deltaTangent = new Vector3(0, 0, 0);
            }
            mtv[0].deltaPosition = new Vector3(5, 0, 0);

            (skinnedMeshNode.RenderCore as BoneSkinRenderCore).MorphTargetWeights = new float[] { 0 };
            (skinnedMeshNode.RenderCore as BoneSkinRenderCore).InitializeMorphTargets(mtv, 4 * 6);

            //Setup identity skelleton, in the future make this a method TODO
            skinnedMeshNode.BoneMatrices = new Matrix[] { Matrix.Identity };
            skinnedMeshNode.Bones = new Bone[] { new Bone() { Name="Identity", BindPose=Matrix.Identity, InvBindPose=Matrix.Identity, BoneLocalTransform=Matrix.Identity } };

            BoneSkinnedMeshGeometry3D geom = skinnedMeshNode.Geometry as BoneSkinnedMeshGeometry3D;
            geom.VertexBoneIds = new BoneIds[geom.Positions.Count];
            for (int i = 0; i < geom.VertexBoneIds.Count; i++)
                geom.VertexBoneIds[i] = new BoneIds() { Bone1 = 0, Weights = new Vector4(1, 0, 0, 0) };
        }

        public void SliderChanged(float value)
        {
            skinnedMeshNode.MorphTargetWeights = new float[] { value };
            skinnedMeshNode.InvalidateRender();
        }
    }
}