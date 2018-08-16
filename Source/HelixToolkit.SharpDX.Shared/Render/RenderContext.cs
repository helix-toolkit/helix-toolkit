// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderContext.cs" company="Helix Toolkit">
//   Copyright (c) 2018 Helix Toolkit contributors
// </copyright>
// <summary>
//   The render-context is currently generated per frame
//   Optimizations might be possible
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Matrix = System.Numerics.Matrix4x4;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Cameras;
    using Mathematics;
    using Model;
    using Render;
    using Shaders;
    using Utilities;
    /// <summary>
    /// The render-context is currently generated per frame
    /// Optimizations might be possible
    /// </summary>
    public sealed class RenderContext : DisposeObject
    {
        /// <summary>
        /// Gets or sets the bounding frustum.
        /// </summary>
        /// <value>
        /// The bounding frustum.
        /// </value>
        public BoundingFrustum BoundingFrustum;

        private CameraCore camera;

        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        /// <value>
        /// The view matrix.
        /// </value>
        public Matrix ViewMatrix;

        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        /// <value>
        /// The projection matrix.
        /// </value>
        public Matrix ProjectionMatrix;

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
        public bool EnableBoundingFrustum = false;

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
                    // viewport: W,H,1/W,1/H
                    globalTransform.Viewport = new Vector4((float)ActualWidth, (float)ActualHeight, 1f/(float)ActualWidth, 1f/(float)ActualHeight);
                    var ar = globalTransform.Viewport.X / globalTransform.Viewport.Y;

                    var fov = (c is PerspectiveCameraCore pc) ? pc.FieldOfView : 90f;

                    var zn = c.NearPlaneDistance > 0 ? c.NearPlaneDistance : 0.1;
                    var zf = c.FarPlaneDistance + 0.0;
                    // frustum: FOV,AR,N,F
                    globalTransform.Frustum = new Vector4((float)fov, (float)ar, (float)zn, (float)zf);
                    if (EnableBoundingFrustum)
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
        /// Gets or sets a value indicating whether is deferred pass.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deferred pass; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeferredPass;

        /// <summary>
        /// Gets or sets a value indicating whether is order independent transparent pass.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is oit pass; otherwise, <c>false</c>.
        /// </value>
        public bool IsOITPass = false;

        /// <summary>
        /// Gets or sets the name of the custom pass.
        /// </summary>
        /// <value>
        /// The name of the custom pass.
        /// </value>
        public string CustomPassName = "";

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        public TimeSpan TimeStamp;

        /// <summary>
        /// Gets or sets the light scene.
        /// </summary>
        /// <value>
        /// The light scene.
        /// </value>
        public readonly Light3DSceneShared LightScene;

        private readonly ConstantBufferProxy cbuffer;

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
        public readonly ContextSharedResource SharedResource;

        /// <summary>
        /// Gets or sets a value indicating whether [update octree] automatically.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [update octree]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoUpdateOctree = true;

        /// <summary>
        /// Gets or sets a value indicating whether this render pass is using inverted cull mode.
        /// If <see cref="global::SharpDX.Direct3D11.CullMode"/>=<see cref="global::SharpDX.Direct3D11.CullMode.None"/>, default state is used.
        /// This is usually used when rendering <see cref="Core.DynamicCubeMapCore"/>.
        /// </summary>
        /// <value>
        ///   <c>true</c> Set invert cullmode flag; otherwise, <c>false</c>.
        /// </value>
        public bool IsInvertCullMode = false;

        /// <summary>
        /// Gets or sets the oit weight power used for color weight calculation. Default = 3;
        /// </summary>
        /// <value>
        /// The oit weight power.
        /// </value>
        public float OITWeightPower
        {
            set
            {
                globalTransform.OITWeightPower = value;
            }
            get
            {
                return globalTransform.OITWeightPower;
            }
        }

        /// <summary>
        /// Gets or sets the oit weight depth slope. Used to increase resolution for particular range of depth values. 
        /// <para>If value = 2, the depth range from 0-0.5 expands to 0-1 to increase resolution. However, values from 0.5 - 1 will be pushed to 1</para>
        /// </summary>
        /// <value>
        /// The oit weight depth slope.
        /// </value>
        public float OITWeightDepthSlope
        {
            set
            {
                globalTransform.OITWeightDepthSlope = value;
            }
            get
            {
                return globalTransform.OITWeightDepthSlope;
            }
        }
        /// <summary>
        /// Gets or sets the oit weight mode.
        /// <para>Please refer to http://jcgt.org/published/0002/02/09/ </para>
        /// <para>Linear0: eq7; Linear1: eq8; Linear2: eq9; NonLinear: eq10</para>
        /// </summary>
        /// <value>
        /// The oit weight mode.
        /// </value>
        public OITWeightMode OITWeightMode
        {
            set
            {
                globalTransform.OITWeightMode = (int)value;
            }
            get
            {
                return (OITWeightMode)globalTransform.OITWeightMode;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is shadow map enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is shadow map enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsShadowMapEnabled => RenderHost.IsShadowMapEnabled;
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContext"/> class.
        /// </summary>
        /// <param name="renderHost">The render host.</param>
        public RenderContext(IRenderHost renderHost)
        {
            this.RenderHost = renderHost;
            this.IsDeferredPass = false;
            cbuffer = renderHost.EffectsManager.ConstantBufferPool.Register(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes);
            LightScene = Collect(new Light3DSceneShared(renderHost.EffectsManager.ConstantBufferPool));
            SharedResource = Collect(new ContextSharedResource());
            OITWeightPower = 3;
            OITWeightDepthSlope = 1;
            OITWeightMode = OITWeightMode.Linear2;
        }

        /// <summary>
        /// Gets the screen view projection matrix.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix GetScreenViewProjectionMatrix()
        {
            return screenViewProjectionMatrix;
        }

        /// <summary>
        /// Call to update constant buffer for per frame
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePerFrameData(DeviceContextProxy deviceContext)
        {
            UpdatePerFrameData(true, true, deviceContext);
        }

        /// <summary>
        /// Call to update constant buffer for per frame
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePerFrameData(bool updateGlobalTransform, bool updateLights, DeviceContextProxy deviceContext)
        {
            if (updateGlobalTransform)
            {
                globalTransform.View = ViewMatrix;
                globalTransform.Projection = ProjectionMatrix;
                globalTransform.ViewProjection = ViewMatrix * ProjectionMatrix;
                screenViewProjectionMatrix = ViewMatrix * ProjectionMatrix * ViewportMatrix;
                cbuffer.UploadDataToBuffer(deviceContext, ref globalTransform);
            }
            if (updateLights)
            {
                LightScene.UploadToBuffer(deviceContext);
            }
        }
    }
}