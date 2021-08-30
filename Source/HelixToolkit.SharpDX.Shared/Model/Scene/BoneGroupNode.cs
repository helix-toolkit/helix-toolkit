/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System.Collections.Generic;
using System.Diagnostics;
using SharpDX;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene
    {
        using Core;

        public sealed class BoneGroupNode : GroupNodeBase, Animations.IBoneMatricesNode
        {
            public Matrix[] BoneMatrices
            {
                set
                {
                    core.BoneMatrices = value;
                }
                get { return core.BoneMatrices; }
            }

            /// <summary>
            /// Gets or sets the bones.
            /// </summary>
            /// <value>
            /// The bones.
            /// </value>
            public Animations.Bone[] Bones { set; get; }

            public float[] MorphTargetWeights
            {
                set; get;
            }
            /// <summary>
            /// Always return false for bone groups
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance has bone group; otherwise, <c>false</c>.
            /// </value>
            public bool HasBoneGroup { get; } = false;

            private readonly BoneUploaderCore core = new BoneUploaderCore();

            public BoneGroupNode()
            {
                ChildNodeAdded += NodeGroup_OnAddChildNode;
                ChildNodeRemoved += NodeGroup_OnRemoveChildNode;
            }

            protected override RenderCore OnCreateRenderCore()
            {
                return core;
            }

            private void NodeGroup_OnRemoveChildNode(object sender, OnChildNodeChangedArgs e)
            {
                if (e.Node is BoneSkinMeshNode b)
                {
                    b.HasBoneGroup = false;
                    (b.RenderCore as BoneSkinRenderCore).SharedBoneBuffer = null;
                }
            }

            private void NodeGroup_OnAddChildNode(object sender, OnChildNodeChangedArgs e)
            {
                if(e.Node is BoneSkinMeshNode b)
                {
                    b.HasBoneGroup = true;
                    (b.RenderCore as BoneSkinRenderCore).SharedBoneBuffer = core;
                }
            }
        }
    }

}
