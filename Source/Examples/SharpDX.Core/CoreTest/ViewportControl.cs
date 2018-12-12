﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreTest
{
    public struct ViewportOptions
    {
        public bool DirectionalLightFollowCamera;
        public bool WalkAround;
        public bool EnableSSAO;
        public bool EnableFXAA;
        public bool EnableFrustum;
        public System.Numerics.Vector3 BackgroundColor;
        public float DirectionLightIntensity;
        public float AmbientLightIntensity;
    }
}