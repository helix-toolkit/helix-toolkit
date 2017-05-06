// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpotLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Decay Exponent of the spotlight.
//   The falloff the spotlight between inner and outer angle
//   depends on this value.
//   For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows;

    using global::SharpDX;

    using HelixToolkit.Wpf.SharpDX.Extensions;

    public sealed class SpotLight3D : PointLightBase3D
    {
        public static readonly DependencyProperty FalloffProperty =
            DependencyProperty.Register("Falloff", typeof(double), typeof(SpotLight3D), new AffectsRenderPropertyMetadata(1.0,
                (d,e)=> {
                    (d as SpotLight3D).FalloffInternal = (double)e.NewValue;
                }));

        public static readonly DependencyProperty InnerAngleProperty =
            DependencyProperty.Register("InnerAngle", typeof(double), typeof(SpotLight3D), new AffectsRenderPropertyMetadata(5.0,
                (d, e) => {
                    (d as SpotLight3D).InnerAngleInternal = (double)e.NewValue;
                }));

        public static readonly DependencyProperty OuterAngleProperty =
            DependencyProperty.Register("OuterAngle", typeof(double), typeof(SpotLight3D), new AffectsRenderPropertyMetadata(45.0,
                (d, e) => {
                    (d as SpotLight3D).OuterAngleInternal = (double)e.NewValue;
                }));


        /// <summary>
        /// Decay Exponent of the spotlight.
        /// The falloff the spotlight between inner and outer angle
        /// depends on this value.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
        /// </summary>
        public double Falloff
        {
            get { return (double)this.GetValue(FalloffProperty); }
            set { this.SetValue(FalloffProperty, value); }
        }
        internal double FalloffInternal { private set; get; } = 1.0;
        /// <summary>
        /// Full outer angle of the spot (Phi) in degrees
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
        /// </summary>
        public double OuterAngle
        {
            get { return (double)this.GetValue(OuterAngleProperty); }
            set { this.SetValue(OuterAngleProperty, value); }
        }

        internal double OuterAngleInternal { private set; get; } = 45.0;
        /// <summary>
        /// Full inner angle of the spot (Theta) in degrees. 
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
        /// </summary>
        public double InnerAngle
        {
            get { return (double)this.GetValue(InnerAngleProperty); }
            set { this.SetValue(InnerAngleProperty, value); }
        }

        internal double InnerAngleInternal { private set; get; } = 5.0;


        public SpotLight3D()
        {
            this.LightType = LightType.Spot;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (base.OnAttach(host))
            {
                // --- light constant params            
                this.vLightPos = this.effect.GetVariableByName("vLightPos").AsVector();
                this.vLightDir = this.effect.GetVariableByName("vLightDir").AsVector();
                this.vLightSpot = this.effect.GetVariableByName("vLightSpot").AsVector();
                this.vLightColor = this.effect.GetVariableByName("vLightColor").AsVector();
                this.vLightAtt = this.effect.GetVariableByName("vLightAtt").AsVector();
                this.iLightType = this.effect.GetVariableByName("iLightType").AsScalar();

                // --- Set light type
                Light3DSceneShared.LightTypes[lightIndex] = (int)this.LightType;

                // --- flush
                // this.Device.ImmediateContext.Flush();
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
            Disposer.RemoveAndDispose(ref this.vLightDir);
            Disposer.RemoveAndDispose(ref this.vLightSpot);
            Disposer.RemoveAndDispose(ref this.vLightColor);
            Disposer.RemoveAndDispose(ref this.vLightAtt);
            Disposer.RemoveAndDispose(ref this.iLightType);
            base.OnDetach();
        }

        protected override bool CanRender(RenderContext context)
        {
            if (base.CanRender(context))
            {
                return !renderHost.IsDeferredLighting;
                //if (renderHost.RenderTechnique == renderHost.RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.Deferred) ||
                //    renderHost.RenderTechnique == renderHost.RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.GBuffer))
                //{
                //    return false;
                //}
                //return true;
            }
            return false;
        }

        protected override void OnRender(RenderContext context)
        {
            // --- turn-on the light            
            Light3DSceneShared.LightColors[lightIndex] = this.ColorInternal;
            // --- Set lighting parameters
            Light3DSceneShared.LightPositions[lightIndex] = this.PositionInternal.ToVector4();
            Light3DSceneShared.LightDirections[lightIndex] = this.DirectionInternal.ToVector4();
            Light3DSceneShared.LightSpots[lightIndex] = new Vector4((float)Math.Cos(this.OuterAngleInternal / 360.0 * Math.PI), (float)Math.Cos(this.InnerAngleInternal / 360.0 * Math.PI), (float)this.FalloffInternal, 0);
            Light3DSceneShared.LightAtt[lightIndex] = new Vector4((float)this.AttenuationInternal.X, (float)this.AttenuationInternal.Y, (float)this.AttenuationInternal.Z, (float)this.RangeInternal);

            // --- Update lighting variables    
            this.vLightPos.Set(Light3DSceneShared.LightPositions);
            this.vLightDir.Set(Light3DSceneShared.LightDirections);
            this.vLightSpot.Set(Light3DSceneShared.LightSpots);
            this.vLightColor.Set(Light3DSceneShared.LightColors);
            this.vLightAtt.Set(Light3DSceneShared.LightAtt);
            this.iLightType.Set(Light3DSceneShared.LightTypes);
        }
    }
}
