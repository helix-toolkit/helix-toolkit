/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Numerics;
using HelixToolkit.Mathematics;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Utilities;
    using Core;
#if !NETFX_CORE
    [Serializable]
#endif
    public class BoneSkinnedMeshGeometry3D : MeshGeometry3D
    {
        public BoneSkinnedMeshGeometry3D()
        {

        }

        public BoneSkinnedMeshGeometry3D(MeshGeometry3D mesh)
        {
            mesh.AssignTo(this);
        }

        private IList<BoneIds> vertexBoneIds;
        /// <summary>
        /// Gets or sets the vertex bone ids and bone weights.
        /// </summary>
        /// <value>
        /// The vertex bone ids.
        /// </value>
        public IList<BoneIds> VertexBoneIds
        {
            set
            {
                Set(ref vertexBoneIds, value);
            }
            get
            {
                return vertexBoneIds;
            }
        }
        /// <summary>
        /// Gets or sets the animations.
        /// </summary>
        /// <value>
        /// The animations.
        /// </value>
        public Dictionary<string, Animations.Animation> Animations { set; get; }
        /// <summary>
        /// Gets or sets the bones.
        /// </summary>
        /// <value>
        /// The bones.
        /// </value>
        public IList<Animations.Bone> Bones { set; get; }
        /// <summary>
        /// Gets or sets the bone names.
        /// </summary>
        /// <value>
        /// The bone names.
        /// </value>
        public IList<string> BoneNames { set; get; }


        public static BoneSkinnedMeshGeometry3D CreateSkeletonMesh(IList<Animations.Bone> bones, float scale = 1f)
        {
            var builder = new MeshBuilder(true, false);
            builder.AddPyramid(new Vector3(0, scale / 2, 0), Vector3.UnitZ, Vector3.UnitY, scale, 0, true);
            var singleBone = builder.ToMesh();
            var boneIds = new List<BoneIds>();
            var positions = new Vector3Collection(bones.Count * singleBone.Positions.Count);
            var tris = new IntCollection(bones.Count * singleBone.Indices.Count);

            int offset = 0;

            for (int i = 0; i < bones.Count; ++i)
            {
                if (bones[i].ParentIndex >= 0)
                {
                    int currPos = positions.Count;
                    tris.AddRange(singleBone.Indices.Select(x => x + offset));
                    int j = 0;
                    for (; j < singleBone.Positions.Count - 6; j += 3)
                    {
                        positions.Add(Vector3Helper.TransformCoordinate(singleBone.Positions[j], bones[bones[i].ParentIndex].BindPose));
                        positions.Add(Vector3Helper.TransformCoordinate(singleBone.Positions[j + 1], bones[bones[i].ParentIndex].BindPose));
                        positions.Add(bones[i].BindPose.Translation);
                        boneIds.Add(new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4(1, 0, 0, 0) });
                        boneIds.Add(new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4(1, 0, 0, 0) });
                        boneIds.Add(new BoneIds() { Bone1 = i, Weights = new Vector4(1, 0, 0, 0) });
                    }
                    for (; j < singleBone.Positions.Count; ++j)
                    {
                        positions.Add(Vector3Helper.TransformCoordinate(singleBone.Positions[j], bones[bones[i].ParentIndex].BindPose));
                        boneIds.Add(new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4(1, 0, 0, 0) });
                    }
                    offset += singleBone.Positions.Count;
                }
            }

            builder = new MeshBuilder(true, false);
            for (int i=0; i < bones.Count; ++i)
            {
                int currPos = builder.Positions.Count;
                builder.AddSphere(Vector3.Zero, scale / 2, 12, 12);
                for (int j = currPos; j < builder.Positions.Count; ++j)
                {
                    builder.Positions[j] = Vector3Helper.TransformCoordinate(builder.Positions[j], bones[i].BindPose);
                    boneIds.Add(new BoneIds() { Bone1 = i, Weights = new Vector4(1, 0, 0, 0) });
                }
            }
            positions.AddRange(builder.Positions);
            tris.AddRange(builder.TriangleIndices.Select(x => x + offset));
            var mesh = new BoneSkinnedMeshGeometry3D() { Positions = positions, Indices = tris, VertexBoneIds = boneIds };
            mesh.Normals = mesh.CalculateNormals();
            return mesh;
        }
    }
}
