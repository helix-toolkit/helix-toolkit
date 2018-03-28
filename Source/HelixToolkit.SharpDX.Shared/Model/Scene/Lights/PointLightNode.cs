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
    public class PointLightNode : LightNode
    {
        public Vector3 Position
        {
            set
            {
                (RenderCore as PointLightCore).Position = value;
            }
            get { return (RenderCore as PointLightCore).Position; }
        }

        public Vector3 Attenuation
        {
            set { (RenderCore as PointLightCore).Attenuation = value; }
            get { return (RenderCore as PointLightCore).Attenuation; }
        }

        public float Range
        {
            set { (RenderCore as PointLightCore).Range = value; }
            get { return (RenderCore as PointLightCore).Range; }
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new PointLightCore();
        }
    }
}
