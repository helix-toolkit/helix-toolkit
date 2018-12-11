/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model.Scene;
    /// <summary>
    /// Used to hold scene nodes without WPF/UWP dependencies.
    /// Used for code behind only. Avoid performance penalty from Dependency Properties.
    /// </summary>
    public class SceneNodeGroupModel3D : Element3D
    {
        public GroupNode GroupNode { get; } = new GroupNode();

        public void AddNode(SceneNode node)
        {
            GroupNode.AddChildNode(node);
        }

        public void RemoveNode(SceneNode node)
        {
            GroupNode.RemoveChildNode(node);
        }

        public void Clear()
        {
            GroupNode.Clear();
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return GroupNode;
        }
    }
}
