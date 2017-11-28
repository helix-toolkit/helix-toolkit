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
        private float relativeScreenLocX = -0.8f;
        public float RelativeScreenLocationX
        {
            set
            {
                if (relativeScreenLocX != value)
                {
                    relativeScreenLocX = value;
                    projectionMatrix.M41 = value;
                }
            }
            get
            {
                return relativeScreenLocX;
            }
        }

        private float relativeScreenLocY = -0.8f;
        public float RelativeScreenLocationY
        {
            set
            {
                if (relativeScreenLocY != value)
                {
                    relativeScreenLocY = value;
                    projectionMatrix.M42 = value;
                }
            }
            get
            {
                return relativeScreenLocY;
            }
        }

        private float sizeScale = 1;
        public float SizeScale
        {
            set
            {
                if (sizeScale == value)
                {
                    return;
                }
                sizeScale = value;
                OnCreateProjectionMatrix(value);
            }
            get
            {
                return sizeScale;
            }
        }

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

        protected Matrix CreateViewMatrix(IRenderMatrices renderContext)
        {
            return Matrix.LookAtRH(
                -renderContext.Camera.LookDirection.ToVector3().Normalized() * 20,
                Vector3.Zero,
                renderContext.Camera.UpDirection.ToVector3());
        }

        protected virtual void OnCreateProjectionMatrix(float scale)
        {
            projectionMatrix = Matrix.OrthoRH(140 * ScreenRatio / scale, 140 / scale, 1f, 200000);
            projectionMatrix.M41 = RelativeScreenLocationX;
            projectionMatrix.M42 = RelativeScreenLocationY;
        }

        protected void UpdateProjectionMatrix(double width, double height)
        {
            var ratio = (float)(width / height);
            if (ScreenRatio != ratio)
            {
                ScreenRatio = ratio;
                OnCreateProjectionMatrix(SizeScale);
            }
        }

        protected override void SetShaderVariables(IRenderMatrices context)
        {
            base.SetShaderVariables(context);
            var worldMatrix = context.WorldMatrix;
            worldMatrix.Row4 = new Vector4(0, 0, 0, 1);
            SetModelWorldMatrix(worldMatrix);
            this.viewMatrixVar.SetMatrix(CreateViewMatrix(context));
            UpdateProjectionMatrix(context.ActualWidth, context.ActualHeight);
            projectionMatrixVar.SetMatrix(projectionMatrix);
        }

        protected override void OnRender(IRenderMatrices renderContext)
        {           
            DepthStencilView dsView;
            renderContext.DeviceContext.OutputMerger.GetRenderTargets(out dsView);
            if (dsView == null)
            {
                return;
            }
            int depthStateRef;
            var depthStateBack = renderContext.DeviceContext.OutputMerger.GetDepthStencilState(out depthStateRef);
            renderContext.DeviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Depth, 1f, 0);

            var pass = EffectTechnique.GetPassByIndex(0);
            pass.Apply(renderContext.DeviceContext);
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(depthStencil);
            OnDraw(renderContext.DeviceContext, null);
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(depthStateBack);
            depthStateBack.Dispose();
            dsView.Dispose();
        }

        protected override void PostRender(IRenderMatrices context)
        {
            base.PostRender(context);
            viewMatrixVar.SetMatrix(context.ViewMatrix);
            projectionMatrixVar.SetMatrix(context.ProjectionMatrix);
        }
    }
}
