/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System.Collections.Generic;
using System.Diagnostics;
using SharpDX;

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
    namespace Model.Scene
    {
        using Core;

        public sealed class BoneGroupNode : GroupNodeBase
        {
            public Matrix[] BoneMatrices
            {
                set
                {
                    core.BoneMatrices = value;
                }
                get { return core.BoneMatrices; }
            }

            private readonly BoneUploaderCore core = new BoneUploaderCore();

            public BoneGroupNode()
            {
                OnAddChildNode += NodeGroup_OnAddChildNode;
                OnRemoveChildNode += NodeGroup_OnRemoveChildNode;
            }

            protected override RenderCore OnCreateRenderCore()
            {
                return core;
            }

            private void NodeGroup_OnRemoveChildNode(object sender, OnChildNodeChangedArgs e)
            {
                if (e.Node is BoneSkinMeshNode b)
                {
                    (b.RenderCore as BoneSkinRenderCore).SharedBoneBuffer = null;
                }
            }

            private void NodeGroup_OnAddChildNode(object sender, OnChildNodeChangedArgs e)
            {
                if(e.Node is BoneSkinMeshNode b)
                {
                    (b.RenderCore as BoneSkinRenderCore).SharedBoneBuffer = core;
                }
            }
        }
    }

}
