namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    public class EllipseModel2D : ShapeModel2D
    {
        protected override ShapeRenderable2DBase CreateShapeRenderCore(ID2DTarget host)
        {
            return new EllipseRenderable();
        }
    }
}
