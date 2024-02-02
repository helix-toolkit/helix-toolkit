using System.Windows.Media.Media3D;
using System.Windows;
using HelixToolkit.Geometry;

namespace HelixToolkit.Wpf;

/// <summary>
/// A visual element that displays a quadrilateral polygon.
/// </summary>
/// <remarks>
/// See http://en.wikipedia.org/wiki/Quadrilateral
/// </remarks>
public class QuadVisual3D : MeshElement3D
{
    // A quadrilateral defined by the four corner points.
    // Point4          Point3
    // +---------------+
    // |               |
    // |               |
    // +---------------+
    // Point1          Point2

    // The texture coordinates are
    // (0,0)           (1,0)
    // +---------------+
    // |               |
    // |               |
    // +---------------+
    // (0,1)          (1,1)

    /// <summary>
    /// Identifies the <see cref="Point1"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point1Property = DependencyProperty.Register(
        "Point1",
        typeof(Point3D),
        typeof(QuadVisual3D),
        new UIPropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Point2"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point2Property = DependencyProperty.Register(
        "Point2",
        typeof(Point3D),
        typeof(QuadVisual3D),
        new UIPropertyMetadata(new Point3D(1, 0, 0), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Point3"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point3Property = DependencyProperty.Register(
        "Point3",
        typeof(Point3D),
        typeof(QuadVisual3D),
        new UIPropertyMetadata(new Point3D(1, 1, 0), GeometryChanged));

    /// <summary>
    /// Identifies the <see cref="Point4"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Point4Property = DependencyProperty.Register(
        "Point4",
        typeof(Point3D),
        typeof(QuadVisual3D),
        new UIPropertyMetadata(new Point3D(0, 1, 0), GeometryChanged));

    /// <summary>
    /// Gets or sets the first point.
    /// </summary>
    /// <value>The point1.</value>
    public Point3D Point1
    {
        get
        {
            return (Point3D)this.GetValue(Point1Property);
        }

        set
        {
            this.SetValue(Point1Property, value);
        }
    }

    /// <summary>
    /// Gets or sets the second point.
    /// </summary>
    /// <value>The point2.</value>
    public Point3D Point2
    {
        get
        {
            return (Point3D)this.GetValue(Point2Property);
        }

        set
        {
            this.SetValue(Point2Property, value);
        }
    }

    /// <summary>
    /// Gets or sets the third point.
    /// </summary>
    /// <value>The point3.</value>
    public Point3D Point3
    {
        get
        {
            return (Point3D)this.GetValue(Point3Property);
        }

        set
        {
            this.SetValue(Point3Property, value);
        }
    }

    /// <summary>
    /// Gets or sets the fourth point.
    /// </summary>
    /// <value>The point4.</value>
    public Point3D Point4
    {
        get
        {
            return (Point3D)this.GetValue(Point4Property);
        }

        set
        {
            this.SetValue(Point4Property, value);
        }
    }

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
    /// </summary>
    /// <returns>A triangular mesh geometry.</returns>
    protected override MeshGeometry3D? Tessellate()
    {
        var builder = new MeshBuilder(false, true);
        builder.AddQuad(
            this.Point1.ToVector3(),
            this.Point2.ToVector3(),
            this.Point3.ToVector3(),
            this.Point4.ToVector3(),
            new System.Numerics.Vector2(0, 1),
            new System.Numerics.Vector2(1, 1),
            new System.Numerics.Vector2(1, 0),
            new System.Numerics.Vector2(0, 0));
        return builder.ToMesh().ToWndMeshGeometry3D();
    }
}
