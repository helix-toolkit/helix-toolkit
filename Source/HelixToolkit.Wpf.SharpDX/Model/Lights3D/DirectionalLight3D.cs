// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectionalLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace HelixToolkit.Wpf.SharpDX
{
    public sealed class DirectionalLight3D : Light3D
    {
        public DirectionalLight3D()
        {
            this.Color = System.Windows.Media.Colors.White;
            this.LightType = LightType.Directional;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (base.OnAttach(host))
            {
                // --- Set light type
                Light3DSceneShared.LightModels.Lights[lightIndex].LightType = (int)this.LightType;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnRender(IRenderContext context)
        {
            Light3DSceneShared.LightModels.Lights[lightIndex].LightColor = this.ColorInternal;
            // --- set lighting parameters
            Light3DSceneShared.LightModels.Lights[lightIndex].LightDir = -this.DirectionInternal.Normalized().ToVector4();
        }
    }
}
