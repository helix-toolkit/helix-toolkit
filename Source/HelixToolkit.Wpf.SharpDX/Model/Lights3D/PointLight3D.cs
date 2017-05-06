// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using global::SharpDX;

    using HelixToolkit.Wpf.SharpDX.Extensions;

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
                // --- light constant params            
                this.vLightPos = this.effect.GetVariableByName("vLightPos").AsVector();
                this.vLightColor = this.effect.GetVariableByName("vLightColor").AsVector();
                this.vLightAtt = this.effect.GetVariableByName("vLightAtt").AsVector();
                this.iLightType = this.effect.GetVariableByName("iLightType").AsScalar();

                // --- Set light type
                Light3DSceneShared.LightTypes[lightIndex] = (int)this.LightType;

                // --- flush
                //this.Device.ImmediateContext.Flush();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref this.vLightPos);
            Disposer.RemoveAndDispose(ref this.vLightColor);
            Disposer.RemoveAndDispose(ref this.vLightAtt);
            Disposer.RemoveAndDispose(ref this.iLightType);
            base.OnDetach();
        }
        protected override bool CanRender(RenderContext context)
        {
            var manager = renderHost.RenderTechniquesManager;
            if (base.CanRender(context))
            {
                return !renderHost.IsDeferredLighting;
            }
            return false;
        }
        protected override void OnRender(RenderContext context)
        {
            // --- turn-on the light            
            Light3DSceneShared.LightColors[lightIndex] = this.ColorInternal;
            // --- Set lighting parameters
            Light3DSceneShared.LightPositions[lightIndex] = this.PositionInternal.ToVector4();
            Light3DSceneShared.LightAtt[lightIndex] = new Vector4((float)this.AttenuationInternal.X, (float)this.AttenuationInternal.Y, (float)this.AttenuationInternal.Z, (float)this.RangeInternal);

            // --- Update lighting variables    
            this.vLightPos.Set(Light3DSceneShared.LightPositions);
            this.vLightColor.Set(Light3DSceneShared.LightColors);
            this.vLightAtt.Set(Light3DSceneShared.LightAtt);
            this.iLightType.Set(Light3DSceneShared.LightTypes);
        }
    }
}
