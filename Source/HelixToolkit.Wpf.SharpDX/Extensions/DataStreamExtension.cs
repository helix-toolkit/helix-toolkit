using SharpDX;


namespace HelixToolkit.Wpf.SharpDX
{
    public static class DataStreamExtension
    {
        public static int ReadInt(this DataStream ds)
        {
            return ds.Read<int>();
        }
    }
}
