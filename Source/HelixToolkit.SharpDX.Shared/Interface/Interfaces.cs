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
        bool HitTest(IRenderContext context, Ray ray, ref List<HitTestResult> hits, IRenderable originalSource);

        /// <summary>
        /// Indicates, if this element should be hit-tested.        
        /// default is true
        /// </summary>
        bool IsHitTestVisible { get; set; }
    }

    public interface IBoundable
    {
        BoundingBox Bounds { get; }
        BoundingBox BoundsWithTransform { get; }
        BoundingSphere BoundsSphere { get; }
        BoundingSphere BoundsSphereWithTransform { get; }
    }

    public interface ITreeNode : IGUID, ITransform
    {
        bool Visible { set; get; }
        /// <summary>
        /// Call update to update <see cref="TotalTransform"/>, <see cref="IsVisible"/>, etc
        /// </summary>
        /// <param name="context"></param>
        void Update(IRenderContext context);
        /// <summary>
        /// Actual visibility after checking different conditions after calling <see cref="Update(IRenderContext)"/>
        /// </summary>
        bool IsVisible { get; }
        /// <summary>
        /// Optional for sub items
        /// </summary>
        IEnumerable<ITreeNode> Items { get; }
    }
}
