namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Model.Scene2D;

    public class RectangleModel2D : ShapeModel2D
    {
        protected override SceneNode2D OnCreateSceneNode()
        {
            return new RectangleNode2D();
        }
    }
}
