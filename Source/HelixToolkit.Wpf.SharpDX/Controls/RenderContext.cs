// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderContext.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   The render-context is currently generated per frame
//   Optimizations might be possible
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;

    using global::SharpDX;

    using global::SharpDX.Direct3D11;
    using HelixToolkit.SharpDX;

    /// <summary>
    /// The render-context is currently generated per frame
    /// Optimizations might be possible
    /// </summary>
    public class RenderContext : IRenderMatrices, IDisposable
    {       
        internal Matrix worldMatrix = Matrix.Identity;
        internal Matrix viewMatrix;
        internal Matrix projectionMatrix;
        internal BoundingFrustum boundingFrustum;
        private Camera camera; 
        private EffectVectorVariable vEyePos, vFrustum, vViewport;
        private EffectMatrixVariable mView, mProjection, mVP;
        private bool matrixChanged = true;

        public Matrix ViewMatrix
        {
            get { return this.viewMatrix; }
            private set
            {
                if (viewMatrix == value) { return; }
                viewMatrix = value;
                matrixChanged = true;
            }
        }

        public Matrix ProjectionMatrix
        {
            get { return this.projectionMatrix; }
            set
            {
                if (projectionMatrix == value)
                {
                    return;
                }
                projectionMatrix = value;
                matrixChanged = true;
            }
        }

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                if (worldMatrix == value)
                {
                    return;
                }
                worldMatrix = value;
                matrixChanged = true;
            }
        }

        public Matrix ViewportMatrix
        {
            get
            {
                return new Matrix((float)(ActualWidth / 2), 0, 0, 0,
                    0, (float)(-ActualHeight / 2), 0, 0, 
                    0, 0, 1, 0,
                    (float)((ActualWidth - 1) / 2), (float)((ActualHeight - 1) / 2), 0, 1);
            }
        }

        private Matrix screenViewProjectionMatrix = Matrix.Identity;
        public Matrix ScreenViewProjectionMatrix
        {
            get
            {
                return GetScreenViewProjectionMatrix();
            }
        }

        private Matrix viewProjectionMatrix = Matrix.Identity;
        public Matrix ViewProjectionMatrix
        {
            get
            {
                return GetViewProjectionMatrix();
            }
        }

        public bool EnableBoundingFrustum = false;

        public DeviceContext DeviceContext { set; get; }

        public double ActualWidth { get; private set; }

        public double ActualHeight { get; private set; }

        public Camera Camera
        {
            get { return this.camera; }
            set
            {                
                this.camera = value;
                ActualHeight = this.Canvas.ActualHeight;
                ActualWidth = this.Canvas.ActualWidth;
                ViewMatrix = this.camera.CreateViewMatrix();
                var aspectRatio = this.ActualWidth / this.ActualHeight;
                ProjectionMatrix = this.camera.CreateProjectionMatrix(aspectRatio);
                if (this.camera is ProjectionCamera)
                {
                    var c = this.camera as ProjectionCamera;
                    // viewport: W,H,0,0   
                    var viewport = new Vector4((float)this.ActualWidth, (float)this.ActualHeight, 0, 0);
                    var ar = viewport.X / viewport.Y;
                    
                    var  pc = c as PerspectiveCamera;
                    var fov = (pc != null) ? pc.FieldOfView : 90f;

                    var zn = c.NearPlaneDistance > 0 ? c.NearPlaneDistance : 0.1;
                    var zf = c.FarPlaneDistance + 0.0;
                    // frustum: FOV,AR,N,F
                    var frustum = new Vector4((float)fov, (float)ar, (float)zn, (float)zf);
                    if(EnableBoundingFrustum)
                        boundingFrustum = new BoundingFrustum(this.viewMatrix * this.projectionMatrix);

                    this.vViewport.Set(ref viewport);
                    this.vFrustum.Set(ref frustum);

                    this.vEyePos.Set(this.camera.Position.ToVector3());
                    this.mView.SetMatrix(ref viewMatrix);
                    this.mProjection.SetMatrix(ref projectionMatrix);

                    UpdateMatrices();
                    this.mVP.SetMatrix(ref viewProjectionMatrix);
                }
            }
        }
            

        public IRenderHost Canvas { get; private set; }

        public bool IsShadowPass { get; set; }

        public bool IsDeferredPass { get; set; }

        public TimeSpan TimeStamp { set; get; }
        
        public RenderContext(IRenderHost canvas, Effect effect, DeviceContext renderContext)
        {
            this.Canvas = canvas;
            this.IsShadowPass = false;
            this.IsDeferredPass = false;

            this.mView = effect.GetVariableByName("mView").AsMatrix();
            this.mProjection = effect.GetVariableByName("mProjection").AsMatrix();
            this.mVP = effect.GetVariableByName("mVP").AsMatrix();
            this.vViewport = effect.GetVariableByName("vViewport").AsVector();
            this.vFrustum = effect.GetVariableByName("vFrustum").AsVector();
            this.vEyePos = effect.GetVariableByName("vEyePos").AsVector();
           
            DeviceContext = renderContext;     
        }

        public Matrix GetScreenViewProjectionMatrix()
        {
            return screenViewProjectionMatrix;
        }
        public Matrix GetViewProjectionMatrix()
        {
            return viewProjectionMatrix;
        }

        private void UpdateMatrices()
        {
            if (matrixChanged)
            {
                viewProjectionMatrix = viewMatrix * projectionMatrix;
                screenViewProjectionMatrix = viewProjectionMatrix * ViewportMatrix;
                matrixChanged = false;
            }
        }

        ~RenderContext()
        {            
            this.Dispose();
        }

        public void Dispose()
        {
            Disposer.RemoveAndDispose(ref this.mView);
            Disposer.RemoveAndDispose(ref this.mProjection);
            Disposer.RemoveAndDispose(ref this.vViewport);
            Disposer.RemoveAndDispose(ref this.vFrustum);
            Disposer.RemoveAndDispose(ref this.vEyePos);                        
        }
    }
}