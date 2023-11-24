using HelixToolkit.SharpDX;
using SharpDX;

namespace BillboardDemo;

public class Flag : ImageInfo
{
    public string Name { get; }

    public Flag(string name, Vector3 pos, Vector4 coord)
    {
        Name = name;
        Position = pos;
        UV_TopLeft = new Vector2(coord.X, coord.Y);
        UV_BottomRight = new Vector2(coord.Z, coord.W);
        Width = 0.5f;
        Height = 0.4f;
    }
}
