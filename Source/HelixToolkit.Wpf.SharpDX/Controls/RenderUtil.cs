namespace HelixToolkit.Wpf.SharpDX
{
    using System.IO;
    using global::SharpDX;
    using global::SharpDX.Direct3D11;
    using Direct3D11 = global::SharpDX.Direct3D11;

    public static class RenderUtil
    {
#if SYSTEM_DRAWING
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        } 
#endif


        public static byte[] ToByteArray(this System.Windows.Media.Imaging.BitmapSource bitmapSource)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmapSource));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Direct3D11.Buffer CreateBuffer<T>(this Direct3D11.Device device, BindFlags flags, int sizeofT, T[] range)
            where T : struct
        {
            //sizeofT = sizeofT == 0 ? sizeofT = Marshal.SizeOf(typeof(T)) : sizeofT;
            using (var stream = new DataStream(range.Length * sizeofT, true, true))
            {                
                stream.WriteRange(range);
                stream.Position = 0;
                return new Direct3D11.Buffer(device, stream, new BufferDescription
                {
                    BindFlags = flags,
                    SizeInBytes = (int)stream.Length,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                    Usage = ResourceUsage.Default,
                });
            }
        }
    }
}
