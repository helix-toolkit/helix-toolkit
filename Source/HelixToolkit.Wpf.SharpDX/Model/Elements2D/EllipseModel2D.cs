namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Model.Scene2D;

    public class EllipseModel2D : ShapeModel2D
    {
        protected override SceneNode2D OnCreateSceneNode()
        {
            return new EllipseNode2D();
        }
    }
}
