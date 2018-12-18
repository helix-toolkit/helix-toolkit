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
