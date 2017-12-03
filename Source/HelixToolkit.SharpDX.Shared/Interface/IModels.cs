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

    public interface IBillboardText
    {
        BillboardType Type { get; }
        Stream Texture { get; }

        Stream AlphaTexture { get; }
        void DrawTexture();
        Vector3Collection Positions { get; }
        IList<Vector2> TextureOffsets { get; }
        Vector2Collection TextureCoordinates { get; }
        Color4Collection Colors { get; }
        float Width { get; }
        float Height { get; }
    }

    public enum BillboardType
    {
        SingleText, MultipleText, SingleImage
    }
}
