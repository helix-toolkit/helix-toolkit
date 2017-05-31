// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DX11ImageSource.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    using global::SharpDX.Direct3D11;

    using global::SharpDX.Direct3D9;
    using System.Threading;

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

    internal class DX11ImageSource : D3DImage, IDisposable
    {
        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();

        private static long activeClients;
        private static Direct3DEx context;
        private static DeviceEx device;

        private readonly int adapterIndex;
        private Texture renderTarget;

        public DX11ImageSource(int adapterIndex = 0)
        {
            this.adapterIndex = adapterIndex;        
            this.StartD3D();
            Interlocked.Increment(ref activeClients);
            
        }

        public void Dispose()
        {
            this.SetRenderTargetDX11(null);
            Interlocked.Decrement(ref activeClients);
            this.EndD3D();
        }

        public void InvalidateD3DImage()
        {
            if (this.renderTarget != null)
            {
                base.Lock();
                base.AddDirtyRect(new Int32Rect(0, 0, base.PixelWidth, base.PixelHeight));
                base.Unlock();
            }
        }

        public void SetRenderTargetDX11(Texture2D target)
        {
            if (this.renderTarget != null)
            {
                Disposer.RemoveAndDispose(ref this.renderTarget);
                base.Lock();
                base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                base.Unlock();
            }

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
                this.renderTarget = new Texture(DX11ImageSource.device, target.Description.Width, target.Description.Height, 1, Usage.RenderTarget, format, Pool.Default, ref handle);            
                using (Surface surface = this.renderTarget.GetSurfaceLevel(0))                
                {
                    base.Lock();
#if NET40
                    base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
#else
                    // "enableSoftwareFallback = true" makes Remote Desktop possible.
                    // See: http://msdn.microsoft.com/en-us/library/hh140978%28v=vs.110%29.aspx
                    base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer, true);
#endif
                    base.Unlock();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void StartD3D()
        {
            if (activeClients != 0)
                return;

            context = new Direct3DEx();

            var presentparams = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                DeviceWindowHandle = GetDesktopWindow(),
                PresentationInterval = PresentInterval.Default,    
            };
                        
            device = new DeviceEx(context, this.adapterIndex, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve, presentparams);
        }

        private void EndD3D()
        {
            Disposer.RemoveAndDispose(ref this.renderTarget);
            if (Interlocked.Read(ref activeClients) != 0)
                return;

            Disposer.RemoveAndDispose(ref device);
            Disposer.RemoveAndDispose(ref context);
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
    }
}
