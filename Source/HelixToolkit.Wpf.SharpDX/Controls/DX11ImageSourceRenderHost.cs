using System;
using global::SharpDX.Direct3D11;

#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#else
using Device = SharpDX.Direct3D11.Device;
#endif
namespace HelixToolkit.Wpf.SharpDX.Controls
{

    using Render;
    using System.Diagnostics;

    public sealed class DX11ImageSourceArgs : EventArgs
    {
        public readonly DX11ImageSource Source;
        public DX11ImageSourceArgs(DX11ImageSource source)
        {
            Source = source;
        }
    }

    public sealed class DX11ImageSourceRenderHost : DefaultRenderHost
    {
        public event EventHandler<DX11ImageSourceArgs> OnImageSourceChanged;

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
            surfaceD3D?.InvalidateD3DImage();
            base.PostRender();
        }

        protected override void DisposeBuffers()
        {
            RemoveAndDispose(ref surfaceD3D);
            base.DisposeBuffers();
        }

        private void DX11ImageSourceRenderer_OnNewBufferCreated(object sender, Texture2DArgs e)
        {
            if (surfaceD3D == null)
            {
                Debug.WriteLine("Create new D3DImageSource");
                surfaceD3D = Collect(new DX11ImageSource(EffectsManager.AdapterIndex));
            }
            surfaceD3D.SetRenderTargetDX11(e);
            OnImageSourceChanged(this, new DX11ImageSourceArgs(surfaceD3D));
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            OnImageSourceChanged = null;
            surfaceD3D?.SetRenderTargetDX11(null);
            base.OnDispose(disposeManagedResources);
        }
    }
}
