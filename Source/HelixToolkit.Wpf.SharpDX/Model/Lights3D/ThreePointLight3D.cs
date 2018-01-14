// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreePointLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Model.Lights3D
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

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            throw new NotImplementedException();
        }
    }
}