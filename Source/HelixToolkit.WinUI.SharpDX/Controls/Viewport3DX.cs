using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Cameras;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.SharpDX.Model.Scene2D;
using HelixToolkit.SharpDX.Utilities;
#if WINUI
using HelixToolkit.WinUI.SharpDX.Elements2D;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using VisibilityEnum = Microsoft.UI.Xaml.Visibility;
#else
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.Wpf.SharpDX.Elements2D;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using HitTestResult = HelixToolkit.SharpDX.HitTestResult;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Provides a Viewport control.
/// </summary>
#if WINUI
[ContentProperty(Name = "Items")]
[TemplatePart(Name = ViewportPartNames.PartCanvas, Type = typeof(ContentPresenter))]
[TemplatePart(Name = ViewportPartNames.PartCoordinateView, Type = typeof(CoordinateSystemModel3D))]
[TemplatePart(Name = ViewportPartNames.PartViewCube, Type = typeof(ViewBoxModel3D))]
[TemplatePart(Name = ViewportPartNames.PartFrameStatisticView, Type = typeof(FrameStatisticsModel2D))]
[TemplatePart(Name = ViewportPartNames.PartTitleView, Type = typeof(StackPanel2D))]
[TemplatePart(Name = ViewportPartNames.PartItems, Type = typeof(HelixItemsControl))]
#else
[ContentProperty("Items")]
[TemplatePart(Name = ViewportPartNames.PartCanvas, Type = typeof(ContentPresenter))]
[TemplatePart(Name = ViewportPartNames.PartAdornerLayer, Type = typeof(AdornerDecorator))]
[TemplatePart(Name = ViewportPartNames.PartCoordinateView, Type = typeof(Viewport3D))]
[TemplatePart(Name = ViewportPartNames.PartViewCube, Type = typeof(Viewport3D))]
[TemplatePart(Name = ViewportPartNames.PartFrameStatisticView, Type = typeof(Viewport3D))]
[TemplatePart(Name = ViewportPartNames.PartTitleView, Type = typeof(StackPanel2D))]
[TemplatePart(Name = ViewportPartNames.PartItems, Type = typeof(ItemsControl))]
[Localizability(LocalizationCategory.NeverLocalize)]
#endif
public partial class Viewport3DX : Control, IViewport3DX
{
    public static bool IsInDesignMode
    {
        get
        {
#if WINUI
            return DesignMode.DesignModeEnabled;
#else
            var prop = DesignerProperties.IsInDesignModeProperty;
            return (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
#endif
        }
    }

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
    /// The nearest valid result during a hit test.
    /// </summary>
    private HitTestResult? currentHit;

    /// <summary>
    /// Gets the render host.
    /// </summary>
    /// <value>
    /// The render host.
    /// </value>
    public IRenderHost? RenderHost => this.renderHostInternal;

    /// <summary>
    /// Gets the camera core.
    /// </summary>
    /// <value>
    /// The camera core.
    /// </value>
    public CameraCore? CameraCore => this.cameraController.ActualCamera;

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>
    /// The items.
    /// </value>
    public ObservableElement3DCollection Items { get; } = new();

#if WINUI
    /// <summary>
    /// Gets the observable collection of <see cref="InputBinding"/>.
    /// </summary>
    public InputBindingCollection InputBindings { get; } = new();

    public ManipulationBindingCollection ManipulationBindings { get; } = new();
#endif

    /// <summary>
    /// <para>Return enumerable of all the rederable elements</para>
    /// <para>If enabled shared model mode, the returned rederables are current viewport renderable plus shared models</para>
    /// </summary>
    public IEnumerable<SceneNode> Renderables
    {
        get
        {
            foreach (Element3D item in Items)
            {
                yield return item.SceneNode;
            }

            if (renderHostInternal is not null && renderHostInternal.EnableSharingModelMode && renderHostInternal.SharedModelContainer is not null)
            {
                foreach (var item in renderHostInternal.SharedModelContainer.Renderables)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    yield return item;
                }
            }

            if (viewCube is not null)
            {
                yield return viewCube.SceneNode;
            }

            if (coordinateView is not null)
            {
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

                if (viewCube is not null)
                {
                    yield return viewCube.SceneNode;
                }

                if (coordinateView is not null)
                {
                    yield return coordinateView.SceneNode;
                }
            }
        }
    }

    /// <summary>
    /// Gets the d2d renderables.
    /// </summary>
    /// <value>
    /// The d2d renderables.
    /// </value>
    public IEnumerable<SceneNode2D> D2DRenderables
    {
        get
        {
            yield return Overlay2D.SceneNode;

            if (frameStatisticModel is not null)
            {
                yield return frameStatisticModel.SceneNode;
            }
        }
    }

    /// <summary>
    /// Get current render context
    /// </summary>
    public RenderContext? RenderContext => this.renderHostInternal?.RenderContext;

    public Rectangle ViewportRectangle => new Rectangle(0, 0, (int)ActualWidth, (int)ActualHeight);

    /// <summary>
    /// Current 2D model hit
    /// </summary>
    private HitTest2DResult? currentHit2D;

    private Element2D? mouseOverModel2D;
    public Element2D? MouseOverModel2D
    {
        private set
        {
            if (mouseOverModel2D == value)
            {
                return;
            }

            mouseOverModel2D?.RaiseEvent(new Mouse2DEventArgs(Element2D.MouseLeave2DEvent, mouseOverModel2D, this));
            mouseOverModel2D = value;
            mouseOverModel2D?.RaiseEvent(new Mouse2DEventArgs(Element2D.MouseEnter2DEvent, mouseOverModel2D, this));
        }
        get
        {
            return mouseOverModel2D;
        }
    }

    /// <summary>
    /// The "control has been loaded before" flag.
    /// </summary>
    private bool hasBeenLoadedBefore;

#if WPF
    /// <summary>
    ///   The rectangle adorner.
    /// </summary>
    private RectangleAdorner? rectangleAdorner;

    /// <summary>
    ///   The target adorner.
    /// </summary>
    private Adorner? targetAdorner;

    /// <summary>
    /// The <see cref="TouchDevice"/> of the first TouchDown.
    /// </summary>
    private TouchDevice? touchDownDevice;
#endif

    /// <summary>
    /// Gets or sets the render host internal.
    /// </summary>
    /// <value>
    /// The render host internal.
    /// </value>
    protected IRenderHost? renderHostInternal;

    private bool IsAttached = false;

    /// <summary>
    /// The view cube.
    /// </summary>
#if WINUI
    private ViewBoxModel3D? viewCube;
#else
    private ScreenSpacedElement3D? viewCube;
#endif

    /// <summary>
    /// The coordinate view.
    /// </summary>
#if WINUI
    private CoordinateSystemModel3D? coordinateView;
#else
    private ScreenSpacedElement3D? coordinateView;
#endif

    private FrameStatisticsModel2D? frameStatisticModel;

    private ItemsControl? partItemsControl;

    private bool enableMouseButtonHitTest = true;

    private Overlay Overlay2D { get; } = new Overlay() { EnableBitmapCache = true };

    internal CameraController CameraController => cameraController;

    private ContentPresenter? hostPresenter;

    /// <summary>
    /// The nearest valid result during a hit test.
    /// </summary>
    private List<HitTestResult> hits = new();

    /// <summary>
    /// Gets or sets the shared model container internal.
    /// </summary>
    /// <value>
    /// The shared model container internal.
    /// </value>
    protected IModelContainer? SharedModelContainerInternal { private set; get; } = null;

    private long visibilityCallbackToken;

    /// <summary>
    /// Occurs when each render frame finished rendering. Called directly from RenderHost after each frame. 
    /// Use this event carefully. Unsubscrible this event when not used. Otherwise may cause performance issue.
    /// </summary>
    public event EventHandler? OnRendered;

#if WPF
    /// <summary>
    /// Initializes static members of the <see cref="Viewport3DX" /> class.
    /// </summary>
    static Viewport3DX()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Viewport3DX), new FrameworkPropertyMetadata(typeof(Viewport3DX)));
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="Viewport3DX" /> class.
    /// </summary>
    public Viewport3DX()
    {
#if WINUI
        this.DefaultStyleKey = typeof(Viewport3DX);
#endif

        this.cameraController = new CameraController(this);
        this.Items.CollectionChanged += Items_CollectionChanged;
        this.perspectiveCamera = new PerspectiveCamera();
        this.orthographicCamera = new OrthographicCamera();
        this.perspectiveCamera.Reset();
        this.orthographicCamera.Reset();

        this.Camera = this.Orthographic ? this.orthographicCamera : this.perspectiveCamera;

        InitCameraController();
        SetDefaultGestures();
#if WINUI
        InputController = new InputController();
#endif

        this.Loaded += this.ControlLoaded;
        this.Unloaded += this.ControlUnloaded;

#if WPF
        this.IsVisibleChanged += (d, e) =>
        {
            if (renderHostInternal != null)
            {
                renderHostInternal.IsRendering = (bool)e.NewValue;
            }
        };
#endif
    }

    private void SetupBindings()
    {
#if WINUI
        var binding = new Binding() { Source = this, Path = new PropertyPath("ShowViewCube") };
        BindingOperations.SetBinding(viewCube, ViewBoxModel3D.IsRenderingProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("ViewCubeHorizontalPosition") };
        BindingOperations.SetBinding(viewCube, ViewBoxModel3D.RelativeScreenLocationXProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("ViewCubeVerticalPosition") };
        BindingOperations.SetBinding(viewCube, ViewBoxModel3D.RelativeScreenLocationYProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("ViewCubeTexture") };
        BindingOperations.SetBinding(viewCube, ViewBoxModel3D.ViewBoxTextureProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("ViewCubeSize") };
        BindingOperations.SetBinding(viewCube, ViewBoxModel3D.SizeScaleProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("IsViewCubeEdgeClicksEnabled") };
        BindingOperations.SetBinding(viewCube, ViewBoxModel3D.EnableEdgeClickProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("ModelUpDirection") };
        BindingOperations.SetBinding(viewCube, ViewBoxModel3D.UpDirectionProperty, binding);

        binding = new Binding() { Source = this, Path = new PropertyPath("ShowCoordinateSystem") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.IsRenderingProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("CoordinateSystemHorizontalPosition") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.RelativeScreenLocationXProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("CoordinateSystemVerticalPosition") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.RelativeScreenLocationYProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("CoordinateSystemLabelForeground") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.LabelColorProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("CoordinateSystemLabelX") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.CoordinateSystemLabelXProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("CoordinateSystemLabelY") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.CoordinateSystemLabelYProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("CoordinateSystemLabelZ") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.CoordinateSystemLabelZProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("CoordinateSystemAxisXColor") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.AxisXColorProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("CoordinateSystemAxisYColor") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.AxisYColorProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("CoordinateSystemAxisZColor") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.AxisZColorProperty, binding);
        binding = new Binding() { Source = this, Path = new PropertyPath("CoordinateSystemSize") };
        BindingOperations.SetBinding(coordinateView, CoordinateSystemModel3D.SizeScaleProperty, binding);
#else
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.ZoomExtents, this.ZoomExtentsHandler));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.SetTarget, this.cameraController.setTargetHandler!.Execute));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.Reset, this.ResetHandler));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.Zoom, this.cameraController.zoomHandler!.Execute));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.Pan, this.cameraController.panHandler!.Execute));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.Rotate, this.cameraController.rotateHandler!.Execute));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.ChangeFieldOfView, this.cameraController.changeFieldOfViewHandler!.Execute));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.ZoomRectangle, this.cameraController.zoomRectangleHandler!.Execute));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.BottomView, this.BottomViewHandler));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.TopView, this.TopViewHandler));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.FrontView, this.FrontViewHandler));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.BackView, this.BackViewHandler));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.LeftView, this.LeftViewHandler));
        this.CommandBindings.Add(new CommandBinding(ViewportCommands.RightView, this.RightViewHandler));
#endif
    }

    //private void Viewport3DX_DpiChanged(DisplayInformation sender, object args)
    //{
    //    //var dpi = sender.RawPixelsPerViewPixel;
    //    var dpi = 1;
    //    if (hostPresenter != null && hostPresenter.Content is HelixToolkitRenderPanel host)
    //    {
    //        host.DpiScale = (float)dpi;
    //    }
    //}

    private void InitCameraController()
    {
        #region Assign Defaults
        this.cameraController.CameraMode = this.CameraMode;
        this.cameraController.CameraRotationMode = this.CameraRotationMode;
        this.cameraController.PageUpDownZoomSensitivity = this.PageUpDownZoomSensitivity;
        this.cameraController.PanCursor = this.PanCursor;
        this.cameraController.RotateAroundMouseDownPoint = this.RotateAroundMouseDownPoint;
        this.cameraController.RotateCursor = this.RotateCursor;
        this.cameraController.RotationSensitivity = this.RotationSensitivity;
        this.cameraController.SpinReleaseTime = this.SpinReleaseTime;
        this.cameraController.UpDownPanSensitivity = this.UpDownPanSensitivity;
        this.cameraController.UpDownRotationSensitivity = this.UpDownRotationSensitivity;
        this.cameraController.ZoomAroundMouseDownPoint = this.ZoomAroundMouseDownPoint;
        this.cameraController.ZoomCursor = this.ZoomCursor;
        this.cameraController.ZoomRectangleCursor = this.ZoomRectangleCursor;
        this.cameraController.ZoomSensitivity = this.ZoomSensitivity;
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
        this.cameraController.PinchZoomAtCenter = this.PinchZoomAtCenter;
        this.cameraController.LeftRightPanSensitivity = this.LeftRightPanSensitivity;
        this.cameraController.LeftRightRotationSensitivity = this.LeftRightRotationSensitivity;
        this.cameraController.MaximumFieldOfView = this.MaximumFieldOfView;
        this.cameraController.MinimumFieldOfView = this.MinimumFieldOfView;
        this.cameraController.ModelUpDirection = this.ModelUpDirection;
        this.cameraController.ZoomDistanceLimitFar = this.ZoomDistanceLimitFar;
        this.cameraController.ZoomDistanceLimitNear = this.ZoomDistanceLimitNear;
        this.cameraController.FixedRotationPoint = this.FixedRotationPoint;
        this.cameraController.FixedRotationPointEnabled = this.FixedRotationPointEnabled;
        #endregion
    }

    private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                partItemsControl?.Items.Remove(item);

                if (item is Element3D element)
                {
                    element.SceneNode.Detach();
                    element.SceneNode.Invalidated -= NodeInvalidated;
                }
            }
        }
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                partItemsControl?.Items.Add(item);

                if (this.IsAttached && item is Element3D element)
                {
                    element.SceneNode.Invalidated += NodeInvalidated;
                    element.SceneNode.Attach(EffectsManager);
                }
            }
        }
        InvalidateRender();
    }


    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (IsInDesignMode && !EnableDesignModeRendering)
        {
            return;
        }

        if (this.renderHostInternal != null)
        {
            this.renderHostInternal.Rendered -= this.RaiseRenderHostRendered;
            this.renderHostInternal.ExceptionOccurred -= this.RenderHostInternal_ExceptionOccurred;
        }

        Disposer.RemoveAndDispose(ref renderHostInternal);

        hostPresenter = this.GetTemplateChild(ViewportPartNames.PartCanvas) as ContentPresenter;

        if (this.hostPresenter?.Content is IRenderCanvas renderCanvas)
        {
            renderCanvas.ExceptionOccurred -= this.RenderHostInternal_ExceptionOccurred;
        }

        if (hostPresenter is not null)
        {
            var host = new HelixToolkitRenderPanel(EnableDeferredRendering);
            hostPresenter.Content = host;
            //var view = DisplayInformation.GetForCurrentView();
            //var dpi = view.RawPixelsPerViewPixel;
            var dpi = 1;
            host.DpiScale = (float)dpi;
            host.EnableDpiScale = EnableDpiScale;
            renderHostInternal = (hostPresenter.Content as HelixToolkitRenderPanel)?.RenderHost;
            if (renderHostInternal is not null)
            {
                renderHostInternal.Rendered += this.RaiseRenderHostRendered;
                renderHostInternal.ExceptionOccurred += RenderHostInternal_ExceptionOccurred;
                renderHostInternal.ClearColor = this.BackgroundColor.ToColor4();
                renderHostInternal.IsShadowMapEnabled = this.IsShadowMappingEnabled;
                renderHostInternal.MSAA = this.MSAA;
                renderHostInternal.EnableRenderFrustum = this.EnableRenderFrustum;
                renderHostInternal.EnableSharingModelMode = this.EnableSharedModelMode;
                renderHostInternal.SharedModelContainer = this.SharedModelContainer;
                renderHostInternal.Viewport = this;
                renderHostInternal.EffectsManager = this.EffectsManager;
                renderHostInternal.IsRendering = this.Visibility == VisibilityEnum.Visible;
                renderHostInternal.RenderConfiguration.RenderD2D = this.EnableD2DRendering;
                renderHostInternal.RenderConfiguration.AutoUpdateOctree = this.EnableAutoOctreeUpdate;
                renderHostInternal.RenderConfiguration.OITRenderType = OITRenderMode;
                renderHostInternal.RenderConfiguration.OITWeightPower = (float)OITWeightPower;
                renderHostInternal.RenderConfiguration.OITWeightDepthSlope = (float)OITWeightDepthSlope;
                renderHostInternal.RenderConfiguration.OITWeightMode = OITWeightMode;
                renderHostInternal.RenderConfiguration.OITDepthPeelingIteration = OITDepthPeelingIteration;
                renderHostInternal.RenderConfiguration.EnableOITDepthPeelingDynamicIteration = EnableOITDepthPeelingDynamicIteration;
                renderHostInternal.RenderConfiguration.FXAALevel = FXAALevel;
                renderHostInternal.RenderConfiguration.EnableRenderOrder = EnableRenderOrder;
                renderHostInternal.RenderConfiguration.EnableSSAO = EnableSSAO;
                renderHostInternal.RenderConfiguration.SSAORadius = (float)SSAOSamplingRadius;
                renderHostInternal.RenderConfiguration.SSAOIntensity = (float)SSAOIntensity;
                renderHostInternal.RenderConfiguration.SSAOQuality = SSAOQuality;
                renderHostInternal.RenderConfiguration.MinimumUpdateCount = (uint)Math.Max(0, MinimumUpdateCount);

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
        }

        this.coordinateView ??= GetTemplateChild(ViewportPartNames.PartCoordinateView) as CoordinateSystemModel3D;
        if (this.coordinateView == null)
        {
            throw new HelixToolkitException("{0} is missing from the template.", ViewportPartNames.PartCoordinateView);
        }

        //if (!this.coordinateView.Items.Contains(viewCube))
        //{
        //    this.coordinateView.Items.Add(viewCube);
        //}
        //if (!this.coordinateView.Items.Contains(coordinateView))
        //{
        //    this.coordinateView.Items.Add(coordinateView);
        //}

        this.viewCube ??= GetTemplateChild(ViewportPartNames.PartViewCube) as ViewBoxModel3D;
        if (this.viewCube == null)
        {
            throw new HelixToolkitException("{0} is missing from the template.", ViewportPartNames.PartViewCube);
        }

        this.frameStatisticModel ??= GetTemplateChild(ViewportPartNames.PartFrameStatisticView) as FrameStatisticsModel2D;
        if (this.frameStatisticModel == null)
        {
            throw new HelixToolkitException("{0} is missing from the template.", ViewportPartNames.PartFrameStatisticView);
        }

        this.partItemsControl ??= GetTemplateChild(ViewportPartNames.PartItems) as HelixItemsControl;
        if (this.partItemsControl == null)
        {
            throw new HelixToolkitException("{0} is missing from the template.", ViewportPartNames.PartItems);
        }
        else
        {
            foreach (var item in Items)
            {
                this.partItemsControl.Items.Remove(item);
            }

            foreach (var item in Items)
            {
                this.partItemsControl.Items.Add(item);
            }
        }

        Overlay2D.Children.Clear();
        this.RemoveLogicalChild(Overlay2D);
        this.AddLogicalChild(Overlay2D);
        var titleView = GetTemplateChild(ViewportPartNames.PartTitleView);
        if (titleView is Element2D element)
        {
            Overlay2D.Children.Add(element);
        }
        if (viewCube != null)
        {
            Overlay2D.Children.Add(viewCube.MoverCanvas);
        }
        if (coordinateView != null)
        {
            Overlay2D.Children.Add(coordinateView.MoverCanvas);
        }
        if (Content2D != null)
        {
            Overlay2D.Children.Add(Content2D);
        }

        SetupBindings();
    }

#if WPF
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

        if (this.hostPresenter is not UIElement visual)
        {
            return;
        }

        var myAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
        if (myAdornerLayer == null)
        {
            return;
        }
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

        if (this.hostPresenter is not UIElement visual)
        {
            return;
        }

        var myAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
        if (myAdornerLayer == null)
        {
            return;
        }
        this.rectangleAdorner = new RectangleAdorner(
            visual, rect, Colors.LightGray, Colors.Black, 3, 1, 10, DashStyles.Solid);
        myAdornerLayer.Add(this.rectangleAdorner);
    }
#endif

    private void RenderHostInternal_ExceptionOccurred(object? sender, RelayExceptionEventArgs e)
    {
        var bindingExpression = this.GetBindingExpression(RenderExceptionProperty);
        if (bindingExpression != null)
        {
            // If RenderExceptionProperty is bound, we assume the exception will be handled.
            this.RenderException = e.Exception;
            e.Handled = true;
        }

        // Fire RenderExceptionOccurred event
        this.RenderExceptionOccurred?.Invoke(sender, e);

        // If the Exception is still unhandled...
        if (!e.Handled)
        {
            // ... prevent a MessageBox.Show().
            this.MessageText = e.Exception.ToString();
            e.Handled = true;
        }

        if (hostPresenter is not null)
        {
            hostPresenter.Content = null;
        }

        Disposer.RemoveAndDispose(ref renderHostInternal);
    }

    private void ControlLoaded(object? sender, RoutedEventArgs e)
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

        //DisplayInformation.GetForCurrentView().DpiChanged += Viewport3DX_DpiChanged;
        InitCameraController();

        if (renderHostInternal != null)
        {
            renderHostInternal.IsRendering = this.Visibility == VisibilityEnum.Visible;
        }

        visibilityCallbackToken = RegisterPropertyChangedCallback(VisibilityProperty, (s, arg) =>
        {
            if (renderHostInternal != null)
            {
                renderHostInternal.IsRendering = (VisibilityEnum)s.GetValue(arg) == VisibilityEnum.Visible;
            }
        });
    }

    private void ControlUnloaded(object? sender, RoutedEventArgs e)
    {
        //DisplayInformation.GetForCurrentView().DpiChanged -= Viewport3DX_DpiChanged;
        UnregisterPropertyChangedCallback(VisibilityProperty, visibilityCallbackToken);
    }

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
                e.Attach(EffectsManager);
                e.Invalidated += NodeInvalidated;
            }
            SharedModelContainerInternal?.Attach(host);
            foreach (var e in this.D2DRenderables)
            {
                e.Attach(host);
            }
            IsAttached = true;
        }
    }

    private void NodeInvalidated(object? sender, InvalidateTypes e)
    {
        renderHostInternal?.Invalidate(e);
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
            if (renderHostInternal is not null)
            {
                SharedModelContainerInternal?.Detach(renderHostInternal);
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
        var p = e.GetCurrentPoint(this).Position;
        if (!ViewBoxHitTest(p))
        {
            MouseDownHitTest(p, e);
            CameraController.OnMouseDown(e);
        }
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        CameraController.OnMouseUp(e);
        MouseUpHitTest(e.GetCurrentPoint(this).Position, e);
        base.OnPointerReleased(e);
    }

    protected override void OnPointerMoved(PointerRoutedEventArgs e)
    {
        MouseMoveHitTest(e.GetCurrentPoint(this).Position, e);
        base.OnPointerMoved(e);
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);
        CameraController.InputController?.OnKeyPressed(e);
    }

    /// <summary>
    /// Called before the ManipulationStarted event occurs.
    /// </summary>
    /// <param name="e">Event data for the event.</param>
    protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
    {
        if (e.PointerDeviceType == PointerDeviceType.Touch)
            CameraController.OnManipulationStarted(e);
        base.OnManipulationStarted(e);
    }

    protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
    {
        if (e.PointerDeviceType == PointerDeviceType.Touch)
            CameraController.OnManipulationCompleted(e);
        base.OnManipulationCompleted(e);
    }


    protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
    {
        if (e.PointerDeviceType == PointerDeviceType.Touch)
            CameraController.OnManipulationDelta(e);
        base.OnManipulationDelta(e);
    }

    protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
    {
        CameraController.OnMouseWheel(e);
        base.OnPointerWheelChanged(e);
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
    /// Sets the default gestures.
    /// </summary>
    private void SetDefaultGestures()
    {
        this.InputBindings.Clear();

        // todo
    }

    private void ViewCubeClicked(Vector3 lookDirection, Vector3 upDirection)
    {
        if (this.Camera is not ProjectionCamera pc)
        {
            return;
        }

        var target = pc.Position + pc.LookDirection;
        float distance = pc.LookDirection.Length();
        var look = lookDirection * distance;
        var newPosition = target - look;
        pc.AnimateTo(newPosition, look, upDirection, 500);
    }

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

    public void Update(TimeSpan timeStamp)
    {
        CameraController.OnTimeStep(timeStamp.Ticks);
        this.FrameRate = Math.Round(renderHostInternal?.RenderStatistics?.FPSStatistics.AverageFrequency ?? 0, 2);
        this.RenderDetailOutput = renderHostInternal?.RenderStatistics?.GetDetailString() ?? string.Empty;
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
        CameraController.AddMoveForce(new Vector3((float)dx, (float)dy, (float)dz));
    }

    /// <summary>
    /// Adds the specified move force.
    /// </summary>
    /// <param name="delta">
    /// The delta. 
    /// </param>
    public void AddMoveForce(Vector3 delta)
    {
        CameraController.AddMoveForce(delta);
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
        CameraController.AddPanForce(dx, dy);
    }

    /// <summary>
    /// The add pan force.
    /// </summary>
    /// <param name="pan">
    /// The pan. 
    /// </param>
    public void AddPanForce(Vector3 pan)
    {
        CameraController.AddPanForce(pan);
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
        CameraController.AddRotateForce(dx, dy);
    }

    /// <summary>
    /// Adds the zoom force.
    /// </summary>
    /// <param name="dx">
    /// The delta. 
    /// </param>
    public void AddZoomForce(double dx)
    {
        CameraController.AddZoomForce(dx);
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
        CameraController.AddZoomForce(dx, zoomOrigin);
    }

    /// <summary>
    ///   Stops the spinning.
    /// </summary>
    public void StopSpin()
    {
        CameraController.StopSpin();
    }

    /// <summary>
    /// Starts spinning.
    /// </summary>
    /// <param name="speed">The speed.</param>
    /// <param name="position">The position.</param>
    /// <param name="aroundPoint">The point to spin around.</param>
    public void StartSpin(Vector2 speed, Point position, Vector3 aroundPoint)
    {
        CameraController.StartSpin(speed, position, aroundPoint);
    }

    private void CameraInternal_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        InvalidateRender();
    }

    /// <summary>
    /// Change the camera to look at the specified point.
    /// </summary>
    /// <param name="p">
    /// The point.
    /// </param>
    public void LookAt(Vector3 p)
    {
        this.CameraController?.ActualCamera?.LookAt(p, 0);
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
    public void LookAt(Vector3 p, double animationTime)
    {
        this.CameraController?.ActualCamera?.LookAt(p, animationTime);
    }

#if WINUI
    public bool HittedSomething(PointerRoutedEventArgs e)
    {
        return this.FindHitsInFrustum(e.GetCurrentPoint(this).Position.ToVector2(), ref hits);
    }
#else
    public bool HittedSomething(MouseEventArgs e)
    {
        return this.FindHitsInFrustum(e.GetPosition(this).ToVector2(), ref hits);
    }
#endif

    /// <summary>
    /// Handles hit testing on mouse down.
    /// </summary>
    /// <param name="pt">The hit point.</param>
    /// <param name="originalInputEventArgs">
    /// The original input event (which mouse button pressed?)
    /// </param>
    private void MouseDownHitTest(Point pt, UIInputEventArgs? originalInputEventArgs = null)
    {
        if (Overlay2D.HitTest(pt.ToVector2(), out currentHit2D))
        {
            if (currentHit2D?.ModelHit is Element2D e)
            {
                e.RaiseEvent(new Mouse2DEventArgs(Element2D.MouseDown2DEvent, currentHit2D.ModelHit, currentHit2D, pt, this, originalInputEventArgs));

                if (originalInputEventArgs is not null)
                {
                    originalInputEventArgs.Handled = true;
                }
            }
            return;
        }

        if (!enableMouseButtonHitTest)
        {
            return;
        }

        if (this.FindHits(pt.ToVector2(), ref hits) && hits.Count > 0)
        {
            this.currentHit = hits.FirstOrDefault(x => x.IsValid);
            if (this.currentHit != null)
            {
                if (currentHit.ModelHit is Element3D ele)
                {
                    ele.RaiseMouseDownEvent(this.currentHit, pt, this, originalInputEventArgs);
                }
                else if (currentHit.ModelHit is SceneNode node)
                {
                    node.RaiseMouseDownEvent(this, pt.ToVector2(), currentHit, originalInputEventArgs);
                }
            }
        }
        else
        {
            currentHit = null;
        }
        if (currentHit is not null)
        {
            this.OnMouse3DDown?.Invoke(this, new MouseDown3DEventArgs(currentHit, pt, this, originalInputEventArgs));
        }
    }

    private bool ViewBoxHitTest(Point p)
    {
        if (RenderContext is null || viewCube is null)
        {
            return false;
        }

        if (Camera is not ProjectionCamera camera)
        {
            return false;
        }
        var vp = p.ToVector2();
        if (!this.UnProject(vp, out var ray))
        {
            return false;
        }
        var hits = new List<HitTestResult>();
        if (viewCube.HitTest(new HitTestContext(RenderContext, ref ray, ref vp), ref hits))
        {
            var normal = hits[0].NormalAtHit;
            if (Vector3.Cross(normal, ModelUpDirection).LengthSquared() < 1e-5)
            {
                var vecLeft = new Vector3(-normal.Y, -normal.Z, -normal.X);
                ViewCubeClicked(hits[0].NormalAtHit, vecLeft);
            }
            else
            {
                ViewCubeClicked(hits[0].NormalAtHit, ModelUpDirection);
            }
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
    /// The original input (which mouse button pressed?)
    /// </param>
    private void MouseMoveHitTest(Point pt, UIInputEventArgs? originalInputEventArgs = null)
    {
        if (Overlay2D.HitTest(pt.ToVector2(), out var hit2D))
        {
            if (hit2D?.ModelHit is Element2D e)
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

        if (!enableMouseButtonHitTest)
        {
            return;
        }

        if (this.currentHit != null)
        {
            if (currentHit.ModelHit is Element3D ele)
            {
                ele.RaiseMouseMoveEvent(this.currentHit, pt, this, originalInputEventArgs);
            }
            else if (currentHit.ModelHit is SceneNode node)
            {
                node.RaiseMouseMoveEvent(this, pt.ToVector2(), currentHit, originalInputEventArgs);
            }
        }

        if (currentHit is not null)
        {
            this.OnMouse3DMove?.Invoke(this, new MouseMove3DEventArgs(currentHit, pt, this, originalInputEventArgs));
        }
    }

    /// <summary>
    /// Handles hit testing on mouse up.
    /// </summary>
    /// <param name="pt">The hit point.</param>
    /// <param name="originalInputEventArgs">
    /// The original input event (which mouse button pressed?)
    /// </param>
    private void MouseUpHitTest(Point pt, UIInputEventArgs? originalInputEventArgs = null)
    {
        if (!enableMouseButtonHitTest)
        {
            return;
        }

        if (this.currentHit2D != null)
        {
            if (this.currentHit2D.ModelHit is Element2D element)
            {
                element.RaiseEvent(new Mouse2DEventArgs(Element2D.MouseUp2DEvent, currentHit2D.ModelHit, currentHit2D, pt, this, originalInputEventArgs));
            }
            this.currentHit2D = null;
        }

        if (this.currentHit != null)
        {
            if (this.currentHit.ModelHit is Element3D ele)
            {
                ele.RaiseMouseUpEvent(this.currentHit, pt, this, originalInputEventArgs);
            }
            else if (this.currentHit.ModelHit is SceneNode node)
            {
                node.RaiseMouseUpEvent(this, pt.ToVector2(), currentHit, originalInputEventArgs);
            }
            this.currentHit = null;
        }

        if (this.currentHit is not null)
        {
            this.OnMouse3DUp?.Invoke(this, new MouseUp3DEventArgs(currentHit, pt, this, originalInputEventArgs));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RaiseRenderHostRendered(object? sender, EventArgs e)
    {
        this.OnRendered?.Invoke(sender, e);
    }

    public static T? FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
    {
        if (obj != null)
        {
            var parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T typed)
                {
                    return typed;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        return null;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (double.IsInfinity(constraint.Width) && double.IsInfinity(constraint.Height))
        {
            if ((_ = FindVisualAncestor<Viewbox>(this)) != null)
            {
                MessageText = "Must specify Width and Height for Viewport3DX in a ViewBox";
                return base.MeasureOverride(new Size(600, 400));
            }
        }

        return base.MeasureOverride(constraint);
    }

    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (!BelongsToParentWindow)
                {
                    if (hostPresenter?.Content is IDisposable d)
                    {
                        hostPresenter.Content = null;
                        d.Dispose();
                    }

                    Camera = null;
                    EffectsManager = null;

                    foreach (Element3D item in Items)
                    {
                        item.Dispose();
                    }

                    viewCube?.Dispose();
                    coordinateView?.Dispose();
                    Items.Clear();

                    RenderHost?.Dispose();
                    //CameraController.Dispose();
                }
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
    }
}
