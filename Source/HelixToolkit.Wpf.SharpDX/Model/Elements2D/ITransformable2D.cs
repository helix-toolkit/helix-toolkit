using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D;

public interface ITransformable2D
{
    Transform Transform
    {
        set; get;
    }
}
