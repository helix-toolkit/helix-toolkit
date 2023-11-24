using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf;

/// <summary>
/// A sphere constraint.
/// </summary>
public sealed class SphereConstraint : Constraint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SphereConstraint"/> class.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="center">
    /// The center.
    /// </param>
    /// <param name="radius">
    /// The radius.
    /// </param>
    public SphereConstraint(int index, Point3D center, double radius)
    {
        this.Index = index;
        this.Center = center;
        this.Radius = radius;
        this.RadiusSquared = radius * radius;
    }

    /// <summary>
    /// Gets or sets the center.
    /// </summary>
    /// <value>The center.</value>
    public Point3D Center { get; set; }

    /// <summary>
    /// Gets or sets the index.
    /// </summary>
    /// <value>The index.</value>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the radius.
    /// </summary>
    /// <value>The radius.</value>
    public double Radius { get; set; }

    /// <summary>
    /// Gets or sets the radius squared.
    /// </summary>
    /// <value>The radius squared.</value>
    public double RadiusSquared { get; set; }

    /// <summary>
    /// Satisfies the constraint.
    /// </summary>
    /// <param name="vs">
    /// The verlet system.
    /// </param>
    /// <param name="iteration">
    /// The iteration.
    /// </param>
    public override void Satisfy(VerletIntegrator vs, int iteration)
    {
        Vector3D vec = Point3D.Subtract(vs.Positions[this.Index], this.Center);
        if (vec.LengthSquared < this.RadiusSquared)
        {
            vec.Normalize();
            vs.Positions[this.Index] = this.Center + vec * this.Radius * 1.1;
            vs.Positions0[this.Index] = vs.Positions[this.Index];
        }
    }
}
