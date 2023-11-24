using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D;

public interface ITextBlock : IBackground
{
    Brush Foreground
    {
        set; get;
    }
}
