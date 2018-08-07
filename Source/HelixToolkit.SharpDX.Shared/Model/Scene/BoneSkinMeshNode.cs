/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using HelixToolkit.Mathematics;
using System.Collections.Generic;
using System.Linq;
using Matrix = System.Numerics.Matrix4x4;
#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using System;
    using Core;
    /// <summary>
    /// 
    /// </summary>
    public class BoneSkinMeshNode : MeshNode
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
    }
}
