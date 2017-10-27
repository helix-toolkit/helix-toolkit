using SharpDX;
using System;
using System.Collections.Generic;
using System.Text;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public interface ITransformable2D
    {
        Media.Transform Transform { set; get; }
    }

    public interface IHitable2D
    {
        bool HitTest(Vector2 mousePoint, out HitTest2DResult hitResult);
        bool IsHitTestVisible { set; get; }
    }
}
