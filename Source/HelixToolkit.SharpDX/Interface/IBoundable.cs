using SharpDX;

namespace HelixToolkit.SharpDX;

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
    bool HasBound
    {
        get;
    }
    /// <summary>
    /// Gets the original bound from the geometry. Same as <see cref="Geometry3D.Bound"/>
    /// </summary>
    /// <value>
    /// The original bound.
    /// </value>
    BoundingBox OriginalBounds
    {
        get;
    }
    /// <summary>
    /// Gets the original bound sphere from the geometry. Same as <see cref="Geometry3D.BoundingSphere"/> 
    /// </summary>
    /// <value>
    /// The original bound sphere.
    /// </value>
    BoundingSphere OriginalBoundsSphere
    {
        get;
    }
    /// <summary>
    /// Gets the bounds. Usually same as <see cref="OriginalBounds"/>. If have instances, the bound will enclose all instances.
    /// </summary>
    /// <value>
    /// The bounds.
    /// </value>
    BoundingBox Bounds
    {
        get;
    }
    /// <summary>
    /// Gets the bounds with transform. Usually same as <see cref="Bounds"/>. If have transform, the bound is the transformed <see cref="Bounds"/>
    /// </summary>
    /// <value>
    /// The bounds with transform.
    /// </value>
    BoundingBox BoundsWithTransform
    {
        get;
    }
    /// <summary>
    /// Gets or sets the bounds sphere. Usually same as <see cref="OriginalBoundsSphere"/>. If have instances, the bound sphere will enclose all instances.
    /// </summary>
    /// <value>
    /// The bounds sphere.
    /// </value>
    BoundingSphere BoundsSphere
    {
        get;
    }
    /// <summary>
    /// Gets or sets the bounds sphere with transform. If have transform, the bound is the transformed <see cref="BoundsSphere"/>
    /// </summary>
    /// <value>
    /// The bounds sphere with transform.
    /// </value>
    BoundingSphere BoundsSphereWithTransform
    {
        get;
    }
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
