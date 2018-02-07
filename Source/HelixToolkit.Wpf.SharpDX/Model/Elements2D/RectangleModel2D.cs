using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    public class RectangleModel2D : ShapeModel2D
    {
        protected override ShapeRenderCore2DBase CreateShapeRenderCore()
        {
            return new RectangleRenderCore2D();
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            return false;
        }
    }
}
