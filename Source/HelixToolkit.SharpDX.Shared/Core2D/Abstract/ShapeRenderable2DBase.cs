using HelixToolkit.Wpf.SharpDX;
using D2D = global::SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
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
