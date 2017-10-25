using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.SharpDX.Core2D;

namespace HelixToolkit.Wpf.SharpDX
{
    public class RectangleModel2D : ShapeModel2D
    {
        protected override ShapeRenderable2DBase CreateShapeRenderCore(IRenderHost host)
        {
            return new RectangleRenderable();
        }
    }
}
