/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Model;
    public class SpotLightCore : PointLightCore
    {
        private Vector3 direction;
        public Vector3 Direction
        {
            set
            {
                SetAffectsRender(ref direction, value);
            }
            get { return direction; }
        }

        private float fallOff = 1;
        public float FallOff
        {
            set { SetAffectsRender(ref fallOff, value); }
            get { return fallOff; }
        }

        private float innerAngle = 5;
        public float InnerAngle
        {
            set { SetAffectsRender(ref innerAngle, value); }
            get { return innerAngle; }
        }


        private float outerAngle = 45;
        public float OuterAngle
        {
            set { SetAffectsRender(ref outerAngle, value); }
            get { return outerAngle; }
        }

        public SpotLightCore()
        {
            LightType = LightType.Spot;
        }

        protected override void OnRender(Light3DSceneShared lightScene, int index)
        {
            base.OnRender(lightScene, index);
            lightScene.LightModels.Lights[index].LightDir = Vector4.Transform(direction.ToVector4(0), ModelMatrix).Normalized();
            lightScene.LightModels.Lights[index].LightSpot = new Vector4((float)Math.Cos(outerAngle / 360.0f * Math.PI), (float)Math.Cos(innerAngle / 360.0f * Math.PI), fallOff, 0);
        }
    }
}
