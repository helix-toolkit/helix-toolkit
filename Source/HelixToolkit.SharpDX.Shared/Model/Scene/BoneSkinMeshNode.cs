/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using System.Collections.Generic;
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
    namespace Model.Scene
    {
        using Core;
        /// <summary>
        /// 
        /// </summary>
        public class BoneSkinMeshNode : MeshNode, Animations.IBoneMatricesNode
        {
            /// <summary>
            /// Gets or sets the bone matrices.
            /// </summary>
            /// <value>
            /// The bone matrices.
            /// </value>
            public Matrix[] BoneMatrices
            {
                set
                {
                    (RenderCore as BoneSkinRenderCore).BoneMatrices = value;
                }
                get
                {
                    return (RenderCore as BoneSkinRenderCore).BoneMatrices;
                }
            }
            /// <summary>
            /// Gets or sets the bones.
            /// </summary>
            /// <value>
            /// The bones.
            /// </value>
            public Animations.Bone[] Bones { set; get; }
            /// <summary>
            /// Gets or sets a value indicating whether this node is used to show skeleton. Only used as an indication.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this node is used to show skeleton; otherwise, <c>false</c>.
            /// </value>
            public bool IsSkeletonNode { set; get; }
            /// <summary>
            /// Gets or sets a value indicating whether this node has bone group. 
            /// <see cref="BoneGroupNode"/> shares bones with multiple <see cref="BoneSkinMeshNode"/>
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance has bone group; otherwise, <c>false</c>.
            /// </value>
            public bool HasBoneGroup { internal set; get; }

            private Vector3[] skinnedVerticesCache = new Vector3[0];
            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                return new BoneSkinRenderCore();
            }

            protected override IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
            {
                return !(EffectsManager.GeometryBufferManager.Register<BoneSkinnedMeshBufferModel>(modelGuid, geometry) is IBoneSkinMeshBufferModel buffer) ? 
                    EmptyGeometryBufferModel.Empty : new BoneSkinPreComputeBufferModel(buffer, buffer.VertexStructSize.FirstOrDefault()) as IAttachableBufferModel;
            }
            /// <summary>
            /// Views the frustum test.
            /// </summary>
            /// <param name="viewFrustum">The view frustum.</param>
            /// <returns></returns>
            public override bool TestViewFrustum(ref BoundingFrustum viewFrustum)
            {
                return BoneMatrices.Length == 0 ? base.TestViewFrustum(ref viewFrustum) : true;
            }

            protected override bool PreHitTestOnBounds(ref Ray ray)
            {
                return BoneMatrices.Length == 0 ? base.PreHitTestOnBounds(ref ray) : true;
            }
            /// <summary>
            /// Creates the skeleton node.
            /// </summary>
            /// <param name="material">The material.</param>
            /// <param name="effectName">Name of the effect.</param>
            /// <param name="scale">The scale.</param>
            /// <returns></returns>
            public BoneSkinMeshNode CreateSkeletonNode(MaterialCore material, string effectName, float scale = 0.1f)
            {
                return CreateSkeletonNode(this, material, effectName, scale);
            }
            /// <summary>
            /// Creates the skeleton node.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <param name="material">The material.</param>
            /// <param name="effectName">Name of the effect.</param>
            /// <param name="scale">The scale.</param>
            /// <returns></returns>
            public static BoneSkinMeshNode CreateSkeletonNode(BoneSkinMeshNode node, MaterialCore material, string effectName, float scale)
            {
                var skNode = new BoneSkinMeshNode()
                {
                    Material = material,
                    IsSkeletonNode = true,
                };
                skNode.Geometry = BoneSkinnedMeshGeometry3D.CreateSkeletonMesh(node.Bones, scale);
                skNode.PostEffects = effectName;
                skNode.Bones = node.Bones;
                return skNode;
            }
            /// <summary>
            /// Try to get skinned vertices. Skinned vertices are copied from GPU.
            /// </summary>
            /// <param name="manager"></param>
            /// <returns>New array with vertex positions</returns>
            public Vector3[] TryGetSkinnedVertices(IEffectsManager manager)
            {
                if (Geometry is BoneSkinnedMeshGeometry3D skGeometry)
                {
                    if (RenderCore is BoneSkinRenderCore skCore)
                    {
#if DX11_1
                        var proxy = new Render.DeviceContextProxy(manager.Device.ImmediateContext1, manager.Device);
#else
                        var proxy = new Render.DeviceContextProxy(manager.Device.ImmediateContext, manager.Device);
#endif
                        var array = new Vector3[skGeometry.Positions.Count];
                        if(skCore.CopySkinnedToArray(proxy, array) > 0)
                        {
                            return array;
                        }
                    }
                }
                return null;
            }
            /// <summary>
            /// Try to get skinned vertices. Skinned vertices are copied from GPU.
            /// </summary>
            /// <param name="manager"></param>
            /// <param name="array">Vertex positions will be copied into this array</param>
            /// <returns></returns>
            public int TryGetSkinnedVertices(IEffectsManager manager, Vector3[] array)
            {
                if (Geometry is BoneSkinnedMeshGeometry3D skGeometry)
                {
                    if (RenderCore is BoneSkinRenderCore skCore)
                    {
#if DX11_1
                        var proxy = new Render.DeviceContextProxy(manager.Device.ImmediateContext1, manager.Device);
#else
                        var proxy = new Render.DeviceContextProxy(manager.Device.ImmediateContext, manager.Device);
#endif
                        return skCore.CopySkinnedToArray(proxy, array);
                    }
                }
                return 0;
            }
            /// <summary>
            /// Get the skinned vertices cache used for hit test.
            /// This cache will only be updated after each hit test.
            /// To get latest skinned vertices, please use <see cref="TryGetSkinnedVertices(IEffectsManager)"/>. 
            /// </summary>
            /// <returns></returns>
            public Vector3[] TryGetSkinnedVerticesCache()
            {
                return skinnedVerticesCache;
            }

            protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray rayWS, ref List<HitTestResult> hits)
            {
                if(BoneMatrices.Length > 0 && Geometry is BoneSkinnedMeshGeometry3D skGeometry)
                {
                    if(RenderCore is BoneSkinRenderCore skCore)
                    {
                        if(skinnedVerticesCache.Length < skGeometry.Positions.Count)
                        {
                            skinnedVerticesCache = new Vector3[skGeometry.Positions.Count];
                        }
                        if (skCore.CopySkinnedToArray(context.RenderHost.ImmediateDeviceContext, skinnedVerticesCache) > 0)
                        {
                            return skGeometry.HitTestWithSkinnedVertices(context, skinnedVerticesCache,
                                totalModelMatrix, ref rayWS, ref hits, WrapperSource);
                        }
                    }
                }
                return base.OnHitTest(context, totalModelMatrix, ref rayWS, ref hits);
            }
        }
    }
}
