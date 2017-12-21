using SharpDX;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Utilities;
    public class BoneSkinRenderCore : PatchMeshRenderCore
    {
        public IElementsBufferModel VertexBoneIdBuffer { set; get; }

        public BoneMatricesStruct BoneMatrices
        {
            set;get;
        }

        private IBufferProxy boneCB;

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                boneCB = technique.ConstantBufferPool.Register(DefaultConstantBufferDescriptions.BoneCB);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool CanRender()
        {
            return base.CanRender()&& VertexBoneIdBuffer != null && VertexBoneIdBuffer.HasElements;
        }

        protected override void OnAttachBuffers(DeviceContext context)
        {
            base.OnAttachBuffers(context);         
            VertexBoneIdBuffer?.AttachBuffer(context, 2);
            if (BoneMatrices.Bones != null)
            {
                boneCB.UploadDataToBuffer(context, BoneMatrices.Bones);
            }
        }

        protected override void OnUpdateModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            model.HasBones = BoneMatrices.Bones != null ? 1 : 0;
            base.OnUpdateModelStruct(ref model, context);           
        }
    }
}
