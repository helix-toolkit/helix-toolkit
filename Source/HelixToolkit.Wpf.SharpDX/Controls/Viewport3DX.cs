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
    using Controls;
    using Elements2D;
    using Cameras;
    using Utilities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;
    using MouseButtons = System.Windows.Forms.MouseButtons;
    using Model;
    using Model.Scene;
    using Model.Scene2D;
    /// <summary>
    /// Provides a Viewport control.
    /// </summary>
    [DefaultEvent("OnChildrenChanged")]
    //[DefaultProperty("Children")]
    [ContentProperty("Items")]
    [TemplatePart(Name = "PART_CameraController", Type = typeof(CameraController))]
    [TemplatePart(Name = "PART_Canvas", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(AdornerDecorator))]
    [TemplatePart(Name = "PART_CoordinateView", Type = typeof(Viewport3D))]
    [TemplatePart(Name = "PART_ViewCube", Type = typeof(Viewport3D))]
    [TemplatePart(Name = "PART_FrameStatisticView", Type = typeof(Viewport3D))]
    [TemplatePart(Name = "PART_TitleView", Type = typeof(StackPanel2D))]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public partial class Viewport3DX : ItemsControl, IViewport3DX
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
        /// The frame statistic view part name
        /// </summary>
        private const string PartFrameStatisticView = "PART_FrameStatisticView";

        /// <summary>
        /// The part title view
        /// </summary>
        private const string PartTitleView = "PART_TitleView";


        /// <summary>
        /// The change field of view handler
        /// </summary>
        private readonly ZoomHandler changeFieldOfViewHandler;

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
        /// The rotate handler
        /// </summary>
        private readonly RotateHandler rotateHandler;

        /// <summary>
        /// The set target handler.
        /// </summary>
        private RotateHandler setTargetHandler;

        /// <summary>
        /// The zoom handler
        /// </summary>
        private readonly ZoomHandler zoomHandler;

        /// <summary>
        /// The zoom rectangle handler
        /// </summary>
        private readonly ZoomRectangleHandler zoomRectangleHandler;

        /// <summary>
        /// The camera controller.
        /// </summary>
        private CameraController cameraController;

        /// <summary>
        /// The coordinate view.
        /// </summary>
        private ScreenSpacedElement3D coordinateView;

        /// <summary>
        /// The nearest valid result during a hit test.
        /// </summary>
        private HitTestResult currentHit;

        /// <summary>
        /// Current 2D model hit
        /// </summary>
        private HitTest2DResult currentHit2D;

        private Element2D mouseOverModel2D;
        public Element2D MouseOverModel2D
        {
            private set
            {
                if (mouseOverModel2D == value) { return; }
                mouseOverModel2D?.RaiseEvent(new Mouse2DEventArgs(Element2D.MouseLeave2DEvent, mouseOverModel2D, this));
                mouseOverModel2D = value;
                mouseOverModel2D?.RaiseEvent(new Mouse2DEventArgs(Element2D.MouseEnter2DEvent, mouseOverModel2D, this));
            }
            get { return mouseOverModel2D; }
        }

        /// <summary>
        /// The "control has been loaded before" flag.
        /// </summary>
        private bool hasBeenLoadedBefore;

        /// <summary>
        /// The is subscribed to rendering event.
        /// </summary>
        private bool isSubscribedToRenderingEvent;

        /// <summary>
        ///   The rectangle adorner.
        /// </summary>
        private RectangleAdorner rectangleAdorner;

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
        private ScreenSpacedElement3D viewCube;

        private FrameStatisticsModel2D frameStatisticModel;

        /// <summary>
        /// Fired whenever an exception occurred at rendering subsystem.
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> RenderExceptionOccurred = delegate { };

        /// <summary>
        /// Get current render context
        /// </summary>
        public IRenderContext RenderContext { get { return this.renderHostInternal?.RenderContext; } }

        /// <summary>
        /// <para>Return enumerable of all the rederable elements</para>
        /// <para>If enabled shared model mode, the returned rederables are current viewport renderable plus shared models</para>
        /// </summary>
        public IEnumerable<SceneNode> Renderables
        {
            get
            {
                if (renderHostInternal != null)
                {
                    foreach (Element3DCore item in Items)
                    {
                        yield return item.SceneNode;
                    }
                    if (renderHostInternal.EnableSharingModelMode && renderHostInternal.SharedModelContainer != null)
                    {
                        foreach (var item in renderHostInternal.SharedModelContainer.Renderables)
                        {
                            yield return item;
                        }
                    }
                    yield return viewCube.SceneNode;
                    yield return coordinateView.SceneNode;
                }
            }
        }

        private IEnumerable<SceneNode> OwnedRenderables
        {
            get
            {
                if (renderHostInternal != null)
                {
                    foreach (Element3DCore item in Items)
                    {
                        yield return item.SceneNode;
                    }
                    yield return viewCube.SceneNode;
                    yield return coordinateView.SceneNode;
                }
            }
        }

        public IEnumerable<SceneNode2D> D2DRenderables
        {
            get
            {
                yield return overlay2D.SceneNode;
                yield return frameStatisticModel.SceneNode;
            }
        }

        public CameraCore CameraCore { get { return this.Camera; } }

        public IRenderHost RenderHost { get { return this.renderHostInternal; } }

        private Window parentWindow;

        private Overlay overlay2D { get; } = new Overlay() { EnableBitmapCache = true };

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
            this.setTargetHandler = new RotateHandler(this, true);
            this.panHandler = new PanHandler(this);
            this.zoomHandler = new ZoomHandler(this);
            this.changeFieldOfViewHandler = new ZoomHandler(this, true);
            this.zoomRectangleHandler = new ZoomRectangleHandler(this);

            this.CommandBindings.Add(new CommandBinding(ViewportCommands.ZoomExtents, this.ZoomExtentsHandler));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.SetTarget, this.setTargetHandler.Execute));
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

            this.Loaded += this.ControlLoaded;
            this.Unloaded += this.ControlUnloaded;
            this.IsVisibleChanged += (d, e) =>
            {
                if (renderHostInternal != null)
                {
                    renderHostInternal.IsRendering = (bool)e.NewValue;
                }
            };
            AddHandler(ViewBoxModel3D.ViewBoxClickedEvent, new EventHandler<ViewBoxModel3D.ViewBoxClickedEventArgs>(ViewCubeClicked));
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
            this.cameraController?.AddMoveForce(new Vector3D(dx, dy, dz));
            // this.AddMoveForce(new Vector3D(dx, dy, dz));
        }

        /// <summary>
        /// Adds the specified move force.
        /// </summary>
        /// <param name="delta">
        /// The delta. 
        /// </param>
        public void AddMoveForce(Vector3D delta)
        {
            this.cameraController?.AddMoveForce(delta);
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
            this.cameraController?.AddPanForce(dx, dy);
        }

        /// <summary>
        /// The add pan force.
        /// </summary>
        /// <param name="pan">
        /// The pan. 
        /// </param>
        public void AddPanForce(Vector3D pan)
        {
            this.cameraController?.AddPanForce(pan);
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
            this.cameraController?.AddRotateForce(dx, dy);
        }

        /// <summary>
        /// Adds the zoom force.
        /// </summary>
        /// <param name="dx">
        /// The delta. 
        /// </param>
        public void AddZoomForce(double dx)
        {
            this.cameraController?.AddZoomForce(dx);
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
            this.cameraController?.AddZoomForce(dx, zoomOrigin);
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
            this.cameraController?.ChangeDirection(lookDir, upDir, animationTime);
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
        public static bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                return (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
            }
        }
        /// <summary>
        /// Hides the target adorner.
        /// </summary>
        public void HideTargetAdorner()
        {
            var visual = this.hostPresenter as Visual;
            if (visual == null)
            {
                return;
            }

            var myAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
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
            var visual = this.hostPresenter as Visual;
            if (visual == null)
            {
                return;
            }

            var myAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
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
            var rh = this.renderHostInternal;
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
            this.cameraController?.ActualCamera?.LookAt(p, 0);
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
            this.cameraController?.ActualCamera?.LookAt(p, animationTime);
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
            this.cameraController?.ActualCamera?.LookAt(p, distance, animationTime);
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
            this.cameraController?.ActualCamera?.LookAt(p, direction, animationTime);
        }

        private ContentPresenter hostPresenter;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        /// <exception cref="HelixToolkitException">{0} is missing from the template.</exception>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (IsInDesignMode)
            { return; }
            Disposer.RemoveAndDispose(ref renderHostInternal);
            hostPresenter = this.GetTemplateChild("PART_Canvas") as ContentPresenter;

            if (EnableSwapChainRendering)
            {
                double dpiXScale = 1;
                double dpiYScale = 1;
                PresentationSource source = PresentationSource.FromVisual(this);
                if (source != null)
                {
                    dpiXScale = 1.0 / source.CompositionTarget.TransformToDevice.M11;
                    dpiYScale = 1.0 / source.CompositionTarget.TransformToDevice.M22;
                }
                hostPresenter.Content = new DPFSurfaceSwapChain(EnableDeferredRendering) { DPIXScale = dpiXScale, DPIYScale = dpiYScale };
            }
            else
            {
                hostPresenter.Content = new DPFCanvas(EnableDeferredRendering);
            }
            renderHostInternal = (hostPresenter.Content as IRenderCanvas).RenderHost;
            if (this.renderHostInternal != null)
            {
                this.renderHostInternal.ClearColor = BackgroundColor.ToColor4();
                this.renderHostInternal.IsShadowMapEnabled = IsShadowMappingEnabled;
                this.renderHostInternal.MSAA = this.MSAA;
                this.renderHostInternal.EnableRenderFrustum = this.EnableRenderFrustum;
                this.renderHostInternal.EnableSharingModelMode = this.EnableSharedModelMode;
                this.renderHostInternal.SharedModelContainer = this.SharedModelContainer;
                this.renderHostInternal.ExceptionOccurred += this.HandleRenderException;
                this.renderHostInternal.Viewport = this;
                this.renderHostInternal.EffectsManager = this.EffectsManager;
                this.renderHostInternal.IsRendering = this.Visibility == System.Windows.Visibility.Visible;
                this.renderHostInternal.RenderConfiguration.RenderD2D = EnableD2DRendering;
                this.renderHostInternal.RenderConfiguration.AutoUpdateOctree = EnableAutoOctreeUpdate;
                if (ShowFrameRate)
                {
                    this.renderHostInternal.ShowRenderDetail |= RenderDetail.FPS;
                }
                else
                {
                    this.renderHostInternal.ShowRenderDetail &= ~RenderDetail.FPS;
                }
                if (ShowFrameDetails)
                {
                    this.renderHostInternal.ShowRenderDetail |= RenderDetail.Statistics;
                }
                else
                {
                    this.renderHostInternal.ShowRenderDetail &= ~RenderDetail.Statistics;
                }
                if (ShowTriangleCountInfo)
                {
                    this.renderHostInternal.ShowRenderDetail |= RenderDetail.TriangleInfo;
                }
                else
                {
                    this.renderHostInternal.ShowRenderDetail &= ~RenderDetail.TriangleInfo;
                }
                if (ShowCameraInfo)
                {
                    this.renderHostInternal.ShowRenderDetail |= RenderDetail.Camera;
                }
                else
                {
                    this.renderHostInternal.ShowRenderDetail &= ~RenderDetail.Camera;
                }
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
                this.coordinateView = this.Template.FindName(PartCoordinateView, this) as ScreenSpacedElement3D;
            }
            if (this.coordinateView == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartCoordinateView);
            }
            if (this.viewCube == null)
            {
                this.viewCube = this.Template.FindName(PartViewCube, this) as ScreenSpacedElement3D;
            }
            if (this.viewCube == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartViewCube);
            }
            else
            {
                this.viewCube.RelativeScreenLocationX = this.ViewCubeHorizontalPosition;
                this.viewCube.RelativeScreenLocationY = this.ViewCubeVerticalPosition;
            }
            if(this.frameStatisticModel == null)
            {
                this.frameStatisticModel = this.Template.FindName(PartFrameStatisticView, this) as FrameStatisticsModel2D;
            }
            if(this.frameStatisticModel == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartFrameStatisticView);
            }

            overlay2D.Children.Clear();
            this.RemoveLogicalChild(overlay2D);
            this.AddLogicalChild(overlay2D);
            var titleView = Template.FindName(PartTitleView, this);
            if (titleView is Element2D element)
            {
                overlay2D.Children.Add(element);
            }
            if(viewCube != null)
            {
                overlay2D.Children.Add(viewCube.MoverCanvas);
            }
            if(coordinateView != null)
            {
                overlay2D.Children.Add(coordinateView.MoverCanvas);
            }
            if (Content2D != null)
            {
                overlay2D.Children.Add(Content2D);
            }
            // update the coordinateview camera
            this.OnCameraChanged();
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

            var visual = this.hostPresenter as UIElement;
            if (visual == null)
            {
                return;
            }

            var myAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
            this.targetAdorner = new TargetSymbolAdorner(visual, position);
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

            var visual = this.hostPresenter as UIElement;
            if (visual == null)
            {
                return;
            }

            var myAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
            this.rectangleAdorner = new RectangleAdorner(
                visual, rect, Colors.LightGray, Colors.Black, 3, 1, 10, DashStyles.Solid);
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
            cameraController?.StartSpin(speed, position, aroundPoint);
        }

        /// <summary>
        ///   Stops the spinning.
        /// </summary>
        public void StopSpin()
        {
            cameraController?.StopSpin();
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

            ViewportExtensions.ZoomExtents(this, animationTime);
        }

        private bool IsAttached = false;
        /// <summary>
        /// Attaches the elements to the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void Attach(IRenderHost host)
        {
            if (!IsAttached)
            {
                foreach (var e in this.OwnedRenderables)
                {
                    e.Attach(host);
                }
                sharedModelContainerInternal?.Attach(host);
                foreach(var e in this.D2DRenderables)
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
                foreach (var e in this.OwnedRenderables)
                {
                    e.Detach();
                }
                sharedModelContainerInternal?.Detach();
                foreach(var e in this.D2DRenderables)
                {
                    e.Detach();
                }
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
            this.InvalidateRender();
        }

        /// <summary>
        /// Gets the pressed mouse buttons as flags of <see cref="MouseButtons"/>.
        /// If no button is pressed (result is zero), then it was a touch down.
        /// </summary>
        /// <returns>
        /// The pressed mouse buttons as flags of <see cref="MouseButtons"/>.
        /// </returns>
        public static MouseButtons GetPressedMouseButtons()
        {
            int flags = 0;
            flags |= (int)Mouse.LeftButton << 20;
            flags |= (int)Mouse.RightButton << 21;
            flags |= (int)Mouse.MiddleButton << 22;
            flags |= (int)Mouse.XButton1 << 23;
            flags |= (int)Mouse.XButton2 << 24;
            return (MouseButtons)flags;
        }

        /// <inheritdoc/>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {            
            base.OnPreviewMouseDown(e);
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

        private void Viewport3DX_FormMouseMove(object sender, WinformHostExtend.FormMouseMoveEventArgs e)
        {
            if (this.touchDownDevice == null)
            {
                var pt = e.Location;
                this.MouseMoveHitTest(pt);
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
        /// <summary>
        /// Emulates the mouse move by touch.
        /// </summary>
        /// <param name="pt">The pt.</param>
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
        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);
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
        /// Handles the change of the effects manager.
        /// </summary>
        private void EffectsManagerPropertyChanged()
        {
            if (this.renderHostInternal != null)
            {
                this.renderHostInternal.EffectsManager = this.EffectsManager;
            }
        }

        /// <summary>
        /// Handles the change of the render technique        
        /// </summary>
        private void RenderTechniquePropertyChanged(IRenderTechnique technique)
        {
            if (this.renderHostInternal != null)
            {
                this.renderHostInternal.RenderTechnique = technique;
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
            parentWindow = FindVisualAncestor<Window>(this);
            if (parentWindow != null)
            {
                parentWindow.Closed += ParentWindow_Closed;
            }
            this.SubscribeToRenderingEvent();
            if (this.ZoomExtentsWhenLoaded)
            {
                this.ZoomExtents();
            }
            FormMouseMove += Viewport3DX_FormMouseMove;
        }

        private void ParentWindow_Closed(object sender, EventArgs e)
        {
            ControlUnloaded(sender, null);
            if (hostPresenter.Content is IDisposable d)
            {
                hostPresenter.Content = null;
                d.Dispose();
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
            FormMouseMove -= Viewport3DX_FormMouseMove;
            this.UnsubscribeRenderingEvent();
            if (parentWindow != null)
            {
                parentWindow.Closed -= ParentWindow_Closed;
            }
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
        private void OnCompositionTargetRendering()
        {
            this.FrameRate = Math.Round(renderHostInternal.RenderStatistics.FPSStatistics.AverageFrequency, 2);
            this.FrameRateText = this.FrameRate + " FPS";
        }
        /// <summary>
        /// </summary>
        /// <param name="timeStamp"></param>
        public void Update(TimeSpan timeStamp)
        {
            OnCompositionTargetRendering();
            cameraController.OnCompositionTargetRendering(timeStamp.Ticks);
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
        /// Handles a rendering exception.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleRenderException(object sender, RelayExceptionEventArgs e)
        {
            var bindingExpression = this.GetBindingExpression(RenderExceptionProperty);
            if (bindingExpression != null)
            {
                // If RenderExceptionProperty is bound, we assume the exception will be handled.
                this.RenderException = e.Exception;
                e.Handled = true;
            }

            // Fire RenderExceptionOccurred event
            this.RenderExceptionOccurred(sender, e);

            // If the Exception is still unhandled...
            if (!e.Handled)
            {
                // ... prevent a MessageBox.Show().
                this.MessageText = e.Exception.ToString();
                e.Handled = true;
            }
            hostPresenter.Content = null;
            Disposer.RemoveAndDispose(ref renderHostInternal);
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
        /// Tunnel Keydown event into camera controller.
        /// There is an issue that camera controller grid cannot get keydown event(maybe not able to get focus properly), has to manually tunnel this event into the controller.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Viewport3DX_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.cameraController != null)
            {
                this.cameraController.OnKeyDown(sender, e);
            }
        }

        /// <summary>
        /// The subscribe to rendering event.
        /// </summary>
        private void SubscribeToRenderingEvent()
        {
            if (!this.isSubscribedToRenderingEvent)
            {
                this.isSubscribedToRenderingEvent = true;
            }
            this.KeyDown += Viewport3DX_KeyDown;
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
                this.isSubscribedToRenderingEvent = false;
            }
            this.KeyDown -= Viewport3DX_KeyDown;
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

        private void ViewCubeClicked(object sender, ViewBoxModel3D.ViewBoxClickedEventArgs e)
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
           // AnimateOpacity(this.viewCubeViewport, 1.0, 200);
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
          //  AnimateOpacity(this.viewCubeViewport, this.ViewCubeOpacity, 200);
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
            if (overlay2D.HitTest(pt.ToVector2(), out currentHit2D))
            {
                if(currentHit2D.ModelHit is Element2D e)
                {
                    e.RaiseEvent(new Mouse2DEventArgs(Element2D.MouseDown2DEvent, currentHit2D.ModelHit, currentHit2D, pt, this, originalInputEventArgs));
                }
                return;
            }
            if (ViewBoxHitTest(pt))
            {
                return;
            }
            if (!EnableMouseButtonHitTest)
            {
                return;
            }

            var hits = this.FindHits(pt);
            //this.CameraPropertyChanged();
            if (hits.Count > 0)
            {
                // We can't capture Touch because that would disable the CameraController which uses Manipulation,
                // but since Manipulation captures touch, we can be quite sure to get every relevant touch event.
                if (this.touchDownDevice == null)
                {
                    Mouse.Capture(this, CaptureMode.SubTree);
                }

                this.currentHit = hits.FirstOrDefault(x => x.IsValid);
                if (this.currentHit != null)
                {
                    (this.currentHit.ModelHit as Element3D)?.RaiseEvent(
                        new MouseDown3DEventArgs(this.currentHit.ModelHit, this.currentHit, pt, this));
                }
            }
            else
            {
                // Raise event from Viewport3DX if there's no hit
                this.RaiseEvent(new MouseDown3DEventArgs(this, null, pt, this));
            }
        }

        private bool ViewBoxHitTest(Point p)
        {
            var camera = Camera as ProjectionCamera;
            if (camera == null)
            {
                return false;
            }

            var ray = this.UnProject(p.ToVector2());
            var hits = new List<HitTestResult>();
            if(viewCube.HitTest(RenderContext, ray, ref hits))
            {
                viewCube.RaiseEvent(new MouseDown3DEventArgs(viewCube, this.currentHit, p, this));
                return true;
            }
            else
            {
                return false;
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
            HitTest2DResult hit2D;
            if (overlay2D.HitTest(pt.ToVector2(), out hit2D))
            {
                if(hit2D.ModelHit is Element2D e)
                {
                    MouseOverModel2D = e;
                    e.RaiseEvent(new Mouse2DEventArgs(Element2D.MouseMove2DEvent, hit2D.ModelHit, hit2D, pt, this, originalInputEventArgs));
                    //Debug.WriteLine("hit 2D, name="+e.Name);
                }
                return;
            }
            else
            {
                MouseOverModel2D = null;
            }
            if (this.currentHit != null)
            {
                (this.currentHit.ModelHit as Element3D)?.RaiseEvent(
                    new MouseMove3DEventArgs(this.currentHit.ModelHit, this.currentHit, pt, this));
            }
            else
            {
                // Raise event from Viewport3DX if there's no hit
                this.RaiseEvent(new MouseMove3DEventArgs(this, null, pt, this));
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
            
            if (currentHit2D != null)
            {
                if (currentHit2D.ModelHit is Element2D element)
                {
                    element.RaiseEvent(new Mouse2DEventArgs(Element2D.MouseUp2DEvent, currentHit2D.ModelHit, currentHit2D, pt, this, originalInputEventArgs));
                }
                currentHit2D = null;
            }           
            if (this.currentHit != null)
            {               
                (this.currentHit.ModelHit as Element3D)?.RaiseEvent(
                    new MouseUp3DEventArgs(this.currentHit.ModelHit, this.currentHit, pt, this));
                this.currentHit = null;
                Mouse.Capture(this, CaptureMode.None);
            }
            else
            {
                // Raise event from Viewport3DX if there's no hit
                this.RaiseEvent(new MouseUp3DEventArgs(this, null, pt, this));
            }
        }

        public static T FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(obj);
                while (parent != null)
                {
                    var typed = parent as T;
                    if (typed != null)
                    {
                        return typed;
                    }

                    parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
                }
            }

            return null;
        }
    }
}
