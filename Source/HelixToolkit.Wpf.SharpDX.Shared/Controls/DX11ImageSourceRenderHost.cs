using System;
using global::SharpDX.Direct3D11;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

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
            static readonly ILogger logger = Logger.LogManager.Create<DX11ImageSourceRenderHost>();
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
                    logger.LogWarning("Back buffer is not set.");
                    return;
                }
                surfaceD3D?.InvalidateD3DImage();
                base.PostRender();
            }

            protected override void DisposeBuffers()
            {
                logger.LogInformation("Dispose buffers.");
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
                try
                {
                    if (surfaceD3D == null)
                    {
                        logger.LogInformation("Create new D3DImageSource");
                        surfaceD3D = new DX11ImageSource(EffectsManager.AdapterIndex);
                        surfaceD3D.IsFrontBufferAvailableChanged += SurfaceD3D_IsFrontBufferAvailableChanged;
                    }
                    surfaceD3D.SetRenderTargetDX11(e.Texture.Resource as Texture2D);
                }
                catch (Exception ex)
                {
                    logger.LogError("Failed to create surfaceD3D. Ex: {}", ex.Message);
                    hasBackBuffer = false;
                    surfaceD3D.IsFrontBufferAvailableChanged -= SurfaceD3D_IsFrontBufferAvailableChanged;
                    RemoveAndDispose(ref surfaceD3D);
                    hasBackBuffer = false;
                    EndD3D();
                    ReinitializeEffectsManager();
                    return;
                }
                hasBackBuffer = e.Texture.Resource is Texture2D;
                OnImageSourceChanged(this, new DX11ImageSourceArgs(surfaceD3D));
                if (hasBackBuffer)
                {
                    logger.LogInformation("New back buffer is set.");
                }
                else
                {
                    logger.LogInformation("Set back buffer failed.");
                }
            }

            private bool lastSurfaceD3DIsFrontBufferAvailable;
            private void SurfaceD3D_IsFrontBufferAvailableChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
            {
                var newValue = (bool)(e.NewValue);
                if (EffectsManager == null || newValue == lastSurfaceD3DIsFrontBufferAvailable)
                {
                    return;
                }

                logger.LogWarning("SurfaceD3D front buffer changed. Value = {}, last value {}", newValue, lastSurfaceD3DIsFrontBufferAvailable);
                if (surfaceD3D != null)
                {
                    hasBackBuffer = false;
                    surfaceD3D.SetRenderTargetDX11(null);
                    surfaceD3D.IsFrontBufferAvailableChanged -= SurfaceD3D_IsFrontBufferAvailableChanged;
                    RemoveAndDispose(ref surfaceD3D);
                }
                if (newValue)
                {
                    frontBufferChange = false;
                    try
                    {

                        if (EffectsManager.Device.DeviceRemovedReason == global::SharpDX.Result.Ok)
                        {
                            Restart(true);
                        }
                        else
                        {
                            EndD3D();
                            ReinitializeEffectsManager();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }
                }
                else
                {
                    frontBufferChange = true;
                    if (EffectsManager.Device.DeviceRemovedReason != global::SharpDX.Result.Ok)
                    {
                        hasBackBuffer = false;
                        EndD3D();
                    }
                }

                lastSurfaceD3DIsFrontBufferAvailable = newValue;
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
