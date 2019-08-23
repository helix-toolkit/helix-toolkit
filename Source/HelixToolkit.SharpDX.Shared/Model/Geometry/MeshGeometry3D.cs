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
    using Core;
    using Utilities;
#if !NETFX_CORE
    [Serializable]
#endif
    [DataContract]
    public class MeshGeometry3D : Geometry3D
    {
        /// <summary>
        /// Does not raise property changed event
        /// </summary>
        [DataMember]
        public Vector3Collection Normals { get; set; }

        private Vector2Collection textureCoordinates = null;
        /// <summary>
        /// Texture Coordinates
        /// </summary>
        [DataMember]
        public Vector2Collection TextureCoordinates
        {
            get
            {
                return textureCoordinates;
            }
            set
            {
                Set(ref textureCoordinates, value);
            }
        }
        /// <summary>
        /// Does not raise property changed event
        /// </summary>
        [DataMember]
        public Vector3Collection Tangents { get; set; }

        /// <summary>
        /// Does not raise property changed event
        /// </summary>
        [DataMember]
        public Vector3Collection BiTangents { get; set; }

        public IEnumerable<Triangle> Triangles
        {
            get
            {
                for (int i = 0; i < Indices.Count; i += 3)
                {
                    yield return new Triangle() { P0 = Positions[Indices[i]], P1 = Positions[Indices[i + 1]], P2 = Positions[Indices[i + 2]], };
                }
            }
        }

        /// <summary>
        /// A proxy member for <see cref="Geometry3D.Indices"/>
        /// </summary>
        [IgnoreDataMember]
        public IntCollection TriangleIndices
        {
            get { return Indices; }
            set { Indices = new IntCollection(value); }
        }

        /// <summary>
        /// Merge meshes into one
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns></returns>
        public static MeshGeometry3D Merge(params MeshGeometry3D[] meshes)
        {
            var positions = new Vector3Collection();
            var indices = new IntCollection();

            var normals = meshes.All(x => x.Normals != null) ? new Vector3Collection() : null;
            var colors = meshes.All(x => x.Colors != null) ? new Color4Collection() : null;
            var textureCoods = meshes.All(x => x.TextureCoordinates != null) ? new Vector2Collection() : null;
            var tangents = meshes.All(x => x.Tangents != null) ? new Vector3Collection() : null;
            var bitangents = meshes.All(x => x.BiTangents != null) ? new Vector3Collection() : null;

            int index = 0;
            foreach (var part in meshes)
            {
                positions.AddRange(part.Positions);
                indices.AddRange(part.Indices.Select(x => x + index));
                index += part.Positions.Count;
            }

            if (normals != null)
            {
                normals = new Vector3Collection(meshes.SelectMany(x => x.Normals));
            }

            if (colors != null)
            {
                colors = new Color4Collection(meshes.SelectMany(x => x.Colors));
            }

            if (textureCoods != null)
            {
                textureCoods = new Vector2Collection(meshes.SelectMany(x => x.TextureCoordinates));
            }

            if (tangents != null)
            {
                tangents = new Vector3Collection(meshes.SelectMany(x => x.Tangents));
            }

            if (bitangents != null)
            {
                bitangents = new Vector3Collection(meshes.SelectMany(x => x.BiTangents));
            }

            var mesh = new MeshGeometry3D()
            {
                Positions = positions,
                Indices = indices,
                Normals = normals,
                Colors = colors,
                TextureCoordinates = textureCoods,
                Tangents = tangents,
                BiTangents = bitangents
            };
            return mesh;
        }


        protected override IOctreeBasic CreateOctree(OctreeBuildParameter parameter)
        {
            return new StaticMeshGeometryOctree(this.Positions, this.Indices, parameter);
        }

        protected override void OnAssignTo(Geometry3D target)
        {
            base.OnAssignTo(target);
            if(target is MeshGeometry3D mesh)
            {
                mesh.Normals = this.Normals;
                mesh.TextureCoordinates = this.TextureCoordinates;
                mesh.Tangents = this.Tangents;
                mesh.BiTangents = this.BiTangents;
            }
        }

        public virtual bool HitTest(RenderContext context, Matrix modelMatrix, ref Ray rayWS, ref List<HitTestResult> hits, object originalSource)
        {
            if(Positions == null || Positions.Count == 0
                || Indices == null || Indices.Count == 0)
            {
                return false;
            }
            bool isHit = false;
            if (Octree != null)
            {
                isHit = Octree.HitTest(context, originalSource, this, modelMatrix, rayWS, ref hits);
            }
            else
            {
                var result = new HitTestResult
                {
                    Distance = double.MaxValue
                };
                var modelInvert = modelMatrix.Inverted();
                if (modelInvert == Matrix.Zero)//Check if model matrix can be inverted.
                {
                    return false;
                }
                //transform ray into model coordinates
                var rayModel = new Ray(Vector3.TransformCoordinate(rayWS.Position, modelInvert), Vector3.Normalize(Vector3.TransformNormal(rayWS.Direction, modelInvert)));

                var b = this.Bound;
                //Do hit test in local space
                if (rayModel.Intersects(ref b))
                {
                    int index = 0;
                    foreach (var t in Triangles)
                    {
                        var v0 = t.P0;
                        var v1 = t.P1;
                        var v2 = t.P2;
                        if (Collision.RayIntersectsTriangle(ref rayModel, ref v0, ref v1, ref v2, out float d))
                        {
                            if (d > 0 && d < result.Distance) // If d is NaN, the condition is false.
                            {
                                result.IsValid = true;
                                result.ModelHit = originalSource;
                                // transform hit-info to world space now:
                                var pointWorld = Vector3.TransformCoordinate(rayModel.Position + (rayModel.Direction * d), modelMatrix);
                                result.PointHit = pointWorld;
                                result.Distance = (rayWS.Position - pointWorld).Length();
                                var p0 = Vector3.TransformCoordinate(v0, modelMatrix);
                                var p1 = Vector3.TransformCoordinate(v1, modelMatrix);
                                var p2 = Vector3.TransformCoordinate(v2, modelMatrix);
                                var n = Vector3.Cross(p1 - p0, p2 - p0);
                                n.Normalize();
                                // transform hit-info to world space now:
                                result.NormalAtHit = n;// Vector3.TransformNormal(n, m).ToVector3D();
                                result.TriangleIndices = new System.Tuple<int, int, int>(Indices[index], Indices[index + 1], Indices[index + 2]);
                                result.Tag = index / 3;
                                result.Geometry = this;
                                isHit = true;
                            }
                        }
                        index += 3;
                    }
                }
                if (isHit)
                {
                    hits.Add(result);
                }
            }
            return isHit;
        }

        /// <summary>
        /// Call to manually update texture coordinate buffer.
        /// </summary>
        public void UpdateTextureCoordinates()
        {
            RaisePropertyChanged(nameof(TextureCoordinates));
        }
    }


    public struct BatchedMeshGeometryConfig : IBatchedGeometry
    {
        public Geometry3D Geometry { private set; get; }
        public Matrix ModelTransform { private set; get; }
        public int MaterialIndex { private set; get; }
        public BatchedMeshGeometryConfig(Geometry3D geometry, Matrix modelTransform, int materialIndex)
        {
            Geometry = geometry;
            ModelTransform = modelTransform;
            MaterialIndex = materialIndex;
        }
    }
}
