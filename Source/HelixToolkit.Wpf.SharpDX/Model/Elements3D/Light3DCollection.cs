// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Light3DCollection.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using HelixToolkit.Mathematics;
using HelixToolkit.Wpf.SharpDX.Core;
using System.Collections.Generic;
namespace HelixToolkit.Wpf.SharpDX
{


    public class Light3DCollection : GroupElement3D, ILight3D
    {
        public LightType LightType
        {
            get
            {
                return LightType.None;
            }
        }

        public override bool HitTest(RenderContext context, Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}