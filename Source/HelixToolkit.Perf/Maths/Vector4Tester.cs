namespace HelixToolkit.Perf.Maths
{
    internal static class Vector4Tester
    {
        public static void Transform(int size, int iteration)
        {
            var data = new Vector4[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = Utils.GetRandomVector4();
            }
            var result = new Vector4[size];
            var transform = Matrix4x4.CreateFromAxisAngle(Vector3.Normalize(Utils.GetRandomVector3()), Utils.GetRandomAngleRadian())
                * Matrix4x4.CreateTranslation(Utils.GetRandomVector3());
            for (int iter = 0; iter < iteration; ++iter)
            {
                data.Transform(ref transform, result);
            }
        }
    }
}
