namespace HelixToolkit.WinUI.Model.Scene
{
    public partial class SceneNode
    {
        public static implicit operator Element3D(SceneNode node)
        {
            return node.WrapperSource as Element3D;
        }
    }
}
