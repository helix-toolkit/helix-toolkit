/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Utilities
#else
namespace HelixToolkit.Wpf.SharpDX.Utilities
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRandomSeed
    {
        /// <summary>
        /// Gets the seed.
        /// </summary>
        /// <value>
        /// The seed.
        /// </value>
        uint Seed { get; }
    }
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
        Vector3 RandomVector3 { get; }
    }
}
