using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf;

/// <summary>
/// A distance constraint.
/// </summary>
public sealed class DistanceConstraint : Constraint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DistanceConstraint"/> class.
    /// </summary>
    /// <param name="A">
    /// The A.
    /// </param>
    /// <param name="B">
    /// The B.
    /// </param>
    public DistanceConstraint(int A, int B)
    {
        this.Index1 = A;
        this.Index2 = B;
    }

    /// <summary>
    /// Gets or sets the index1.
    /// </summary>
    /// <value>The index1.</value>
    public int Index1 { get; set; }

    /// <summary>
    /// Gets or sets the index2.
    /// </summary>
    /// <value>The index2.</value>
    public int Index2 { get; set; }

    /// <summary>
    /// Gets or sets the iterations.
    /// </summary>
    /// <value>The iterations.</value>
    public int Iterations { get; set; }

    /// <summary>
    /// Gets or sets the relaxation factor.
    /// </summary>
    /// <value>The relaxation factor.</value>
    public double RelaxationFactor { get; set; }

    /// <summary>
    /// Gets or sets the restlength.
    /// </summary>
    /// <value>The restlength.</value>
    public double Restlength { get; set; }

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
        if (this.Iterations > iteration)
        {
            Point3D x1 = vs.Positions[this.Index1];
            Point3D x2 = vs.Positions[this.Index2];
            Vector3D delta = x2 - x1;

            double deltalength = delta.Length;
            double diff = deltalength - this.Restlength;

            double div = deltalength * (vs.InverseMass[this.Index1] + vs.InverseMass[this.Index2]);

            if (Math.Abs(div) > 1e-8)
            {
                diff /= div;
                if (vs.InverseMass[this.Index1] != 0)
                {
                    vs.Positions[this.Index1] += delta * diff * vs.InverseMass[this.Index1] * this.RelaxationFactor;
                }

                if (vs.InverseMass[this.Index2] != 0)
                {
                    vs.Positions[this.Index2] -= delta * diff * vs.InverseMass[this.Index2] * this.RelaxationFactor;
                }
            }
        }
    }
}
