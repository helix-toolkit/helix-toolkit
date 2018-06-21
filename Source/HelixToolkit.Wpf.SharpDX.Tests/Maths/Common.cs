using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Mathematics;

using V2H = System.Numerics.Vector2;
using V3H = System.Numerics.Vector3;
using V4H = System.Numerics.Vector4;

using V2S = SharpDX.Vector2;
using V3S = SharpDX.Vector3;
using V4S = SharpDX.Vector4;

namespace HelixToolkit.Maths
{
    public static class Common
    {
        public static bool Equal(V2H vh, V2S vs)
        {
            return Math.Abs(vh.X - vs.X) < 1e-4
                && Math.Abs(vh.Y - vs.Y) < 1e-4;
        }

        public static bool Equal(V3H vh, V3S vs)
        {
            return Math.Abs(vh.X - vs.X) < 1e-4
                && Math.Abs(vh.Y - vs.Y) < 1e-4
                && Math.Abs(vh.Z - vs.Z) < 1e-4;
        }

        public static bool Equal(V4H vh, V4S vs)
        {
            return Math.Abs(vh.X - vs.X) < 1e-4
                && Math.Abs(vh.Y - vs.Y) < 1e-4
                && Math.Abs(vh.Z - vs.Z) < 1e-4
                && Math.Abs(vh.W - vs.W) < 1e-4;
        }
    }
}
