// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreePointLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace HelixToolkit.Wpf.SharpDX.Model.Lights3D
{
    public class ThreePointLight3D : GroupElement3D, ILight3D
    {
        public ThreePointLight3D()
        {
            //TODO: http://www.3drender.com/light/3point.html
        }

        public Light3DSceneShared Light3DSceneShared
        {
            private set; get;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            Light3DSceneShared = host.Light3DSceneShared;
            foreach (var c in this.Items)
            {
                c.Attach(host);
            }
            return true;
        }

        protected override void OnDetach()
        {
            base.OnDetach();
            foreach (var c in this.Items)
            {
                c.Detach();
            }
        }

        protected override void OnRender(RenderContext context)
        {
            foreach (var c in this.Items)
            {
                c.Render(context);
            }
        }
    }
}