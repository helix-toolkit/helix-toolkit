

namespace HelixToolkit.Perf.Maths
{
    internal static class Normalization
    {
        public static void NormalizeInPlaceTest(int size, int iterations = 10000)
        {
            var positions = new Vector3Collection(size);

            for (int i = 0; i < size; i++)
            {
                positions.Add(Utils.GetRandomVector3());
            }
            for (int i = 0; i < iterations; ++i)
            {
                MeshGeometryHelper.NormalizeInPlace(positions);
            }
        }
    }
}
