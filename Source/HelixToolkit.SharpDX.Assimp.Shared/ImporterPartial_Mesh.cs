/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Assimp;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

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
    using Model;
    using System.Threading;
    using HxAnimations = Animations;
    using HxScene = Model.Scene;
    namespace Assimp
    {
        public partial class Importer
        {
            /// <summary>
            ///     To the hx mesh nodes.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <param name="scene">The scene.</param>
            /// <param name="transform"></param>
            /// <returns></returns>
            /// <exception cref="System.NotSupportedException">Mesh Type {mesh.Type}</exception>
            protected virtual HxScene.SceneNode OnCreateHxMeshNode(MeshInfo mesh, HelixInternalScene scene, Matrix transform)
            {
                switch (mesh.Type)
                {
                    case PrimitiveType.Triangle:
                        var material = scene.Materials[mesh.MaterialIndex];
                        var cullMode = material.Key.HasTwoSided && material.Key.IsTwoSided
                            ? CullMode.Back
                            : CullMode.None;
                        if (Configuration.ForceCullMode)
                            cullMode = Configuration.CullMode;
                        var fillMode = material.Key.HasWireFrame && material.Key.IsWireFrameEnabled
                            ? FillMode.Wireframe
                            : FillMode.Solid;

                        //Determine if has bones
                        HxScene.MeshNode mnode;
                        if (mesh.AssimpMesh.HasBones || mesh.AssimpMesh.HasMeshAnimationAttachments)
                        {
                            var mn = new HxScene.BoneSkinMeshNode();

                            //Bones
                            if (mesh.AssimpMesh.HasBones)
                                mn.Bones = mesh.AssimpMesh.Bones.Select(x => new HxAnimations.Bone()
                                {
                                    Name = x.Name,
                                    BindPose = x.OffsetMatrix.ToSharpDXMatrix(configuration.IsSourceMatrixColumnMajor).Inverted(),
                                    BoneLocalTransform = Matrix.Identity,
                                    InvBindPose = x.OffsetMatrix.ToSharpDXMatrix(configuration.IsSourceMatrixColumnMajor),//Documented at https://github.com/assimp/assimp/pull/1803
                                }).ToArray();
                            else
                            {
                                mn.Geometry = mesh.Mesh;
                                mn.SetupIdentitySkeleton();
                            }

                            //Morph targets
                            if (mesh.AssimpMesh.HasMeshAnimationAttachments)
                            {
                                int attCount = mesh.AssimpMesh.MeshAnimationAttachmentCount;
                                var mtv = new FastList<MorphTargetVertex>(attCount * mesh.AssimpMesh.VertexCount);
                                foreach (var att in mesh.AssimpMesh.MeshAnimationAttachments)
                                {
                                    //NOTE: It seems some files may have invalid normal/tangent data for morph targets.
                                    //May need to provide option in future to use 0 normal/tangent deltas or recalculate
                                    mtv.AddRange(new MorphTargetVertex[mesh.AssimpMesh.VertexCount].Select((x, i) => 
                                    new MorphTargetVertex() {
                                        deltaPosition = (att.Vertices[i] - mesh.AssimpMesh.Vertices[i]).ToSharpDXVector3(),
                                        deltaNormal = att.HasNormals ? (att.Normals[i] - mesh.AssimpMesh.Normals[i]).ToSharpDXVector3() : Vector3.Zero,
                                        deltaTangent = att.HasTangentBasis ? (att.Tangents[i] - mesh.AssimpMesh.Tangents[i]).ToSharpDXVector3() : Vector3.Zero
                                    }));
                                }

                                mn.MorphTargetWeights = new float[attCount];
                                mn.InitializeMorphTargets(mtv.ToArray(), mesh.AssimpMesh.VertexCount);
                            }

                            mnode = mn;
                        }
                        else
                            mnode = new HxScene.MeshNode();

                        mnode.Name = string.IsNullOrEmpty(mesh.AssimpMesh.Name) ? $"{nameof(HxScene.MeshNode)}_{Interlocked.Increment(ref MeshIndexForNoName)}" : mesh.AssimpMesh.Name;
                        mnode.Geometry = mesh.Mesh;
                        mnode.Material = material.Value;
                        mnode.ModelMatrix = transform;
                        mnode.CullMode = cullMode;
                        mnode.FillMode = fillMode;
                        return mnode;
                    case PrimitiveType.Line:
                        var lnode = new HxScene.LineNode
                        {
                            Name = string.IsNullOrEmpty(mesh.AssimpMesh.Name)
                                ? $"{nameof(HxScene.LineNode)}_{Interlocked.Increment(ref MeshIndexForNoName)}"
                                : mesh.AssimpMesh.Name,
                            Geometry = mesh.Mesh,
                            ModelMatrix = transform
                        };
                        var lmaterial = new LineMaterialCore(); //Must create separate line material
                        lnode.Material = lmaterial;
                        var ml = scene.Materials[mesh.MaterialIndex].Value;
                        if (ml is DiffuseMaterialCore diffuse) lmaterial.LineColor = diffuse.DiffuseColor;
                        return lnode;
                    case PrimitiveType.Point:
                        var pnode = new HxScene.PointNode
                        {
                            Name = string.IsNullOrEmpty(mesh.AssimpMesh.Name)
                                ? $"{nameof(HxScene.PointNode)}_{Interlocked.Increment(ref MeshIndexForNoName)}"
                                : mesh.AssimpMesh.Name,
                            Geometry = mesh.Mesh,
                            ModelMatrix = transform
                        };
                        var pmaterial = new PointMaterialCore(); //Must create separate point material
                        pnode.Material = pmaterial;
                        var pm = scene.Materials[mesh.MaterialIndex].Value;
                        if (pm is DiffuseMaterialCore diffuse1) pmaterial.PointColor = diffuse1.DiffuseColor;
                        return pnode;
                    default:
                        throw new NotSupportedException($"Mesh Type {mesh.Type} does not supported");
                }
            }

            /// <summary>
            ///     To the helix mesh.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <returns></returns>
            protected virtual MeshGeometry3D OnCreateHelixMesh(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var builder = new MeshBuilder(false, false);
                builder.Positions.AddRange(hVertices);
                for (var i = 0; i < mesh.FaceCount; ++i)
                {
                    if (!mesh.Faces[i].HasIndices)
                    { continue; }
                    if (mesh.Faces[i].IndexCount == 3)
                    {
                        builder.AddTriangle(mesh.Faces[i].Indices);
                    }
                    else if (mesh.Faces[i].IndexCount == 4)
                    {
                        builder.AddTriangleFan(mesh.Faces[i].Indices);
                    }
                }
                var hMesh = new MeshGeometry3D { Positions = hVertices, Indices = builder.TriangleIndices };
                if (mesh.HasNormals && mesh.Normals.Count == hMesh.Positions.Count)
                {
                    hMesh.Normals = new Vector3Collection(mesh.Normals.Select(x => Vector3.Normalize(x.ToSharpDXVector3())));
                }
                else
                {
                    hMesh.Normals = hMesh.CalculateNormals();
                }
                if (mesh.HasVertexColors(0))
                {
                    hMesh.Colors =
                       new Color4Collection(mesh.VertexColorChannels[0].Select(x => new Color4(x.R, x.G, x.B, x.A)));
                }
                if (mesh.HasTextureCoords(0))
                {
                    hMesh.TextureCoordinates =
                       new Vector2Collection(mesh.TextureCoordinateChannels[0].Select(x => x.ToSharpDXVector2()));
                }
                if (mesh.HasTangentBasis && mesh.Tangents.Count == hMesh.Positions.Count && mesh.BiTangents.Count == hMesh.Positions.Count)
                {
                    hMesh.Tangents = new Vector3Collection(mesh.Tangents.Select(x => x.ToSharpDXVector3()));
                    hMesh.BiTangents = new Vector3Collection(mesh.BiTangents.Select(x => x.ToSharpDXVector3()));
                }
                else
                {
                    builder.Normals = hMesh.Normals;
                    builder.TextureCoordinates = hMesh.TextureCoordinates;
                    builder.ComputeTangents(MeshFaces.Default);
                    hMesh.Tangents = builder.Tangents;
                    hMesh.BiTangents = builder.BiTangents;
                }

                hMesh.UpdateBounds();
                if (configuration.BuildOctree)
                {
                    hMesh.UpdateOctree();
                }
                return hMesh;
            }
            /// <summary>
            /// To the helix mesh with bones.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <returns></returns>
            protected virtual BoneSkinnedMeshGeometry3D OnCreateHelixMeshWithBones(Mesh mesh)
            {
                var m = OnCreateHelixMesh(mesh);
                var vertBoneIds = new FastList<BoneIds>(Enumerable.Repeat(new BoneIds(), m.Positions.Count));
                var vertBoneInternal = vertBoneIds.GetInternalArray();
                var accumArray = new int[m.Positions.Count];
                var boneMesh = new BoneSkinnedMeshGeometry3D(m)
                {
                    VertexBoneIds = vertBoneIds
                };
                for (var j = 0; j < mesh.BoneCount; ++j)
                {
                    if (mesh.Bones[j].HasVertexWeights)
                    {
                        for (var i = 0; i < mesh.Bones[j].VertexWeightCount; ++i)
                        {
                            var vWeight = mesh.Bones[j].VertexWeights[i];
                            if (vWeight.VertexID >= accumArray.Length)
                            {
                                logger.LogWarning("Bone weight index is out of range. Num verts: {0}; Bone vert index: {1}", 
                                    accumArray.Length, vWeight.VertexID);
                                continue;
                            }
                            var currIdx = accumArray[vWeight.VertexID]++;
                            ref var id = ref vertBoneInternal[vWeight.VertexID];
                            switch (currIdx)
                            {
                                case 0:
                                    id.Bone1 = j;
                                    id.Weights.X = vWeight.Weight;
                                    break;
                                case 1:
                                    id.Bone2 = j;
                                    id.Weights.Y = vWeight.Weight;
                                    break;
                                case 2:
                                    id.Bone3 = j;
                                    id.Weights.Z = vWeight.Weight;
                                    break;
                                case 3:
                                    id.Bone4 = j;
                                    id.Weights.W = vWeight.Weight;
                                    break;
                                default:
                                    logger.LogWarning("Bone index count {0} is out of range. Maximum 4 bone indices per vertex are supported.", currIdx);
                                    break;
                            }
                        }
                    }
                }

                return boneMesh;
            }

            /// <summary>
            ///     To the helix point.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <returns></returns>
            protected virtual PointGeometry3D OnCreateHelixPoint(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var hMesh = new PointGeometry3D { Positions = hVertices };
                if (mesh.HasVertexColors(0))
                {
                    hMesh.Colors = new Color4Collection(mesh.VertexColorChannels[0].Select(x => x.ToSharpDXColor4()));
                }
                return hMesh;
            }

            /// <summary>
            ///     To the helix line.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <returns></returns>
            protected virtual LineGeometry3D OnCreateHelixLine(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var hIndices = new IntCollection(mesh.Faces.SelectMany(x => x.Indices));
                var hMesh = new LineGeometry3D { Positions = hVertices, Indices = hIndices };
                if (mesh.HasVertexColors(0))
                {
                    hMesh.Colors =
                       new Color4Collection(mesh.VertexColorChannels[0].Select(x => new Color4(x.R, x.G, x.B, x.A)));
                }
                return hMesh;
            }

            private MeshInfo OnCreateHelixGeometry(Mesh mesh)
            {
                switch (mesh.PrimitiveType)
                {
                    case PrimitiveType.Triangle:
                        if (mesh.HasBones || mesh.HasMeshAnimationAttachments)
                        {
                            return new MeshInfo(PrimitiveType.Triangle, mesh, OnCreateHelixMeshWithBones(mesh),
                                mesh.MaterialIndex);
                        }
                        else
                        {
                            return new MeshInfo(PrimitiveType.Triangle, mesh, OnCreateHelixMesh(mesh), mesh.MaterialIndex);
                        }
                    case PrimitiveType.Point:
                        return new MeshInfo(PrimitiveType.Point, mesh, OnCreateHelixPoint(mesh), mesh.MaterialIndex);
                    case PrimitiveType.Line:
                        return new MeshInfo(PrimitiveType.Line, mesh, OnCreateHelixLine(mesh), mesh.MaterialIndex);
                    default:
                        throw new NotSupportedException($"MeshType : {mesh.PrimitiveType} does not supported");
                }
            }

            /// <summary>
            /// </summary>
            protected sealed class MeshInfo
            {
                /// <summary>
                ///     The Assimp mesh
                /// </summary>
                public Mesh AssimpMesh;

                /// <summary>
                ///     The material index
                /// </summary>
                public int MaterialIndex;

                /// <summary>
                ///     The Helix mesh
                /// </summary>
                public Geometry3D Mesh;

                /// <summary>
                ///     The mesh type
                /// </summary>
                public PrimitiveType Type;

                /// <summary>
                ///     Initializes a new instance of the <see cref="MeshInfo" /> class.
                /// </summary>
                public MeshInfo()
                {
                }

                /// <summary>
                ///     Initializes a new instance of the <see cref="MeshInfo" /> class.
                /// </summary>
                /// <param name="type">The type.</param>
                /// <param name="assimpMesh">The assimp mesh.</param>
                /// <param name="mesh">The mesh.</param>
                /// <param name="materialIndex">Index of the material.</param>
                public MeshInfo(PrimitiveType type, Mesh assimpMesh, Geometry3D mesh, int materialIndex)
                {
                    Type = type;
                    Mesh = mesh;
                    AssimpMesh = assimpMesh;
                    MaterialIndex = materialIndex;
                }
            }
        }
    }
}
