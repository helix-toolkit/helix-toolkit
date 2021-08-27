/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace TT.HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities
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

}
