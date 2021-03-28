// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Light3DCollection.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    using Core;
    using System.Collections.Generic;

    public class Light3DCollection : GroupElement3D, ILight3D
    {
        public LightType LightType
        {
            get
            {
                return LightType.None;
            }
        }

        public override bool HitTest(HitTestContext context, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}