using System;

namespace HelixToolkit.Wpf.SharpDX.Controls
{
    using global::SharpDX.Direct3D11;
    using Render;
    public sealed class DX11ImageSourceRenderHost : DefaultRenderHost
    {
        public event EventHandler<DX11ImageSource> OnImageSourceChanged;

        private DX11ImageSource surfaceD3D;

        public DX11ImageSourceRenderHost(Func<Device, IRenderer> createRenderer) : base(createRenderer)
        {
            this.OnNewRenderTargetTexture += DX11ImageSourceRenderer_OnNewBufferCreated;
        }

        public DX11ImageSourceRenderHost()
        {
            this.OnNewRenderTargetTexture += DX11ImageSourceRenderer_OnNewBufferCreated;
        }

        protected override void PreRender()
        {
            base.PreRender();
        }

        protected override void PostRender()
        {
            surfaceD3D.InvalidateD3DImage();
            base.PostRender();
        }

        private void DX11ImageSourceRenderer_OnNewBufferCreated(object sender, Texture2D e)
        {
            if (surfaceD3D == null)
            {
                surfaceD3D = Collect(new DX11ImageSource(EffectsManager.AdapterIndex));
            }
            surfaceD3D.SetRenderTargetDX11(e);
            OnImageSourceChanged(this, surfaceD3D);
        }
    }
}
