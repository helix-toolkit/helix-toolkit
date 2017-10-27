using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Text;
using D2D = global::SharpDX.Direct2D1;
using Media = System.Windows.Media;
using HelixToolkit.Wpf.SharpDX.Extensions;

namespace HelixToolkit.SharpDX.Core2D
{
    public abstract class ShapeRenderable2DBase : Renderable2DBase
    {
        public D2D.Brush FillBrush = null;
        public D2D.Brush StrokeBrush = null;
        public int StrokeWidth
        {
            set; get;
        } = 1;
        public D2D.StrokeStyle StrokeStyle = null;

        public override void Dispose()
        {
            Disposer.RemoveAndDispose(ref FillBrush);
            Disposer.RemoveAndDispose(ref StrokeBrush);
            Disposer.RemoveAndDispose(ref StrokeStyle);
            base.Dispose();
        }
    }
}
