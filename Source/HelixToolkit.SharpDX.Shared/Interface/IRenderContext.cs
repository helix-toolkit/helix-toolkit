/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Cameras;
    using Model;
    /// <summary>
    /// Contains all the global transformation matrices, camera info, light info for rendering
    /// </summary>
    public interface IRenderContext
    {
        /// <summary>
        /// Gets or sets the camera.
        /// </summary>
        /// <value>
        /// The camera.
        /// </value>
        CameraCore Camera { set; get; }
        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        /// <value>
        /// The view matrix.
        /// </value>
        Matrix ViewMatrix { get; }
        /// <summary>
        /// Gets the projection matrix.
        /// </summary>
        /// <value>
        /// The projection matrix.
        /// </value>
        Matrix ProjectionMatrix { get; }
        /// <summary>
        /// Gets or sets the world matrix.
        /// </summary>
        /// <value>
        /// The world matrix.
        /// </value>
        Matrix WorldMatrix { set; get; }
        /// <summary>
        /// Gets the viewport matrix.
        /// </summary>
        /// <value>
        /// The viewport matrix.
        /// </value>
        Matrix ViewportMatrix { get; }
        /// <summary>
        /// Gets the screen view projection matrix.
        /// </summary>
        /// <value>
        /// The screen view projection matrix.
        /// </value>
        Matrix ScreenViewProjectionMatrix { get; }
        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        double ActualWidth { get; }
        /// <summary>
        /// Gets the actual height.
        /// </summary>
        /// <value>
        /// The actual height.
        /// </value>
        double ActualHeight { get; }
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        TimeSpan TimeStamp { get; }
        /// <summary>
        /// Gets the light scene.
        /// </summary>
        /// <value>
        /// The light scene.
        /// </value>
        Light3DSceneShared LightScene { get; }
        /// <summary>
        /// Gets the global transform.
        /// </summary>
        /// <value>
        /// The global transform.
        /// </value>
        GlobalTransformStruct GlobalTransform { get; }
        /// <summary>
        /// Updates the per frame data.
        /// </summary>
        void UpdatePerFrameData();
        /// <summary>
        /// Gets or sets a value indicating whether this instance is shadow pass.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is shadow pass; otherwise, <c>false</c>.
        /// </value>
        bool IsShadowPass { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [enable bounding frustum].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable bounding frustum]; otherwise, <c>false</c>.
        /// </value>
        bool EnableBoundingFrustum { get; }
        /// <summary>
        /// Gets or sets the bounding frustum.
        /// </summary>
        /// <value>
        /// The bounding frustum.
        /// </value>
        BoundingFrustum BoundingFrustum { set; get; }
        /// <summary>
        /// Gets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        IRenderHost RenderHost { get; }
        /// <summary>
        /// Gets the shared resource.
        /// </summary>
        /// <value>
        /// The shared resource.
        /// </value>
        IContextSharedResource SharedResource { get; }
        /// <summary>
        /// Gets or sets the name of the custom shader pass. This is used to do special effects only
        /// </summary>
        /// <value>
        /// The name of the shader pass.
        /// </value>
        string CustomPassName { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this render pass is using custom pass specified by <see cref="CustomPassName"/>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is custom pass; otherwise, <c>false</c>.
        /// </value>
        bool IsCustomPass { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [update octree].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [update octree]; otherwise, <c>false</c>.
        /// </value>
        bool AutoUpdateOctree { get; }
    }
    /// <summary>
    /// Contains the shared resources across all renderables. 
    /// </summary>
    public interface IContextSharedResource : IDisposable
    {
        /// <summary>
        /// Gets or sets the shadow texture view.
        /// </summary>
        /// <value>
        /// The shadow view.
        /// </value>
        ShaderResourceView ShadowView { set; get; }
    }
}
