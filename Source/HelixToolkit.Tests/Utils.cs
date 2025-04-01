using System.Numerics;

namespace HelixToolkit.Tests
{
    public static class Utils
    {
        public readonly static Random random = new();
        public static float GetRandomFloatinUnit()
        {
#if NET6_0_OR_GREATER
            return (random.NextSingle() - 0.5f) * 2.0f;
#else
            return (float)random.Next(-10000, 10000) / 10000f;
#endif
        }
        public static Vector3 GetRandomVector3()
        {
            return new Vector3(GetRandomFloatinUnit(), GetRandomFloatinUnit(), GetRandomFloatinUnit());
        }

        public static Vector4 GetRandomVector4()
        {
            return new Vector4(GetRandomFloatinUnit(), GetRandomFloatinUnit(), GetRandomFloatinUnit(), GetRandomFloatinUnit());
        }

        public static float GetRandomAngleRadian()
        {
            return (float)(Utils.random.Next(-360, 360) / 180.0 * Math.PI);
        }
    }
}
