// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Light3DCollection.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using HelixToolkit.Wpf.SharpDX.Core;
    using SharpDX;

    public class Light3DCollection : GroupElement3D, ILight3D
    {
        public LightType LightType
        {
            get
            {
                return LightType.None;
            }
        }

        public override bool HitTest(IRenderContext context, global::SharpDX.Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}