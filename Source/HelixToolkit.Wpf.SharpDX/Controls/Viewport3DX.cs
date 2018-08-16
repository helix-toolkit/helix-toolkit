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
    using System.Runtime.CompilerServices;
    using Vector3 = System.Numerics.Vector3;

    /// <summary>
    /// Provides a Viewport control.
    /// </summary>
    [ContentProperty("Items")]
    [TemplatePart(Name = "PART_Canvas", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(AdornerDecorator))]
    [TemplatePart(Name = "PART_CoordinateView", Type = typeof(Viewport3D))]
    [TemplatePart(Name = "PART_ViewCube", Type = typeof(Viewport3D))]
    [TemplatePart(Name = "PART_FrameStatisticView", Type = typeof(Viewport3D))]
    [TemplatePart(Name = "PART_TitleView", Type = typeof(StackPanel2D))]
    [TemplatePart(Name = "PART_Items", Type = typeof(ItemsControl))]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public partial class Viewport3DX : Control, IViewport3DX
    {
        /// <summary>
        /// The adorner layer part name.
        /// </summary>
        private const string PartAdornerLayer = "PART_AdornerLayer";

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
        /// The part items used to inherit datacontext for children
        /// </summary>
        private const string PartItems = "PART_Items";

        /// <summary>
        /// The orthographic camera.
        /// </summary>
        private readonly Camera orthographicCamera;

        /// <summary>
        /// The perspective camera.
        /// </summary>
        private readonly Camera perspectiveCamera;

        /// <summary>
        /// The camera controller.
        /// </summary>
        private readonly CameraController cameraController;

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

        private ItemsControl partItemsControl;
        /// <summary>
        /// Fired whenever an exception occurred at rendering subsystem.
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> RenderExceptionOccurred = delegate { };

        /// <summary>
        /// Get current render context
        /// </summary>
        public RenderContext RenderContext { get { return this.renderHostInternal?.RenderContext; } }

        public ObservableElement3DCollection Items { get; } = new ObservableElement3DCollection();
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
                    foreach (Element3D item in Items)
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
                    foreach (Element3D item in Items)
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
                yield return Overlay2D.SceneNode;
                yield return frameStatisticModel.SceneNode;
            }
        }

        public CameraCore CameraCore { get { return this.Camera; } }

        public IRenderHost RenderHost { get { return this.renderHostInternal; } }

        private Window parentWindow;

        private Overlay Overlay2D { get; } = new Overlay() { EnableBitmapCache = true };
        private bool enableMouseButtonHitTest = true;
        private ContentPresenter hostPresenter;
        /// <summary>
        /// Occurs when each render frame finished rendering. Called directly from RenderHost after each frame. 
        /// Use this event carefully. Unsubscrible this event when not used. Otherwise may cause performance issue.
        /// </summary>
        public event EventHandler OnRendered;
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
            this.cameraController = new CameraController(this);
            Items.CollectionChanged += Items_CollectionChanged;
            this.perspectiveCamera = new PerspectiveCamera();
            this.orthographicCamera = new OrthographicCamera();
            this.perspectiveCamera.Reset();
            this.orthographicCamera.Reset();

            this.Camera = this.Orthographic ? this.orthographicCamera : this.perspectiveCamera;
         
            InitCameraController();
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.ZoomExtents, this.ZoomExtentsHandler));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.SetTarget, this.cameraController.setTargetHandler.Execute));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.Reset, this.ResetHandler));

            this.CommandBindings.Add(new CommandBinding(ViewportCommands.Zoom, this.cameraController.zoomHandler.Execute));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.Pan, this.cameraController.panHandler.Execute));
            this.CommandBindings.Add(new CommandBinding(ViewportCommands.Rotate, this.cameraController.rotateHandler.Execute));
            this.CommandBindings.Add(
                new CommandBinding(ViewportCommands.ChangeFieldOfView, this.cameraController.changeFieldOfViewHandler.Execute));
            this.CommandBindings.Add(
                new CommandBinding(ViewportCommands.ZoomRectangle, this.cameraController.zoomRectangleHandler.Execute));
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

        private void InitCameraController()
        {
            #region Assign Defaults
            this.cameraController.ActualCamera = this.Camera;
            this.cameraController.DefaultCamera = this.DefaultCamera;
            this.cameraController.CameraMode = this.CameraMode;
            this.cameraController.CameraRotationMode = this.CameraRotationMode;
            this.cameraController.PageUpDownZoomSensitivity = this.PageUpDownZoomSensitivity;
            this.cameraController.PanCursor = this.PanCursor;
            this.cameraController.RotateAroundMouseDownPoint = this.RotateAroundMouseDownPoint;
            this.cameraController.RotateCursor = this.RotateCursor;
            this.cameraController.RotationSensitivity = this.RotationSensitivity;
            this.cameraController.ShowCameraTarget = this.ShowCameraTarget;
            this.cameraController.SpinReleaseTime = this.SpinReleaseTime;
            this.cameraController.UpDownPanSensitivity = this.UpDownPanSensitivity;
            this.cameraController.UpDownRotationSensitivity = this.UpDownRotationSensitivity;
            this.cameraController.ZoomAroundMouseDownPoint = this.ZoomAroundMouseDownPoint;
            this.cameraController.ZoomCursor = this.ZoomCursor;
            this.cameraController.ZoomRectangleCursor = this.ZoomRectangleCursor;
            this.cameraController.ZoomSensitivity = this.ZoomSensitivity;
            this.cameraController.ChangeFieldOfViewCursor = this.ChangeFieldOfViewCursor;
            this.cameraController.InertiaFactor = this.CameraInertiaFactor;
            this.cameraController.InfiniteSpin = this.InfiniteSpin;
            this.cameraController.IsChangeFieldOfViewEnabled = this.IsChangeFieldOfViewEnabled;
            this.cameraController.IsInertiaEnabled = this.IsInertiaEnabled;
            this.cameraController.IsMoveEnabled = this.IsMoveEnabled;
            this.cameraController.IsPanEnabled = this.IsPanEnabled;
            this.cameraController.IsRotationEnabled = this.IsRotationEnabled;
            this.cameraController.EnableTouchRotate = this.IsTouchRotateEnabled;
            this.cameraController.EnablePinchZoom = this.IsPinchZoomEnabled;
            this.cameraController.EnableThreeFingerPan = this.IsThreeFingerPanningEnabled;
            this.cameraController.LeftRightPanSensitivity = this.LeftRightPanSensitivity;
            this.cameraController.LeftRightRotationSensitivity = this.LeftRightRotationSensitivity;
            this.cameraController.MaximumFieldOfView = this.MaximumFieldOfView;
            this.cameraController.MinimumFieldOfView = this.MinimumFieldOfView;
            this.cameraController.ModelUpDirection = this.ModelUpDirection.ToVector3();
            this.cameraController.ZoomDistanceLimitFar = this.ZoomDistanceLimitFar;
            this.cameraController.ZoomDistanceLimitNear = this.ZoomDistanceLimitNear;
            this.cameraController.FixedRotationPoint = this.FixedRotationPoint.ToVector3();
            this.cameraController.FixedRotationPointEnabled = this.FixedRotationPointEnabled;
            #endregion
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach(var item in e.OldItems)
                {
                    partItemsControl?.Items.Remove(item);
                    if(item is Element3D element)
                    {
                        element.SceneNode.Detach();
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach(var item in e.NewItems)
                {
                    partItemsControl?.Items.Add(item);
                    if(this.IsAttached && item is Element3D element)
                    {
                        element.SceneNode.Attach(renderHostInternal);
                    }
                }
            }
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
            this.cameraController.AddMoveForce(new Vector3((float)dx, (float)dy, (float)dz));
        }

        /// <summary>
        /// Adds the specified move force.
        /// </summary>
        /// <param name="delta">
        /// The delta. 
        /// </param>
        public void AddMoveForce(Vector3D delta)
        {
            this.cameraController.AddMoveForce(delta.ToVector3());
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
            this.cameraController.AddPanForce((float)dx, (float)dy);
        }

        /// <summary>
        /// The add pan force.
        /// </summary>
        /// <param name="pan">
        /// The pan. 
        /// </param>
        public void AddPanForce(Vector3D pan)
        {
            this.cameraController.AddPanForce(pan.ToVector3());
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
            this.cameraController.AddRotateForce((float)dx, (float)dy);
        }

        /// <summary>
        /// Adds the zoom force.
        /// </summary>
        /// <param name="dx">
        /// The delta. 
        /// </param>
        public void AddZoomForce(double dx)
        {
            this.cameraController.AddZoomForce((float)dx);
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
            this.cameraController.AddZoomForce((float)dx, zoomOrigin.ToVector3());
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
            this.cameraController.ChangeDirection(lookDir, upDir, animationTime);
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
        public Vector3? FindNearestPoint(Point pt)
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
            if (!(this.hostPresenter is Visual visual))
            {
                return;
            }

            var myAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
            if (this.targetAdorner != null)
            {
                myAdornerLayer.Remove(this.targetAdorner);
            }

            this.targetAdorner = null;

            InvalidateRender();
        }

        /// <summary>
        /// Hides the zoom rectangle.
        /// </summary>
        public void HideZoomRectangle()
        {
            if (!(this.hostPresenter is Visual visual))
            {
                return;
            }

            var myAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
            if (this.rectangleAdorner != null)
            {
                myAdornerLayer.Remove(this.rectangleAdorner);
            }

            this.rectangleAdorner = null;

            InvalidateRender();
        }

        /// <summary>
        /// Tries to invalidate the current render.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvalidateRender()
        {
            renderHostInternal?.InvalidateRender();
        }
        /// <summary>
        /// Invalidates the scene graph.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvalidateSceneGraph()
        {
            renderHostInternal?.InvalidateSceneGraph();
        }

        /// <summary>
        /// Change the camera to look at the specified point.
        /// </summary>
        /// <param name="p">
        /// The point.
        /// </param>
        public void LookAt(Point3D p)
        {
            this.cameraController.ActualCamera?.LookAt(p, 0);
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
            this.cameraController.ActualCamera?.LookAt(p, animationTime);
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
            this.cameraController.ActualCamera?.LookAt(p, distance, animationTime);
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
            this.cameraController.ActualCamera?.LookAt(p, direction, animationTime);
        }


        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        /// <exception cref="HelixToolkitException">{0} is missing from the template.</exception>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (IsInDesignMode && !EnableDesignModeRendering)
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
                this.renderHostInternal.OnRendered -= this.OnRendered;
                this.renderHostInternal.OnRendered += this.OnRendered;
                this.renderHostInternal.ExceptionOccurred -= this.HandleRenderException;
                this.renderHostInternal.ExceptionOccurred += this.HandleRenderException;
                this.renderHostInternal.ClearColor = BackgroundColor.ToColor4();
                this.renderHostInternal.IsShadowMapEnabled = IsShadowMappingEnabled;
                this.renderHostInternal.MSAA = this.MSAA;
                this.renderHostInternal.EnableRenderFrustum = this.EnableRenderFrustum;
                this.renderHostInternal.EnableSharingModelMode = this.EnableSharedModelMode;
                this.renderHostInternal.SharedModelContainer = this.SharedModelContainer;
                this.renderHostInternal.Viewport = this;
                this.renderHostInternal.EffectsManager = this.EffectsManager;
                this.renderHostInternal.IsRendering = this.Visibility == System.Windows.Visibility.Visible;
                this.renderHostInternal.RenderConfiguration.RenderD2D = EnableD2DRendering;
                this.renderHostInternal.RenderConfiguration.AutoUpdateOctree = EnableAutoOctreeUpdate;
                this.renderHostInternal.RenderConfiguration.EnableOITRendering = EnableOITRendering;
                this.renderHostInternal.RenderConfiguration.OITWeightPower = (float)OITWeightPower;
                this.renderHostInternal.RenderConfiguration.OITWeightDepthSlope = (float)OITWeightDepthSlope;
                this.renderHostInternal.RenderConfiguration.OITWeightMode = OITWeightMode;
                this.renderHostInternal.RenderConfiguration.FXAALevel = FXAALevel;
                this.renderHostInternal.RenderConfiguration.EnableRenderOrder = EnableRenderOrder;
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
            if(this.frameStatisticModel == null)
            {
                this.frameStatisticModel = this.Template.FindName(PartFrameStatisticView, this) as FrameStatisticsModel2D;
            }
            if(this.frameStatisticModel == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartFrameStatisticView);
            }
            if(this.partItemsControl == null)
            {
                this.partItemsControl = this.Template.FindName(PartItems, this) as ItemsControl;
            }
            if (this.partItemsControl == null)
            {
                throw new HelixToolkitException("{0} is missing from the template.", PartItems);
            }
            else
            {
                foreach (var item in Items)
                {
                    partItemsControl.Items.Remove(item);
                }
                foreach (var item in Items)
                {
                    partItemsControl.Items.Add(item);
                }
            }
            Overlay2D.Children.Clear();
            this.RemoveLogicalChild(Overlay2D);
            this.AddLogicalChild(Overlay2D);
            var titleView = Template.FindName(PartTitleView, this);
            if (titleView is Element2D element)
            {
                Overlay2D.Children.Add(element);
            }
            if(viewCube != null)
            {
                Overlay2D.Children.Add(viewCube.MoverCanvas);
            }
            if(coordinateView != null)
            {
                Overlay2D.Children.Add(coordinateView.MoverCanvas);
            }
            if (Content2D != null)
            {
                Overlay2D.Children.Add(Content2D);
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

            if (this.DefaultCamera != null)
            {
                this.DefaultCamera.CopyTo(this.Camera);
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
            Camera.AnimateTo(newPosition, newDirection, newUpDirection, animationTime);
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

            if (!(this.hostPresenter is UIElement visual))
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
                rectangleAdorner.Rectangle = rect;
                return;
            }

            if (!(this.hostPresenter is UIElement visual))
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
            cameraController.StartSpin(speed.ToVector2(), position, aroundPoint.ToVector3());
        }

        /// <summary>
        ///   Stops the spinning.
        /// </summary>
        public void StopSpin()
        {
            cameraController.StopSpin();
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
        public void ZoomExtents(Mathematics.BoundingBox bounds, double animationTime = 0)
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
                SharedModelContainerInternal?.Attach(host);
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
                SharedModelContainerInternal?.Detach();
                foreach(var e in this.D2DRenderables)
                {
                    e.Detach();
                }
            }
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

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            this.cameraController.OnMouseDown(e);
            base.OnMouseDown(e);
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
                    this.CurrentPosition = pos.Value.ToPoint3D();
                }
                else
                {
                    var p = this.UnProjectOnPlane(pt);
                    if (p != null)
                    {
                        this.CurrentPosition = p.Value.ToPoint3D();
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

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            cameraController.OnMouseWheel(this, e);
            base.OnMouseWheel(e);
        }

        private void Viewport3DX_FormMouseWheel(object sender, WinformHostExtend.FormMouseWheelEventArgs e)
        {
            cameraController.OnMouseWheel(this, e);
            base.OnMouseWheel(e);
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

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            cameraController.OnManipulationStarted(e);
            base.OnManipulationStarted(e);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            cameraController.OnManipulationDelta(e);
            base.OnManipulationDelta(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            cameraController.OnKeyDown(e);
            base.OnKeyDown(e);
        }

        /// <inheritdoc/>
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            cameraController.OnManipulationCompleted(e);
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
        private void CameraPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (CameraController.ActualCamera != null)
                {
                    CameraController.ActualCamera.CameraInternal.PropertyChanged -= CameraInternal_PropertyChanged;
                }
                CameraController.ActualCamera = e.NewValue == null ?
                    (Orthographic ? orthographicCamera : perspectiveCamera as Camera) : e.NewValue as Camera;
                if (CameraController.ActualCamera != null)
                {
                    CameraController.ActualCamera.CameraInternal.PropertyChanged += CameraInternal_PropertyChanged;
                }
            }
        }

        private void CameraInternal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateRender();
            // Raise notification
            this.RaiseCameraChangedEvent();
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

            if (this.ZoomExtentsWhenLoaded)
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() => 
                {
                    this.ZoomExtents();
                }));              
            }
            if (EnableSwapChainRendering)
            {
                FormMouseMove += Viewport3DX_FormMouseMove;
                FormMouseWheel += Viewport3DX_FormMouseWheel;
            }
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
            FormMouseWheel -= Viewport3DX_FormMouseWheel;
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

            if (oldCamera is IProjectionCameraModel projectionCamera)
            {
                projectionCamera.CopyTo(this.Camera);
            }
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
            var target = cameraController.ActualCamera.Position + cameraController.ActualCamera.LookDirection;
            double distance = cameraController.ActualCamera.LookDirection.Length;
            e.LookDirection *= distance;
            var newPosition = target - e.LookDirection;
            cameraController.ActualCamera.AnimateTo(newPosition, e.LookDirection, e.UpDirection, 500);
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
            if (Overlay2D.HitTest(pt.ToVector2(), out currentHit2D))
            {
                if(currentHit2D.ModelHit is Element2D e)
                {
                    e.RaiseEvent(new Mouse2DEventArgs(Element2D.MouseDown2DEvent, currentHit2D.ModelHit, currentHit2D, pt, this, originalInputEventArgs));
                    originalInputEventArgs.Handled = true;
                }
                return;
            }
            if (ViewBoxHitTest(pt))
            {
                originalInputEventArgs.Handled = true;
                return;
            }
            if (!enableMouseButtonHitTest)
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
                currentHit = null;
                // Raise event from Viewport3DX if there's no hit
                this.RaiseEvent(new MouseDown3DEventArgs(this, null, pt, this));
            }
        }

        private bool ViewBoxHitTest(Point p)
        {
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
            if (Overlay2D.HitTest(pt.ToVector2(), out HitTest2DResult hit2D))
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
            if (enableMouseButtonHitTest)
            {
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
            if (enableMouseButtonHitTest)
            {
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
        }

        public static T FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(obj);
                while (parent != null)
                {
                    if (parent is T typed)
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
