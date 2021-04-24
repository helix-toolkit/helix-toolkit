/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Assimp;
using System;
using System.Linq;

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
    using System.Collections.Generic;
    using System.Threading;
    using HxScene = Model.Scene;
    namespace Assimp
    {
        public partial class Exporter
        {
            /// <summary>
            /// Gets the geometry from node. Currently only supports <see cref="HxScene.GeometryNode"/>
            /// </summary>
            /// <param name="node">The node.</param>
            /// <param name="geometry">The geometry.</param>
            /// <returns></returns>
            protected virtual bool GetGeometryFromNode(HxScene.SceneNode node, out Geometry3D geometry)
            {
                if (node is HxScene.GeometryNode geo)
                {
                    geometry = geo.Geometry;
                    return true;
                }
                else
                {
                    geometry = null;
                    return false;
                }
            }

            /// <summary>
            /// Creates the mesh information. Since Assimp requires each mesh has one material only.
            /// Geometry has to be duplicated if they are using different material
            /// </summary>
            /// <param name="geoNode">The geo node.</param>
            /// <returns></returns>
            private MeshInfo OnCreateMeshInfo(HxScene.GeometryNode geoNode)
            {
                MeshInfo info = null;
                if (geoNode is HxScene.MaterialGeometryNode materialNode && materialNode.Material != null)
                {
                    var key = GetMaterialGeoKey(geoNode, out var materialIndex, out var geoIndex);
                    if (!meshInfos.TryGetValue(key, out var existing))
                    {
                        info = new MeshInfo(key, geoNode.Geometry, geoNode.Name, geoIndex, materialIndex);
                    }
                    else
                    {
                        info = existing;
                    }
                    if(info != null && info.Bones == null && materialNode is HxScene.BoneSkinMeshNode boneNode)
                    {
                        info.Bones = boneNode.Bones;
                    }
                }
                return info;
            }

            private ulong GetMaterialGeoKey(HxScene.GeometryNode node, out int materialIndex, out int geoIndex)
            {
                if(geometryCollection.TryGetValue(node.Geometry, out geoIndex))
                {
                    if(node is HxScene.MaterialGeometryNode materialNode && materialNode.Material != null
                        && materialCollection.TryGetValue(materialNode.Material, out materialIndex))
                    {

                        return GetMaterialGeoKey(materialIndex, geoIndex);
                    }
                    else
                    {
                        materialIndex = 0;
                        return GetMaterialGeoKey(0, geoIndex);
                    }
                }
                else
                {
                    throw new ArgumentException("Geometry Key not found.");
                }
            }

            private ulong GetMaterialGeoKey(int materialIndex, int geometryIndex)
            {
                ulong key = (ulong)materialIndex << 32 | (uint)geometryIndex;
                return key;
            }
            /// <summary>
            /// Called when [create assimp mesh] from <see cref="MeshInfo"/>.
            /// </summary>
            /// <param name="info">The information.</param>
            /// <returns></returns>
            protected virtual Mesh OnCreateAssimpMesh(MeshInfo info)
            {
                var assimpMesh = new Mesh(string.IsNullOrEmpty(info.Name) ? $"Mesh_{Interlocked.Increment(ref MeshIndexForNoName)}" : info.Name) { MaterialIndex = info.MaterialIndex };
                if (info.Mesh.Positions != null && info.Mesh.Positions.Count > 0)
                {
                    assimpMesh.Vertices.AddRange(info.Mesh.Positions.Select(x => x.ToAssimpVector3D()));
                }

                if(info.Mesh.Indices != null && info.Mesh.Indices.Count > 0)
                {
                    for(int i = 0; i < info.Mesh.Indices.Count; i += 3)
                    {
                        assimpMesh.Faces.Add(new Face(new int[] { info.Mesh.Indices[i], info.Mesh.Indices[i + 1], info.Mesh.Indices[i + 2] }));
                    }
                }
                if (info.Mesh.Colors != null && info.Mesh.Colors.Count > 0)
                {
                    assimpMesh.VertexColorChannels[0] = new List<Color4D>(info.Mesh.Colors.Select(x => x.ToAssimpColor4D()));
                }
                if (info.Mesh is MeshGeometry3D mesh)
                {
                    assimpMesh.PrimitiveType = PrimitiveType.Triangle;
                    if(mesh.Normals != null && mesh.Normals.Count > 0)
                    {
                        assimpMesh.Normals.AddRange(mesh.Normals.Select(x => x.ToAssimpVector3D()));
                        if(mesh.BiTangents != null && mesh.BiTangents.Count > 0)
                        {
                            assimpMesh.BiTangents.AddRange(mesh.BiTangents.Select(x => x.ToAssimpVector3D()));
                        }
                        if(mesh.Tangents != null && mesh.Tangents.Count > 0)
                        {
                            assimpMesh.Tangents.AddRange(mesh.Tangents.Select(x => x.ToAssimpVector3D()));
                        }
                    }
                    if(mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count > 0)
                    {
                        assimpMesh.TextureCoordinateChannels[0] = new List<Vector3D>(mesh.TextureCoordinates.Select(x => x.ToAssimpVector3D()));
                    }
                    if(info.Bones != null &&
                        mesh is BoneSkinnedMeshGeometry3D boneSkinMesh 
                        && boneSkinMesh.VertexBoneIds.Count == boneSkinMesh.Positions.Count)
                    {
                        foreach(var b in info.Bones)
                        {
                            var bone = new Bone
                            {
                                Name = b.Name,
                                OffsetMatrix = b.InvBindPose.ToAssimpMatrix(configuration.ToSourceMatrixColumnMajor)
                            };
                            assimpMesh.Bones.Add(bone);
                        }
                        int boneCount = assimpMesh.Bones.Count;
                        for(int i = 0; i < boneSkinMesh.VertexBoneIds.Count; ++i)
                        {
                            var id = boneSkinMesh.VertexBoneIds[i];
                            
                            if (id.Weights.X != 0 && id.Bone1 < boneCount)
                            {
                                var bone = assimpMesh.Bones[id.Bone1];
                                bone.VertexWeights.Add(new VertexWeight(i, id.Weights.X));
                            }
                            if (id.Weights.Y != 0 && id.Bone2 < boneCount)
                            {
                                var bone = assimpMesh.Bones[id.Bone2];
                                bone.VertexWeights.Add(new VertexWeight(i, id.Weights.Y));
                            }
                            if(id.Weights.Z != 0 && id.Bone3 < boneCount)
                            {
                                var bone = assimpMesh.Bones[id.Bone3];
                                bone.VertexWeights.Add(new VertexWeight(i, id.Weights.Z));
                            }
                            if(id.Weights.W != 0 && id.Bone4 < boneCount)
                            {
                                var bone = assimpMesh.Bones[id.Bone4];
                                bone.VertexWeights.Add(new VertexWeight(i, id.Weights.W));
                            }
                        }                      
                    }
                }
                else if(info.Mesh is PointGeometry3D pgeo)
                {
                    assimpMesh.PrimitiveType = PrimitiveType.Point;
                }
                else if(info.Mesh is LineGeometry3D lgeo)
                {
                    assimpMesh.PrimitiveType = PrimitiveType.Line;
                }
                else
                {
                    Log(HelixToolkit.Logger.LogLevel.Warning, $"Geometry type does not support yet. Type: {info.Mesh.GetType().Name}");
                }
                return assimpMesh;
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
                /// The material mesh key
                /// </summary>
                public readonly ulong MaterialMeshKey;

                /// <summary>
                ///     The Helix mesh
                /// </summary>
                public readonly Geometry3D Mesh;
                /// <summary>
                /// The mesh index
                /// </summary>
                public readonly int MeshIndex;
                /// <summary>
                /// The material index
                /// </summary>
                public readonly int MaterialIndex;
                /// <summary>
                /// The name
                /// </summary>
                public readonly string Name;
                /// <summary>
                /// The bones if have
                /// </summary>
                public Animations.Bone[] Bones { set; get; }
                /// <summary>
                /// Initializes a new instance of the <see cref="MeshInfo"/> class.
                /// </summary>
                /// <param name="materialMeshKey">The material mesh key.</param>
                /// <param name="mesh">The mesh.</param>
                /// <param name="name">The name.</param>
                /// <param name="meshIndex">Index of the mesh.</param>
                /// <param name="materialIndex">Index of the material.</param>
                /// <param name="bones">The bones.</param>
                public MeshInfo(ulong materialMeshKey, Geometry3D mesh, string name, int meshIndex, int materialIndex, Animations.Bone[] bones = null)
                {
                    Mesh = mesh;
                    MaterialMeshKey = materialMeshKey;
                    MeshIndex = meshIndex;
                    MaterialIndex = materialIndex;
                    Name = name;
                    Bones = bones;
                }
            }
        }
    }

}
