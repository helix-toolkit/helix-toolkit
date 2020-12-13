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
                mtv[i].deltaPosition = new SharpDX.Vector3(0, 0, 0);
                mtv[i].deltaNormal = new SharpDX.Vector3(0, 0, 0);
                mtv[i].deltaTangent = new SharpDX.Vector3(0, 0, 0);
            }
            mtv[0].deltaPosition = new SharpDX.Vector3(5, 0, 0);

            (skinnedMeshNode.RenderCore as BoneSkinRenderCore).MorphTargetWeights = new float[] { .5f };
            (skinnedMeshNode.RenderCore as BoneSkinRenderCore).InitializeMorphTargets(mtv, 4 * 6);

            //TODO: Need to make it utilize an identity skeleton with 1 bone matrix by default
        }
    }
}