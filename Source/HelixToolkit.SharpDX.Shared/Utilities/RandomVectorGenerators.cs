/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities
    {
        public class UniformRandomVectorGenerator : IRandomVector
        {
            public Vector3 MinVector { set; get; } = -Vector3.One;

            public Vector3 MaxVector { set; get; } = Vector3.One;

            public Vector3 RandomVector3
            {
                get
                {
                    return random.NextVector3(MinVector, MaxVector);
                }
            }

            public uint Seed
            {
                get
                {
                    return (uint)Math.Abs(random.Next());
                }
            }

            private readonly Random random = new Random(Environment.TickCount);
        }
    }

}
