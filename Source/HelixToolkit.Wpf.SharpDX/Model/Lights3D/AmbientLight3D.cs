namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows;    

    using global::SharpDX.Direct3D11;

    public sealed class AmbientLight3D : Light3D
    {        
        private EffectVectorVariable vLightAmbient;

        public AmbientLight3D()
        {            
            this.Color = new global::SharpDX.Color4(0.2f, 0.2f, 0.2f, 1f);
            this.LightType = LightType.Ambient;
        }

        public override void Attach(IRenderHost host)
        {
            /// --- attach
            base.Attach(host);

            /// --- light constant params              
            this.vLightAmbient = this.effect.GetVariableByName("vLightAmbient").AsVector();
            this.vLightAmbient.Set(this.Color);            

            /// --- flush
            this.Device.ImmediateContext.Flush();            
        }

        public override void Detach()
        {
            if (this.vLightAmbient != null)
            {
                this.vLightAmbient.Set(new global::SharpDX.Color4(0, 0, 0, 0));
                Disposer.RemoveAndDispose(ref this.vLightAmbient);
            }
            base.Detach();
        }

        protected override void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsAttached)
            {
                this.vLightAmbient.Set(this.Color);
            }
        }
    }
}