using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf;

/// <summary>
/// A floor constraint.
/// </summary>
public sealed class FloorConstraint : Constraint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FloorConstraint"/> class.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="friction">
    /// The friction.
    /// </param>
    public FloorConstraint(int index, double friction = 1.0)
    {
        this.Index = index;
        this.Friction = friction;
    }

    /// <summary>
    /// Gets or sets the friction.
    /// </summary>
    /// <value>The friction.</value>
    public double Friction { get; set; }

    /// <summary>
    /// Gets or sets the index.
    /// </summary>
    /// <value>The index.</value>
    public int Index { get; set; }

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
        int i = this.Index;
        Point3D x = vs.Positions[i];
        if (x.Z <= 0)
        {
            if (this.Friction != 0)
            {
                double f = -x.Z * this.Friction;
                Vector3D v = vs.Positions[i] - vs.Positions0[i];
                v.Z = 0;

                if (v.X > 0)
                {
                    v.X -= f * v.X;
                    if (v.X < 0)
                    {
                        v.X = 0;
                    }
                }
                else
                {
                    v.X += f;
                    if (v.X > 0)
                    {
                        v.X = 0;
                    }
                }

                if (v.Y > 0)
                {
                    v.Y -= f * v.Y;
                    if (v.Y < 0)
                    {
                        v.Y = 0;
                    }
                }
                else
                {
                    v.Y += f;
                    if (v.Y > 0)
                    {
                        v.Y = 0;
                    }
                }

                vs.Positions0[i] = vs.Positions[i] - v;
            }

            vs.Positions[i].Z = 0;
        }
    }

}
