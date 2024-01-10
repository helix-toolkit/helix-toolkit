using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf;

/// <summary>
/// Extension methods for <see cref="Point3D"/>.
/// </summary>
public static class Point3DExtensions
{
    /// <summary>
    /// Calculates the distance from p1 to p2.
    /// </summary>
    /// <param name="p1">
    /// The point p1.
    /// </param>
    /// <param name="p2">
    /// The point p2.
    /// </param>
    /// <returns>
    /// The distance.
    /// </returns>
    public static double DistanceTo(this Point3D p1, Point3D p2)
    {
        return (p2 - p1).Length;
    }

    /// <summary>
    /// Calculates the squared distance from p1 to p2.
    /// </summary>
    /// <param name="p1">
    /// The p1.
    /// </param>
    /// <param name="p2">
    /// The p2.
    /// </param>
    /// <returns>
    /// The squared distance.
    /// </returns>
    public static double DistanceToSquared(this Point3D p1, Point3D p2)
    {
        return (p2 - p1).LengthSquared;
    }

    /// <summary>
    /// Multiplies the specified point with a scalar.
    /// </summary>
    /// <param name="p">
    /// The point.
    /// </param>
    /// <param name="d">
    /// The scalar.
    /// </param>
    /// <returns>
    /// A point.
    /// </returns>
    public static Point3D Multiply(this Point3D p, double d)
    {
        return new Point3D(p.X * d, p.Y * d, p.Z * d);
    }

    /// <summary>
    /// Sums the specified points.
    /// </summary>
    /// <param name="points">
    /// The points.
    /// </param>
    /// <returns>
    /// The summed point.
    /// </returns>
    public static Point3D Sum(params Point3D[] points)
    {
        double x = 0;
        double y = 0;
        double z = 0;
        foreach (var p in points)
        {
            x += p.X;
            y += p.Y;
            z += p.Z;
        }

        return new Point3D(x, y, z);
    }
}
