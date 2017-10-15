// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Light3DCollection.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using HelixToolkit.Wpf.SharpDX.Model.Lights3D;

namespace HelixToolkit.Wpf.SharpDX
{
    public class Light3DCollection : GroupElement3D, ILight3D
    {
        public Light3DSceneShared Light3DSceneShared
        {
            private set; get;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            Light3DSceneShared = host.Light3DSceneShared;
            return base.OnAttach(host);
        }
    }
}