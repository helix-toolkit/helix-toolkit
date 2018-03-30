/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    /// <summary>
    /// 
    /// </summary>
    public class BoneSkinMeshNode : MeshNode
    {
        /// <summary>
        /// Gets or sets the vertex bone ids.
        /// </summary>
        /// <value>
        /// The vertex bone ids.
        /// </value>
        public IList<BoneIds> VertexBoneIds
        {
            set
            {
                bonesBufferModel.Elements = value;
            }
            get { return bonesBufferModel.Elements; }
        }
        /// <summary>
        /// Gets or sets the bone matrices.
        /// </summary>
        /// <value>
        /// The bone matrices.
        /// </value>
        public BoneMatricesStruct BoneMatrices
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
        /// The bones buffer model
        /// </summary>
        protected readonly IElementsBufferModel<BoneIds> bonesBufferModel = new VertexBoneIdBufferModel<BoneIds>(BoneIds.SizeInBytes);
        private IBoneSkinRenderParams boneSkinRenderCore
        {
            get { return (IBoneSkinRenderParams)RenderCore; }
        }
        /// <summary>
        /// Called when [create render technique].
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.BoneSkinBlinn];
        }
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new BoneSkinRenderCore();
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToCore(RenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            boneSkinRenderCore.BoneMatrices = BoneMatrices;
        }
        /// <summary>
        /// Called when [attach].
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                bonesBufferModel.Initialize();
                boneSkinRenderCore.VertexBoneIdBuffer = bonesBufferModel;
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Called when [detach].
        /// </summary>
        protected override void OnDetach()
        {
            bonesBufferModel.DisposeAndClear();
            base.OnDetach();
        }
        /// <summary>
        /// Checks the bounding frustum.
        /// </summary>
        /// <param name="boundingFrustum">The bounding frustum.</param>
        /// <returns></returns>
        protected override bool CheckBoundingFrustum(BoundingFrustum boundingFrustum)
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
        protected override bool CanHitTest(IRenderContext context)
        {
            return false;//return base.CanHitTest(context) && !hasBoneParameter;
        }
    }
}
