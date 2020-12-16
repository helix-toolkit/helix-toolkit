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
            mb.ComputeNormalsAndTangents(MeshFaces.Default, true);

            //Setup bone skinned geometry
            BoneSkinnedMeshGeometry3D skinnedGeometry = new BoneSkinnedMeshGeometry3D(mb.ToMesh());

            //Setup skinned mesh node
            skinnedMeshNode = new BoneSkinMeshNode();
            skinnedMeshNode.Geometry = skinnedGeometry;
            skinnedMeshNode.Material = new PhongMaterial();

            //Add skinned mesh node to model group
            ModelGroup.AddNode(skinnedMeshNode);

            //We dont have a skeleton for this cube, just use identity
            skinnedMeshNode.SetupIdentitySkeleton();

            //Create morph targets
            System.Random r = new System.Random();
            int pitch = mb.ToMesh().Positions.Count;
            MorphTargetVertex[] mtv = new MorphTargetVertex[pitch * 4];
            for (int i = 0; i < mtv.Length; i++)
            {
                mtv[i].deltaPosition = new Vector3(0, 0, 0);
                mtv[i].deltaNormal = new Vector3(0, 0, 0);
                mtv[i].deltaTangent = new Vector3(0, 0, 0);

                if (i < pitch)
                    mtv[i].deltaPosition = skinnedGeometry.Normals[i];
                else if (i < pitch * 2)
                    mtv[i].deltaPosition = skinnedGeometry.Tangents[i % pitch];
                else if (i < pitch * 3)
                {
                    Vector3 p = skinnedGeometry.Positions[i % pitch] - new Vector3(.5f, .5f, .5f);
                    Vector3 n = skinnedGeometry.Normals[i % pitch];
                    mtv[i].deltaPosition = p - (Vector3.Dot(p, n) / n.LengthSquared() * n);
                }
                else if (i < pitch * 4)
                {
                    mtv[i].deltaPosition = new Vector3(r.NextFloat(0, 1), r.NextFloat(0, 1), r.NextFloat(0, 1));
                }
            }

            (skinnedMeshNode.RenderCore as BoneSkinRenderCore).MorphTargetWeights = new float[] { 0, 0, 0, 0 };
            (skinnedMeshNode.RenderCore as BoneSkinRenderCore).InitializeMorphTargets(mtv, pitch);
        }

        public void SliderChanged(int id, float value)
        {
            skinnedMeshNode.SetWeight(id, value);
            skinnedMeshNode.InvalidateRender();
        }
    }
}