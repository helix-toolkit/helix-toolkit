// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using global::SharpDX;

    public sealed class PointLight3D : PointLightBase3D
    {
        public PointLight3D()
        {
            this.LightType = LightType.Point;
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
            // --- turn-on the light            
            Light3DSceneShared.LightModels.Lights[lightIndex].LightColor = this.ColorInternal;
            // --- Set lighting parameters
            Light3DSceneShared.LightModels.Lights[lightIndex].LightPos = this.PositionInternal.ToVector4() + modelMatrix.Row4;
            Light3DSceneShared.LightModels.Lights[lightIndex].LightAtt = new Vector4((float)this.AttenuationInternal.X, (float)this.AttenuationInternal.Y, (float)this.AttenuationInternal.Z, (float)this.RangeInternal);
        }
    }
}
