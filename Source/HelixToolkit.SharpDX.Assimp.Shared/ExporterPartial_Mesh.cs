/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Assimp;
using SharpDX;
using SharpDX.Direct3D11;
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
    using Model;
    using System.Collections.Generic;
    using HxAnimations = Animations;
    using HxScene = Model.Scene;
    namespace Assimp
    {
        public partial class Exporter
        {

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
            private MeshInfo CreateMeshInfo(HxScene.GeometryNode geoNode)
            {
                if (geoNode is HxScene.MaterialGeometryNode materialNode)
                {
                    if(materialNode.Material != null)
                    {
                        if(materialCollection.TryGetValue(materialNode.Material, out var materialIndex))
                        {
                            if (geometryCollection.TryGetValue(geoNode.Geometry, out var geoIndex))
                            {
                                var key = GetMaterialGeoKey(materialIndex, geoIndex);
                                if (!meshInfos.ContainsKey(key))
                                {
                                    var info = new MeshInfo(key, ToAssimpMesh(geoNode.Geometry), geoNode.Geometry, (int)geoIndex);
                                    return info;
                                }
                            }
                        }
                        else
                        {
                            Log(HelixToolkit.Logger.LogLevel.Warning, $"Material not found, Name: {materialNode.Material.Name}");
                        }
                    }
                }
                //else if(geometryCollection.TryGetValue(geoNode.Geometry, out var geoIndex))
                //{
                //    return new MeshInfo(GetMaterialGeoKey(0, geoIndex), ToAssimpMesh(geoNode.Geometry), geoNode.Geometry);
                //}
                return null;
            }

            private ulong GetMaterialGeoKey(uint materialIndex, uint geometryIndex)
            {
                ulong key = (ulong)materialIndex << 32 | geometryIndex;
                return key;
            }

            protected virtual Mesh ToAssimpMesh(Geometry3D geometry)
            {
                var assimpMesh = new Mesh(geometry.Name);
                if (geometry.Positions != null && geometry.Positions.Count > 0)
                {
                    assimpMesh.Vertices.AddRange(geometry.Positions.Select(x => x.ToAssimpVector3D()));
                }

                if(geometry.Indices != null && geometry.Indices.Count > 0)
                {
                    for(int i=0; i < geometry.Indices.Count; i += 3)
                    {
                        assimpMesh.Faces.Add(new Face(new int[] { geometry.Indices[i], geometry.Indices[i + 1], geometry.Indices[i + 2] }));
                    }
                }
                if (geometry.Colors != null && geometry.Colors.Count > 0)
                {
                    assimpMesh.VertexColorChannels[0] = new List<Color4D>(geometry.Colors.Select(x => x.ToAssimpColor4D()));
                }
                if (geometry is MeshGeometry3D mesh)
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
                }
                else if(geometry is PointGeometry3D pgeo)
                {
                    assimpMesh.PrimitiveType = PrimitiveType.Point;
                }
                else if(geometry is LineGeometry3D lgeo)
                {
                    assimpMesh.PrimitiveType = PrimitiveType.Line;
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
                public readonly Mesh AssimpMesh;

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
                /// Gets the index of the material.
                /// </summary>
                /// <value>
                /// The index of the material.
                /// </value>
                public int MaterialIndex { get => AssimpMesh.MaterialIndex; }

                public MeshInfo(ulong materialMeshKey, Mesh assimpMesh, Geometry3D mesh, int meshIndex)
                {
                    Mesh = mesh;
                    AssimpMesh = assimpMesh;
                    MaterialMeshKey = materialMeshKey;
                    MeshIndex = meshIndex;
                }
            }
        }
    }

}
