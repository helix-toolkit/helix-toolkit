namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows;

    using global::SharpDX;

    public sealed class SpotLight3D : PointLightBase3D
    {
        public static readonly DependencyProperty FalloffProperty =
            DependencyProperty.Register("Falloff", typeof(double), typeof(SpotLight3D), new UIPropertyMetadata(1.0));
        
        public static readonly DependencyProperty InnerAngleProperty =
            DependencyProperty.Register("InnerAngle", typeof(double), typeof(SpotLight3D), new UIPropertyMetadata(5.0));
        
        public static readonly DependencyProperty OuterAngleProperty =
            DependencyProperty.Register("OuterAngle", typeof(double), typeof(SpotLight3D), new UIPropertyMetadata(45.0));


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
        
        /// <summary>
        /// Full outer angle of the spot (Phi) in degrees
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
        /// </summary>
        public double OuterAngle
        {
            get { return (double)this.GetValue(OuterAngleProperty); }
            set { this.SetValue(OuterAngleProperty, value); }
        }
        
        /// <summary>
        /// Full inner angle of the spot (Theta) in degrees. 
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
        /// </summary>
        public double InnerAngle
        {
            get { return (double)this.GetValue(InnerAngleProperty); }
            set { this.SetValue(InnerAngleProperty, value); }
        }
       


        public SpotLight3D()
        {            
            this.LightType = LightType.Spot;
        }
        
        public override void Attach(IRenderHost host)
        {
            /// --- attach
            base.Attach(host);

            /// --- light constant params            
            this.vLightPos = this.effect.GetVariableByName("vLightPos").AsVector();
            this.vLightDir = this.effect.GetVariableByName("vLightDir").AsVector();
            this.vLightSpot = this.effect.GetVariableByName("vLightSpot").AsVector();
            this.vLightColor = this.effect.GetVariableByName("vLightColor").AsVector();
            this.vLightAtt = this.effect.GetVariableByName("vLightAtt").AsVector();            
            this.iLightType = this.effect.GetVariableByName("iLightType").AsScalar();            
                       
            /// --- Set light type
            lightTypes[lightIndex] = (int)Light3D.Type.Spot;   

            /// --- flush
            this.Device.ImmediateContext.Flush();
        }

        public override void Detach()
        {
            Disposer.RemoveAndDispose(ref this.vLightPos);
            Disposer.RemoveAndDispose(ref this.vLightDir);
            Disposer.RemoveAndDispose(ref this.vLightSpot);
            Disposer.RemoveAndDispose(ref this.vLightColor);            
            Disposer.RemoveAndDispose(ref this.vLightAtt);
            Disposer.RemoveAndDispose(ref this.iLightType);
            base.Detach();
        }

        public override void Render(RenderContext context)
        {
#if DEFERRED  
            if (renderHost.RenderTechnique == Techniques.RenderDeferred || renderHost.RenderTechnique == Techniques.RenderGBuffer)
            {
                return;
            }
#endif

            if (this.IsRendering)
            {
                /// --- turn-on the light            
                lightColors[lightIndex] = this.Color;
            }
            else
            {
                // --- turn-off the light
                lightColors[lightIndex] = new global::SharpDX.Color4(0, 0, 0, 0);
            }
          
            /// --- Set lighting parameters
            lightPositions[lightIndex] = this.Position.ToVector4();
            lightDirections[lightIndex] = this.Direction.ToVector4();
            lightSpots[lightIndex] = new Vector4((float)Math.Cos(this.OuterAngle / 360.0 * Math.PI), (float)Math.Cos(this.InnerAngle / 360.0 * Math.PI), (float)this.Falloff, 0);            
            lightAtt[lightIndex] = new Vector4((float)this.Attenuation.X, (float)this.Attenuation.Y, (float)this.Attenuation.Z, (float)this.Range);

            /// --- Update lighting variables    
            this.vLightPos.Set(lightPositions);
            this.vLightDir.Set(lightDirections);
            this.vLightSpot.Set(lightSpots);
            this.vLightColor.Set(lightColors);
            this.vLightAtt.Set(lightAtt);
            this.iLightType.Set(lightTypes);
        }
    }
}
