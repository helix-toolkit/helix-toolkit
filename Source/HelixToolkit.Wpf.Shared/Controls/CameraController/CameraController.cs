// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraController.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a control that manipulates the camera by mouse and keyboard gestures.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides a control that manipulates the camera by mouse and keyboard gestures.
    /// </summary>
    public class CameraController : Grid
    {
        /// <summary>
        /// Identifies the <see cref="CameraMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CameraModeProperty = DependencyProperty.Register(
            "CameraMode", typeof(CameraMode), typeof(CameraController), new UIPropertyMetadata(CameraMode.Inspect));

        /// <summary>
        /// Identifies the <see cref="Camera"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
            "Camera",
            typeof(ProjectionCamera),
            typeof(CameraController),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CameraChanged));

        /// <summary>
        /// Identifies the <see cref="CameraRotationMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CameraRotationModeProperty =
            DependencyProperty.Register(
                "CameraRotationMode",
                typeof(CameraRotationMode),
                typeof(CameraController),
                new UIPropertyMetadata(CameraRotationMode.Turntable));

        /// <summary>
        /// Identifies the <see cref="ChangeFieldOfViewCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChangeFieldOfViewCursorProperty =
            DependencyProperty.Register(
                "ChangeFieldOfViewCursor",
                typeof(Cursor),
                typeof(CameraController),
                new UIPropertyMetadata(Cursors.ScrollNS));

        /// <summary>
        /// Identifies the <see cref="DefaultCamera"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultCameraProperty = DependencyProperty.Register(
            "DefaultCamera", typeof(ProjectionCamera), typeof(CameraController), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Enabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register(
            "Enabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="InertiaFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InertiaFactorProperty = DependencyProperty.Register(
            "InertiaFactor", typeof(double), typeof(CameraController), new UIPropertyMetadata(0.9));

        /// <summary>
        /// Identifies the <see cref="InfiniteSpin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfiniteSpinProperty = DependencyProperty.Register(
            "InfiniteSpin", typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsChangeFieldOfViewEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsChangeFieldOfViewEnabledProperty =
            DependencyProperty.Register(
                "IsChangeFieldOfViewEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsInertiaEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInertiaEnabledProperty =
            DependencyProperty.Register(
                "IsInertiaEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsMoveEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMoveEnabledProperty = DependencyProperty.Register(
            "IsMoveEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsPanEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
            "IsPanEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsRotationEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRotationEnabledProperty =
            DependencyProperty.Register(
                "IsRotationEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsTouchZoomEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTouchZoomEnabledProperty =
            DependencyProperty.Register(
                "IsTouchZoomEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsZoomEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
            "IsZoomEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="LeftRightPanSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftRightPanSensitivityProperty =
            DependencyProperty.Register(
                "LeftRightPanSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="LeftRightRotationSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftRightRotationSensitivityProperty =
            DependencyProperty.Register(
                "LeftRightRotationSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        /// The look at (target) point changed event
        /// </summary>
        public static readonly RoutedEvent LookAtChangedEvent = EventManager.RegisterRoutedEvent(
            "LookAtChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CameraController));

        /// <summary>
        /// Identifies the <see cref="MaximumFieldOfView"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumFieldOfViewProperty =
            DependencyProperty.Register(
                "MaximumFieldOfView", typeof(double), typeof(CameraController), new UIPropertyMetadata(160.0));

        /// <summary>
        /// Identifies the <see cref="MinimumFieldOfView"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumFieldOfViewProperty =
            DependencyProperty.Register(
                "MinimumFieldOfView", typeof(double), typeof(CameraController), new UIPropertyMetadata(5.0));

        /// <summary>
        /// Identifies the <see cref="ModelUpDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModelUpDirectionProperty =
            DependencyProperty.Register(
                "ModelUpDirection",
                typeof(Vector3D),
                typeof(CameraController),
                new UIPropertyMetadata(new Vector3D(0, 0, 1)));

        /// <summary>
        /// Identifies the <see cref="MoveSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MoveSensitivityProperty =
            DependencyProperty.Register(
                "MoveSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="PageUpDownZoomSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PageUpDownZoomSensitivityProperty =
            DependencyProperty.Register(
                "PageUpDownZoomSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="PanCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanCursorProperty = DependencyProperty.Register(
            "PanCursor", typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.Hand));

        /// <summary>
        /// Identifies the <see cref="RotateAroundMouseDownPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateAroundMouseDownPointProperty =
            DependencyProperty.Register(
                "RotateAroundMouseDownPoint", typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="FixedRotationPointEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FixedRotationPointEnabledProperty =
            DependencyProperty.Register(
                "FixedRotationPointEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="FixedRotationPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FixedRotationPointProperty =
            DependencyProperty.Register(
                "FixedRotationPoint", typeof(Point3D), typeof(CameraController), new UIPropertyMetadata(default(Point3D)));

        /// <summary>
        /// Identifies the <see cref="RotateCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateCursorProperty = DependencyProperty.Register(
            "RotateCursor", typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.SizeAll));

        /// <summary>
        /// Identifies the <see cref="RotationSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationSensitivityProperty =
            DependencyProperty.Register(
                "RotationSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="ShowCameraTarget"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraTargetProperty =
            DependencyProperty.Register(
                "ShowCameraTarget", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="SpinReleaseTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpinReleaseTimeProperty =
            DependencyProperty.Register(
                "SpinReleaseTime", typeof(int), typeof(CameraController), new UIPropertyMetadata(200));

        /// <summary>
        /// Identifies the <see cref="UpDownPanSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpDownPanSensitivityProperty =
            DependencyProperty.Register(
                "UpDownPanSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="UpDownRotationSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpDownRotationSensitivityProperty =
            DependencyProperty.Register(
                "UpDownRotationSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="Viewport"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(
            "Viewport", typeof(Viewport3D), typeof(CameraController), new PropertyMetadata(null, ViewportChanged));

        /// <summary>
        /// Identifies the <see cref="ZoomAroundMouseDownPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomAroundMouseDownPointProperty =
            DependencyProperty.Register(
                "ZoomAroundMouseDownPoint", typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="SnapMouseDownPoint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SnapMouseDownPointProperty =
            DependencyProperty.Register(
                "SnapMouseDownPoint", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ZoomCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomCursorProperty = DependencyProperty.Register(
            "ZoomCursor", typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.SizeNS));

        /// <summary>
        /// Identifies the <see cref="ZoomRectangleCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleCursorProperty =
            DependencyProperty.Register(
                "ZoomRectangleCursor",
                typeof(Cursor),
                typeof(CameraController),
                new UIPropertyMetadata(Cursors.ScrollSE));

        /// <summary>
        /// Identifies the <see cref="ZoomSensitivity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomSensitivityProperty =
            DependencyProperty.Register(
                "ZoomSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        /// The zoomed by rectangle event
        /// </summary>
        public static readonly RoutedEvent ZoomedByRectangleEvent = EventManager.RegisterRoutedEvent(
            "ZoomedByRectangle", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CameraController));

        /// <summary>
        /// The camera history stack.
        /// </summary>
        /// <remarks>
        /// Implemented as a linkedlist since we want to remove items at the bottom of the stack.
        /// </remarks>
        private readonly LinkedList<CameraSetting> cameraHistory = new LinkedList<CameraSetting>();

        /// <summary>
        /// The rendering event listener.
        /// </summary>
        private readonly RenderingEventListener renderingEventListener;

        /// <summary>
        /// The stacked cursors - used for restoring to original cursor
        /// </summary>
        private readonly Stack<Cursor> cursorStack = new Stack<Cursor>();

        /// <summary>
        /// The change field of view event handler.
        /// </summary>
        private ZoomHandler changeFieldOfViewHandler;

        /// <summary>
        /// The change look at event handler.
        /// </summary>
        private RotateHandler changeLookAtHandler;

        /// <summary>
        /// The is spinning flag.
        /// </summary>
        private bool isSpinning;

        /// <summary>
        /// The last tick.
        /// </summary>
        private long lastTick;

        /// <summary>
        /// The move speed.
        /// </summary>
        private Vector3D moveSpeed;

        /// <summary>
        /// The pan event handler.
        /// </summary>
        private PanHandler panHandler;

        /// <summary>
        /// The pan speed.
        /// </summary>
        private Vector3D panSpeed;

        /// <summary>
        /// The rectangle adorner.
        /// </summary>
        private RectangleAdorner rectangleAdorner;

        /// <summary>
        /// The rotation event handler.
        /// </summary>
        private RotateHandler rotateHandler;

        /// <summary>
        /// The 3D rotation point.
        /// </summary>
        private Point3D rotationPoint3D;

        /// <summary>
        /// The rotation position.
        /// </summary>
        private Point rotationPosition;

        /// <summary>
        /// The rotation speed.
        /// </summary>
        private Vector rotationSpeed;

        /// <summary>
        /// The 3D point to spin around.
        /// </summary>
        private Point3D spinningPoint3D;

        /// <summary>
        /// The spinning position.
        /// </summary>
        private Point spinningPosition;

        /// <summary>
        /// The spinning speed.
        /// </summary>
        private Vector spinningSpeed;

        /// <summary>
        /// The target adorner.
        /// </summary>
        private Adorner targetAdorner;

        /// <summary>
        /// The touch point in the last touch delta event
        /// </summary>
        private Point touchPreviousPoint;

        /// <summary>
        /// The number of touch manipulators (fingers) in the last touch delta event
        /// </summary>
        private int manipulatorCount;

        /// <summary>
        /// The zoom event handler.
        /// </summary>
        private ZoomHandler zoomHandler;

        /// <summary>
        /// The point to zoom around.
        /// </summary>
        private Point3D zoomPoint3D;

        /// <summary>
        /// The zoom rectangle event handler.
        /// </summary>
        private ZoomRectangleHandler zoomRectangleHandler;

        /// <summary>
        /// The zoom speed.
        /// </summary>
        private double zoomSpeed;

        /// <summary>
        /// Initializes static members of the <see cref="CameraController" /> class.
        /// </summary>
        static CameraController()
        {
            BackgroundProperty.OverrideMetadata(typeof(CameraController), new FrameworkPropertyMetadata(Brushes.Transparent));
            FocusVisualStyleProperty.OverrideMetadata(typeof(CameraController), new FrameworkPropertyMetadata(null));
            BackViewCommand = new RoutedCommand();
            BottomViewCommand = new RoutedCommand();
            ChangeFieldOfViewCommand = new RoutedCommand();
            ChangeLookAtCommand = new RoutedCommand();
            FrontViewCommand = new RoutedCommand();
            LeftViewCommand = new RoutedCommand();
            PanCommand = new RoutedCommand();
            ResetCameraCommand = new RoutedCommand();
            RightViewCommand = new RoutedCommand();
            RotateCommand = new RoutedCommand();
            TopViewCommand = new RoutedCommand();
            ZoomCommand = new RoutedCommand();
            ZoomExtentsCommand = new RoutedCommand();
            ZoomRectangleCommand = new RoutedCommand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraController" /> class.
        /// </summary>
        public CameraController()
        {
            this.Loaded += this.CameraControllerLoaded;
            this.Unloaded += this.CameraControllerUnloaded;

            // Must be focusable to received key events
            this.Focusable = true;
            this.FocusVisualStyle = null;

            this.IsManipulationEnabled = true;
            this.RotataAroundClosestVertexComplexity = 5000;

            this.InitializeBindings();
            this.renderingEventListener = new RenderingEventListener(this.OnCompositionTargetRendering);
        }

        /// <summary>
        /// Occurs when the look at/target point changed.
        /// </summary>
        public event RoutedEventHandler LookAtChanged
        {
            add
            {
                this.AddHandler(LookAtChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(LookAtChangedEvent, value);
            }
        }

        /// <summary>
        /// Occurs when the view is zoomed by rectangle.
        /// </summary>
        public event RoutedEventHandler ZoomedByRectangle
        {
            add
            {
                this.AddHandler(ZoomedByRectangleEvent, value);
            }

            remove
            {
                this.RemoveHandler(ZoomedByRectangleEvent, value);
            }
        }

        /// <summary>
        /// Gets the back view command.
        /// </summary>
        public static RoutedCommand BackViewCommand { get; private set; }

        /// <summary>
        /// Gets the bottom view command.
        /// </summary>
        public static RoutedCommand BottomViewCommand { get; private set; }

        /// <summary>
        /// Gets the change field of view command.
        /// </summary>
        public static RoutedCommand ChangeFieldOfViewCommand { get; private set; }

        /// <summary>
        /// Gets the change look at command.
        /// </summary>
        public static RoutedCommand ChangeLookAtCommand { get; private set; }

        /// <summary>
        /// Gets the front view command.
        /// </summary>
        public static RoutedCommand FrontViewCommand { get; private set; }

        /// <summary>
        /// Gets the left view command.
        /// </summary>
        public static RoutedCommand LeftViewCommand { get; private set; }

        /// <summary>
        /// Gets the pan command.
        /// </summary>
        public static RoutedCommand PanCommand { get; private set; }

        /// <summary>
        /// Gets the reset camera command.
        /// </summary>
        public static RoutedCommand ResetCameraCommand { get; private set; }

        /// <summary>
        /// Gets the right view command.
        /// </summary>
        public static RoutedCommand RightViewCommand { get; private set; }

        /// <summary>
        /// Gets the rotate command.
        /// </summary>
        public static RoutedCommand RotateCommand { get; private set; }

        /// <summary>
        /// Gets the top view command.
        /// </summary>
        public static RoutedCommand TopViewCommand { get; private set; }

        /// <summary>
        /// Gets the zoom command.
        /// </summary>
        public static RoutedCommand ZoomCommand { get; private set; }

        /// <summary>
        /// Gets the zoom extents command.
        /// </summary>
        public static RoutedCommand ZoomExtentsCommand { get; private set; }

        /// <summary>
        /// Gets the zoom rectangle command.
        /// </summary>
        public static RoutedCommand ZoomRectangleCommand { get; private set; }

        /// <summary>
        /// Gets ActualCamera.
        /// </summary>
        public ProjectionCamera ActualCamera
        {
            get
            {
                if (this.Camera != null)
                {
                    return this.Camera;
                }

                if (this.Viewport != null)
                {
                    return this.Viewport.Camera as ProjectionCamera;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets Camera.
        /// </summary>
        public ProjectionCamera Camera
        {
            get
            {
                return (ProjectionCamera)this.GetValue(CameraProperty);
            }

            set
            {
                this.SetValue(CameraProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets CameraLookDirection.
        /// </summary>
        public Vector3D CameraLookDirection
        {
            get
            {
                return this.ActualCamera.LookDirection;
            }

            set
            {
                this.ActualCamera.LookDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets CameraMode.
        /// </summary>
        public CameraMode CameraMode
        {
            get
            {
                return (CameraMode)this.GetValue(CameraModeProperty);
            }

            set
            {
                this.SetValue(CameraModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets CameraPosition.
        /// </summary>
        public Point3D CameraPosition
        {
            get
            {
                return this.ActualCamera.Position;
            }

            set
            {
                this.ActualCamera.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets CameraRotationMode.
        /// </summary>
        public CameraRotationMode CameraRotationMode
        {
            get
            {
                return (CameraRotationMode)this.GetValue(CameraRotationModeProperty);
            }

            set
            {
                this.SetValue(CameraRotationModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets CameraTarget.
        /// </summary>
        public Point3D CameraTarget
        {
            get
            {
                return this.CameraPosition + this.CameraLookDirection;
            }

            set
            {
                this.CameraLookDirection = value - this.CameraPosition;
            }
        }

        /// <summary>
        /// Gets or sets CameraUpDirection.
        /// </summary>
        public Vector3D CameraUpDirection
        {
            get
            {
                return this.ActualCamera.UpDirection;
            }

            set
            {
                this.ActualCamera.UpDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets the change field of view cursor.
        /// </summary>
        /// <value> The change field of view cursor. </value>
        public Cursor ChangeFieldOfViewCursor
        {
            get
            {
                return (Cursor)this.GetValue(ChangeFieldOfViewCursorProperty);
            }

            set
            {
                this.SetValue(ChangeFieldOfViewCursorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the default camera (used when resetting the view).
        /// </summary>
        /// <value> The default camera. </value>
        public ProjectionCamera DefaultCamera
        {
            get
            {
                return (ProjectionCamera)this.GetValue(DefaultCameraProperty);
            }

            set
            {
                this.SetValue(DefaultCameraProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return (bool)this.GetValue(EnabledProperty);
            }

            set
            {
                this.SetValue(EnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets InertiaFactor.
        /// </summary>
        public double InertiaFactor
        {
            get
            {
                return (double)this.GetValue(InertiaFactorProperty);
            }

            set
            {
                this.SetValue(InertiaFactorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether InfiniteSpin.
        /// </summary>
        public bool InfiniteSpin
        {
            get
            {
                return (bool)this.GetValue(InfiniteSpinProperty);
            }

            set
            {
                this.SetValue(InfiniteSpinProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsActive.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.Enabled && this.Viewport != null && this.ActualCamera != null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether field of view can be changed.
        /// </summary>
        public bool IsChangeFieldOfViewEnabled
        {
            get
            {
                return (bool)this.GetValue(IsChangeFieldOfViewEnabledProperty);
            }

            set
            {
                this.SetValue(IsChangeFieldOfViewEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether inertia is enabled for the camera manipulations.
        /// </summary>
        /// <value><c>true</c> if inertia is enabled; otherwise, <c>false</c>.</value>
        public bool IsInertiaEnabled
        {
            get
            {
                return (bool)this.GetValue(IsInertiaEnabledProperty);
            }

            set
            {
                this.SetValue(IsInertiaEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether move is enabled.
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

        /// <summary>
        /// Gets or sets a value indicating whether pan is enabled.
        /// </summary>
        public bool IsPanEnabled
        {
            get
            {
                return (bool)this.GetValue(IsPanEnabledProperty);
            }

            set
            {
                this.SetValue(IsPanEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsRotationEnabled.
        /// </summary>
        public bool IsRotationEnabled
        {
            get
            {
                return (bool)this.GetValue(IsRotationEnabledProperty);
            }

            set
            {
                this.SetValue(IsRotationEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether touch zoom (pinch gesture) is enabled.
        /// </summary>
        /// <value> <c>true</c> if touch zoom is enabled; otherwise, <c>false</c> . </value>
        public bool IsTouchZoomEnabled
        {
            get
            {
                return (bool)this.GetValue(IsTouchZoomEnabledProperty);
            }

            set
            {
                this.SetValue(IsTouchZoomEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsZoomEnabled.
        /// </summary>
        public bool IsZoomEnabled
        {
            get
            {
                return (bool)this.GetValue(IsZoomEnabledProperty);
            }

            set
            {
                this.SetValue(IsZoomEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sensitivity for pan by the left and right keys.
        /// </summary>
        /// <value> The pan sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the pan direction.
        /// </remarks>
        public double LeftRightPanSensitivity
        {
            get
            {
                return (double)this.GetValue(LeftRightPanSensitivityProperty);
            }

            set
            {
                this.SetValue(LeftRightPanSensitivityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sensitivity for rotation by the left and right keys.
        /// </summary>
        /// <value> The rotation sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the rotation direction.
        /// </remarks>
        public double LeftRightRotationSensitivity
        {
            get
            {
                return (double)this.GetValue(LeftRightRotationSensitivityProperty);
            }

            set
            {
                this.SetValue(LeftRightRotationSensitivityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum field of view.
        /// </summary>
        /// <value> The maximum field of view. </value>
        public double MaximumFieldOfView
        {
            get
            {
                return (double)this.GetValue(MaximumFieldOfViewProperty);
            }

            set
            {
                this.SetValue(MaximumFieldOfViewProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum field of view.
        /// </summary>
        /// <value> The minimum field of view. </value>
        public double MinimumFieldOfView
        {
            get
            {
                return (double)this.GetValue(MinimumFieldOfViewProperty);
            }

            set
            {
                this.SetValue(MinimumFieldOfViewProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the model up direction.
        /// </summary>
        public Vector3D ModelUpDirection
        {
            get
            {
                return (Vector3D)this.GetValue(ModelUpDirectionProperty);
            }

            set
            {
                this.SetValue(ModelUpDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the move sensitivity.
        /// </summary>
        /// <value> The move sensitivity. </value>
        public double MoveSensitivity
        {
            get
            {
                return (double)this.GetValue(MoveSensitivityProperty);
            }

            set
            {
                this.SetValue(MoveSensitivityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sensitivity for zoom by the page up and page down keys.
        /// </summary>
        /// <value> The zoom sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the zoom direction.
        /// </remarks>
        public double PageUpDownZoomSensitivity
        {
            get
            {
                return (double)this.GetValue(PageUpDownZoomSensitivityProperty);
            }

            set
            {
                this.SetValue(PageUpDownZoomSensitivityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pan cursor.
        /// </summary>
        /// <value> The pan cursor. </value>
        public Cursor PanCursor
        {
            get
            {
                return (Cursor)this.GetValue(PanCursorProperty);
            }

            set
            {
                this.SetValue(PanCursorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to rotate around the mouse down point.
        /// </summary>
        /// <value> <c>true</c> if rotation around the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool RotateAroundMouseDownPoint
        {
            get
            {
                return (bool)this.GetValue(RotateAroundMouseDownPointProperty);
            }

            set
            {
                this.SetValue(RotateAroundMouseDownPointProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to rotate around a fixed point.
        /// </summary>
        /// <value> <c>true</c> if rotation around a fixed point is enabled; otherwise, <c>false</c> . </value>
        public bool FixedRotationPointEnabled
        {
            get
            {
                return (bool)this.GetValue(FixedRotationPointEnabledProperty);
            }

            set
            {
                this.SetValue(FixedRotationPointEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the center of rotation.
        /// </summary>
        /// <value> <c>true</c> if rotation around a fixed point is enabled; otherwise, <c>false</c> . </value>
        public Point3D FixedRotationPoint
        {
            get
            {
                return (Point3D)this.GetValue(FixedRotationPointProperty);
            }

            set
            {
                this.SetValue(FixedRotationPointProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rotate cursor.
        /// </summary>
        /// <value> The rotate cursor. </value>
        public Cursor RotateCursor
        {
            get
            {
                return (Cursor)this.GetValue(RotateCursorProperty);
            }

            set
            {
                this.SetValue(RotateCursorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rotation sensitivity (degrees/pixel).
        /// </summary>
        /// <value> The rotation sensitivity. </value>
        public double RotationSensitivity
        {
            get
            {
                return (double)this.GetValue(RotationSensitivityProperty);
            }

            set
            {
                this.SetValue(RotationSensitivityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show a target adorner when manipulating the camera.
        /// </summary>
        public bool ShowCameraTarget
        {
            get
            {
                return (bool)this.GetValue(ShowCameraTargetProperty);
            }

            set
            {
                this.SetValue(ShowCameraTargetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the max duration of mouse drag to activate spin.
        /// </summary>
        /// <remarks>
        /// If the time between mouse down and mouse up is less than this value, spin is activated.
        /// </remarks>
        public int SpinReleaseTime
        {
            get
            {
                return (int)this.GetValue(SpinReleaseTimeProperty);
            }

            set
            {
                this.SetValue(SpinReleaseTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sensitivity for pan by the up and down keys.
        /// </summary>
        /// <value> The pan sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the pan direction.
        /// </remarks>
        public double UpDownPanSensitivity
        {
            get
            {
                return (double)this.GetValue(UpDownPanSensitivityProperty);
            }

            set
            {
                this.SetValue(UpDownPanSensitivityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sensitivity for rotation by the up and down keys.
        /// </summary>
        /// <value> The rotation sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the rotation direction.
        /// </remarks>
        public double UpDownRotationSensitivity
        {
            get
            {
                return (double)this.GetValue(UpDownRotationSensitivityProperty);
            }

            set
            {
                this.SetValue(UpDownRotationSensitivityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Viewport.
        /// </summary>
        public Viewport3D Viewport
        {
            get
            {
                return (Viewport3D)this.GetValue(ViewportProperty);
            }

            set
            {
                this.SetValue(ViewportProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to zoom around mouse down point.
        /// </summary>
        /// <value> <c>true</c> if zooming around the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool ZoomAroundMouseDownPoint
        {
            get
            {
                return (bool)this.GetValue(ZoomAroundMouseDownPointProperty);
            }

            set
            {
                this.SetValue(ZoomAroundMouseDownPointProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to snap the mouse down point to a model.
        /// </summary>
        /// <value> <c>true</c> if snapping the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool SnapMouseDownPoint
        {
            get
            {
                return (bool)this.GetValue(SnapMouseDownPointProperty);
            }

            set
            {
                this.SetValue(SnapMouseDownPointProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the zoom cursor.
        /// </summary>
        /// <value> The zoom cursor. </value>
        public Cursor ZoomCursor
        {
            get
            {
                return (Cursor)this.GetValue(ZoomCursorProperty);
            }

            set
            {
                this.SetValue(ZoomCursorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the zoom rectangle cursor.
        /// </summary>
        /// <value> The zoom rectangle cursor. </value>
        public Cursor ZoomRectangleCursor
        {
            get
            {
                return (Cursor)this.GetValue(ZoomRectangleCursorProperty);
            }

            set
            {
                this.SetValue(ZoomRectangleCursorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ZoomSensitivity.
        /// </summary>
        public double ZoomSensitivity
        {
            get
            {
                return (double)this.GetValue(ZoomSensitivityProperty);
            }

            set
            {
                this.SetValue(ZoomSensitivityProperty, value);
            }
        }


        /// <summary>
        /// Efficiency option, lower values decrease computation time for camera interaction when
        /// RotateAroundMouseDownPoint or ZoomAroundMouseDownPoint is set to true in inspect mode.
        /// Note: Will mostly save on computation time once the bounds are already calculated and cashed within the MeshGeometry3D.
        /// </summary>
        public int RotataAroundClosestVertexComplexity { get; set; }

        /// <summary>
        /// Gets a value indicating whether IsOrthographicCamera.
        /// </summary>
        protected bool IsOrthographicCamera
        {
            get
            {
                return this.ActualCamera is OrthographicCamera;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsPerspectiveCamera.
        /// </summary>
        protected bool IsPerspectiveCamera
        {
            get
            {
                return this.ActualCamera is PerspectiveCamera;
            }
        }

        /// <summary>
        /// Gets OrthographicCamera.
        /// </summary>
        protected OrthographicCamera OrthographicCamera
        {
            get
            {
                return this.ActualCamera as OrthographicCamera;
            }
        }

        /// <summary>
        /// Gets PerspectiveCamera.
        /// </summary>
        protected PerspectiveCamera PerspectiveCamera
        {
            get
            {
                return this.ActualCamera as PerspectiveCamera;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [limit FPS].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [limit FPS]; otherwise, <c>false</c>.
        /// </value>
        public bool LimitFPS { set; get; } = true;
        #region Private Variables
        private TimeSpan prevTime;
        #endregion
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
            this.AddPanForce(this.FindPanVector(dx, dy));
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
            if (this.IsInertiaEnabled)
            {
                this.panSpeed += pan * 40;
            }
            else
            {
                this.panHandler.Pan(pan);
            }
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
            if (!this.IsRotationEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            if (this.IsInertiaEnabled)
            {
                this.rotationPoint3D = this.CameraTarget;
                this.rotationPosition = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
                this.rotationSpeed.X += dx * 40;
                this.rotationSpeed.Y += dy * 40;
            }
            else
            {
                this.rotationPosition = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
                this.rotateHandler.Rotate(
                    this.rotationPosition, this.rotationPosition + new Vector(dx, dy), this.CameraTarget);
            }
        }

        /// <summary>
        /// Adds the zoom force.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        public void AddZoomForce(double delta)
        {
            this.AddZoomForce(delta, this.CameraTarget);
        }

        /// <summary>
        /// Adds the zoom force.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        /// <param name="zoomOrigin">
        /// The zoom origin.
        /// </param>
        public void AddZoomForce(double delta, Point3D zoomOrigin)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            this.PushCameraSetting();

            if (this.IsInertiaEnabled)
            {
                this.zoomPoint3D = zoomOrigin;
                this.zoomSpeed += delta * 8;
            }
            else
            {
                this.zoomHandler.Zoom(delta, zoomOrigin);
            }
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
            if (!this.IsRotationEnabled)
            {
                return;
            }

            this.StopAnimations();
            this.PushCameraSetting();
            this.ActualCamera.ChangeDirection(lookDir, upDir, animationTime);
        }

        /// <summary>
        /// Changes the direction of the camera.
        /// </summary>
        /// <param name="lookDir">
        /// The look direction.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void ChangeDirection(Vector3D lookDir, double animationTime = 500)
        {
            if (!this.IsRotationEnabled)
            {
                return;
            }

            this.StopAnimations();
            this.PushCameraSetting();
            this.ActualCamera.ChangeDirection(lookDir, this.ActualCamera.UpDirection, animationTime);
        }

        /// <summary>
        /// Hides the rectangle.
        /// </summary>
        public void HideRectangle()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            if (myAdornerLayer == null) { return; }
            if (this.rectangleAdorner != null)
            {
                myAdornerLayer.Remove(this.rectangleAdorner);
            }

            this.rectangleAdorner = null;

            this.Viewport.InvalidateVisual();
        }

        /// <summary>
        /// Hides the target adorner.
        /// </summary>
        public void HideTargetAdorner()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            if (myAdornerLayer == null) { return; }
            if (this.targetAdorner != null)
            {
                myAdornerLayer.Remove(this.targetAdorner);
            }

            this.targetAdorner = null;

            // the adorner sometimes leaves some 'dust', so refresh the viewport
            this.RefreshViewport();
        }

        /// <summary>
        /// Change the "look-at" point.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        [Obsolete]
        public void LookAt(Point3D target, double animationTime)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            this.Camera.LookAt(target, animationTime);
        }

        /// <summary>
        /// Push the current camera settings on an internal stack.
        /// </summary>
        public void PushCameraSetting()
        {
            this.cameraHistory.AddLast(new CameraSetting(this.ActualCamera));
            if (this.cameraHistory.Count > 100)
            {
                this.cameraHistory.RemoveFirst();
            }
        }

        /// <summary>
        /// Resets the camera.
        /// </summary>
        public void ResetCamera()
        {
            if (!this.IsZoomEnabled || !this.IsRotationEnabled || !this.IsPanEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            if (this.DefaultCamera != null)
            {
                this.DefaultCamera.Copy(this.ActualCamera);
            }
            else
            {
                this.ActualCamera.Reset();
                this.ActualCamera.ZoomExtents(this.Viewport);
            }
        }

        /// <summary>
        /// Resets the camera up direction.
        /// </summary>
        public void ResetCameraUpDirection()
        {
            this.CameraUpDirection = this.ModelUpDirection;
        }

        /// <summary>
        /// Restores the most recent camera setting from the internal stack.
        /// </summary>
        /// <returns> The restore camera setting. </returns>
        public bool RestoreCameraSetting()
        {
            if (this.cameraHistory.Count > 0)
            {
                var cs = this.cameraHistory.Last.Value;
                this.cameraHistory.RemoveLast();
                cs.UpdateCamera(this.ActualCamera);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Shows the rectangle.
        /// </summary>
        /// <param name="rect">
        /// The rectangle.
        /// </param>
        /// <param name="color1">
        /// The color 1.
        /// </param>
        /// <param name="color2">
        /// The color 2.
        /// </param>
        public void ShowRectangle(Rect rect, Color color1, Color color2)
        {
            if (this.rectangleAdorner != null)
            {
                return;
            }

            var myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            if (myAdornerLayer == null) { return; }
            this.rectangleAdorner = new RectangleAdorner(
                this.Viewport, rect, color1, color2, 3, 1, 10, DashStyles.Solid);
            myAdornerLayer.Add(this.rectangleAdorner);
        }

        /// <summary>
        /// Shows the target adorner.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
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

            var myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            if (myAdornerLayer == null) { return; }
            this.targetAdorner = new TargetSymbolAdorner(this.Viewport, position);
            myAdornerLayer.Add(this.targetAdorner);
        }

        /// <summary>
        /// Starts the spin.
        /// </summary>
        /// <param name="speed">
        /// The speed.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="aroundPoint">
        /// The spin around point.
        /// </param>
        public void StartSpin(Vector speed, Point position, Point3D aroundPoint)
        {
            this.spinningSpeed = speed;
            this.spinningPosition = position;
            this.spinningPoint3D = aroundPoint;
            this.isSpinning = true;
        }

        /// <summary>
        /// Stops the spin.
        /// </summary>
        public void StopSpin()
        {
            this.isSpinning = false;
        }

        /// <summary>
        /// Updates the rectangle.
        /// </summary>
        /// <param name="rect">
        /// The rectangle.
        /// </param>
        public void UpdateRectangle(Rect rect)
        {
            if (this.rectangleAdorner == null)
            {
                return;
            }

            this.rectangleAdorner.Rectangle = rect;
            this.rectangleAdorner.InvalidateVisual();
        }

        /// <summary>
        /// Zooms by the specified delta value.
        /// </summary>
        /// <param name="delta">
        /// The delta value.
        /// </param>
        public void Zoom(double delta)
        {
            this.zoomHandler.Zoom(delta);
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
            this.ActualCamera.ZoomExtents(this.Viewport, animationTime);
        }

        /// <summary>
        /// Restores the cursor from the cursor stack.
        /// </summary>
        public void RestoreCursor()
        {
            this.Cursor = this.cursorStack.Pop();
        }

        /// <summary>
        /// Sets the cursor and pushes the current cursor to the cursor stack.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        /// <remarks>Use <see cref="RestoreCursor" /> to restore the cursor again.</remarks>
        public void SetCursor(Cursor cursor)
        {
            this.cursorStack.Push(this.Cursor);
            this.Cursor = cursor;
        }

        /// <summary>
        /// Raises the LookAtChanged event.
        /// </summary>
        protected internal virtual void OnLookAtChanged()
        {
            var args = new RoutedEventArgs(LookAtChangedEvent);
            this.RaiseEvent(args);
        }

        /// <summary>
        /// Raises the ZoomedByRectangle event.
        /// </summary>
        protected internal virtual void OnZoomedByRectangle()
        {
            var args = new RoutedEventArgs(ZoomedByRectangleEvent);
            this.RaiseEvent(args);
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.UIElement.ManipulationCompleted"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            var p = e.ManipulationOrigin + e.TotalManipulation.Translation;

            if (this.manipulatorCount == 1)
            {
                this.rotateHandler.Completed(new ManipulationEventArgs(p));
            }

            if (this.manipulatorCount == 2)
            {
                this.panHandler.Completed(new ManipulationEventArgs(p));
                this.zoomHandler.Completed(new ManipulationEventArgs(p));
            }

            e.Handled = true;
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.UIElement.ManipulationDelta"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);

            // number of manipulators (fingers)
            int n = e.Manipulators.Count();
            var position = this.touchPreviousPoint + e.DeltaManipulation.Translation;
            this.touchPreviousPoint = position;

            // http://msdn.microsoft.com/en-us/library/system.windows.uielement.manipulationdelta.aspx

            //// System.Diagnostics.Debug.WriteLine("OnManipulationDelta: T={0}, S={1}, R={2}, O={3}", e.DeltaManipulation.Translation, e.DeltaManipulation.Scale, e.DeltaManipulation.Rotation, e.ManipulationOrigin);
            //// System.Diagnostics.Debug.WriteLine(n + " Delta:" + e.DeltaManipulation.Translation + " Origin:" + e.ManipulationOrigin + " pos:" + position);

            if (this.manipulatorCount != n)
            {
                // the number of manipulators has changed
                if (this.manipulatorCount == 1)
                {
                    this.rotateHandler.Completed(new ManipulationEventArgs(position));
                }

                if (this.manipulatorCount == 2)
                {
                    this.panHandler.Completed(new ManipulationEventArgs(position));
                    this.zoomHandler.Completed(new ManipulationEventArgs(position));
                }

                if (n == 2)
                {
                    this.panHandler.Started(new ManipulationEventArgs(position));
                    this.zoomHandler.Started(new ManipulationEventArgs(e.ManipulationOrigin));
                }
                else
                {
                    this.rotateHandler.Started(new ManipulationEventArgs(position));
                }

                // skip this event, the origin may have changed
                this.manipulatorCount = n;
                e.Handled = true;
                return;
            }

            if (n == 1)
            {
                // one finger rotates
                this.rotateHandler.Delta(new ManipulationEventArgs(position));
            }

            if (n == 2)
            {
                // two fingers pans
                this.panHandler.Delta(new ManipulationEventArgs(position));
            }

            if (this.IsTouchZoomEnabled && n == 2)
            {
                var zoomAroundPoint = this.zoomHandler.UnProject(
                    e.ManipulationOrigin, this.zoomHandler.Origin, this.CameraLookDirection);
                if (zoomAroundPoint != null)
                {
                    this.zoomHandler.Zoom(1 - (e.DeltaManipulation.Scale.Length / Math.Sqrt(2)), zoomAroundPoint.Value);
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.UIElement.ManipulationStarted"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);
            this.Focus();
            this.touchPreviousPoint = e.ManipulationOrigin;
            this.manipulatorCount = 0;

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled MouseDown attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.
        /// </param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();
            if (e.ChangedButton == MouseButton.XButton1)
            {
                this.RestoreCameraSetting();
            }
        }

        /// <summary>
        /// Invoked when an unhandled StylusSystemGesture attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.StylusSystemGestureEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            base.OnStylusSystemGesture(e);

            // Debug.WriteLine("OnStylusSystemGesture: " + e.SystemGesture);
            if (e.SystemGesture == SystemGesture.HoldEnter)
            {
                var p = e.GetPosition(this);
                this.changeLookAtHandler.Started(new ManipulationEventArgs(p));
                this.changeLookAtHandler.Completed(new ManipulationEventArgs(p));
                e.Handled = true;
            }

            if (e.SystemGesture == SystemGesture.TwoFingerTap)
            {
                this.ZoomExtents();
                e.Handled = true;
            }
        }

        /// <summary>
        /// The camera changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void CameraChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CameraController)d).OnCameraChanged();
        }

        /// <summary>
        /// The viewport changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void ViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CameraController)d).OnViewportChanged();
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
            this.ChangeDirection(new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
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
            this.ChangeDirection(new Vector3D(0, 0, 1), new Vector3D(0, -1, 0));
        }

        /// <summary>
        /// The camera controller_ loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void CameraControllerLoaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeEvents();
        }

        /// <summary>
        /// Called when the CameraController is unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void CameraControllerUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnSubscribeEvents();
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
        /// Finds the pan vector.
        /// </summary>
        /// <param name="dx">
        /// The delta x.
        /// </param>
        /// <param name="dy">
        /// The delta y.
        /// </param>
        /// <returns>
        /// The <see cref="Vector3D"/> .
        /// </returns>
        private Vector3D FindPanVector(double dx, double dy)
        {
            var axis1 = Vector3D.CrossProduct(this.CameraLookDirection, this.CameraUpDirection);
            var axis2 = Vector3D.CrossProduct(axis1, this.CameraLookDirection);
            axis1.Normalize();
            axis2.Normalize();
            var l = this.CameraLookDirection.Length;
            var f = l * 0.001;
            var move = (-axis1 * f * dx) + (axis2 * f * dy);

            // this should be dependent on distance to target?
            return move;
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
            this.ChangeDirection(new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1));
        }

        /// <summary>
        /// Initializes the input bindings.
        /// </summary>
        private void InitializeBindings()
        {
            this.changeLookAtHandler = new RotateHandler(this, true);
            this.rotateHandler = new RotateHandler(this);
            this.zoomRectangleHandler = new ZoomRectangleHandler(this);
            this.zoomHandler = new ZoomHandler(this);
            this.panHandler = new PanHandler(this);
            this.changeFieldOfViewHandler = new ZoomHandler(this, true);

            this.CommandBindings.Add(new CommandBinding(ZoomRectangleCommand, this.zoomRectangleHandler.Execute));
            this.CommandBindings.Add(new CommandBinding(ZoomExtentsCommand, this.ZoomExtentsHandler));
            this.CommandBindings.Add(new CommandBinding(RotateCommand, this.rotateHandler.Execute));
            this.CommandBindings.Add(new CommandBinding(ZoomCommand, this.zoomHandler.Execute));
            this.CommandBindings.Add(new CommandBinding(PanCommand, this.panHandler.Execute));
            this.CommandBindings.Add(new CommandBinding(ResetCameraCommand, this.ResetCameraHandler));
            this.CommandBindings.Add(new CommandBinding(ChangeLookAtCommand, this.changeLookAtHandler.Execute));
            this.CommandBindings.Add(
                new CommandBinding(ChangeFieldOfViewCommand, this.changeFieldOfViewHandler.Execute));

            this.CommandBindings.Add(new CommandBinding(TopViewCommand, this.TopViewHandler));
            this.CommandBindings.Add(new CommandBinding(BottomViewCommand, this.BottomViewHandler));
            this.CommandBindings.Add(new CommandBinding(LeftViewCommand, this.LeftViewHandler));
            this.CommandBindings.Add(new CommandBinding(RightViewCommand, this.RightViewHandler));
            this.CommandBindings.Add(new CommandBinding(FrontViewCommand, this.FrontViewHandler));
            this.CommandBindings.Add(new CommandBinding(BackViewCommand, this.BackViewHandler));
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
            this.ChangeDirection(new Vector3D(0, -1, 0), new Vector3D(0, 0, 1));
        }

        /// <summary>
        /// The on camera changed.
        /// </summary>
        private void OnCameraChanged()
        {
            this.cameraHistory.Clear();
            this.PushCameraSetting();
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
            if (LimitFPS && prevTime == e.RenderingTime)
            {
                return;
            }
            prevTime = e.RenderingTime;
            var ticks = e.RenderingTime.Ticks;
            var time = 100e-9 * (ticks - this.lastTick);

            if (this.lastTick != 0)
            {
                this.OnTimeStep(time);
            }

            this.lastTick = ticks;
        }

        /// <summary>
        /// Called when a key is pressed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.
        /// </param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
            var shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            var control = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            var f = control ? 0.25 : 1;

            if (!shift)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        this.AddRotateForce(-1 * f * this.LeftRightRotationSensitivity, 0);
                        e.Handled = true;
                        break;
                    case Key.Right:
                        this.AddRotateForce(1 * f * this.LeftRightRotationSensitivity, 0);
                        e.Handled = true;
                        break;
                    case Key.Up:
                        this.AddRotateForce(0, -1 * f * this.UpDownRotationSensitivity);
                        e.Handled = true;
                        break;
                    case Key.Down:
                        this.AddRotateForce(0, 1 * f * this.UpDownRotationSensitivity);
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Left:
                        this.AddPanForce(-5 * f * this.LeftRightPanSensitivity, 0);
                        e.Handled = true;
                        break;
                    case Key.Right:
                        this.AddPanForce(5 * f * this.LeftRightPanSensitivity, 0);
                        e.Handled = true;
                        break;
                    case Key.Up:
                        this.AddPanForce(0, -5 * f * this.UpDownPanSensitivity);
                        e.Handled = true;
                        break;
                    case Key.Down:
                        this.AddPanForce(0, 5 * f * this.UpDownPanSensitivity);
                        e.Handled = true;
                        break;
                }
            }

            switch (e.Key)
            {
                case Key.PageUp:
                    this.AddZoomForce(-0.1 * f * this.PageUpDownZoomSensitivity);
                    e.Handled = true;
                    break;
                case Key.PageDown:
                    this.AddZoomForce(0.1 * f * this.PageUpDownZoomSensitivity);
                    e.Handled = true;
                    break;
                case Key.Back:
                    if (this.RestoreCameraSetting())
                    {
                        e.Handled = true;
                    }

                    break;
            }

            switch (e.Key)
            {
                case Key.W:
                    this.AddMoveForce(0, 0, 0.1 * f * this.MoveSensitivity);
                    break;
                case Key.A:
                    this.AddMoveForce(-0.1 * f * this.LeftRightPanSensitivity, 0, 0);
                    break;
                case Key.S:
                    this.AddMoveForce(0, 0, -0.1 * f * this.MoveSensitivity);
                    break;
                case Key.D:
                    this.AddMoveForce(0.1 * f * this.LeftRightPanSensitivity, 0, 0);
                    break;
                case Key.Z:
                    this.AddMoveForce(0, -0.1 * f * this.LeftRightPanSensitivity, 0);
                    break;
                case Key.Q:
                    this.AddMoveForce(0, 0.1 * f * this.LeftRightPanSensitivity, 0);
                    break;
            }
        }

        /// <summary>
        /// Called when the mouse wheel is moved.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.
        /// </param>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            if (this.ZoomAroundMouseDownPoint)
            {
                var point = e.GetPosition(this);

                Point3D? nearestPoint = new Closest3DPointHitTester(this.Viewport, this.RotataAroundClosestVertexComplexity)
                    .CalculateMouseDownNearestPoint(point, SnapMouseDownPoint).MouseDownNearestPoint3D;
                if (nearestPoint.HasValue)
                {
                    this.AddZoomForce(-e.Delta * 0.001, nearestPoint.Value);
                    e.Handled = true;
                    return;
                }
            }

            this.AddZoomForce(-e.Delta * 0.001);
            e.Handled = true;
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
            var factor = this.IsInertiaEnabled ? Math.Pow(this.InertiaFactor, time / 0.012) : 0;
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
                this.zoomHandler.MoveCameraPosition(this.moveSpeed * time);
                this.moveSpeed *= factor;
            }

            if (Math.Abs(this.zoomSpeed) > 0.1)
            {
                this.zoomHandler.Zoom(this.zoomSpeed * time, this.zoomPoint3D);
                this.zoomSpeed *= factor;
            }
        }

        /// <summary>
        /// The on viewport changed.
        /// </summary>
        private void OnViewportChanged()
        {
        }

        /// <summary>
        /// The refresh viewport.
        /// </summary>
        private void RefreshViewport()
        {
            // todo: this is a hack, should be improved

            // var mg = new ModelVisual3D { Content = new AmbientLight(Colors.White) };
            // Viewport.Children.Add(mg);
            // Viewport.Children.Remove(mg);
            var c = this.Viewport.Camera;
            this.Viewport.Camera = null;
            this.Viewport.Camera = c;

            // var w = Viewport.Width;
            // Viewport.Width = w-1;
            // Viewport.Width = w;

            // Viewport.InvalidateVisual();
        }

        /// <summary>
        /// The reset camera event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void ResetCameraHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.IsPanEnabled && this.IsZoomEnabled && this.CameraMode != CameraMode.FixedPosition)
            {
                this.StopAnimations();
                this.ResetCamera();
            }
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
            this.ChangeDirection(new Vector3D(0, 1, 0), new Vector3D(0, 0, 1));
        }

        /// <summary>
        /// The stop animations.
        /// </summary>
        private void StopAnimations()
        {
            this.rotationSpeed = new Vector();
            this.panSpeed = new Vector3D();
            this.zoomSpeed = 0;
        }

        /// <summary>
        /// The subscribe events.
        /// </summary>
        private void SubscribeEvents()
        {
            this.MouseWheel += this.OnMouseWheel;
            this.KeyDown += this.OnKeyDown;
            RenderingEventManager.AddListener(this.renderingEventListener);
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
            this.ChangeDirection(new Vector3D(0, 0, -1), new Vector3D(0, 1, 0));
        }

        /// <summary>
        /// The un subscribe events.
        /// </summary>
        private void UnSubscribeEvents()
        {
            this.MouseWheel -= this.OnMouseWheel;
            this.KeyDown -= this.OnKeyDown;
            RenderingEventManager.RemoveListener(this.renderingEventListener);
        }

        /// <summary>
        /// The Zoom extents event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void ZoomExtentsHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.StopAnimations();
            this.ZoomExtents();
        }
    }
}
