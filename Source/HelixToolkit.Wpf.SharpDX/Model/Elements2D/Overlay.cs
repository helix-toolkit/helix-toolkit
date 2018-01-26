using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public sealed class Overlay : ContentElement2D
    {
        //protected override Vector2 MeasureOverride(Vector2 availableSizeWithoutMargins)
        //{
        //    Content2D.Measure(Size);
        //    return Size;
        //}

        protected override void OnRender(IRenderContext2D context)
        {
            base.OnRender(context);
            foreach(var item in Items)
            {
                item.Render(context);
            }
        }
    }
}
