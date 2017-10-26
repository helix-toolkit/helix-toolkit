using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.SharpDX.Core2D;
using System.Windows;
using D2D = global::SharpDX.Direct2D1;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public class EllipseModel2D : ShapeModel2D
    {
        protected override ShapeRenderable2DBase CreateShapeRenderCore(IRenderHost host)
        {
            return new EllipseRenderable();
        }
    }
}
