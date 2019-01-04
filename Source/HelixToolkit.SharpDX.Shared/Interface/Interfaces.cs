/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using BoundingSphere = global::SharpDX.BoundingSphere;
    /// <summary>
    /// 
    /// </summary>
    public interface IGUID
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        Guid GUID { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IAttachable
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsAttached { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        void Attach(IRenderHost host);
        /// <summary>
        /// 
        /// </summary>
        void Detach();
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IResourceSharing : IDisposable
    {
        /// <summary>
        /// Attaches the specified model unique identifier.
        /// </summary>
        /// <param name="modelGuid">The model unique identifier.</param>
        void Attach(Guid modelGuid);

        /// <summary>
        /// Detaches the specified model unique identifier.
        /// </summary>
        /// <param name="modelGuid">The model unique identifier.</param>
        void Detach(Guid modelGuid);
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IHitable
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="context">Used to get view/projection matrices during hit test. <para>Only needs for screen space model hit test(line/point/billboard). Can be set to null for mesh geometry hit test.</para></param>
        /// <param name="ray"></param>
        /// <param name="hits"></param>
        /// <returns>Return all hitted details with distance from nearest to farest.</returns>
        bool HitTest(RenderContext context, Ray ray, ref List<HitTestResult> hits);

        /// <summary>
        /// Indicates, if this element should be hit-tested.        
        /// default is true
        /// </summary>
        bool IsHitTestVisible { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class BoundChangeArgs<T> : EventArgs where T : struct
    {
        /// <summary>
        /// Gets or sets the new bound.
        /// </summary>
        /// <value>
        /// The new bound.
        /// </value>
        public T NewBound { private set; get; }
        /// <summary>
        /// Gets or sets the old bound.
        /// </summary>
        /// <value>
        /// The old bound.
        /// </value>
        public T OldBound { private set; get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="BoundChangeArgs{T}"/> class.
        /// </summary>
        /// <param name="newBound">The new bound.</param>
        /// <param name="oldBound">The old bound.</param>
        public BoundChangeArgs(ref T newBound, ref T oldBound)
        {
            NewBound = newBound;
            OldBound = oldBound;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IBoundable
    {
        /// <summary>
        /// Gets or sets a value indicating whether [bound enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [bound enabled]; otherwise, <c>false</c>.
        /// </value>
        bool HasBound { get; }
        /// <summary>
        /// Gets the original bound from the geometry. Same as <see cref="Geometry3D.Bound"/>
        /// </summary>
        /// <value>
        /// The original bound.
        /// </value>
        BoundingBox OriginalBounds { get; }
        /// <summary>
        /// Gets the original bound sphere from the geometry. Same as <see cref="Geometry3D.BoundingSphere"/> 
        /// </summary>
        /// <value>
        /// The original bound sphere.
        /// </value>
        BoundingSphere OriginalBoundsSphere { get; }
        /// <summary>
        /// Gets the bounds. Usually same as <see cref="OriginalBounds"/>. If have instances, the bound will enclose all instances.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        BoundingBox Bounds { get; }
        /// <summary>
        /// Gets the bounds with transform. Usually same as <see cref="Bounds"/>. If have transform, the bound is the transformed <see cref="Bounds"/>
        /// </summary>
        /// <value>
        /// The bounds with transform.
        /// </value>
        BoundingBox BoundsWithTransform { get; }
        /// <summary>
        /// Gets or sets the bounds sphere. Usually same as <see cref="OriginalBoundsSphere"/>. If have instances, the bound sphere will enclose all instances.
        /// </summary>
        /// <value>
        /// The bounds sphere.
        /// </value>
        BoundingSphere BoundsSphere { get; }
        /// <summary>
        /// Gets or sets the bounds sphere with transform. If have transform, the bound is the transformed <see cref="BoundsSphere"/>
        /// </summary>
        /// <value>
        /// The bounds sphere with transform.
        /// </value>
        BoundingSphere BoundsSphereWithTransform { get; }
        /// <summary>
        /// Occurs when [on bound changed].
        /// </summary>
        event EventHandler<BoundChangeArgs<BoundingBox>> BoundChanged;
        /// <summary>
        /// Occurs when [on transform bound changed].
        /// </summary>
        event EventHandler<BoundChangeArgs<BoundingBox>> TransformBoundChanged;
        /// <summary>
        /// Occurs when [on bound sphere changed].
        /// </summary>
        event EventHandler<BoundChangeArgs<BoundingSphere>> BoundSphereChanged;
        /// <summary>
        /// Occurs when [on transform bound sphere changed].
        /// </summary>
        event EventHandler<BoundChangeArgs<BoundingSphere>> TransformBoundSphereChanged;
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IInstancing
    {
        /// <summary>
        /// Gets the instance buffer.
        /// </summary>
        /// <value>
        /// The instance buffer.
        /// </value>
        IElementsBufferModel<Matrix> InstanceBuffer { get; }
    }
}
