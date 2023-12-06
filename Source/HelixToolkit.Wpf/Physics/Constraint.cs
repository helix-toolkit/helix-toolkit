namespace HelixToolkit.Wpf;

/// <summary>
/// Constraint base class.
/// </summary>
public abstract class Constraint
{
    /// <summary>
    /// Satisfies the constraint.
    /// </summary>
    /// <param name="vs">
    /// The verlet system.
    /// </param>
    /// <param name="iteration">
    /// The iteration.
    /// </param>
    public abstract void Satisfy(VerletIntegrator vs, int iteration);
}
