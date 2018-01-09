// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Light3DCollection.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using Model;
    using SharpDX;

    public class Light3DCollection : GroupElement3D, ILight3D
    {
        public Light3DSceneShared Light3DSceneShared
        {
            private set; get;
        }

        public LightType LightType
        {
            get
            {
                return LightType.None;
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            Light3DSceneShared = host.Light3DSceneShared;
            return base.OnAttach(host);
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