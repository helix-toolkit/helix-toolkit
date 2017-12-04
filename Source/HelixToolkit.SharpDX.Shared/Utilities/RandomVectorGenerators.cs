using SharpDX;
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Utility
#else
namespace HelixToolkit.Wpf.SharpDX.Utility
#endif
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

        private readonly Random random = new Random(DateTime.Now.Millisecond);
    }
}
