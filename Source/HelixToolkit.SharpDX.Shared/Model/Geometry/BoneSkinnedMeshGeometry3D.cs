/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using global::SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
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
        /// Creates the node based bone matrices.
        /// </summary>
        /// <param name="bones">The bones.</param>
        /// <param name="rootInvTransform">The root inv transform.</param>
        /// <returns></returns>
        public static Matrix[] CreateNodeBasedBoneMatrices(IList<Animations.Bone> bones, ref Matrix rootInvTransform)
        {
            Matrix[] m = null;
            CreateNodeBasedBoneMatrices(bones, ref rootInvTransform, ref m);
            return m;
        }

        /// <summary>
        /// Creates the node based bone matrices.
        /// </summary>
        /// <param name="bones">The bones.</param>
        /// <param name="matrices"></param>
        /// <param name="rootInvTransform"></param>
        /// <returns></returns>
        public static void CreateNodeBasedBoneMatrices(IList<Animations.Bone> bones, ref Matrix rootInvTransform, ref Matrix[] matrices)
        {
            var m = matrices ?? new Matrix[bones.Count];
            for(int i = 0; i <bones.Count; ++i)
            {
                if(bones[i].Node != null)
                {
                    m[i] = bones[i].InvBindPose * bones[i].Node.TotalModelMatrixInternal * rootInvTransform;
                }
                else
                {
                    m[i] = Matrix.Identity;
                }
            }
            matrices = m;
        }


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
                        positions.Add(Vector3.TransformCoordinate(singleBone.Positions[j], bones[bones[i].ParentIndex].BindPose));
                        positions.Add(Vector3.TransformCoordinate(singleBone.Positions[j + 1], bones[bones[i].ParentIndex].BindPose));
                        positions.Add(bones[i].BindPose.TranslationVector);
                        boneIds.Add(new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4(1, 0, 0, 0) });
                        boneIds.Add(new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4(1, 0, 0, 0) });
                        boneIds.Add(new BoneIds() { Bone1 = i, Weights = new Vector4(1, 0, 0, 0) });
                    }
                    for (; j < singleBone.Positions.Count; ++j)
                    {
                        positions.Add(Vector3.TransformCoordinate(singleBone.Positions[j], bones[bones[i].ParentIndex].BindPose));
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
                    builder.Positions[j] = Vector3.TransformCoordinate(builder.Positions[j], bones[i].BindPose);
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
