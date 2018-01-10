/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using BoundingSphere = global::SharpDX.BoundingSphere;

    public interface IGUID
    {
        Guid GUID { get; }
    }

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

    public interface IResourceSharing : IDisposable
    {
        /// <summary>
        /// Get reference count
        /// </summary>
        int ReferenceCount { get; }

        /// <summary>
        /// Add reference counter;
        /// </summary>
        /// <returns>Current count</returns>
        int AddReference();
    }

    public interface IHitable
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="context">Used to get view/projection matrices during hit test. <para>Only needs for screen space model hit test(line/point/billboard). Can be set to null for mesh geometry hit test.</para></param>
        /// <param name="ray"></param>
        /// <param name="hits"></param>
        /// <returns>Return all hitted details with distance from nearest to farest.</returns>
        bool HitTest(IRenderContext context, Ray ray, ref List<HitTestResult> hits);

        /// <summary>
        /// Indicates, if this element should be hit-tested.        
        /// default is true
        /// </summary>
        bool IsHitTestVisible { get; set; }
    }

    public sealed class BoundChangeArgs<T> : EventArgs where T : struct
    {
        public T NewBound { private set; get; }
        public T OldBound { private set; get; }
        public BoundChangeArgs(ref T newBound, ref T oldBound)
        {
            NewBound = newBound;
            OldBound = oldBound;
        }
    }

    public interface IBoundable
    {
        BoundingBox Bounds { get; }
        BoundingBox BoundsWithTransform { get; }
        BoundingSphere BoundsSphere { get; }
        BoundingSphere BoundsSphereWithTransform { get; }

        event EventHandler<BoundChangeArgs<BoundingBox>> OnBoundChanged;

        event EventHandler<BoundChangeArgs<BoundingBox>> OnTransformBoundChanged;

        event EventHandler<BoundChangeArgs<BoundingSphere>> OnBoundSphereChanged;

        event EventHandler<BoundChangeArgs<BoundingSphere>> OnTransformBoundSphereChanged;
    }

    public interface IInstancing
    {
        IList<Matrix> Instances { set; get; }
    }
}
