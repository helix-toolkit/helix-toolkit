using System;
using global::SharpDX.Direct3D11;
using System.Diagnostics;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#else
using Device = SharpDX.Direct3D11.Device;
#endif
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Render;
#endif

namespace HelixToolkit.Wpf.SharpDX
{
#if !COREWPF
    using Render;
#endif

    namespace Controls
    {
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

            private bool frontBufferChange = false;
            private bool hasBackBuffer = false;

            public DX11ImageSourceRenderHost(Func<IDevice3DResources, IRenderer> createRenderer) : base(createRenderer)
            {
                this.OnNewRenderTargetTexture += DX11ImageSourceRenderer_OnNewBufferCreated;
            }

            public DX11ImageSourceRenderHost()
            {
                this.OnNewRenderTargetTexture += DX11ImageSourceRenderer_OnNewBufferCreated;
            }

            protected override void PostRender()
            {
                if (!hasBackBuffer)
                {
                    Logger.Log(HelixToolkit.Logger.LogLevel.Warning, $"Back buffer is not set.");
                    return;
                }
                surfaceD3D?.InvalidateD3DImage();
                base.PostRender();
            }

            protected override void DisposeBuffers()
            {
                Logger.Log(HelixToolkit.Logger.LogLevel.Information, $"Dispose buffers.");
                if (surfaceD3D != null)
                {
                    hasBackBuffer = false;
                    surfaceD3D.SetRenderTargetDX11(null);
                    if (!frontBufferChange)
                    {
                        surfaceD3D.IsFrontBufferAvailableChanged -= SurfaceD3D_IsFrontBufferAvailableChanged;
                    }
                    RemoveAndDispose(ref surfaceD3D);
                }
                base.DisposeBuffers();
            }

            private void DX11ImageSourceRenderer_OnNewBufferCreated(object sender, Texture2DArgs e)
            {
                if (surfaceD3D == null)
                {
                    Debug.WriteLine("Create new D3DImageSource");
                    surfaceD3D = Collect(new DX11ImageSource(EffectsManager.AdapterIndex));
                    surfaceD3D.IsFrontBufferAvailableChanged += SurfaceD3D_IsFrontBufferAvailableChanged;
                }
                surfaceD3D.SetRenderTargetDX11(e.Texture.Resource as Texture2D);
                hasBackBuffer = e.Texture.Resource is Texture2D;
                OnImageSourceChanged(this, new DX11ImageSourceArgs(surfaceD3D));
                if (hasBackBuffer)
                { 
                    Logger.Log(HelixToolkit.Logger.LogLevel.Information, $"New back buffer is set.");
                }
                else
                {
                    Logger.Log(HelixToolkit.Logger.LogLevel.Information, $"Set back buffer failed.");
                }
            }

            private void SurfaceD3D_IsFrontBufferAvailableChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
            {
                if (EffectsManager == null)
                {
                    return;
                }
                Logger.Log(HelixToolkit.Logger.LogLevel.Warning, $"SurfaceD3D front buffer changed. Value = {(bool)e.NewValue}");
                if ((bool)e.NewValue)
                {
                    frontBufferChange = false;
                    try
                    {
                        if (EffectsManager.Device.DeviceRemovedReason.Success)
                        {
                            StartRendering();
                        }
                        else
                        {
                            EndD3D();
                            if (surfaceD3D != null)
                            {
                                hasBackBuffer = false;
                                surfaceD3D.SetRenderTargetDX11(null);
                                surfaceD3D.IsFrontBufferAvailableChanged -= SurfaceD3D_IsFrontBufferAvailableChanged;
                                RemoveAndDispose(ref surfaceD3D);
                            }

                            EffectsManager.DisposeAllResources();
                            EffectsManager.Reinitialize();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(HelixToolkit.Logger.LogLevel.Error, ex.Message);
                    }
                }
                else
                {
                    frontBufferChange = true;
                    if (EffectsManager.Device.DeviceRemovedReason.Success)
                    {
                        //StopRendering();
                    }
                    else
                    {
                        hasBackBuffer = false;
                        surfaceD3D?.SetRenderTargetDX11(null);
                        EndD3D();
                    }
                }
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                OnImageSourceChanged = null;
                if (surfaceD3D != null)
                {
                    hasBackBuffer = false;
                    surfaceD3D?.SetRenderTargetDX11(null);
                    surfaceD3D.IsFrontBufferAvailableChanged -= SurfaceD3D_IsFrontBufferAvailableChanged;
                    RemoveAndDispose(ref surfaceD3D);
                }
                base.OnDispose(disposeManagedResources);
            }
        }
    }
}
