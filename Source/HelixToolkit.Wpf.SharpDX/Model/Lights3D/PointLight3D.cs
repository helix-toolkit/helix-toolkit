namespace HelixToolkit.Wpf.SharpDX
{    
    using global::SharpDX;

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
            lightTypes[lightIndex] = (int)Light3D.Type.Point;   

            /// --- flush
            this.Device.ImmediateContext.Flush();
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
            lightAtt[lightIndex] = new Vector4((float)this.Attenuation.X, (float)this.Attenuation.Y, (float)this.Attenuation.Z, (float)this.Range);

            /// --- Update lighting variables    
            this.vLightPos.Set(lightPositions);
            this.vLightColor.Set(lightColors);
            this.vLightAtt.Set(lightAtt);
            this.iLightType.Set(lightTypes);
        }
    }
}