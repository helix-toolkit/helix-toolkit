/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using HxScene = Model.Scene;

    namespace Assimp
    {
#if !CORE
        public partial class Exporter
        {
            public ErrorCode ExportToFile(string filePath, Element3D root, string formatId)
            {
                SyncNamesWithElement3DAndSceneNode(root.SceneNode);
                return ExportToFile(filePath, root.SceneNode, formatId);
            }

            private void SyncNamesWithElement3DAndSceneNode(HxScene.SceneNode node)
            {
                foreach(var n in node.Traverse())
                {
                    if(n.WrapperSource is Element3D ele)
                    {
                        if(string.IsNullOrEmpty(ele.Name))
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
        }
#endif
    }
}
