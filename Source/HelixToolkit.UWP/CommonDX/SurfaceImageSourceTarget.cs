// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SurfaceImageSourceTarget.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Target to render to a SurfaceImageSource, used to integrate
//   DirectX content into a XAML brush.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DXGI;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;

namespace HelixToolkit.UWP.CommonDX
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
        private global::SharpDX.Point position;

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
            surfaceImageSourceNative = Collect(ComObject.As<global::SharpDX.DXGI.ISurfaceImageSourceNative>(surfaceImageSource));
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
            surfaceImageSourceNative.Device = Collect(DeviceManager.DeviceDirect3D.QueryInterface<global::SharpDX.DXGI.Device>());
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
        public global::SharpDX.Point DrawingPosition { get { return position; } }

        /// <inveritdoc/>
        public override void RenderAll()
        {
            SurfaceViewData viewData;

            var regionToDraw = new Rectangle(0, 0, pixelWidth, pixelHeight);

            // Unlike other targets, we can only get the DXGI surface to render to
            // just before rendering.

            global::SharpDX.Mathematics.Interop.RawPoint rawPoint;

            using (var surface = surfaceImageSourceNative.BeginDraw(regionToDraw, out rawPoint))
            {
                position = rawPoint;

                // Cache DXGI surface in order to avoid recreate all render target view, depth stencil...etc.
                // Is it the right way to do it?
                // It seems that ISurfaceImageSourceNative.BeginDraw is returning 2 different DXGI surfaces
                if (!mapSurfaces.TryGetValue(surface.NativePointer, out viewData))
                {
                    viewData = new SurfaceViewData();
                    mapSurfaces.Add(surface.NativePointer, viewData);

                    // Allocate a new renderTargetView if size is different
                    // Cache the rendertarget dimensions in our helper class for convenient use.
                    viewData.BackBuffer = Collect(surface.QueryInterface<global::SharpDX.Direct3D11.Texture2D>());
                    {
                        var desc = viewData.BackBuffer.Description;
                        viewData.RenderTargetSize = new Size(desc.Width, desc.Height);
                        viewData.RenderTargetView = Collect(new global::SharpDX.Direct3D11.RenderTargetView(DeviceManager.DeviceDirect3D, viewData.BackBuffer));
                    }

                    // Create a descriptor for the depth/stencil buffer.
                    // Allocate a 2-D surface as the depth/stencil buffer.
                    // Create a DepthStencil view on this surface to use on bind.
                    // TODO: Recreate a DepthStencilBuffer is inefficient. We should only have one depth buffer. Shared depth buffer?
                    using (var depthBuffer = new global::SharpDX.Direct3D11.Texture2D(DeviceManager.DeviceDirect3D, new global::SharpDX.Direct3D11.Texture2DDescription()
                    {
                        Format = global::SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                        ArraySize = 1,
                        MipLevels = 1,
                        Width = (int)viewData.RenderTargetSize.Width,
                        Height = (int)viewData.RenderTargetSize.Height,
                        SampleDescription = new global::SharpDX.DXGI.SampleDescription(1, 0),
                        BindFlags = global::SharpDX.Direct3D11.BindFlags.DepthStencil,
                    }))
                        viewData.DepthStencilView = Collect(new global::SharpDX.Direct3D11.DepthStencilView(DeviceManager.DeviceDirect3D, depthBuffer, new global::SharpDX.Direct3D11.DepthStencilViewDescription() { Dimension = global::SharpDX.Direct3D11.DepthStencilViewDimension.Texture2D }));

                    // Now we set up the Direct2D render target bitmap linked to the swapchain. 
                    // Whenever we render to this bitmap, it will be directly rendered to the 
                    // swapchain associated with the window.
                    var bitmapProperties = new global::SharpDX.Direct2D1.BitmapProperties1(
                        new global::SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, global::SharpDX.Direct2D1.AlphaMode.Premultiplied),
                        DeviceManager.Dpi,
                        DeviceManager.Dpi,
                        global::SharpDX.Direct2D1.BitmapOptions.Target | global::SharpDX.Direct2D1.BitmapOptions.CannotDraw);

                    // Direct2D needs the dxgi version of the backbuffer surface pointer.
                    // Get a D2D surface from the DXGI back buffer to use as the D2D render target.
                    viewData.BitmapTarget = Collect(new global::SharpDX.Direct2D1.Bitmap1(DeviceManager.ContextDirect2D, surface, bitmapProperties));

                    // Create a viewport descriptor of the full window size.
                    viewData.Viewport = new global::SharpDX.ViewportF(position.X, position.Y, (float)viewData.RenderTargetSize.Width - position.X, (float)viewData.RenderTargetSize.Height - position.Y, 0.0f, 1.0f);
                }

                backBuffer = viewData.BackBuffer;
                renderTargetView = viewData.RenderTargetView;
                depthStencilView = viewData.DepthStencilView;
                RenderTargetBounds = new Rect(viewData.Viewport.X, viewData.Viewport.Y, viewData.Viewport.Width, viewData.Viewport.Height);
                bitmapTarget = viewData.BitmapTarget;

                DeviceManager.ContextDirect2D.Target = viewData.BitmapTarget;

                // Set the current viewport using the descriptor.
                DeviceManager.ContextDirect3D.Rasterizer.SetViewport(viewData.Viewport);

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
            public global::SharpDX.Direct3D11.Texture2D BackBuffer;
            public global::SharpDX.Direct3D11.RenderTargetView RenderTargetView;
            public global::SharpDX.Direct3D11.DepthStencilView DepthStencilView;
            public global::SharpDX.Direct2D1.Bitmap1 BitmapTarget;
            public global::SharpDX.ViewportF Viewport;
            public Size RenderTargetSize;
        }
    }
}