/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX.Model;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
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
            lightScene.LightModels.Lights[index].LightDir = -Vector4.Transform(direction.ToVector4(0f), ModelMatrix).Normalized();
        }
    }
}
