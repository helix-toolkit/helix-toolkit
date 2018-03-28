// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderContext.cs" company="Helix Toolkit">
//   Copyright (c) 2018 Helix Toolkit contributors
// </copyright>
// <summary>
//   The render-context is currently generated per frame
//   Optimizations might be possible
// </summary>
// --------------------------------------------------------------------------------------------------------------------
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using System;

    using global::SharpDX;

    using global::SharpDX.Direct3D11;
    using Utilities;
    using Shaders;
    using Model;
    using Cameras;

    /// <summary>
    /// The render-context is currently generated per frame
    /// Optimizations might be possible
    /// </summary>
    public class RenderContext : DisposeObject, IRenderContext
    {       
        private Matrix worldMatrix = Matrix.Identity;
        private Matrix viewMatrix;
        private Matrix projectionMatrix;
        /// <summary>
        /// Gets or sets the bounding frustum.
        /// </summary>
        /// <value>
        /// The bounding frustum.
        /// </value>
        public BoundingFrustum BoundingFrustum { set; get; }
        private CameraCore camera; 

        private bool matrixChanged = true;
        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        /// <value>
        /// The view matrix.
        /// </value>
        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
            private set
            {
                if (viewMatrix == value) { return; }
                viewMatrix = value;
                matrixChanged = true;
            }
        }
        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        /// <value>
        /// The projection matrix.
        /// </value>
        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
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
        /// <summary>
        /// Gets or sets the world matrix.
        /// </summary>
        /// <value>
        /// The world matrix.
        /// </value>
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
        /// <summary>
        /// Gets the viewport matrix.
        /// </summary>
        /// <value>
        /// The viewport matrix.
        /// </value>
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
        /// <summary>
        /// Gets the screen view projection matrix.
        /// </summary>
        /// <value>
        /// The screen view projection matrix.
        /// </value>
        public Matrix ScreenViewProjectionMatrix
        {
            get
            {
                return GetScreenViewProjectionMatrix();
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable bounding frustum].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable bounding frustum]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableBoundingFrustum { set; get; } = false;
        /// <summary>
        /// Gets or sets the device context.
        /// </summary>
        /// <value>
        /// The device context.
        /// </value>
        public DeviceContext DeviceContext { private set; get; }
        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        public double ActualWidth { get { return RenderHost.ActualWidth; } }
        /// <summary>
        /// Gets the actual height.
        /// </summary>
        /// <value>
        /// The actual height.
        /// </value>
        public double ActualHeight { get { return RenderHost.ActualHeight; } }
        /// <summary>
        /// Gets or sets the camera.
        /// </summary>
        /// <value>
        /// The camera.
        /// </value>
        public CameraCore Camera
        {
            get { return this.camera; }
            set
            {                
                this.camera = value;
                ViewMatrix = this.camera.CreateViewMatrix();
                var aspectRatio = this.ActualWidth / this.ActualHeight;
                ProjectionMatrix = this.camera.CreateProjectionMatrix((float)aspectRatio);
                if (this.camera is ProjectionCameraCore c)
                {
                    // viewport: W,H,0,0   
                    globalTransform.Viewport = new Vector4((float)ActualWidth, (float)ActualHeight, 0, 0);
                    var ar = globalTransform.Viewport.X / globalTransform.Viewport.Y;
                    
                    var  pc = c as PerspectiveCameraCore;
                    var fov = (pc != null) ? pc.FieldOfView : 90f;

                    var zn = c.NearPlaneDistance > 0 ? c.NearPlaneDistance : 0.1;
                    var zf = c.FarPlaneDistance + 0.0;
                    // frustum: FOV,AR,N,F
                    globalTransform.Frustum = new Vector4((float)fov, (float)ar, (float)zn, (float)zf);
                    if(EnableBoundingFrustum)
                        BoundingFrustum = new BoundingFrustum(ViewMatrix * ProjectionMatrix);
                    globalTransform.EyePos = this.camera.Position;
                }
            }
        }

        /// <summary>
        /// Gets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        public IRenderHost RenderHost { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether is shadow pass.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is shadow pass; otherwise, <c>false</c>.
        /// </value>
        public bool IsShadowPass { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is deferred pass.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deferred pass; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeferredPass { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is custom pass.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is custom pass; otherwise, <c>false</c>.
        /// </value>
        public bool IsCustomPass { set; get; } = false;
        /// <summary>
        /// Gets or sets the name of the custom pass.
        /// </summary>
        /// <value>
        /// The name of the custom pass.
        /// </value>
        public string CustomPassName { set; get; } = "";
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        public TimeSpan TimeStamp { set; get; }
        /// <summary>
        /// Gets or sets the light scene.
        /// </summary>
        /// <value>
        /// The light scene.
        /// </value>
        public Light3DSceneShared LightScene { private set; get; }

        private IConstantBufferProxy cbuffer;
        
        private GlobalTransformStruct globalTransform;
        /// <summary>
        /// Gets the global transform.
        /// </summary>
        /// <value>
        /// The global transform.
        /// </value>
        public GlobalTransformStruct GlobalTransform { get { return globalTransform; } }
        /// <summary>
        /// Gets or sets the shared resource.
        /// </summary>
        /// <value>
        /// The shared resource.
        /// </value>
        public IContextSharedResource SharedResource
        {
            private set;get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [update octree] automatically.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [update octree]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoUpdateOctree { set; get; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContext"/> class.
        /// </summary>
        /// <param name="renderHost">The render host.</param>
        /// <param name="renderContext">The render context.</param>
        public RenderContext(IRenderHost renderHost, DeviceContext renderContext)
        {
            this.RenderHost = renderHost;
            this.IsShadowPass = false;
            this.IsDeferredPass = false;
            cbuffer = renderHost.EffectsManager.ConstantBufferPool.Register(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes);
            DeviceContext = renderContext;
            LightScene = Collect(new Light3DSceneShared(renderHost.EffectsManager.ConstantBufferPool));
            SharedResource = Collect(new ContextSharedResource());
        }
        /// <summary>
        /// Gets the screen view projection matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix GetScreenViewProjectionMatrix()
        {
            return screenViewProjectionMatrix;
        }
        /// <summary>
        /// Call to update constant buffer for per frame
        /// </summary>
        public void UpdatePerFrameData()
        {
            if (matrixChanged)
            {
                globalTransform.View = ViewMatrix;
                globalTransform.Projection = ProjectionMatrix;
                globalTransform.ViewProjection = globalTransform.View * globalTransform.Projection;
                screenViewProjectionMatrix = ViewMatrix * ProjectionMatrix * ViewportMatrix;                        
                matrixChanged = false;
            }
            cbuffer.UploadDataToBuffer(DeviceContext, ref globalTransform);
            LightScene.UploadToBuffer(DeviceContext);
        }
    }
}