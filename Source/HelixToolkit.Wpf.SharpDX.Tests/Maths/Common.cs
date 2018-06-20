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
        public static bool Equal(ref V2H vh, ref V2S vs)
        {
            return MathUtil.NearEqual(vh.X, vs.X)
                && MathUtil.NearEqual(vh.Y, vs.Y);
        }

        public static bool Equal(ref V3H vh, ref V3S vs)
        {
            return MathUtil.NearEqual(vh.X, vs.X)
                && MathUtil.NearEqual(vh.Y, vs.Y)
                && MathUtil.NearEqual(vh.Z, vs.Z);
        }

        public static bool Equal(ref V4H vh, ref V4S vs)
        {
            return MathUtil.NearEqual(vh.X, vs.X)
                && MathUtil.NearEqual(vh.Y, vs.Y)
                && MathUtil.NearEqual(vh.Z, vs.Z)
                && MathUtil.NearEqual(vh.W, vs.W);
        }
    }
}
