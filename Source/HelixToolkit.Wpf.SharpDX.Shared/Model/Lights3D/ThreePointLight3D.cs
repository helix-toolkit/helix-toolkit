// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreePointLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SharpDX;
#if COREWPF
using HelixToolkit.SharpDX.Core;
#endif
namespace HelixToolkit.Wpf.SharpDX
{

    public class ThreePointLight3D : GroupElement3D, ILight3D
    {
        public ThreePointLight3D()
        {
        }

        public LightType LightType
        {
            get
            {
                return LightType.ThreePoint;
            }
        }
    }
}