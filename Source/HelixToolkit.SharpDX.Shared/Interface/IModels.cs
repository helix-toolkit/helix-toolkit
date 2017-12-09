using SharpDX;
using System.Collections.Generic;
using System.IO;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core;
    using global::SharpDX.Direct3D11;
    using Utilities;

    public interface IBillboardText
    {
        BillboardType Type { get; }
        Stream Texture { get; }

        Stream AlphaTexture { get; }
        void DrawTexture();
        IList<BillboardVertex> BillboardVertices { get; }
        float Width { get; }
        float Height { get; }
    }

    public enum BillboardType
    {
        SingleText, MultipleText, SingleImage
    }

    public interface ILightsBufferProxy<T> where T : struct
    {
        int BufferSize { get; }
        T[] Lights { get; }
        Color4 AmbientLight { set; get; }
        void UploadToBuffer(IBufferProxy buffer, DeviceContext context);
    }
}
