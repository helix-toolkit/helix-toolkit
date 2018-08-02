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
    }
}
