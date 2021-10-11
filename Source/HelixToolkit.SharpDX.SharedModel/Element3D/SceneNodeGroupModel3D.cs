/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
namespace HelixToolkit.UWP
#elif WINUI
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Model.Scene;
#endif
    /// <summary>
    /// Used to hold scene nodes without WPF/UWP dependencies.
    /// Used for code behind only. Avoid performance penalty from Dependency Properties.
    /// </summary>
    public sealed class SceneNodeGroupModel3D : Element3D
    {
        public GroupNode GroupNode { get; } = new GroupNode();
        /// <summary>
        /// Adds a child node. <see cref="GroupNodeBase.AddChildNode(SceneNode)"/>
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Success or not</returns>
        public bool AddNode(SceneNode node)
        {
            return GroupNode.AddChildNode(node);
        }
        /// <summary>
        /// Removes child node. <see cref="GroupNodeBase.RemoveChildNode(SceneNode)"/>
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Sucess or not</returns>
        public bool RemoveNode(SceneNode node)
        {
            return GroupNode.RemoveChildNode(node);
        }
        /// <summary>
        /// Clears this group. <see cref="GroupNodeBase.Clear"/>
        /// </summary>
        public void Clear()
        {
            GroupNode.Clear();
        }
        /// <summary>
        /// Moves the child node order. <see cref="GroupNodeBase.MoveChildNode(int, int)"/>
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        public void MoveNode(int fromIndex, int toIndex)
        {
            GroupNode.MoveChildNode(fromIndex, toIndex);
        }
        /// <summary>
        /// Transfers the node to another group. <see cref="GroupNodeBase.TransferChildNode(SceneNode, GroupNodeBase)"/>
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="target">The target.</param>
        public void TransferNode(SceneNode item, GroupNodeBase target)
        {
            GroupNode.TransferChildNode(item, target);
        }
        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return GroupNode;
        }
    }
}
