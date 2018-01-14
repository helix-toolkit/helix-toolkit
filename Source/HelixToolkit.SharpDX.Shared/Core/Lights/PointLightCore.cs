/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Model;
    public class PointLightCore : LightCoreBase
    {
        private Vector3 position;
        public Vector3 Position
        {
            set
            {
                SetAffectsRender(ref position, value);
            }
            get { return position; }
        }

        private Vector3 attenuation = new Vector3(1, 0, 0);
        public Vector3 Attenuation
        {
            set { SetAffectsRender(ref attenuation, value); }
            get { return attenuation; }
        }

        private float range = 1000;
        public float Range
        {
            set { SetAffectsRender(ref range, value); }
            get { return range; }
        }

        public PointLightCore()
        {
            LightType = LightType.Point;
        }

        protected override void OnRender(Light3DSceneShared lightScene, int index)
        {
            base.OnRender(lightScene, index);
            lightScene.LightModels.Lights[index].LightPos = (position + ModelMatrix.Row4.ToVector3()).ToVector4();
            lightScene.LightModels.Lights[index].LightAtt = attenuation.ToVector4(range);
        }
    }
}
