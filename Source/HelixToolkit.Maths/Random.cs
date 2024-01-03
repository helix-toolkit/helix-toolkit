namespace HelixToolkit.Maths
{
    public static class RandomUtils
    {
        public static float NextFloat(this Random random, float min, float max)
        {
            return MathUtil.Lerp(min, max, (float)random.NextDouble());
        }
        public static double NextDouble(this Random random, float min, float max)
        {
            return MathUtil.Lerp(min, max, random.NextDouble());
        }
        public static Vector3 NextVector3(this Random random)
        {
            return Vector3.Normalize(new Vector3(random.NextFloat(-1, 1), random.NextFloat(-1, 1), random.NextFloat(-1, 1)));
        }
        public static Vector3 NextVector3(this Random random, Vector3 min, Vector3 max)
        {
            return Vector3.Normalize(new Vector3(random.NextFloat(min.X, max.X), random.NextFloat(min.Y, max.Y), random.NextFloat(min.Z, max.Z)));
        }
        public static Color4 NextColor(this Random random, float alpha = 1)
        {
            return new Color4(random.NextFloat(0, 1), random.NextFloat(0, 1), random.NextFloat(0, 1), alpha);
        }
    }
}
