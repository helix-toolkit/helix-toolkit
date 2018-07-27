/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using global::SharpDX;

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

        public Dictionary<string, Animations.Animation> Animations { set; get; }
        /// <summary>
        /// Merge meshes into one
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns></returns>
        public static BoneSkinnedMeshGeometry3D Merge(params BoneSkinnedMeshGeometry3D[] meshes)
        {
            var positions = new Vector3Collection();
            var indices = new IntCollection();

            var normals = meshes.All(x => x.Normals != null) ? new Vector3Collection() : null;
            var colors = meshes.All(x => x.Colors != null) ? new Color4Collection() : null;
            var textureCoods = meshes.All(x => x.TextureCoordinates != null) ? new Vector2Collection() : null;
            var tangents = meshes.All(x => x.Tangents != null) ? new Vector3Collection() : null;
            var bitangents = meshes.All(x => x.BiTangents != null) ? new Vector3Collection() : null;
            var vertexIds = meshes.All(x => x.VertexBoneIds != null) ? new List<BoneIds>() : null;
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

            if(vertexIds != null)
            {
                vertexIds = new List<BoneIds>(meshes.SelectMany(x => x.VertexBoneIds));
            }

            var mesh = new BoneSkinnedMeshGeometry3D()
            {
                Positions = positions,
                Indices = indices,
                Normals = normals,
                Colors = colors,
                TextureCoordinates = textureCoods,
                Tangents = tangents,
                BiTangents = bitangents,
                VertexBoneIds = vertexIds
            };
            return mesh;
        }
    }
}
