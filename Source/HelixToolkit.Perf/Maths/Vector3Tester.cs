namespace HelixToolkit.Perf.Maths
{
    internal static class Vector3Tester
    {
        public static void MinMax(int size, int iteration = 10000)
        {
            var data = new Vector3[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = Utils.GetRandomVector3();
            }
            for (int i = 0; i < iteration; i++)
            { data.MinMax(out var _, out var _); }
        }

        public static void TransformCoordinate(int size, int iteration = 10000)
        {
            var data = new Vector3[size];
            var result = new Vector3[size];
            var transform = Matrix4x4.CreateLookAt(Utils.GetRandomVector3() * 100, Vector3.Zero, Vector3.UnitY);
            for (int i = 0; i < size; i++)
            {
                data[i] = Utils.GetRandomVector3();
            }
            for (int i = 0; i < iteration; i++)
            {
                data.TransformCoordinate(ref transform, result);
            }
        }
    }
}
