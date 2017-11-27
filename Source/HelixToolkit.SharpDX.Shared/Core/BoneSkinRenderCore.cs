using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class BoneSkinRenderCore : MeshRenderCore
    {
        public IElementsBufferModel VertexBoneIdBuffer { set; get; }

        public BoneMatricesStruct BoneMatrices
        {
            set;get;
        }

        private EffectMatrixVariable boneMatricesVar;

        protected override bool OnAttach(IRenderHost host, RenderTechnique technique)
        {
            if(base.OnAttach(host, technique))
            {
                boneMatricesVar = Effect.GetVariableByName("SkinMatrices").AsMatrix();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool CanRender()
        {
            return base.CanRender() && VertexBoneIdBuffer != null && VertexBoneIdBuffer.HasElements;
        }

        protected override void OnAttachBuffers(DeviceContext context)
        {
            GeometryBuffer.AttachBuffers(context, this.VertexLayout, 0);            
            VertexBoneIdBuffer?.AttachBuffer(context, 1);
            InstanceBuffer?.AttachBuffer(context, 2);
        }

        protected override void SetShaderVariables(IRenderMatrices context)
        {
            base.SetShaderVariables(context);
            boneMatricesVar.SetMatrix(BoneMatrices.Bones);
        }

        protected override void PostRender(IRenderMatrices context)
        {
            VertexBoneIdBuffer?.ResetHasElementsVariable();
            base.PostRender(context);
        }
    }
}
