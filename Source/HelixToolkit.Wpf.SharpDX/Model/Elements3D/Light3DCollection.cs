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

    public class Light3DCollection : GroupElement3D, ILight3D
    {
        public LightType LightType
        {
            get
            {
                return LightType.None;
            }
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }

        protected override bool OnHitTest(IRenderContext context, global::SharpDX.Matrix totalModelMatrix, ref global::SharpDX.Ray ray, ref List<HitTestResult> hits)
        {
            throw new NotImplementedException();
        }
    }
}