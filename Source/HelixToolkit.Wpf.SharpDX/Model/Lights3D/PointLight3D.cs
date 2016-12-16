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

        public override void Attach(IRenderHost host)
        {
            /// --- attach
            base.Attach(host);

            /// --- light constant params            
            this.vLightPos = this.effect.GetVariableByName("vLightPos").AsVector();
            this.vLightColor = this.effect.GetVariableByName("vLightColor").AsVector();
            this.vLightAtt = this.effect.GetVariableByName("vLightAtt").AsVector();
            this.iLightType = this.effect.GetVariableByName("iLightType").AsScalar();

            /// --- Set light type
            Light3DSceneShared.LightTypes[lightIndex] = (int)Light3D.Type.Point;

            /// --- flush
            //this.Device.ImmediateContext.Flush();
        }

        public override void Detach()
        {
            Disposer.RemoveAndDispose(ref this.vLightPos);
            Disposer.RemoveAndDispose(ref this.vLightColor);
            Disposer.RemoveAndDispose(ref this.vLightAtt);
            Disposer.RemoveAndDispose(ref this.iLightType);
            base.Detach();
        }

        public override void Render(RenderContext context)
        {
            if (renderHost.RenderTechnique == renderHost.RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.Deferred) ||
                renderHost.RenderTechnique == renderHost.RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.GBuffer))
            {
                return;
            }

            if (this.IsRendering)
            {
                /// --- turn-on the light            
                Light3DSceneShared.LightColors[lightIndex] = this.Color;
            }
            else
            {
                // --- turn-off the light
                Light3DSceneShared.LightColors[lightIndex] = new global::SharpDX.Color4(0, 0, 0, 0);
            }

            /// --- Set lighting parameters
            Light3DSceneShared.LightPositions[lightIndex] = this.Position.ToVector4();
            Light3DSceneShared.LightAtt[lightIndex] = new Vector4((float)this.Attenuation.X, (float)this.Attenuation.Y, (float)this.Attenuation.Z, (float)this.Range);

            /// --- Update lighting variables    
            this.vLightPos.Set(Light3DSceneShared.LightPositions);
            this.vLightColor.Set(Light3DSceneShared.LightColors);
            this.vLightAtt.Set(Light3DSceneShared.LightAtt);
            this.iLightType.Set(Light3DSceneShared.LightTypes);
        }
    }
}
