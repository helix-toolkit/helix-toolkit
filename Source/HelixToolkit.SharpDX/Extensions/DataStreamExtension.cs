using SharpDX;

namespace HelixToolkit.SharpDX;

public static class DataStreamExtension
{
    public static int ReadInt(this DataStream ds)
    {
        return ds.Read<int>();
    }

    public static float ReadFloat(this DataStream ds)
    {
        return ds.Read<float>();
    }

    public static Vector4 ReadVector4(this DataStream ds)
    {
        return ds.Read<Vector4>();
    }

    public static Matrix ReadMatrix(this DataStream ds)
    {
        return ds.Read<Matrix>();
    }
}
