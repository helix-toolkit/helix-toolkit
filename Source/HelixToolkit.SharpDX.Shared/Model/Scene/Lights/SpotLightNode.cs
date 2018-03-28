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
    public class SpotLightNode : PointLightNode
    {
        public Vector3 Direction
        {
            set { (RenderCore as SpotLightCore).Direction = value; }
            get { return (RenderCore as SpotLightCore).Direction; }
        }

        public float FallOff
        {
            set { (RenderCore as SpotLightCore).FallOff = value; }
            get { return (RenderCore as SpotLightCore).FallOff; }
        }

        public float InnerAngle
        {
            set { (RenderCore as SpotLightCore).InnerAngle = value; }
            get { return (RenderCore as SpotLightCore).InnerAngle; }
        }

        public float OuterAngle
        {
            set { (RenderCore as SpotLightCore).OuterAngle = value; }
            get { return (RenderCore as SpotLightCore).OuterAngle; }
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new SpotLightCore();
        }
    }
}
