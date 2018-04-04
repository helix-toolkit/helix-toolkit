/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Point = Windows.Foundation.Point;


namespace HelixToolkit.UWP
{
    using Cameras;
    using Model.Scene;
    using Model.Scene2D;
    using System.Runtime.CompilerServices;
    using Windows.UI.Xaml.Input;
    using Visibility = Windows.UI.Xaml.Visibility;
    /// <summary>
    /// 
    /// </summary>
    public static class ViewportPartNames
    {
        public const string PART_RenderTarget = "PART_RenderTarget";
        public const string PART_ViewCube = "PART_ViewCube";
        public const string PART_CoordinateView = "PART_CoordinateView";
    }

    /// <summary>
    /// Renders the contained 3-D content within the 2-D layout bounds of the Viewport3DX element.
    /// </summary>
    [ContentProperty(Name = "Items")]
    [TemplatePart(Name = "PART_RenderTarget", Type =typeof(SwapChainRenderHost))]
    [TemplatePart(Name = "PART_ViewCube", Type = typeof(ViewBoxModel3D))]
    [TemplatePart(Name = "PART_CoordinateView", Type =typeof(CoordinateSystemModel3D))]
    public partial class Viewport3DX : ItemsControl, IViewport3DX
    {
        ///// <summary>
        ///// Changes the dpi of the device manager when the DisplayProperties.LogicalDpi has changed.
        ///// </summary>
        ///// <param name="sender">The sender.</param>
        //private void DisplayPropertiesLogicalDpiChanged(object sender)
        //{
        //    this.deviceManager.Dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
        //}
        /// <summary>
        /// Gets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        public IRenderHost RenderHost { get { return this.renderHostInternal; } }
        /// <summary>
        /// Gets the camera core.
        /// </summary>
        /// <value>
        /// The camera core.
        /// </value>
        public CameraCore CameraCore { get { return this.Camera; } }
        /// <summary>
        /// Gets the world matrix.
        /// </summary>
        /// <value>
        /// The world matrix.
        /// </value>
        public Matrix WorldMatrix { get; } = Matrix.Identity;
        /// <summary>
        /// Gets the renderables.
        /// </summary>
        /// <value>
        /// The renderables.
        /// </value>
        public IEnumerable<SceneNode> Renderables
        {
            get
            {
                foreach(Element3D item in Items)
                {
                    yield return item.SceneNode;
                }
                yield return viewCube;
                yield return coordinateSystem;
            }
        }
        /// <summary>
        /// Gets the d2d renderables.
        /// </summary>
        /// <value>
        /// The d2 d renderables.
        /// </value>
        public IEnumerable<SceneNode2D> D2DRenderables
        {
            get
            {
                return Enumerable.Empty<SceneNode2D>();
            }
        }
        /// <summary>
        /// Get current render context
        /// </summary>
        public IRenderContext RenderContext { get { return this.renderHostInternal?.RenderContext; } }

        /// <summary>
        /// Gets or sets the render host internal.
        /// </summary>
        /// <value>
        /// The render host internal.
        /// </value>
        protected IRenderHost renderHostInternal;
        private bool IsAttached = false;
        private ViewBoxModel3D viewCube;
        private CoordinateSystemModel3D coordinateSystem;
        private CameraController cameraController;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport3DX"/> class.
        /// </summary>
        public Viewport3DX()
        {
            this.DefaultStyleKey = typeof(Viewport3DX);
            this.Loaded += Viewport3DXLoaded;
            this.Unloaded += Viewport3DX_Unloaded;
            cameraController = new CameraController(this);
            Camera = new PerspectiveCamera() { Position = new Vector3(0, 0, -10), LookDirection = new Vector3(0, 0, 10), UpDirection = new Vector3(0, 1, 0) };
            InputController = new InputController();
            RegisterPropertyChangedCallback(VisibilityProperty, (s, e) => 
            {
                if(renderHostInternal != null)
                {
                    renderHostInternal.IsRendering = (Visibility)s.GetValue(e) == Visibility.Visible;
                }
            });            
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if(viewCube == null)
            {
                viewCube = GetTemplateChild(ViewportPartNames.PART_ViewCube) as ViewBoxModel3D;
                if(viewCube != null)
                {
                    viewCube.ViewBoxClickedEvent += ViewCube_ViewBoxClickedEvent;
                }
            }

            if (viewCube == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", ViewportPartNames.PART_ViewCube);
            }
            else
            {
                viewCube.RelativeScreenLocationX = ViewCubeHorizontalPosition;
                viewCube.RelativeScreenLocationY = ViewCubeVerticalPosition;
                viewCube.UpDirection = ModelUpDirection;
            }

            if (coordinateSystem == null)
            {
                coordinateSystem = GetTemplateChild(ViewportPartNames.PART_CoordinateView) as CoordinateSystemModel3D;
            }
            if (coordinateSystem == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", ViewportPartNames.PART_CoordinateView);
            }
            else
            {
                coordinateSystem.RelativeScreenLocationX = CoordinateSystemHorizontalPosition;
                coordinateSystem.RelativeScreenLocationY = CoordinateSystemVerticalPosition;
            }
        }

        private void Viewport3DXLoaded(object sender, RoutedEventArgs e)
        {
            renderHostInternal = (ItemsPanelRoot as SwapChainRenderHost).RenderHost;
            if (renderHostInternal != null)
            {
                renderHostInternal.Viewport = this;
                renderHostInternal.IsRendering = Visibility == Visibility.Visible;
                renderHostInternal.EffectsManager = this.EffectsManager;
                renderHostInternal.RenderTechnique = this.RenderTechnique;
                renderHostInternal.ClearColor = this.BackgroundColor.ToColor4();               
            }
        }

        private void Viewport3DX_Unloaded(object sender, RoutedEventArgs e)
        {

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

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            var p = e.GetCurrentPoint(this).Position;
            if (!ViewBoxHitTest(p))
            {
                cameraController.OnMouseDown(e);
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            cameraController.OnMouseUp(e);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            cameraController.InputController.OnKeyPressed(e);
        }

        /// <summary>
        /// Called before the ManipulationStarted event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);
            if(e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                cameraController.OnManipulationStarted(e);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                cameraController.OnManipulationCompleted(e);
        }


        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            base.OnManipulationDelta(e);
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                cameraController.OnManipulationDelta(e);
        }

        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            cameraController.OnMouseWheel(e);
        }

        private bool ViewBoxHitTest(Point p)
        {
            var camera = Camera as ProjectionCamera;
            if (camera == null)
            {
                return false;
            }

            var ray = this.UnProject(p);
            var hits = new List<HitTestResult>();
            if (viewCube.HitTest(RenderContext, ray, ref hits))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ViewCube_ViewBoxClickedEvent(object sender, ViewBoxNode.ViewBoxClickedEventArgs e)
        {
            var pc = this.Camera as ProjectionCamera;
            if (pc == null)
            {
                return;
            }

            var target = pc.Position + pc.LookDirection;
            float distance = pc.LookDirection.Length();
            var look = e.LookDirection * distance;
            var newPosition = target - look;
            pc.AnimateTo(newPosition, look, e.UpDirection, 500);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvalidateRender()
        {
            renderHostInternal?.InvalidateRender();
        }

        public void Update(TimeSpan timeStamp)
        {
            cameraController.OnTimeStep(timeStamp.Ticks);   
        }

        /// <summary>
        /// Adds the specified move force.
        /// </summary>
        /// <param name="dx">
        /// The delta x. 
        /// </param>
        /// <param name="dy">
        /// The delta y. 
        /// </param>
        /// <param name="dz">
        /// The delta z. 
        /// </param>
        public void AddMoveForce(double dx, double dy, double dz)
        {
            cameraController.AddMoveForce(new Vector3((float)dx, (float)dy, (float)dz));
        }

        /// <summary>
        /// Adds the specified move force.
        /// </summary>
        /// <param name="delta">
        /// The delta. 
        /// </param>
        public void AddMoveForce(Vector3 delta)
        {
            cameraController.AddMoveForce(delta);
        }

        /// <summary>
        /// Adds the specified pan force.
        /// </summary>
        /// <param name="dx">
        /// The delta x. 
        /// </param>
        /// <param name="dy">
        /// The delta y. 
        /// </param>
        public void AddPanForce(double dx, double dy)
        {
            cameraController.AddPanForce(dx, dy);
        }

        /// <summary>
        /// The add pan force.
        /// </summary>
        /// <param name="pan">
        /// The pan. 
        /// </param>
        public void AddPanForce(Vector3 pan)
        {
            cameraController.AddPanForce(pan);
        }

        /// <summary>
        /// The add rotate force.
        /// </summary>
        /// <param name="dx">
        /// The delta x. 
        /// </param>
        /// <param name="dy">
        /// The delta y. 
        /// </param>
        public void AddRotateForce(double dx, double dy)
        {
            cameraController.AddRotateForce(dx, dy);
        }

        /// <summary>
        /// Adds the zoom force.
        /// </summary>
        /// <param name="dx">
        /// The delta. 
        /// </param>
        public void AddZoomForce(double dx)
        {
            cameraController.AddZoomForce(dx);
        }

        /// <summary>
        /// Adds the zoom force.
        /// </summary>
        /// <param name="dx">
        /// The delta. 
        /// </param>
        /// <param name="zoomOrigin">
        /// The zoom origin. 
        /// </param>
        public void AddZoomForce(double dx, Vector3 zoomOrigin)
        {
            cameraController.AddZoomForce(dx, zoomOrigin);
        }

        /// <summary>
        ///   Stops the spinning.
        /// </summary>
        public void StopSpin()
        {
            cameraController.StopSpin();
        }

        /// <summary>
        /// Starts spinning.
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <param name="position">The position.</param>
        /// <param name="aroundPoint">The point to spin around.</param>
        public void StartSpin(Vector2 speed, Point position, Vector3 aroundPoint)
        {
            cameraController.StartSpin(speed, position, aroundPoint);
        }

        private void CameraInternal_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateRender();
        }
    }
}