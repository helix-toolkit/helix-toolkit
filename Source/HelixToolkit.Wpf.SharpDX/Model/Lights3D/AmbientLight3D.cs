// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    public sealed class AmbientLight3D : Light3D
    {
        public AmbientLight3D()
        {
            this.Color = new global::SharpDX.Color4(0.2f, 0.2f, 0.2f, 1f);
            this.LightType = LightType.Ambient;
        }

        protected override void OnRender(IRenderContext context)
        {
            Light3DSceneShared.LightModels.AmbientLight = this.ColorInternal;
        }
    }
}