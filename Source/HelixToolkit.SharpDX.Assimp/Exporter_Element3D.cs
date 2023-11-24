using HxAnimations = HelixToolkit.SharpDX.Animations;
using HxScene = HelixToolkit.SharpDX.Model.Scene;

namespace HelixToolkit.SharpDX.Assimp;

public partial class Exporter
{
    // todo
    /*
    public ErrorCode ExportToFile(string filePath, Element3D root, string formatId)
    {
        SyncNamesWithElement3DAndSceneNode(root.SceneNode);
        return ExportToFile(filePath, root.SceneNode, formatId);
    }

    private void SyncNamesWithElement3DAndSceneNode(HxScene.SceneNode node)
    {
        foreach (var n in node.Traverse())
        {
            if (n.WrapperSource is Element3D ele)
            {
                if (string.IsNullOrEmpty(ele.Name))
                {
                    continue;
                }
                else if (string.IsNullOrEmpty(node.Name))
                {
                    node.Name = ele.Name;
                }
            }
        }
    }
    */
}
