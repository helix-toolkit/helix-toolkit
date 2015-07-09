// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Viewport3DX.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a Viewport control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Provides a Viewport control.
    /// </summary>
    [DefaultEvent("OnChildrenChanged")]
    [DefaultProperty("Children")]
    [ContentProperty("Items")]
    [TemplatePart(Name = "PART_CameraController", Type = typeof(CameraController))]
    [TemplatePart(Name = "PART_Canvas", Type = typeof(DPFCanvas))]
    [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(AdornerDecorator))]
    [TemplatePart(Name = "PART_CoordinateView", Type = typeof(Viewport3D))]
    [TemplatePart(Name = "PART_ViewCubeViewport", Type = typeof(Viewport3D))]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public partial class Viewport3DX : ItemsControl, IRenderer
    {
        /// <summary>
        /// The adorner layer part name.
        /// </summary>
        private const string PartAdornerLayer = "PART_AdornerLayer";

        /// <summary>
        /// The camera controller part name.
        /// </summary>
        private const string PartCameraController = "PART_CameraController";

        /// <summary>
        /// The coordinate view part name.
        /// </summary>
        private const string PartCoordinateView = "PART_CoordinateView";

        /// <summary>
        /// The view cube part name.
        /// </summary>
        private const string PartViewCube = "PART_ViewCube";

        /// <summary>
        /// The view cube viewport part name.
        /// </summary>
        private const string PartViewCubeViewport = "PART_ViewCubeViewport";

        /// <summary>
        ///   The is move enabled property.
        /// </summary>
        public static readonly DependencyProperty IsMoveEnabledProperty = DependencyProperty.Register(
            "IsMoveEnabled", typeof(bool), typeof(Viewport3DX), new UIPropertyMetadata(true));

        /// <summary>
        /// The stop watch
        /// </summary>
        public static readonly Stopwatch StopWatch = new Stopwatch();

        /// <summary>
        ///   The camera history stack.
        /// </summary>
        /// <remarks>
        ///   Implemented as a list since we want to remove items at the bottom of the stack.
        /// </remarks>
        private readonly List<CameraSetting> cameraHistory = new List<CameraSetting>();

        /// <summary>
        /// The change field of view handler
        /// </summary>
        private readonly ZoomHandler changeFieldOfViewHandler;

        /// <summary>
        /// The frame rate stopwatch.
        /// </summary>
        private readonly Stopwatch fpsWatch = new Stopwatch();

        /// <summary>
        /// The orthographic camera.
        /// </summary>
        private readonly OrthographicCamera orthographicCamera;

        /// <summary>
        /// The pan handler
        /// </summary>
        private readonly PanHandler panHandler;

        /// <summary>
        /// The perspective camera.
        /// </summary>
        private readonly PerspectiveCamera perspectiveCamera;

        /// <summary>
        /// The rendering event listener.
        /// </summary>
        private readonly RenderingEventListener renderingEventListener;

        /// <summary>
        /// The rotate handler
        /// </summary>
        private readonly RotateHandler rotateHandler;

        /// <summary>
        /// The zoom handler
        /// </summary>
        private readonly ZoomHandler zoomHandler;

        /// <summary>
        /// The zoom rectangle handler
        /// </summary>
        private readonly ZoomRectangleHandler zoomRectangleHandler;

        /// <summary>
        /// The adorner layer.
        /// </summary>
        private AdornerDecorator adornerLayer;

        /// <summary>
        /// The camera controller.
        /// </summary>
        private CameraController cameraController;

        /// <summary>
        /// The coordinate system lights.
        /// </summary>
        private Model3DGroup coordinateSystemLights;

        /// <summary>
        /// The coordinate view.
        /// </summary>
        private Viewport3D coordinateView;

        /// <summary>
        /// The frame counter.
        /// </summary>
        private int frameCounter;

        /// <summary>
        /// The "control has been loaded before" flag.
        /// </summary>
        private bool hasBeenLoadedBefore;

        /// <summary>
        /// The frame counter for info field updates.
        /// </summary>
        private int infoFrameCounter;

        /// <summary>
        ///   The is spinning flag.
        /// </summary>
        private bool isSpinning;

        /// <summary>
        /// The is subscribed to rendering event.
        /// </summary>
        private bool isSubscribedToRenderingEvent;

        /// <summary>
        ///   The last tick.
        /// </summary>
        private long lastTick;

        /// <summary>
        ///   The move speed.
        /// </summary>
        private Vector3D moveSpeed;

        /// <summary>
        ///   The pan speed.
        /// </summary>
        private Vector3D panSpeed;

        /// <summary>
        ///   The rectangle adorner.
        /// </summary>
        private RectangleAdorner rectangleAdorner;

        /// <summary>
        ///   The 3D rotation point.
        /// </summary>
        private Point3D rotationPoint3D;

        /// <summary>
        ///   The rotation position.
        /// </summary>
        private Point rotationPosition;

        /// <summary>
        ///   The rotation speed.
        /// </summary>
        private Vector rotationSpeed;

        /// <summary>
        ///   The 3D point to spin around.
        /// </summary>
        private Point3D spinningPoint3D;

        /// <summary>
        ///   The spinning position.
        /// </summary>
        private Point spinningPosition;

        /// <summary>
        ///   The spinning speed.
        /// </summary>
        private Vector spinningSpeed;

        /// <summary>
        ///   The target adorner.
        /// </summary>
        private Adorner targetAdorner;

        /// <summary>
        /// The <see cref="TouchDevice"/> of the first TouchDown.
        /// </summary>
        private TouchDevice touchDownDevice;

        /// <summary>
        /// The view cube.
        /// </summary>
        private ViewCubeVisual3D viewCube;

        /// <summary>
        /// The view cube lights.
        /// </summary>
        private Model3DGroup viewCubeLights;

        /// <summary>
        /// The view cube view.
        /// </summary>
        private Viewport3D viewCubeViewport;

        /// <summary>
        ///   The point to zoom around.
        /// </summary>
        private Point3D zoomPoint3D;

        /// <summary>
        ///   The zoom speed.
        /// </summary>
        private double zoomSpeed;

        /// <summary>
        /// Initializes static members of the <see cref="Viewport3DX" /> class.
        /// </summary>
        static Viewport3DX()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(Viewport3DX), new FrameworkPropertyMetadata(typeof(Viewport3DX)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport3DX" /> class.
        /// </summary>
        public Viewport3DX()
        {
            this.perspectiveCamera = new PerspectiveCamera();
            this.orthographicCamera = new OrthographicCamera();
            this.perspectiveCamera.Reset();
            this.orthographicCamera.Reset();

            this.Camera = this.Orthographic ? (ProjectionCamera)this.orthographicCamera : this.perspectiveCamera;

            //this.Children = new Element3DCollection();

            this.rotateHandler = new RotateHandler(this);
            this.panHandler = new PanHandler(this);
            this.zoomHandler = new ZoomHandler(this);
            this.changeFieldOfViewHandler = new ZoomHandler(this, true);
            this.zoomRectangleHandler = new ZoomRectangleHandler(this);

            this.CommandBindings.Add(new CommandBinding(ViewportCommands.ZoomExtents, this.ZoomExtentsHandler));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.SetTarget, this.SetTargetHandler));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.Reset, this.ResetHandler));

            this.CommandBindings.Add(new CommandBinding(ViewportCommands.Zoom, this.zoomHandler.Execute));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.Pan, this.panHandler.Execute));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.Rotate, this.rotateHandler.Execute));
            this.CommandBindings.Add(
                new CommandBinding(ViewportCommands.ChangeFieldOfView, this.changeFieldOfViewHandler.Execute));
            this.CommandBindings.Add(
                new CommandBinding(ViewportCommands.ZoomRectangle, this.zoomRectangleHandler.Execute));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.BottomView, this.BottomViewHandler));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.TopView, this.TopViewHandler));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.FrontView, this.FrontViewHandler));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.BackView, this.BackViewHandler));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.LeftView, this.LeftViewHandler));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.RightView, this.RightViewHandler));

            this.SetDefaultGestures();

            this.fpsWatch.Start();

            this.renderingEventListener = new RenderingEventListener(this.OnCompositionTargetRendering);

            this.Loaded += this.ControlLoaded;
            this.Unloaded += this.ControlUnloaded;
        }

        /// <summary>
        ///   Gets or sets a value indicating whether move is enabled.
        /// </summary>
        /// <value> <c>true</c> if move is enabled; otherwise, <c>false</c> . </value>
        public bool IsMoveEnabled
        {
            get
            {
                return (bool)this.GetValue(IsMoveEnabledProperty);
            }

            set
            {
                this.SetValue(IsMoveEnabledProperty, value);
            }
        }

        public static DependencyProperty RenderHostProperty = DependencyProperty.Register(
            "RenderHost", typeof(DPFCanvas), typeof(Viewport3DX), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the canvas.
        /// </summary>
        /// <value>
        /// The canvas.
        /// </value>
        //protected DPFCanvas RenderHost { get; private set; }
        public DPFCanvas RenderHost
        {
            get { return (DPFCanvas)this.GetValue(RenderHostProperty); }
            set { this.SetValue(RenderHostProperty, value); }
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
            this.AddMoveForce(new Vector3D(dx, dy, dz));
        }

        /// <summary>
        /// Adds the specified move force.
        /// </summary>
        /// <param name="delta">
        /// The delta. 
        /// </param>
        public void AddMoveForce(Vector3D delta)
        {
            if (!this.IsMoveEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            this.moveSpeed += delta * 40;
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
            var pcam = this.Camera as ProjectionCamera;
            this.AddPanForce(pcam.FindPanVector(dx, dy));
        }

        /// <summary>
        /// The add pan force.
        /// </summary>
        /// <param name="pan">
        /// The pan. 
        /// </param>
        public void AddPanForce(Vector3D pan)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            this.panSpeed += pan * 40;
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
            var pcam = this.Camera as ProjectionCamera;

            if (!this.IsRotationEnabled || pcam == null)
            {
                return;
            }

            this.PushCameraSetting();
            this.rotationPoint3D = pcam.Target;
            this.rotationPosition = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            this.rotationSpeed.X += dx * 40;
            this.rotationSpeed.Y += dy * 40;
        }

        /// <summary>
        /// Adds the zoom force.
        /// </summary>
        /// <param name="dx">
        /// The delta. 
        /// </param>
        public void AddZoomForce(double dx)
        {
            var pcam = this.Camera as ProjectionCamera;
            if (!this.IsZoomEnabled || pcam == null)
            {
                return;
            }

            this.AddZoomForce(dx, pcam.Target);
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
        public void AddZoomForce(double dx, Point3D zoomOrigin)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            this.zoomPoint3D = zoomOrigin;
            this.zoomSpeed += dx * 8;
        }

        /// <summary>
        /// Changes the direction of the camera.
        /// </summary>
        /// <param name="lookDir">
        /// The look direction. 
        /// </param>
        /// <param name="upDir">
        /// The up direction. 
        /// </param>
        /// <param name="animationTime">
        /// The animation time. 
        /// </param>
        public void ChangeDirection(Vector3D lookDir, Vector3D upDir, double animationTime = 500)
        {
            this.StopAnimations();
            this.PushCameraSetting();
            var pcam = this.Camera as ProjectionCamera;
            if (pcam != null)
            {
                pcam.ChangeDirection(lookDir, upDir, animationTime);
            }
        }

        /// <summary>
        /// Finds the nearest 3D point in the scene.
        /// </summary>
        /// <param name="pt">
        /// The screen point (2D).
        /// </param>
        /// <returns>
        /// A Point3D or null.
        /// </returns>
        public Point3D? FindNearestPoint(Point pt)
        {
            return ViewportExtensions.FindNearestPoint(this, pt);
        }

        /// <summary>
        /// Hides the target adorner.
        /// </summary>
        public void HideTargetAdorner()
        {
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(this.RenderHost);
            if (this.targetAdorner != null)
            {
                myAdornerLayer.Remove(this.targetAdorner);
            }

            this.targetAdorner = null;

            // the adorner sometimes leaves some 'dust', so refresh the viewport
            this.RefreshViewport();
        }

        /// <summary>
        /// Hides the zoom rectangle.
        /// </summary>
        public void HideZoomRectangle()
        {
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(this.RenderHost);
            if (this.rectangleAdorner != null)
            {
                myAdornerLayer.Remove(this.rectangleAdorner);
            }

            this.rectangleAdorner = null;

            this.RefreshViewport();
        }

        /// <summary>
        /// Tries to invalidate the current render.
        /// </summary>
        public void InvalidateRender()
        {
            var rh = this.RenderHost;
            if (rh != null)
            {
                rh.InvalidateRender();
            }
        }

        /// <summary>
        /// Change the camera to look at the specified point.
        /// </summary>
        /// <param name="p">
        /// The point.
        /// </param>
        public void LookAt(Point3D p)
        {
            this.LookAt(p, 0);
        }

        /// <summary>
        /// Change the camera to look at the specified point.
        /// </summary>
        /// <param name="p">
        /// The point.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void LookAt(Point3D p, double animationTime)
        {
            var projectionCamera = this.Camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return;
            }

            projectionCamera.LookAt(p, animationTime);
        }

        /// <summary>
        /// Change the camera to look at the specified point.
        /// </summary>
        /// <param name="p">
        /// The point.
        /// </param>
        /// <param name="distance">
        /// The distance.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void LookAt(Point3D p, double distance, double animationTime)
        {
            var projectionCamera = this.Camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return;
            }

            projectionCamera.LookAt(p, distance, animationTime);
        }

        /// <summary>
        /// Change the camera to look at the specified point.
        /// </summary>
        /// <param name="p">
        /// The point.
        /// </param>
        /// <param name="direction">
        /// The direction.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void LookAt(Point3D p, Vector3D direction, double animationTime)
        {
            var projectionCamera = this.Camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return;
            }

            projectionCamera.LookAt(p, direction, animationTime);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        /// <exception cref="HelixToolkitException">{0} is missing from the template.</exception>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.RenderHost = this.GetTemplateChild("PART_Canvas") as DPFCanvas;
            if (this.RenderHost != null)
            {
                this.RenderHost.Renderable = this;
            }

            if (this.adornerLayer == null)
            {
                this.adornerLayer = this.Template.FindName(PartAdornerLayer, this) as AdornerDecorator;
            }

            if (this.adornerLayer == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartAdornerLayer);
            }

            if (this.cameraController == null)
            {
                this.cameraController = this.Template.FindName(PartCameraController, this) as CameraController;
                if (this.cameraController != null)
                {
                    this.cameraController.Viewport = this;
                    this.cameraController.LookAtChanged += (s, e) => this.OnCameraChanged();
                    this.cameraController.ZoomedByRectangle += (s, e) => this.OnCameraChanged();
                }
            }

            if (this.coordinateView == null)
            {
                this.coordinateView = this.Template.FindName(PartCoordinateView, this) as Viewport3D;

                this.coordinateSystemLights = new Model3DGroup();

                // coordinateSystemLights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 1, 1)));
                // coordinateSystemLights.Children.Add(new AmbientLight(Colors.DarkGray));
                this.coordinateSystemLights.Children.Add(new System.Windows.Media.Media3D.AmbientLight(Colors.LightGray));

                if (this.coordinateView != null)
                {
                    this.coordinateView.Camera = new System.Windows.Media.Media3D.PerspectiveCamera();
                    this.coordinateView.Children.Add(new ModelVisual3D { Content = this.coordinateSystemLights });
                }
            }

            if (this.coordinateView == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartCoordinateView);
            }

            if (this.viewCubeViewport == null)
            {
                this.viewCubeViewport = this.Template.FindName(PartViewCubeViewport, this) as Viewport3D;

                this.viewCubeLights = new Model3DGroup();
                this.viewCubeLights.Children.Add(new System.Windows.Media.Media3D.AmbientLight(Colors.White));
                if (this.viewCubeViewport != null)
                {
                    this.viewCubeViewport.Camera = new System.Windows.Media.Media3D.PerspectiveCamera();
                    this.viewCubeViewport.Children.Add(new ModelVisual3D { Content = this.viewCubeLights });
                    this.viewCubeViewport.MouseEnter += this.ViewCubeViewportMouseEnter;
                    this.viewCubeViewport.MouseLeave += this.ViewCubeViewportMouseLeave;
                }

                this.viewCube = this.Template.FindName(PartViewCube, this) as ViewCubeVisual3D;
                if (this.viewCube != null)
                {
                    this.viewCube.Clicked += this.ViewCubeClicked;

                    // this.viewCube.Viewport = this.Viewport;
                }
            }

            // update the coordinateview camera
            this.OnCameraChanged();
        }

        /// <summary>
        /// Pushes the camera setting.
        /// </summary>
        public void PushCameraSetting()
        {
            this.cameraHistory.Add(new CameraSetting(this.Camera as ProjectionCamera));
            if (this.cameraHistory.Count > 100)
            {
                this.cameraHistory.RemoveAt(0);
            }
        }

        /// <summary>
        /// Detaches the current scene and attaches it again. 
        /// Call it if you want to repeat the entire Attach-Pass
        /// </summary>
        public void ReAttach()
        {
            if (this.RenderHost != null)
            {
                this.RenderHost.Renderable = null; 
                this.RenderHost.Renderable = this;
            }
        }

        /// <summary>
        /// Detaches the current scene.         
        /// Call it if you want to detouch the scene from the renderer.
        /// Call <see cref="ReAttach"/> in order to attach the current scene again.
        /// </summary>
        public void Detach()
        {
            if (this.RenderHost != null)
            {
                this.RenderHost.Renderable = null;                
            }
        }

        /// <summary>
        /// Resets the view.
        /// </summary>
        public void Reset()
        {
            if (!this.IsZoomEnabled || !this.IsRotationEnabled || !this.IsPanEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            if (this.DefaultCamera != null)
            {
                this.DefaultCamera.CopyTo(this.Camera as ProjectionCamera);
            }
            else
            {
                this.Camera.Reset();
                this.ZoomExtents();
            }
        }

        /// <summary>
        /// Change the camera position and directions.
        /// </summary>
        /// <param name="newPosition">
        /// The new camera position.
        /// </param>
        /// <param name="newDirection">
        /// The new camera look direction.
        /// </param>
        /// <param name="newUpDirection">
        /// The new camera up direction.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void SetView(Point3D newPosition, Vector3D newDirection, Vector3D newUpDirection, double animationTime)
        {
            var projectionCamera = this.Camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return;
            }

            projectionCamera.AnimateTo(newPosition, newDirection, newUpDirection, animationTime);
        }

        /// <summary>
        /// Shows the target adorner.
        /// </summary>
        /// <param name="position">The position.</param>
        public void ShowTargetAdorner(Point position)
        {
            if (!this.ShowCameraTarget)
            {
                return;
            }

            if (this.targetAdorner != null)
            {
                return;
            }

            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(this.RenderHost);
            this.targetAdorner = new TargetSymbolAdorner(this.RenderHost, position);
            myAdornerLayer.Add(this.targetAdorner);
        }

        /// <summary>
        /// Shows the zoom rectangle.
        /// </summary>
        /// <param name="rect">The zoom rectangle.</param>
        public void ShowZoomRectangle(Rect rect)
        {
            if (this.rectangleAdorner != null)
            {
                return;
            }

            var myAdornerLayer = AdornerLayer.GetAdornerLayer(this.RenderHost);
            this.rectangleAdorner = new RectangleAdorner(
                this.RenderHost, rect, Colors.LightGray, Colors.Black, 3, 1, 10, DashStyles.Solid);
            myAdornerLayer.Add(this.rectangleAdorner);
        }

        /// <summary>
        /// Starts spinning.
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <param name="position">The position.</param>
        /// <param name="aroundPoint">The point to spin around.</param>
        public void StartSpin(Vector speed, Point position, Point3D aroundPoint)
        {
            this.spinningSpeed = speed;
            this.spinningPosition = position;
            this.spinningPoint3D = aroundPoint;
            this.isSpinning = true;
        }

        /// <summary>
        ///   Stops the spinning.
        /// </summary>
        public void StopSpin()
        {
            this.isSpinning = false;
        }

        /// <summary>
        /// Zooms to the extents of the specified bounding box.
        /// </summary>
        /// <param name="bounds">
        /// The bounding box.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void ZoomExtents(Rect3D bounds, double animationTime = 0)
        {
            ViewportExtensions.ZoomExtents(this, bounds, animationTime);
        }

        /// <summary>
        /// Zooms to the extents of the model.
        /// </summary>
        /// <param name="animationTime">
        /// The animation time (milliseconds). 
        /// </param>
        public void ZoomExtents(double animationTime = 200)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            ViewportExtensions.ZoomExtents(this, animationTime);
        }

        /// <summary>
        /// Attaches the elements to the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        void IRenderer.Attach(IRenderHost host)
        {
            foreach (IRenderable e in this.Items)
            {
                e.Attach(host);
            }

            StopWatch.Start();
        }

        /// <summary>
        /// Detaches the elements.
        /// </summary>
        void IRenderer.Detach()
        {
            foreach (IRenderable e in this.Items)
            {
                e.Detach();
            }
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        void IRenderer.Render(RenderContext context)
        {   
            context.Camera = this.Camera;

            var sortedItems = SortItemsCollectionForTransparencyAndDistance();

            foreach (IRenderable e in sortedItems)
            {
                e.Render(context);
            }
        }

        private IEnumerable<Model3D> SortItemsCollectionForTransparencyAndDistance()
        {
            var models = this.Items.Cast<Model3D>().ToArray();
            var geoms = models.Where(m => m is MaterialGeometryModel3D)
                .Cast<MaterialGeometryModel3D>().ToArray();

            // Don't transparency sort if there are
            // no items marked as having transparency.
            if (!geoms.Any(g => g.HasTransparency))
            {
                return models;
            }

            var nonGeoms = models.Where(m => !(m is MaterialGeometryModel3D));
            var nonTrans = geoms.Where(g => !g.HasTransparency);
            var transGeoms = geoms.Where(g => g.HasTransparency).OrderByDescending(g=>g.SquareDistanceToCamera(this.Camera));

            var finalColl = nonGeoms.Concat(nonTrans).Concat(transGeoms);

            return finalColl;
        }

        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        void IRenderer.Update(TimeSpan timeSpan)
        {
            this.FpsCounter.AddFrame(timeSpan);
            foreach (IRenderable e in this.Items)
            {
                e.Update(timeSpan);
            }
        }

        /// <summary>
        /// Called when the camera is changed.
        /// </summary>
        protected virtual void OnCameraChanged()
        {
            var projectionCamera = this.Camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return;
            }

            var lookdir = projectionCamera.LookDirection;
            lookdir.Normalize();
            var origin = new Point3D();

            // update the camera of the coordinate system
            if (this.coordinateView != null)
            {
                var pc = (System.Windows.Media.Media3D.PerspectiveCamera)this.coordinateView.Camera;
                pc.LookDirection = lookdir * 30;
                pc.Position = origin - pc.LookDirection;
                pc.UpDirection = projectionCamera.UpDirection;
            }

            // update the camera of the view cube
            if (this.viewCubeViewport != null)
            {
                var pc = (System.Windows.Media.Media3D.PerspectiveCamera)this.viewCubeViewport.Camera;
                pc.LookDirection = lookdir * 20;
                pc.Position = origin - pc.LookDirection;
                pc.UpDirection = projectionCamera.UpDirection;
            }

            // update the headlight and coordinate system light
            if (this.Camera != null)
            {
                if (this.coordinateSystemLights != null)
                {
                    var cshl = this.coordinateSystemLights.Children[0] as System.Windows.Media.Media3D.DirectionalLight;
                    if (cshl != null)
                    {
                        cshl.Direction = projectionCamera.LookDirection;
                    }
                }
            }

            if (this.ShowFieldOfView)
            {
                this.UpdateFieldOfViewInfo();
            }

            if (this.ShowCameraInfo)
            {
                this.UpdateCameraInfo();
            }

            this.InvalidateRender();
        }

        /// <summary>
        /// Gets the pressed mouse buttons as flags of <see cref="MouseButton"/>.
        /// If not button is pressed (result is zero), then it was a touch down.
        /// </summary>
        /// <returns>
        /// The pressed mouse buttons as flags of <see cref="MouseButton"/>.
        /// </returns>
        public static MouseButton GetPressedMouseButtons()
        {
            int flags = 0;
            flags |= (int)Mouse.LeftButton << 0;
            flags |= (int)Mouse.MiddleButton << 1;
            flags |= (int)Mouse.RightButton << 2;
            flags |= (int)Mouse.XButton1 << 3;
            flags |= (int)Mouse.XButton2 << 4;
            return (MouseButton)flags;
        }

        /// <inheritdoc/>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.touchDownDevice == null)
            {
                this.Focus();
                this.MouseDownHitTest(e.GetPosition(this), e);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This makes selection via Touch work without disabling the CameraController which uses Manipulation.
        /// </remarks>>
        protected override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);
            if (this.touchDownDevice == null)
            {
                this.touchDownDevice = e.TouchDevice;
                this.Focus();
                this.MouseDownHitTest(e.GetTouchPoint(this).Position, e);
            }
        }

        public void EmulateMouseDownByTouch(MouseButtonEventArgs e)
        {
            this.Focus();
            this.MouseDownHitTest(e.GetPosition(this), e);
        }

        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.touchDownDevice == null)
            {
                var pt = e.GetPosition(this);
                this.MouseMoveHitTest(pt, e);
                this.UpdateCurrentPosition(pt);
            }
        }

        /// <inheritdoc/>
        protected override void OnPreviewTouchMove(TouchEventArgs e)
        {
            base.OnPreviewTouchMove(e);
            if (this.touchDownDevice == e.TouchDevice)
            {
                var tp = e.GetTouchPoint(this);
                var pt = tp.Position;
                this.MouseMoveHitTest(pt, e);
                this.UpdateCurrentPosition(pt);
            }
        }

        public void EmulateMouseMoveByTouch(Point pt)
        {
            this.MouseMoveHitTest(pt);
            this.UpdateCurrentPosition(pt);
        }

        /// <summary>
        /// Updates the property <see cref="CurrentPosition"/>.
        /// </summary>
        /// <param name="pt">The current mouse hit point.</param>
        private void UpdateCurrentPosition(Point pt)
        {
            if (this.EnableCurrentPosition)
            {
                var pos = this.FindNearestPoint(pt);
                if (pos != null)
                {
                    this.CurrentPosition = pos.Value;
                }
                else
                {
                    var p = this.UnProjectOnPlane(pt);
                    if (p != null)
                    {
                        this.CurrentPosition = p.Value;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            this.MouseUpHitTest(e.GetPosition(this), e);
        }

        /// <inheritdoc/>
        protected override void OnPreviewTouchUp(TouchEventArgs e)
        {
            base.OnPreviewTouchUp(e);
            if (this.touchDownDevice == e.TouchDevice)
            {
                this.touchDownDevice = null;
                this.MouseUpHitTest(e.GetTouchPoint(this).Position, e);
            }
        }

        public void EmulateMouseUpByTouch(MouseButtonEventArgs e)
        {
            this.MouseUpHitTest(e.GetPosition(this), e);
        }

        /// <inheritdoc/>
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            if (this.touchDownDevice != null)
            {
                // If this.touchDownDevice has not come up for some reason, do it now.
                this.touchDownDevice = null;
                var pt = e.ManipulationOrigin + e.TotalManipulation.Translation;
                this.MouseUpHitTest(pt, e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled MouseWheel attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs" /> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!this.IsZoomEnabled)
            {
                return;
            }

            if (this.ZoomAroundMouseDownPoint)
            {
                Point point = e.GetPosition(this);
                Point3D nearestPoint;
                Vector3D normal;
                Model3D visual;
                if (this.FindNearest(point, out nearestPoint, out normal, out visual))
                {
                    this.AddZoomForce(-e.Delta * 0.001, nearestPoint);
                    e.Handled = true;
                    return;
                }
            }

            this.AddZoomForce(-e.Delta * 0.001);
            e.Handled = true;
        }

        /// <summary>
        /// Raises the camera changed event.
        /// </summary>
        protected virtual void RaiseCameraChangedEvent()
        {
            // e.Handled = true;
            var args = new RoutedEventArgs(CameraChangedEvent);
            this.RaiseEvent(args);
        }

        /// <summary>
        /// Animates the opacity of the specified object.
        /// </summary>
        /// <param name="obj">
        /// The object to animate.
        /// </param>
        /// <param name="toOpacity">
        /// The to opacity.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        private static void AnimateOpacity(UIElement obj, double toOpacity, double animationTime)
        {
            var a = new DoubleAnimation(toOpacity, new Duration(TimeSpan.FromMilliseconds(animationTime)))
                {
                    AccelerationRatio = 0.3,
                    DecelerationRatio = 0.5
                };
            obj.BeginAnimation(OpacityProperty, a);
        }

        /// <summary>
        /// The back view event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void BackViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.IsModelUpDirectionY())
            {
                this.ChangeDirection(new Vector3D(0, 0, 1), new Vector3D(0, 1, 0));
            }
            else
            {
                this.ChangeDirection(new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
            }
        }

        /// <summary>
        /// The bottom view event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void BottomViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.IsModelUpDirectionY())
            {
                this.ChangeDirection(new Vector3D(0, 1, 0), new Vector3D(0, 0, 1));
            }
            else
            {
                this.ChangeDirection(new Vector3D(0, 0, 1), new Vector3D(0, -1, 0));
            }
        }

        /// <summary>
        /// Determines whether the model up direction is (0,1,0).
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the up direction is (0,1,0); otherwise, <c>false</c>.
        /// </returns>
        public bool IsModelUpDirectionY()
        {
            return this.ModelUpDirection.Y.Equals(1);
        }


        /// <summary>
        /// Handles the change of the render technique        
        /// </summary>
        private void RenderTechniquePropertyChanged()
        {
            if (this.RenderHost != null)
            {
                // remove the scene
                this.RenderHost.Renderable = null;

                // if new rendertechnique set, attach the scene
                if (this.RenderTechnique != null)
                {
                    this.RenderHost.Renderable = this;
                }
            }
        }

        /// <summary>
        /// Handles changes in the camera properties.
        /// </summary>
        private void CameraPropertyChanged()
        {
            // Raise notification
            this.RaiseCameraChangedEvent();

            // Update the CoordinateView camera direction
            this.OnCameraChanged();
        }

        /// <summary>
        /// Clamps the specified value between the limits.
        /// </summary>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <param name="min">
        /// The min. 
        /// </param>
        /// <param name="max">
        /// The max. 
        /// </param>
        /// <returns>
        /// The clamp. 
        /// </returns>
        private double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        /// Called when the control is loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            if (!this.hasBeenLoadedBefore)
            {
                if (this.DefaultCamera != null)
                {
                    this.DefaultCamera.CopyTo(this.perspectiveCamera);
                    this.DefaultCamera.CopyTo(this.orthographicCamera);
                }

                this.hasBeenLoadedBefore = true;
            }

            this.SubscribeToRenderingEvent();
            if (this.ZoomExtentsWhenLoaded)
            {
                this.ZoomExtents();
            }
        }

        /// <summary>
        /// Called when the control is unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void ControlUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnsubscribeRenderingEvent();
        }

        /// <summary>
        /// The front view event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void FrontViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.IsModelUpDirectionY())
            {
                this.ChangeDirection(new Vector3D(0, 0, -1), new Vector3D(0, 1, 0));
            }
            else
            {
                this.ChangeDirection(new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1));
            }
        }

        /// <summary>
        /// The left view event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void LeftViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.IsModelUpDirectionY())
            {
                this.ChangeDirection(new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
            }
            else
            {
                this.ChangeDirection(new Vector3D(0, 1, 0), new Vector3D(0, 0, 1));
            }
        }

        /// <summary>
        /// The rendering event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void OnCompositionTargetRendering(object sender, RenderingEventArgs e)
        {
            var ticks = e.RenderingTime.Ticks;
            double time = 100e-9 * (ticks - this.lastTick);

            if (this.lastTick != 0)
            {
                this.OnTimeStep(time);
            }

            this.lastTick = ticks;

            this.frameCounter++;
            if (this.ShowFrameRate && this.fpsWatch.ElapsedMilliseconds > 500)
            {
                this.FrameRate = (int)(this.frameCounter / (0.001 * this.fpsWatch.ElapsedMilliseconds));
                this.FrameRateText = this.FrameRate + " FPS";
                this.frameCounter = 0;
                this.fpsWatch.Reset();
                this.fpsWatch.Start();
            }

            // update the info fields every 100 frames
            // (it would be better to update only when the visual model of the Viewport3D changes)
            this.infoFrameCounter++;
            if (this.ShowTriangleCountInfo && this.infoFrameCounter > 100)
            {
                int count = this.GetTotalNumberOfTriangles();
                this.TriangleCountInfo = string.Format("Triangles: {0}", count);
                this.infoFrameCounter = 0;
            }
        }

        /// <summary>
        /// The on time step.
        /// </summary>
        /// <param name="time">
        /// The time. 
        /// </param>
        private void OnTimeStep(double time)
        {
            // should be independent of time
            double factor = Math.Pow(this.CameraInertiaFactor, time / 0.012);
            factor = this.Clamp(factor, 0.2, 1);

            if (this.isSpinning && this.spinningSpeed.LengthSquared > 0)
            {
                this.rotateHandler.Rotate(
                    this.spinningPosition, this.spinningPosition + (this.spinningSpeed * time), this.spinningPoint3D);

                if (!this.InfiniteSpin)
                {
                    this.spinningSpeed *= factor;
                }
            }

            if (this.rotationSpeed.LengthSquared > 0.1)
            {
                this.rotateHandler.Rotate(
                    this.rotationPosition, this.rotationPosition + (this.rotationSpeed * time), this.rotationPoint3D);
                this.rotationSpeed *= factor;
            }

            if (Math.Abs(this.panSpeed.LengthSquared) > 0.0001)
            {
                this.panHandler.Pan(this.panSpeed * time);
                this.panSpeed *= factor;
            }

            if (Math.Abs(this.moveSpeed.LengthSquared) > 0.0001)
            {
                switch (this.CameraMode)
                {
                    case CameraMode.Inspect:
                    case CameraMode.WalkAround:
                        var pcam = this.Camera as ProjectionCamera;
                        pcam.MoveCameraPosition(this.moveSpeed * time);
                        break;
                }

                this.moveSpeed *= factor;
            }

            if (Math.Abs(this.zoomSpeed) > 0.1)
            {
                this.zoomHandler.Zoom(this.zoomSpeed * time, this.zoomPoint3D);
                this.zoomSpeed *= factor;
            }
        }

        /// <summary>
        /// Called when the camera type is changed.
        /// </summary>
        private void OrthographicChanged()
        {
            var oldCamera = this.Camera;
            if (this.Orthographic)
            {
                this.Camera = this.orthographicCamera;
            }
            else
            {
                this.Camera = this.perspectiveCamera;
            }

            var projectionCamera = oldCamera as ProjectionCamera;
            if (projectionCamera != null)
            {
                projectionCamera.CopyTo(this.Camera);
            }
        }

        /// <summary>
        ///   Refreshes viewport.
        /// </summary>
        private void RefreshViewport()
        {
            // todo
        }

        /// <summary>
        /// Handles the reset command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void ResetHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Reset();
        }

        /// <summary>
        /// The right view event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void RightViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.IsModelUpDirectionY())
            {
                this.ChangeDirection(new Vector3D(-1, 0, 0), new Vector3D(0, 1, 0));
            }
            else
            {
                this.ChangeDirection(new Vector3D(0, -1, 0), new Vector3D(0, 0, 1));
            }
        }

        /// <summary>
        /// Sets the default gestures.
        /// </summary>
        private void SetDefaultGestures()
        {
            this.InputBindings.Clear();
            // TODO:
            // Runtime error: 'None+U' key and modifier combination is not supported for KeyGesture.
            // But this works when defining in xaml...
            //this.InputBindings.Add(new KeyBinding(ViewportCommands.TopView, Key.U, ModifierKeys.None));
            //this.InputBindings.Add(new KeyBinding(ViewportCommands.BottomView, Key.D, ModifierKeys.None));
            //this.InputBindings.Add(new KeyBinding(ViewportCommands.FrontView, Key.F, ModifierKeys.None));
            //this.InputBindings.Add(new KeyBinding(ViewportCommands.BackView, Key.B, ModifierKeys.None));
            //this.InputBindings.Add(new KeyBinding(ViewportCommands.LeftView, Key.L, ModifierKeys.None));
            //this.InputBindings.Add(new KeyBinding(ViewportCommands.RightView, Key.R, ModifierKeys.None));

            this.InputBindings.Add(new KeyBinding(ViewportCommands.ZoomExtents, Key.E, ModifierKeys.Control));
            this.InputBindings.Add(
                new MouseBinding(
                    ViewportCommands.ZoomExtents, new MouseGesture(MouseAction.LeftDoubleClick, ModifierKeys.Control)));
            this.InputBindings.Add(
                new MouseBinding(ViewportCommands.Rotate, new MouseGesture(MouseAction.RightClick, ModifierKeys.None)));
            this.InputBindings.Add(
                new MouseBinding(ViewportCommands.Zoom, new MouseGesture(MouseAction.RightClick, ModifierKeys.Control)));
            this.InputBindings.Add(
                new MouseBinding(ViewportCommands.Pan, new MouseGesture(MouseAction.RightClick, ModifierKeys.Shift)));
            this.InputBindings.Add(
                new MouseBinding(
                    ViewportCommands.ChangeFieldOfView, new MouseGesture(MouseAction.RightClick, ModifierKeys.Alt)));
            this.InputBindings.Add(
                new MouseBinding(
                    ViewportCommands.ZoomRectangle,
                    new MouseGesture(MouseAction.RightClick, ModifierKeys.Control | ModifierKeys.Shift)));
            this.InputBindings.Add(
                new MouseBinding(
                    ViewportCommands.SetTarget, new MouseGesture(MouseAction.RightDoubleClick, ModifierKeys.Control)));
            this.InputBindings.Add(
                new MouseBinding(
                    ViewportCommands.Reset, new MouseGesture(MouseAction.MiddleDoubleClick, ModifierKeys.Control)));
        }

        /// <summary>
        /// Handles the SetTarget command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void SetTargetHandler(object sender, ExecutedRoutedEventArgs e)
        {
        }

        /// <summary>
        ///   The stop animations.
        /// </summary>
        private void StopAnimations()
        {
            this.rotationSpeed = new Vector();
            this.panSpeed = new Vector3D();
            this.zoomSpeed = 0;
        }

        /// <summary>
        /// The subscribe to rendering event.
        /// </summary>
        private void SubscribeToRenderingEvent()
        {
            if (!this.isSubscribedToRenderingEvent)
            {
                RenderingEventManager.AddListener(this.renderingEventListener);
                this.isSubscribedToRenderingEvent = true;
            }
        }

        /// <summary>
        /// The top view event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void TopViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.IsModelUpDirectionY())
            {
                this.ChangeDirection(new Vector3D(0, -1, 0), new Vector3D(0, 0, 1));
            }
            else
            {
                this.ChangeDirection(new Vector3D(0, 0, -1), new Vector3D(0, 1, 0));
            }
        }

        /// <summary>
        /// The unsubscribe rendering event.
        /// </summary>
        private void UnsubscribeRenderingEvent()
        {
            if (this.isSubscribedToRenderingEvent)
            {
                RenderingEventManager.RemoveListener(this.renderingEventListener);
                this.isSubscribedToRenderingEvent = false;
            }
        }

        /// <summary>
        /// Updates the camera info.
        /// </summary>
        private void UpdateCameraInfo()
        {
            this.CameraInfo = this.Camera.GetInfo();
        }

        /// <summary>
        /// The update field of view info.
        /// </summary>
        private void UpdateFieldOfViewInfo()
        {
            var pc = this.Camera as PerspectiveCamera;
            this.FieldOfViewText = pc != null ? string.Format("FoV ∠ {0:0}°", pc.FieldOfView) : null;
        }

        /// <summary>
        /// The UseDefaultGestures property changed.
        /// </summary>
        private void UseDefaultGesturesChanged()
        {
            if (this.UseDefaultGestures)
            {
                this.SetDefaultGestures();
            }
            else
            {
                this.InputBindings.Clear();
            }
        }

        /// <summary>
        /// Handles clicks on the view cube.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewCubeVisual3D.ClickedEventArgs" /> instance containing the event data.</param>
        private void ViewCubeClicked(object sender, ViewCubeVisual3D.ClickedEventArgs e)
        {
            var pc = this.Camera as ProjectionCamera;
            if (pc == null)
            {
                return;
            }

            var target = pc.Position + pc.LookDirection;
            double distance = pc.LookDirection.Length;
            e.LookDirection *= distance;
            var newPosition = target - e.LookDirection;
            pc.AnimateTo(newPosition, e.LookDirection, e.UpDirection, 500);
        }

        /// <summary>
        /// Called when the mouse enters the view cube.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void ViewCubeViewportMouseEnter(object sender, MouseEventArgs e)
        {
            AnimateOpacity(this.viewCubeViewport, 1.0, 200);
        }

        /// <summary>
        /// Called when the mouse leaves the view cube.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void ViewCubeViewportMouseLeave(object sender, MouseEventArgs e)
        {
            AnimateOpacity(this.viewCubeViewport, this.ViewCubeOpacity, 200);
        }

        /// <summary>
        /// Handles the zoom extents command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs" /> instance containing the event data.</param>
        private void ZoomExtentsHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ZoomExtents();
        }

        /// <summary>
        /// 
        /// </summary>
        private List<HitTestResult> mouseHitModels = new List<HitTestResult>();

        public bool HittedSomething(MouseEventArgs e)
        {
            var hits = this.FindHits(e.GetPosition(this));
            if (hits.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// Handles hit testing on mouse down.
        /// </summary>
        /// <param name="pt">The hit point.</param>
        /// <param name="originalInputEventArgs">
        /// The original input event for future use (which mouse button pressed?)
        /// </param>
        private void MouseDownHitTest(Point pt, InputEventArgs originalInputEventArgs = null)
        {
            var hits = this.FindHits(pt);
            if (hits.Count > 0)
            {
                // We can't capture Touch because that would disable the CameraController which uses Manipulation,
                // but since Manipulation captures touch, we can be quite sure to get every relevant touch event.
                if (this.touchDownDevice == null)
                {
                    Mouse.Capture(this, CaptureMode.SubTree);
                }

                foreach (var hit in hits.Where(x => x.IsValid))
                {
                    hit.ModelHit.RaiseEvent(new MouseDown3DEventArgs(hit.ModelHit, hit, pt, this));
                    this.mouseHitModels.Add(hit);

                    // the winner takes it all: only the nearest hit is taken!
                    break;
                }
            }
        }

        /// <summary>
        /// Handles hit testing on mouse move.
        /// </summary>
        /// <param name="pt">The hit point.</param>
        /// <param name="originalInputEventArgs">
        /// The original input event for future use (which mouse button pressed?)
        /// </param>
        private void MouseMoveHitTest(Point pt, InputEventArgs originalInputEventArgs = null)
        {
            if (mouseHitModels.Count > 0)
            {
                foreach (var hit in this.mouseHitModels)
                {
                    hit.ModelHit.RaiseEvent(new MouseMove3DEventArgs(hit.ModelHit, hit, pt, this));
                }
            }
        }

        /// <summary>
        /// Handles hit testing on mouse up.
        /// </summary>
        /// <param name="pt">The hit point.</param>
        /// <param name="originalInputEventArgs">
        /// The original input event for future use (which mouse button pressed?)
        /// </param>
        private void MouseUpHitTest(Point pt, InputEventArgs originalInputEventArgs = null)
        {
            if (mouseHitModels.Count > 0)
            {
                Mouse.Capture(this, CaptureMode.None);
                foreach (var hit in this.mouseHitModels)
                {
                    hit.ModelHit.RaiseEvent(new MouseUp3DEventArgs(hit.ModelHit, hit, pt, this));
                }

                this.mouseHitModels.Clear();
            }
        }
    }
}
