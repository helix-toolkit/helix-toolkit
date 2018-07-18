/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
using System;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    /// <summary>
    /// Interface for basic octree. Used to implement static octree and dynamic octree
    /// </summary>
    public interface IOctreeBasic
    {
        /// <summary>
        /// Whether the tree has been built.
        /// </summary>
        bool TreeBuilt { get; }

        /// <summary>
        ///
        /// </summary>
        event EventHandler<EventArgs> OnHit;

        /// <summary>
        /// Output the hit path of the tree traverse. Only for debugging
        /// </summary>
        IList<BoundingBox> HitPathBoundingBoxes { get; }

        /// <summary>
        /// Octree parameter
        /// </summary>
        OctreeBuildParameter Parameter { get; }

        /// <summary>
        /// Gets the bound.
        /// </summary>
        /// <value>
        /// The bound.
        /// </value>
        BoundingBox Bound { get; }

        /// <summary>
        /// Build the static octree
        /// </summary>
        void BuildTree();

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="geometry"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        bool HitTest(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits);

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="geometry"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        bool HitTest(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, float hitThickness);

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="point"></param>
        /// <param name="results"></param>
        /// <param name="heuristicSearchFactor"></param>
        /// <returns></returns>
        bool FindNearestPointFromPoint(RenderContext context, ref Vector3 point, ref List<HitTestResult> results, float heuristicSearchFactor = 1f);

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        bool FindNearestPointBySphere(RenderContext context, ref BoundingSphere sphere, ref List<HitTestResult> points);
        /// <summary>
        /// Finds the nearest point by point and search radius.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="point">The point.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        bool FindNearestPointByPointAndSearchRadius(RenderContext context, ref Vector3 point, float radius, ref List<HitTestResult> result);
        /// <summary>
        /// Creates the octree line model for debugging or visualize the octree
        /// </summary>
        /// <returns></returns>
        LineGeometry3D CreateOctreeLineModel();
    }

    /// <summary>
    /// Interface for dynamic octree
    /// </summary>
    public interface IDynamicOctree : IOctreeBasic
    {
        /// <summary>
        /// Gets the self as array.
        /// </summary>
        /// <value>
        /// The self array.
        /// </value>
        IDynamicOctree[] SelfArray { get; }
        /// <summary>
        /// This is a bitmask indicating which child nodes are actively being used.
        /// It adds slightly more complexity, but is faster for performance since there is only one comparison instead of 8.
        /// </summary>
        byte ActiveNodes { set; get; }
        /// <summary>
        /// Has child octants
        /// </summary>
        bool HasChildren { get; }
        /// <summary>
        /// If this node is root node
        /// </summary>
        bool IsRoot { get; }
        /// <summary>
        /// Parent node
        /// </summary>
        IDynamicOctree Parent { get; set; }
        /// <summary>
        /// Child octants
        /// </summary>
        IDynamicOctree[] ChildNodes { get; }
        /// <summary>
        /// Octant bounds
        /// </summary>
        BoundingBox[] Octants { get; }

        /// <summary>
        /// Delete self if is empty;
        /// </summary>
        bool AutoDeleteIfEmpty { set; get; }

        /// <summary>
        /// Returns true if this node tree and all children have no content
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Hit test for only this node, not its child node
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="geometry"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness">Only used for point/line hit test</param>
        /// <param name="rayModel"></param>
        /// <returns></returns>
        bool HitTestCurrentNodeExcludeChild(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel,
            ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness);

        /// <summary>
        /// Search nearest point by a search sphere at this node only
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        bool FindNearestPointBySphereExcludeChild(RenderContext context, ref BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect);

        /// <summary>
        /// Build current node level only, this will only build current node and create children, but not build its children. 
        /// To build from top to bottom, call BuildTree
        /// </summary>
        void BuildCurretNodeOnly();
        /// <summary>
        /// Clear the tree
        /// </summary>
        void Clear();
        /// <summary>
        /// Remove self from parent node
        /// </summary>
        void RemoveSelf();
        /// <summary>
        /// Remove child from ChildNodes
        /// </summary>
        /// <param name="child"></param>
        void RemoveChild(IDynamicOctree child);
    }
}
