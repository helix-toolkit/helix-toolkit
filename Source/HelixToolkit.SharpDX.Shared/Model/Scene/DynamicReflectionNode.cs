/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    public class DynamicReflectionNode : GroupNode
    {
        public DynamicReflectionNode()
        {
            this.OnAddChildNode += DynamicReflectionNode_OnAddChildNode;
            this.OnRemoveChildNode += DynamicReflectionNode_OnRemoveChildNode;
            this.OnClear += DynamicReflectionNode_OnClear;
        }

        private void DynamicReflectionNode_OnClear(object sender, OnChildNodeChangedArgs e)
        {
            (RenderCore as DynamicCubeMapCore).IgnoredGuid.Clear();
        }

        private void DynamicReflectionNode_OnRemoveChildNode(object sender, OnChildNodeChangedArgs e)
        {
            (RenderCore as DynamicCubeMapCore).IgnoredGuid.Remove(e.Node.RenderCore.GUID);
        }

        private void DynamicReflectionNode_OnAddChildNode(object sender, OnChildNodeChangedArgs e)
        {
            (RenderCore as DynamicCubeMapCore).IgnoredGuid.Add(e.Node.RenderCore.GUID);
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new DynamicCubeMapCore();
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
