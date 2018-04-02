// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Viewport3DX.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Renders the contained 3-D content within the 2-D layout bounds of the Viewport3DX element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using HelixToolkit.UWP.Cameras;
    using HelixToolkit.UWP.CommonDX;
    using HelixToolkit.UWP.Model.Scene;
    using HelixToolkit.UWP.Model.Scene2D;
    using SharpDX;
    using Windows.Graphics.Display;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml.Media;
    using System.Linq;

    /// <summary>
    /// Renders the contained 3-D content within the 2-D layout bounds of the Viewport3DX element.
    /// </summary>
    [ContentProperty(Name = "Items")]
    public partial class Viewport3DX : ItemsControl, IViewport3DX
    {

        ///// <summary>
        ///// The image source target
        ///// </summary>
        //private SwapChainTarget d3dTarget;

        ///// <summary>
        ///// The device manager
        ///// </summary>
        //private DeviceManager deviceManager;

        ///// <summary>
        ///// The image brush
        ///// </summary>
        //private ImageBrush imageBrush;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="Viewport3DX" /> class.
        ///// </summary>
        //public Viewport3DX()
        //{
        //    this.DefaultStyleKey = typeof(Viewport3DX);
        //    this.Loaded += this.Viewport3DXLoaded;
        //}

        ///// <summary>
        ///// Called before the PointerPressed event occurs.
        ///// </summary>
        ///// <param name="e">Event data for the event.</param>
        //protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    base.OnPointerPressed(e);
        //    Debug.WriteLine("Pointer pressed.");
        //}

        ///// <summary>
        ///// Called before the ManipulationStarted event occurs.
        ///// </summary>
        ///// <param name="e">Event data for the event.</param>
        //protected override void OnManipulationStarted(Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        //{
        //    base.OnManipulationStarted(e);
        //    Debug.WriteLine("Manipulation started.");
        //}

        ///// <summary>
        ///// Renders all content on the CompositionTarget.Rendering event.
        ///// </summary>
        ///// <param name="sender">The sender.</param>
        ///// <param name="e">The event args.</param>
        //private void CompositionTargetRendering(object sender, object e)
        //{
        //    this.d3dTarget.RenderAll();
        //}

        ///// <summary>
        ///// Changes the dpi of the device manager when the DisplayProperties.LogicalDpi has changed.
        ///// </summary>
        ///// <param name="sender">The sender.</param>
        //private void DisplayPropertiesLogicalDpiChanged(object sender)
        //{
        //    this.deviceManager.Dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
        //}

        ///// <summary>
        ///// Initializes the model.
        ///// </summary>
        ///// <param name="deviceManager">The device manager.</param>
        //private void Initialize(DeviceManager deviceManager)
        //{
        //    foreach (Element3D e in this.Items)
        //    {
        //        e.Initialize(deviceManager);
        //    }
        //}

        ///// <summary>
        ///// Renders the model.
        ///// </summary>
        ///// <param name="render">The render.</param>
        //private void Render(TargetBase render)
        //{
        //    foreach (Element3D e in this.Items)
        //    {
        //        e.Render(render);
        //    }
        //}

        ///// <summary>
        ///// Creates the device manager and image source when the viewport is loaded.
        ///// </summary>
        ///// <param name="sender">The sender.</param>
        ///// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        //private void Viewport3DXLoaded(object sender, RoutedEventArgs e)
        //{
        //    var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
        //    int pixelWidth = (int)(this.ActualWidth * logicalDpi / 96.0);
        //    int pixelHeight = (int)(this.ActualHeight * logicalDpi / 96.0);

        //    // Safely dispose any previous instance
        //    // Creates a new DeviceManager (Direct3D, Direct2D, DirectWrite, WIC)
        //    this.deviceManager = new DeviceManager();

        //    // Use CoreWindowTarget as the rendering target (Initialize SwapChain, RenderTargetView, DepthStencilView, BitmapTarget)
        //    this.d3dTarget = new SwapChainTarget((SwapChainPanel)this.ItemsPanelRoot, pixelWidth, pixelHeight);

        //    this.deviceManager.OnInitialize += this.d3dTarget.Initialize;
        //    this.deviceManager.OnInitialize += this.Initialize;

        //    this.d3dTarget.OnRender += this.Render;

        //    // Initialize the device manager and all registered deviceManager.OnInitialize 
        //    this.deviceManager.Initialize(DisplayInformation.GetForCurrentView().LogicalDpi);

        //    // Setup rendering callback
        //    CompositionTarget.Rendering += this.CompositionTargetRendering;

        //    // Callback on DpiChanged
        //    DisplayProperties.LogicalDpiChanged += this.DisplayPropertiesLogicalDpiChanged;
        //}
        public IRenderHost RenderHost { get { return this.renderHostInternal; } }

        public CameraCore CameraCore { get { return this.Camera; } }

        public SharpDX.Matrix WorldMatrix { get; } = SharpDX.Matrix.Identity;

        public IEnumerable<SceneNode> Renderables
        {
            get
            {
                foreach(Element3D item in Items)
                {
                    yield return item.SceneNode;
                }
            }
        }

        public IEnumerable<SceneNode2D> D2DRenderables
        {
            get
            {
                return Enumerable.Empty<SceneNode2D>();
            }
        }

        private bool IsAttached = false;

        public Viewport3DX()
        {
            this.DefaultStyleKey = typeof(Viewport3DX);
            this.Loaded += Viewport3DXLoaded;
            this.Unloaded += Viewport3DX_Unloaded;
            Camera = new PerspectiveCamera() { Position = new Vector3(0, 0, -10), LookDirection = new Vector3(0, 0, 10), UpDirection = new Vector3(0, 1, 0) };

        }

        private void Viewport3DX_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Viewport3DXLoaded(object sender, RoutedEventArgs e)
        {
            renderHostInternal = (ItemsPanelRoot as SwapChainRenderHost).RenderHost;
            renderHostInternal.Viewport = this;
            renderHostInternal.EffectsManager = this.EffectsManager;
            renderHostInternal.RenderTechnique = this.RenderTechnique;           
        }

        /// <summary>
        /// Attaches the elements to the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void Attach(IRenderHost host)
        {
            if (!IsAttached)
            {
                foreach (var e in this.Renderables)
                {
                    e.Attach(host);
                }

                foreach (var e in this.D2DRenderables)
                {
                    e.Attach(host);
                }
                IsAttached = true;
            }
        }

        /// <summary>
        /// Detaches the elements.
        /// </summary>
        public void Detach()
        {
            if (IsAttached)
            {
                IsAttached = false;
                foreach (var e in this.Renderables)
                {
                    e.Detach();
                }
                foreach (var e in this.D2DRenderables)
                {
                    e.Detach();
                }
            }
        }

        public void InvalidateRender()
        {
            renderHostInternal?.InvalidateRender();
        }

        public void Update(TimeSpan timeStamp)
        {
            
        }
    }
}