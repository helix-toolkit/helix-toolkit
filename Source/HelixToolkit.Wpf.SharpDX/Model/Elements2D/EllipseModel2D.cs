namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using SharpDX;

    public class EllipseModel2D : ShapeModel2D
    {
        protected override ShapeRenderCore2DBase CreateShapeRenderCore()
        {
            return new EllipseRenderCore2D();
        }

        protected override bool OnHitTest(ref global::SharpDX.Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            return false;
        }
    }
}
