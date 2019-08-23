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
                return true;
            }
            /// <summary>
            /// Determines whether this instance [can hit test] the specified context.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <returns>
            ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
            /// </returns>
            protected override bool CanHitTest(RenderContext context)
            {
                return false;//return base.CanHitTest(context) && !hasBoneParameter;
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
        }
    }
}
