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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DXGI;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;

namespace HelixToolkit.Win8.CommonDX
{
    /// <summary>
    /// Target to render to a <see cref="SurfaceImageSource"/>, used to integrate
    /// DirectX content into a XAML brush.
    /// </summary>
    public class SurfaceImageSourceTarget : TargetBase
    {
        private Dictionary<IntPtr, SurfaceViewData> mapSurfaces = new Dictionary<IntPtr, SurfaceViewData>();

        private int pixelWidth;
        private int pixelHeight;
        private SurfaceImageSource surfaceImageSource;
        private ISurfaceImageSourceNative surfaceImageSourceNative;
        private DrawingPoint position;

        /// <summary>
        /// Initialzes a new <see cref="SurfaceImageSourceTarget"/> instance.
        /// </summary>
        /// <param name="pixelWidth">Width of the target in pixels</param>
        /// <param name="pixelHeight">Height of the target in pixels</param>
        public SurfaceImageSourceTarget(int pixelWidth, int pixelHeight, bool supportOpacity = false)
        {
            this.pixelWidth = pixelWidth;
            this.pixelHeight = pixelHeight;
            this.surfaceImageSource = new SurfaceImageSource(pixelWidth, pixelHeight, supportOpacity);
            surfaceImageSourceNative = ToDispose(ComObject.As<SharpDX.DXGI.ISurfaceImageSourceNative>(surfaceImageSource));
        }

        /// <summary>
        /// Gets the <see cref="SurfaceImageSource"/> to be used by brushes.
        /// </summary>
        public SurfaceImageSource ImageSource
        {
            get
            {
                return surfaceImageSource;
            }
        }

        /// <inveritdoc/>
        public override void Initialize(DeviceManager deviceManager)
        {
            base.Initialize(deviceManager);
            surfaceImageSourceNative.Device = ToDispose(DeviceManager.DeviceDirect3D.QueryInterface<SharpDX.DXGI.Device>());
        }

        /// <inveritdoc/>
        protected override Windows.Foundation.Rect CurrentControlBounds
        {
            get { 
                return new Windows.Foundation.Rect(0, 0, pixelWidth, pixelHeight); 
            }
        }

        /// <summary>
        /// Gets the relative position to use to draw on the surface.
        /// </summary>
        public DrawingPoint DrawingPosition { get { return position; } }

        /// <inveritdoc/>
        public override void RenderAll()
        {
            SurfaceViewData viewData;

            var regionToDraw = new SharpDX.Rectangle(0, 0, pixelWidth, pixelHeight);

            // Unlike other targets, we can only get the DXGI surface to render to
            // just before rendering.
            using (var surface = surfaceImageSourceNative.BeginDraw(regionToDraw, out position))
            {
                // Cache DXGI surface in order to avoid recreate all render target view, depth stencil...etc.
                // Is it the right way to do it?
                // It seems that ISurfaceImageSourceNative.BeginDraw is returning 2 different DXGI surfaces
                if (!mapSurfaces.TryGetValue(surface.NativePointer, out viewData))
                {
                    viewData = new SurfaceViewData();
                    mapSurfaces.Add(surface.NativePointer, viewData);

                    // Allocate a new renderTargetView if size is different
                    // Cache the rendertarget dimensions in our helper class for convenient use.
                    viewData.BackBuffer = ToDispose(surface.QueryInterface<SharpDX.Direct3D11.Texture2D>());
                    {
                        var desc = viewData.BackBuffer.Description;
                        viewData.RenderTargetSize = new Size(desc.Width, desc.Height);
                        viewData.RenderTargetView = ToDispose(new SharpDX.Direct3D11.RenderTargetView(DeviceManager.DeviceDirect3D, viewData.BackBuffer));
                    }

                    // Create a descriptor for the depth/stencil buffer.
                    // Allocate a 2-D surface as the depth/stencil buffer.
                    // Create a DepthStencil view on this surface to use on bind.
                    // TODO: Recreate a DepthStencilBuffer is inefficient. We should only have one depth buffer. Shared depth buffer?
                    using (var depthBuffer = new SharpDX.Direct3D11.Texture2D(DeviceManager.DeviceDirect3D, new SharpDX.Direct3D11.Texture2DDescription()
                    {
                        Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                        ArraySize = 1,
                        MipLevels = 1,
                        Width = (int)viewData.RenderTargetSize.Width,
                        Height = (int)viewData.RenderTargetSize.Height,
                        SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                        BindFlags = SharpDX.Direct3D11.BindFlags.DepthStencil,
                    }))
                        viewData.DepthStencilView = ToDispose(new SharpDX.Direct3D11.DepthStencilView(DeviceManager.DeviceDirect3D, depthBuffer, new SharpDX.Direct3D11.DepthStencilViewDescription() { Dimension = SharpDX.Direct3D11.DepthStencilViewDimension.Texture2D }));

                    // Now we set up the Direct2D render target bitmap linked to the swapchain. 
                    // Whenever we render to this bitmap, it will be directly rendered to the 
                    // swapchain associated with the window.
                    var bitmapProperties = new SharpDX.Direct2D1.BitmapProperties1(
                        new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
                        DeviceManager.Dpi,
                        DeviceManager.Dpi,
                        SharpDX.Direct2D1.BitmapOptions.Target | SharpDX.Direct2D1.BitmapOptions.CannotDraw);

                    // Direct2D needs the dxgi version of the backbuffer surface pointer.
                    // Get a D2D surface from the DXGI back buffer to use as the D2D render target.
                    viewData.BitmapTarget = ToDispose(new SharpDX.Direct2D1.Bitmap1(DeviceManager.ContextDirect2D, surface, bitmapProperties));

                    // Create a viewport descriptor of the full window size.
                    viewData.Viewport = new SharpDX.Direct3D11.Viewport(position.X, position.Y, (float)viewData.RenderTargetSize.Width - position.X, (float)viewData.RenderTargetSize.Height - position.Y, 0.0f, 1.0f);
                }

                backBuffer = viewData.BackBuffer;
                renderTargetView = viewData.RenderTargetView;
                depthStencilView = viewData.DepthStencilView;
                RenderTargetBounds = new Rect(viewData.Viewport.TopLeftX, viewData.Viewport.TopLeftY, viewData.Viewport.Width, viewData.Viewport.Height);
                bitmapTarget = viewData.BitmapTarget;

                DeviceManager.ContextDirect2D.Target = viewData.BitmapTarget;

                // Set the current viewport using the descriptor.
                DeviceManager.ContextDirect3D.Rasterizer.SetViewports(viewData.Viewport);

                // Perform the actual rendering of this target
                base.RenderAll();
            }

            surfaceImageSourceNative.EndDraw();
        }

        /// <summary>
        /// This class is used to store attached render target view to DXGI surfaces.
        /// </summary>
        class SurfaceViewData
        {
            public SharpDX.Direct3D11.Texture2D BackBuffer;
            public SharpDX.Direct3D11.RenderTargetView RenderTargetView;
            public SharpDX.Direct3D11.DepthStencilView DepthStencilView;
            public SharpDX.Direct2D1.Bitmap1 BitmapTarget;
            public SharpDX.Direct3D11.Viewport Viewport;
            public Size RenderTargetSize;
        }
    }
}
