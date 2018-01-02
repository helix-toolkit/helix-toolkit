namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    public class EllipseModel2D : ShapeModel2D
    {
        protected override ShapeRenderable2DBase CreateShapeRenderCore(IRenderHost host)
        {
            return new EllipseRenderable();
        }
    }
}
