using SharpDX;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public interface IRandomVector : IRandomSeed
{
    /// <summary>
    /// Gets the random vector3.
    /// </summary>
    /// <value>
    /// The random vector3.
    /// </value>
    Vector3 RandomVector3
    {
        get;
    }
}
