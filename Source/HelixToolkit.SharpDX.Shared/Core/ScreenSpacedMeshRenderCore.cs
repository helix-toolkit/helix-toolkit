using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    /// <summary>
    /// Used to change view matrix and projection matrix to screen spaced coordinate system.
    /// <para>Usage: Call SetScreenSpacedCoordinates(RenderHost) to move coordinate system. Call other render functions for sub models. Finally call RestoreCoordinates(RenderHost) to restore original coordinate system.</para>
    /// </summary>
    public class ScreenSpacedMeshRenderCore : RenderCoreBase
    {
        private EffectMatrixVariable viewMatrixVar;
        private EffectMatrixVariable projectionMatrixVar;
        private Matrix projectionMatrix;
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
                viewMatrixVar = Collect(Effect.GetVariableByName(ShaderVariableNames.ViewMatrix).AsMatrix());
                projectionMatrixVar = Collect(Effect.GetVariableByName(ShaderVariableNames.ProjectionMatrix).AsMatrix());
                return true;
            }
            else
            {
                return false;
            }
        }

        protected Matrix CreateViewMatrix(IRenderMatrices renderContext)
        {
#if !NETFX_CORE
            return Matrix.LookAtRH(
                -renderContext.Camera.LookDirection.ToVector3().Normalized() * 20,
                Vector3.Zero,
                renderContext.Camera.UpDirection.ToVector3());
#else
            return Matrix.LookAtRH(-renderContext.Camera.LookDirection.Normalized() * 20, Vector3.Zero, renderContext.Camera.UpDirection);
#endif
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
        }

        protected override bool CanRender()
        {
            return false;
        }

        protected override void OnRender(IRenderMatrices renderContext)
        {           
        }

        protected override void PostRender(IRenderMatrices context)
        {
        }

        public virtual void SetScreenSpacedCoordinates(IRenderMatrices context, bool clearDepthBuffer = true)
        {
            DepthStencilView dsView;
            context.DeviceContext.OutputMerger.GetRenderTargets(out dsView);
            if (dsView == null)
            {
                return;
            }

            context.DeviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Depth, 1f, 0);
            dsView.Dispose();

            this.viewMatrixVar.SetMatrix(CreateViewMatrix(context));
            UpdateProjectionMatrix(context.ActualWidth, context.ActualHeight);
            projectionMatrixVar.SetMatrix(projectionMatrix);
        }

        public virtual void RestoreCoordinates(IRenderMatrices context)
        {
            viewMatrixVar.SetMatrix(context.ViewMatrix);
            projectionMatrixVar.SetMatrix(context.ProjectionMatrix);
        }
    }
}
