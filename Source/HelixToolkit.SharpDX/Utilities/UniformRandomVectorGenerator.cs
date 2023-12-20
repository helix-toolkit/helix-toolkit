using SharpDX;

namespace HelixToolkit.SharpDX.Utilities;

public class UniformRandomVectorGenerator : IRandomVector
{
    public Vector3 MinVector { set; get; } = -Vector3.One;

    public Vector3 MaxVector { set; get; } = Vector3.One;

    public Vector3 RandomVector3
    {
        get
        {
            var x = random.NextFloat(MinVector.X, MaxVector.X);
            var y = random.NextFloat(MinVector.Y, MaxVector.Z);
            var z = random.NextFloat(MinVector.Y, MaxVector.Z);
            return new Vector3(x, y, z);
        }
    }

    public uint Seed
    {
        get
        {
            return (uint)Math.Abs(random.Next());
        }
    }

    private readonly Random random = new(Environment.TickCount);
}
