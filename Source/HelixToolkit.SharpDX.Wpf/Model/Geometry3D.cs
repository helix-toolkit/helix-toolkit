namespace HelixToolkit.SharpDX.Wpf
{
    using global::SharpDX;
    using Direct3D = global::SharpDX.Direct3D;
    using Direct3D10 = global::SharpDX.Direct3D10;

    public abstract class Geometry3D
    {
        /// <summary>
        /// 
        /// </summary>
        protected static Direct3D10.Buffer CreateBuffer<T>(Direct3D10.Device device, int sizeofT, T[] range)
            where T : struct
        {
            //sizeofT = sizeofT == 0 ? sizeofT = Marshal.SizeOf(typeof(T)) : sizeofT;
            using (var stream = new DataStream(range.Length * sizeofT, true, true))
            {
                stream.WriteRange(range);
                return new Direct3D10.Buffer(device, stream, new Direct3D10.BufferDescription
                {
                    BindFlags = Direct3D10.BindFlags.VertexBuffer,
                    SizeInBytes = (int)stream.Length,
                    CpuAccessFlags = Direct3D10.CpuAccessFlags.None,
                    OptionFlags = Direct3D10.ResourceOptionFlags.None,
                    Usage = Direct3D10.ResourceUsage.Default,
                });
            }
        }  
    }
}