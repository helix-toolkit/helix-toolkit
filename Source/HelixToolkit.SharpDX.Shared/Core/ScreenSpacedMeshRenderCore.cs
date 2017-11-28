using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class ScreenSpacedMeshRenderCore : MeshRenderCore
    {
        private EffectMatrixVariable viewMatrixVar;
        private EffectMatrixVariable projectionMatrixVar;
        private Matrix projectionMatrix;
        private DepthStencilState depthStencil;
        public float ScreenRatio = 1f;

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                viewMatrixVar = Effect.GetVariableByName(ShaderVariableNames.ViewMatrix).AsMatrix();
                projectionMatrixVar = Effect.GetVariableByName(ShaderVariableNames.ProjectionMatrix).AsMatrix();
                RemoveAndDispose(ref depthStencil);
                depthStencil = Collect(CreateDepthStencilState(this.Device));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual DepthStencilState CreateDepthStencilState(Device device)
        {
            return new DepthStencilState(device, new DepthStencilStateDescription() { IsDepthEnabled = true, IsStencilEnabled = false, DepthWriteMask = DepthWriteMask.All, DepthComparison = Comparison.LessEqual });
        }

        protected Matrix CreateViewMatrix(RenderContext renderContext)
        {
            return Matrix.LookAtRH(
                -renderContext.Camera.LookDirection.ToVector3().Normalized() * 20,
                Vector3.Zero,
                renderContext.Camera.UpDirection.ToVector3());
        }
    }
}
