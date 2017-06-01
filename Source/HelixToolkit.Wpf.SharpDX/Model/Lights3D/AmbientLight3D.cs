// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach
            if (base.OnAttach(host))
            {
                // --- light constant params              
                this.vLightAmbient = this.effect.GetVariableByName("vLightAmbient").AsVector();
                this.vLightAmbient.Set(this.ColorInternal);

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
            if (this.vLightAmbient != null)
            {
                if(!this.effect.IsDisposed)
                    this.vLightAmbient.Set(new global::SharpDX.Color4(0, 0, 0, 0));
                Disposer.RemoveAndDispose(ref this.vLightAmbient);
            }
            base.OnDetach();
        }

        protected override void OnColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsAttached)
            {
                this.vLightAmbient.Set(this.ColorInternal);
            }
        }
    }
}