// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DX11ImageSource.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using global::SharpDX.Direct3D11;
using global::SharpDX.Direct3D9;
using System.Diagnostics.CodeAnalysis;
#if COREWPF
using HelixToolkit.SharpDX.Core;
#endif
namespace HelixToolkit.Wpf.SharpDX
{
    // Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
    // 
    // Permission is hereby granted, free of charge, to any person obtaining a copy
    // of this software and associated documentation files (the "Software"), to deal
    // in the Software without restriction, including without limitation the rights
    // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // copies of the Software, and to permit persons to whom the Software is
    // furnished to do so, subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in
    // all copies or substantial portions of the Software.
    // 
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    // THE SOFTWARE.

    public sealed class DX11ImageSource : D3DImage, IDisposable
    {
        private Direct3DEx context;
        private DeviceEx device;

        private readonly int adapterIndex;
        private Texture renderTarget;
        private Surface surface;

        public DX11ImageSource(int adapterIndex = 0)
        {
            this.adapterIndex = adapterIndex;        
            this.StartD3D();
            
        }

        public void InvalidateD3DImage()
        {
            if (this.renderTarget != null)
            {
                base.Lock();
#if NET40
                base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
#else
                // "enableSoftwareFallback = true" makes Remote Desktop possible.
                // See: http://msdn.microsoft.com/en-us/library/hh140978%28v=vs.110%29.aspx
                base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer, true);
#endif
                base.AddDirtyRect(new Int32Rect(0, 0, base.PixelWidth, base.PixelHeight));
                base.Unlock();
            }
        }

        public void SetRenderTargetDX11(Texture2D target)
        {
            EndD3D(false);
            if (target == null)
                return;

            if (!IsShareable(target))
                throw new ArgumentException("Texture must be created with ResourceOptionFlags.Shared");

            var format = TranslateFormat(target);
            if (format == Format.Unknown)
                throw new ArgumentException("Texture format is not compatible with OpenSharedResource");

            var handle = GetSharedHandle(target);
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException("Handle");
                       
            try
            {
                this.renderTarget = new Texture(device, target.Description.Width, target.Description.Height, 1, Usage.RenderTarget, format, Pool.Default, ref handle);
                surface = this.renderTarget.GetSurfaceLevel(0);
                base.Lock();
#if NET40
                base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
#else
                // "enableSoftwareFallback = true" makes Remote Desktop possible.
                // See: http://msdn.microsoft.com/en-us/library/hh140978%28v=vs.110%29.aspx
                base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer, true);
#endif
                base.AddDirtyRect(new Int32Rect(0, 0, base.PixelWidth, base.PixelHeight));
                base.Unlock();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void StartD3D()
        {
            context = new Direct3DEx();
            // Ref: https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/wpf-and-direct3d9-interoperation
            var presentparams = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                //DeviceWindowHandle = GetDesktopWindow(),
                PresentationInterval = PresentInterval.Default,
                BackBufferHeight = 1, BackBufferWidth = 1, BackBufferFormat = Format.Unknown
            };
                        
            device = new DeviceEx(context, this.adapterIndex, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve, presentparams);
        }

        private void EndD3D(bool disposeDevices)
        {
            base.Lock();
            base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            base.Unlock();
            Disposer.RemoveAndDispose(ref surface);
            Disposer.RemoveAndDispose(ref renderTarget);
            if (disposeDevices)
            {
                Disposer.RemoveAndDispose(ref device);
                Disposer.RemoveAndDispose(ref context);
            }           
        }

        private static IntPtr GetSharedHandle(Texture2D sharedTexture)
        {
            using (var resource = sharedTexture.QueryInterface<global::SharpDX.DXGI.Resource>())
            {
                IntPtr result = resource.SharedHandle;                
                return result;
            }
        }

        private static Format TranslateFormat(Texture2D sharedTexture)
        {
            switch (sharedTexture.Description.Format)
            {
                case global::SharpDX.DXGI.Format.R10G10B10A2_UNorm:
                    return Format.A2B10G10R10;

                case global::SharpDX.DXGI.Format.R16G16B16A16_Float:
                    return Format.A16B16G16R16F;

                case global::SharpDX.DXGI.Format.B8G8R8A8_UNorm:
                    return Format.A8R8G8B8;

                default:
                    return Format.Unknown;
            }
        }

        private static bool IsShareable(Texture2D sharedTexture)
        {
            return (sharedTexture.Description.OptionFlags & ResourceOptionFlags.Shared) != 0;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "False positive.")]
        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EndD3D(true);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DX11ImageSource() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

    public static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();
    }
}
