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
        }
    }
}