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

    private readonly Random random = new(Environment.TickCount);
}
