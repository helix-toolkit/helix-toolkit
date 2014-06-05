// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Viewport3DX.cs" company="Helix Toolkit">
//   
// </copyright>
// <summary>
//   Renders the contained 3-D content within the 2-D layout bounds of the Viewport3DX element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Win8
{
    using System.Diagnostics;

    using HelixToolkit.Win8.CommonDX;

    using Windows.Graphics.Display;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Renders the contained 3-D content within the 2-D layout bounds of the Viewport3DX element.
    /// </summary>
    [ContentProperty(Name = "Items")]
    public sealed class Viewport3DX : ItemsControl
    {
        /// <summary>
        /// The image source target
        /// </summary>
        private SurfaceImageSourceTarget d3dTarget;

        /// <summary>
        /// The device manager
        /// </summary>
        private DeviceManager deviceManager;

        /// <summary>
        /// The image brush
        /// </summary>
        private ImageBrush imageBrush;

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport3DX" /> class.
        /// </summary>
        public Viewport3DX()
        {
            this.DefaultStyleKey = typeof(Viewport3DX);
            this.Loaded += this.Viewport3DXLoaded;
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            Debug.WriteLine("Pointer pressed.");
        }

        /// <summary>
        /// Called before the ManipulationStarted event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationStarted(Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);
            Debug.WriteLine("Manipulation started.");
        }

        /// <summary>
        /// Renders all content on the CompositionTarget.Rendering event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void CompositionTargetRendering(object sender, object e)
        {
            this.d3dTarget.RenderAll();
        }

        /// <summary>
        /// Changes the dpi of the device manager when the DisplayProperties.LogicalDpi has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void DisplayPropertiesLogicalDpiChanged(object sender)
        {
            this.deviceManager.Dpi = DisplayProperties.LogicalDpi;
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <param name="deviceManager">The device manager.</param>
        private void Initialize(DeviceManager deviceManager)
        {
            foreach (Element3D e in this.Items)
            {
                e.Initialize(deviceManager);
            }
        }

        /// <summary>
        /// Renders the model.
        /// </summary>
        /// <param name="render">The render.</param>
        private void Render(TargetBase render)
        {
            foreach (Element3D e in this.Items)
            {
                e.Render(render);
            }
        }

        /// <summary>
        /// Creates the device manager and image source when the viewport is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Viewport3DXLoaded(object sender, RoutedEventArgs e)
        {
            int pixelWidth = (int)(this.ActualWidth * DisplayProperties.LogicalDpi / 96.0);
            int pixelHeight = (int)(this.ActualHeight * DisplayProperties.LogicalDpi / 96.0);

            // Safely dispose any previous instance
            // Creates a new DeviceManager (Direct3D, Direct2D, DirectWrite, WIC)
            this.deviceManager = new DeviceManager();

            // Use CoreWindowTarget as the rendering target (Initialize SwapChain, RenderTargetView, DepthStencilView, BitmapTarget)
            this.d3dTarget = new SurfaceImageSourceTarget(pixelWidth, pixelHeight);
            this.imageBrush = new ImageBrush();
            this.imageBrush.ImageSource = this.d3dTarget.ImageSource;
            this.Background = this.imageBrush;

            this.deviceManager.OnInitialize += this.d3dTarget.Initialize;
            this.deviceManager.OnInitialize += this.Initialize;

            this.d3dTarget.OnRender += this.Render;

            // Initialize the device manager and all registered deviceManager.OnInitialize 
            this.deviceManager.Initialize(DisplayProperties.LogicalDpi);

            // Setup rendering callback
            CompositionTarget.Rendering += this.CompositionTargetRendering;

            // Callback on DpiChanged
            DisplayProperties.LogicalDpiChanged += this.DisplayPropertiesLogicalDpiChanged;
        }
    }
}