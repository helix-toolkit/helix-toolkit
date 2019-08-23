/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    namespace Core
    {
        using Model;
        public class DirectionalLightCore : LightCoreBase
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

            public DirectionalLightCore()
            {
                LightType = LightType.Directional;
            }

            protected override void OnRender(Light3DSceneShared lightScene, int index)
            {
                base.OnRender(lightScene, index);
                lightScene.LightModels.Lights[index].LightDir = -Vector3.TransformNormal(direction, ModelMatrix).Normalized().ToVector4(0);
            }
        }
    }

}
