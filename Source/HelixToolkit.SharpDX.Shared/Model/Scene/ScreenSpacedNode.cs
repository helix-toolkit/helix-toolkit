/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using System.Collections.Generic;
    using Core;

    public class ScreenSpacedNode : SceneNode
    {
        public float RelativeScreenLocationX
        {
            set
            {
                (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationX = value;
            }
            get
            {
                return (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationX;
            }
        }

        public float RelativeScreenLocationY
        {
            set
            {
                (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationY = value;
            }
            get
            {
                return (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationY;
            }
        }

        public float SizeScale
        {
            set
            {
                (RenderCore as IScreenSpacedRenderParams).SizeScale = value;
            }
            get
            {
                return (RenderCore as IScreenSpacedRenderParams).SizeScale;
            }
        }

        public bool IsRightHand
        {
            set
            {
                (RenderCore as IScreenSpacedRenderParams).IsRightHand = value;
            }
            get
            {
                return (RenderCore as IScreenSpacedRenderParams).IsRightHand;
            }
        }

        protected bool NeedClearDepthBuffer { set; get; } = true;


        protected IScreenSpacedRenderParams screenSpaceCore { get { return (IScreenSpacedRenderParams)RenderCore; } }

        protected override RenderCore OnCreateRenderCore()
        {
            return new ScreenSpacedMeshRenderCore();
        }

        protected override bool OnAttach(IRenderHost host)
        {
            RenderCore.Attach(renderTechnique);
            screenSpaceCore.RelativeScreenLocationX = (float)this.RelativeScreenLocationX;
            screenSpaceCore.RelativeScreenLocationY = (float)this.RelativeScreenLocationY;
            screenSpaceCore.SizeScale = (float)this.SizeScale;
            return base.OnAttach(host);
        }

        protected override void OnDetach()
        {
            RenderCore.Detach();
            base.OnDetach();
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
