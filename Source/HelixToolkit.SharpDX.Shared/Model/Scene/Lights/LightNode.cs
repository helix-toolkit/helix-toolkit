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
    using Core;
    public abstract class LightNode : SceneNode, ILight3D
    {
        public Color4 Color
        {
            set { (RenderCore as LightCoreBase).Color = value; }
            get { return (RenderCore as LightCoreBase).Color; }
        }

        public LightType LightType
        {
            get { return (RenderCore as ILight3D).LightType; }
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref System.Collections.Generic.List<HitTestResult> hits)
        {
            return false;
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }
    }
}
