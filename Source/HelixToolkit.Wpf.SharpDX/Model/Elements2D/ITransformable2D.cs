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
}
