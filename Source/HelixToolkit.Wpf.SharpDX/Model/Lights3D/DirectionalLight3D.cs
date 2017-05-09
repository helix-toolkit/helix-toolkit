// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectionalLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using HelixToolkit.Wpf.SharpDX.Extensions;

    public sealed class DirectionalLight3D : Light3D
    {
        public DirectionalLight3D()
        {
            this.Color = global::SharpDX.Color.White;
            this.LightType = LightType.Directional;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (base.OnAttach(host))
            {

                // --- light constant params            
                this.vLightDir = this.effect.GetVariableByName("vLightDir").AsVector();
                this.vLightColor = this.effect.GetVariableByName("vLightColor").AsVector();
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
            Disposer.RemoveAndDispose(ref this.vLightDir);
            Disposer.RemoveAndDispose(ref this.vLightColor);
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
            Light3DSceneShared.LightColors[lightIndex] = this.ColorInternal;
            // --- set lighting parameters
            Light3DSceneShared.LightDirections[lightIndex] = -this.DirectionInternal.ToVector4();

            // --- update lighting variables               
            this.vLightDir.Set(Light3DSceneShared.LightDirections);
            this.vLightColor.Set(Light3DSceneShared.LightColors);
            this.iLightType.Set(Light3DSceneShared.LightTypes);


            // --- if shadow-map enabled
            if (this.renderHost.IsShadowMapEnabled)
            {
                // update shader
                this.mLightView.SetMatrix(Light3DSceneShared.LightViewMatrices);
                this.mLightProj.SetMatrix(Light3DSceneShared.LightProjMatrices);
            }
        }
    }
}
