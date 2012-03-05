// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraController.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// <summary>
//   A control that manipulates the camera by mouse and keyboard gestures.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    ///   A control that manipulates the camera by mouse and keyboard gestures.
    /// </summary>
    public class CameraController : Grid
    {
        /// <summary>
        ///   Gets or sets the minimum field of view.
        /// </summary>
        /// <value> The minimum fov. </value>
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
        ///   The minimum fov property.
        /// </summary>
        public static readonly DependencyProperty MinimumFieldOfViewProperty =
            DependencyProperty.Register(
                "MinimumFieldOfView", typeof(double), typeof(CameraController), new UIPropertyMetadata(5.0));

        /// <summary>
        ///   Gets or sets the maximum field of view.
        /// </summary>
        /// <value> The maximum fov. </value>
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
        ///   The maximum fov property.
        /// </summary>
        public static readonly DependencyProperty MaximumFieldOfViewProperty =
            DependencyProperty.Register(
                "MaximumFieldOfView", typeof(double), typeof(CameraController), new UIPropertyMetadata(160.0));

        /// <summary>
        ///   The rotation sensitivity property.
        /// </summary>
        public static readonly DependencyProperty RotationSensitivityProperty =
            DependencyProperty.Register(
                "RotationSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        ///   The camera property.
        /// </summary>
        public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
            "Camera",
            typeof(ProjectionCamera),
            typeof(CameraController),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CameraChanged));

        /// <summary>
        ///   Gets or sets the default camera (used when resetting the view).
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
        ///   The default camera property.
        /// </summary>
        public static readonly DependencyProperty DefaultCameraProperty = DependencyProperty.Register(
            "DefaultCamera", typeof(ProjectionCamera), typeof(CameraController), new UIPropertyMetadata(null));

        /// <summary>
        ///   The camera changed.
        /// </summary>
        /// <param name="d"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private static void CameraChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CameraController)d).OnCameraChanged();
        }

        /// <summary>
        ///   The on camera changed.
        /// </summary>
        private void OnCameraChanged()
        {
            this.cameraHistory.Clear();
            this.PushCameraSetting();
        }

        /// <summary>
        ///   The fixed mouse down point property.
        /// </summary>
        public static readonly DependencyProperty RotateAroundMouseDownPointProperty =
            DependencyProperty.Register(
                "RotateAroundMouseDownPoint", typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

        /// <summary>
        ///   The inertia factor property.
        /// </summary>
        public static readonly DependencyProperty InertiaFactorProperty = DependencyProperty.Register(
            "InertiaFactor", typeof(double), typeof(CameraController), new UIPropertyMetadata(0.9));

        /// <summary>
        ///   The infinite spin property.
        /// </summary>
        public static readonly DependencyProperty InfiniteSpinProperty = DependencyProperty.Register(
            "InfiniteSpin", typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

        /// <summary>
        ///   The is pan enabled property.
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
            "IsPanEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        ///   Gets or sets a value indicating whether field of view can be changed.
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
        ///   The is change fov enabled property.
        /// </summary>
        public static readonly DependencyProperty IsChangeFieldOfViewEnabledProperty =
            DependencyProperty.Register(
                "IsChangeFieldOfViewEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        ///   The is zoom enabled property.
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
            "IsZoomEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        ///   The show camera target property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraTargetProperty =
            DependencyProperty.Register(
                "ShowCameraTarget", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        ///   The spin release time property.
        /// </summary>
        public static readonly DependencyProperty SpinReleaseTimeProperty =
            DependencyProperty.Register(
                "SpinReleaseTime", typeof(int), typeof(CameraController), new UIPropertyMetadata(200));

        /// <summary>
        ///   The camera mode property.
        /// </summary>
        public static readonly DependencyProperty CameraModeProperty = DependencyProperty.Register(
            "CameraMode", typeof(CameraMode), typeof(CameraController), new UIPropertyMetadata(CameraMode.Inspect));

        /// <summary>
        ///   The camera rotation mode property.
        /// </summary>
        public static readonly DependencyProperty CameraRotationModeProperty =
            DependencyProperty.Register(
                "CameraRotationMode",
                typeof(CameraRotationMode),
                typeof(CameraController),
                new UIPropertyMetadata(CameraRotationMode.Turntable));

        /// <summary>
        ///   The enabled property.
        /// </summary>
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register(
            "Enabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        ///   The viewport property.
        /// </summary>
        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(
            "Viewport", typeof(Viewport3D), typeof(CameraController), new PropertyMetadata(null, ViewportChanged));

        /// <summary>
        ///   The up direction property.
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register(
            "ModelUpDirection",
            typeof(Vector3D),
            typeof(CameraController),
            new UIPropertyMetadata(new Vector3D(0, 0, 1)));

        /// <summary>
        ///   Gets or sets the rotate cursor.
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
        ///   The rotate cursor property.
        /// </summary>
        public static readonly DependencyProperty RotateCursorProperty = DependencyProperty.Register(
            "RotateCursor", typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.SizeAll));

        /// <summary>
        ///   Gets or sets the zoom cursor.
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
        ///   The zoom cursor property.
        /// </summary>
        public static readonly DependencyProperty ZoomCursorProperty = DependencyProperty.Register(
            "ZoomCursor", typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.SizeNS));

        /// <summary>
        ///   Gets or sets the zoom rectangle cursor.
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
        ///   The zoom rectangle cursor property.
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleCursorProperty =
            DependencyProperty.Register(
                "ZoomRectangleCursor",
                typeof(Cursor),
                typeof(CameraController),
                new UIPropertyMetadata(Cursors.ScrollSE));

        /// <summary>
        ///   Gets or sets the change fov cursor.
        /// </summary>
        /// <value> The change fov cursor. </value>
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
        ///   The change fov cursor property.
        /// </summary>
        public static readonly DependencyProperty ChangeFieldOfViewCursorProperty =
            DependencyProperty.Register(
                "ChangeFieldOfViewCursor",
                typeof(Cursor),
                typeof(CameraController),
                new UIPropertyMetadata(Cursors.ScrollNS));

        /// <summary>
        ///   Gets or sets the pan cursor.
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
        ///   The pan cursor property.
        /// </summary>
        public static readonly DependencyProperty PanCursorProperty = DependencyProperty.Register(
            "PanCursor", typeof(Cursor), typeof(CameraController), new UIPropertyMetadata(Cursors.Hand));

        /// <summary>
        ///   The 3D rotation point.
        /// </summary>
        private Point3D rotationPoint3D;

        /// <summary>
        ///   The is spinning flag.
        /// </summary>
        private bool isSpinning;

        /// <summary>
        ///   The last tick.
        /// </summary>
        private long lastTick;

        /// <summary>
        ///   The pan speed.
        /// </summary>
        private Vector3D panSpeed;

        /// <summary>
        ///   The rotation speed.
        /// </summary>
        private Vector rotationSpeed;

        /// <summary>
        ///   The spinning speed.
        /// </summary>
        private Vector spinningSpeed;

        /// <summary>
        ///   The target adorner.
        /// </summary>
        private Adorner targetAdorner;

        /// <summary>
        ///   The rectangle adorner.
        /// </summary>
        private RectangleAdorner rectangleAdorner;

        /// <summary>
        ///   The zoom speed.
        /// </summary>
        private double zoomSpeed;

        /// <summary>
        ///   Initializes static members of the <see cref="CameraController" /> class.
        /// </summary>
        static CameraController()
        {
            BackgroundProperty.OverrideMetadata(
                typeof(CameraController), new FrameworkPropertyMetadata(Brushes.Transparent));
            FocusVisualStyleProperty.OverrideMetadata(typeof(CameraController), new FrameworkPropertyMetadata(null));
        }

        /// <summary>
        ///   The zoom rectangle command.
        /// </summary>
        public static RoutedCommand ZoomRectangleCommand = new RoutedCommand();

        /// <summary>
        ///   The zoom extents command.
        /// </summary>
        public static RoutedCommand ZoomExtentsCommand = new RoutedCommand();

        /// <summary>
        ///   The rotate command.
        /// </summary>
        public static RoutedCommand RotateCommand = new RoutedCommand();

        /// <summary>
        ///   The pan command.
        /// </summary>
        public static RoutedCommand PanCommand = new RoutedCommand();

        /// <summary>
        ///   The zoom command.
        /// </summary>
        public static RoutedCommand ZoomCommand = new RoutedCommand();

        /// <summary>
        ///   The change fov command.
        /// </summary>
        public static RoutedCommand ChangeFieldOfViewCommand = new RoutedCommand();

        /// <summary>
        ///   The reset camera command.
        /// </summary>
        public static RoutedCommand ResetCameraCommand = new RoutedCommand();

        /// <summary>
        ///   The change look at command.
        /// </summary>
        public static RoutedCommand ChangeLookAtCommand = new RoutedCommand();

        /// <summary>
        ///   The top view command.
        /// </summary>
        public static RoutedCommand TopViewCommand = new RoutedCommand();

        /// <summary>
        ///   The bottom view command.
        /// </summary>
        public static RoutedCommand BottomViewCommand = new RoutedCommand();

        /// <summary>
        ///   The left view command.
        /// </summary>
        public static RoutedCommand LeftViewCommand = new RoutedCommand();

        /// <summary>
        ///   The right view command.
        /// </summary>
        public static RoutedCommand RightViewCommand = new RoutedCommand();

        /// <summary>
        ///   The front view command.
        /// </summary>
        public static RoutedCommand FrontViewCommand = new RoutedCommand();

        /// <summary>
        ///   The back view command.
        /// </summary>
        public static RoutedCommand BackViewCommand = new RoutedCommand();

        /// <summary>
        ///   Initializes a new instance of the <see cref="CameraController" /> class.
        /// </summary>
        public CameraController()
        {
            this.Loaded += this.CameraControllerLoaded;
            this.Unloaded += this.CameraControllerUnloaded;

            // Must be focusable to received key events
            this.Focusable = true;
            this.FocusVisualStyle = null;

            this.IsManipulationEnabled = true;

            this.InitializeBindings();
            this.renderingEventListener = new RenderingEventListener(this.OnCompositionTargetRendering);
        }

        /// <summary>
        ///   Called when the <see cref="E:System.Windows.UIElement.ManipulationStarted" /> event occurs.
        /// </summary>
        /// <param name="e"> The data for the event. </param>
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);
            this.Focus();
            this.touchDownPoint = e.ManipulationOrigin;
            this.panHandler.Started(new ManipulationEventArgs(this.touchDownPoint));
            this.rotateHandler.Started(new ManipulationEventArgs(this.touchDownPoint));
            this.zoomHandler.Started(new ManipulationEventArgs(this.touchDownPoint));
            e.Handled = true;
        }

        /// <summary>
        ///   Gets or sets the touch mode.
        /// </summary>
        /// <value> The touch mode. </value>
        public TouchMode TouchMode
        {
            get
            {
                return (TouchMode)this.GetValue(TouchModeProperty);
            }

            set
            {
                this.SetValue(TouchModeProperty, value);
            }
        }

        /// <summary>
        ///   The touch mode property.
        /// </summary>
        public static readonly DependencyProperty TouchModeProperty = DependencyProperty.Register(
            "TouchMode", typeof(TouchMode), typeof(CameraController), new UIPropertyMetadata(TouchMode.Panning));

        /// <summary>
        ///   The touch down point.
        /// </summary>
        private Point touchDownPoint;

        /// <summary>
        ///   Called when the <see cref="E:System.Windows.UIElement.ManipulationDelta" /> event occurs.
        /// </summary>
        /// <param name="e"> The data for the event. </param>
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);

            // http://msdn.microsoft.com/en-us/library/system.windows.uielement.manipulationdelta.aspx

            // Debug.WriteLine("OnManipulationDelta: T={0}, S={1}, R={2}, O={3}", e.DeltaManipulation.Translation, e.DeltaManipulation.Scale, e.DeltaManipulation.Rotation, e.ManipulationOrigin);
            Point position = this.touchDownPoint + e.CumulativeManipulation.Translation;

            switch (this.TouchMode)
            {
                case TouchMode.Panning:
                    this.panHandler.Delta(new ManipulationEventArgs(position));
                    break;
                case TouchMode.Rotating:
                    this.rotateHandler.Delta(new ManipulationEventArgs(position));
                    break;
            }

            Point3D? zoomAroundPoint = this.zoomHandler.UnProject(
                e.ManipulationOrigin, this.zoomHandler.Origin, this.CameraLookDirection);
            if (zoomAroundPoint != null)
            {
                this.zoomHandler.Zoom(1 - e.DeltaManipulation.Scale.Length / Math.Sqrt(2), zoomAroundPoint.Value);
            }

            e.Handled = true;
        }

        /// <summary>
        ///   Called when the <see cref="E:System.Windows.UIElement.ManipulationCompleted" /> event occurs.
        /// </summary>
        /// <param name="e"> The data for the event. </param>
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            Point p = this.touchDownPoint + e.TotalManipulation.Translation;

            switch (this.TouchMode)
            {
                case TouchMode.Panning:
                    this.panHandler.Completed(new ManipulationEventArgs(p));
                    break;
                case TouchMode.Rotating:
                    this.rotateHandler.Completed(new ManipulationEventArgs(p));
                    break;
            }

            this.zoomHandler.Completed(new ManipulationEventArgs(p));

            // Tap
            double l = e.TotalManipulation.Translation.Length;
            if (l < 4)
            {
                this.TouchMode = this.TouchMode == TouchMode.Panning ? TouchMode.Rotating : TouchMode.Panning;
            }

            e.Handled = true;
        }

        /// <summary>
        ///   Invoked when an unhandled StylusSystemGesture attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e"> The <see cref="T:System.Windows.Input.StylusSystemGestureEventArgs" /> that contains the event data. </param>
        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            base.OnStylusSystemGesture(e);

            // Debug.WriteLine("OnStylusSystemGesture: " + e.SystemGesture);
            if (e.SystemGesture == SystemGesture.HoldEnter)
            {
                Point p = e.GetPosition(this);
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
        ///   Initializes the input bindings.
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
        ///   The change look at event handler.
        /// </summary>
        private RotateHandler changeLookAtHandler;

        /// <summary>
        ///   The rotation event handler.
        /// </summary>
        private RotateHandler rotateHandler;

        /// <summary>
        ///   The zoom rectangle event handler.
        /// </summary>
        private ZoomRectangleHandler zoomRectangleHandler;

        /// <summary>
        ///   The zoom event handler.
        /// </summary>
        private ZoomHandler zoomHandler;

        /// <summary>
        ///   The change field of view event handler.
        /// </summary>
        private ZoomHandler changeFieldOfViewHandler;

        /// <summary>
        ///   The pan event handler.
        /// </summary>
        private PanHandler panHandler;

        /// <summary>
        ///   The top view event handler.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void TopViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ChangeDirection(new Vector3D(0, 0, -1), new Vector3D(0, 1, 0));
        }

        /// <summary>
        ///   The bottom view event handler.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void BottomViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ChangeDirection(new Vector3D(0, 0, 1), new Vector3D(0, -1, 0));
        }

        /// <summary>
        ///   The left view event handler.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void LeftViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ChangeDirection(new Vector3D(0, 1, 0), new Vector3D(0, 0, 1));
        }

        /// <summary>
        ///   The right view event handler.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void RightViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ChangeDirection(new Vector3D(0, -1, 0), new Vector3D(0, 0, 1));
        }

        /// <summary>
        ///   The front view event handler.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void FrontViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ChangeDirection(new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1));
        }

        /// <summary>
        ///   The back view event handler.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void BackViewHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ChangeDirection(new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
        }

        /// <summary>
        ///   Changes the direction of the camera.
        /// </summary>
        /// <param name="lookDir"> The look dir. </param>
        /// <param name="upDir"> Up dir. </param>
        /// <param name="animationTime"> The animation time. </param>
        public void ChangeDirection(Vector3D lookDir, Vector3D upDir, double animationTime = 500)
        {
            this.StopAnimations();
            this.PushCameraSetting();
            CameraHelper.ChangeDirection(this.ActualCamera, lookDir, upDir, animationTime);
        }

        /// <summary>
        ///   Changes the direction of the camera.
        /// </summary>
        /// <param name="lookDir"> The look dir. </param>
        /// <param name="animationTime"> The animation time. </param>
        public void ChangeDirection(Vector3D lookDir, double animationTime = 500)
        {
            this.StopAnimations();
            this.PushCameraSetting();
            CameraHelper.ChangeDirection(this.ActualCamera, lookDir, this.ActualCamera.UpDirection, animationTime);
        }

        /// <summary>
        ///   Called when the CameraController is unloaded.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void CameraControllerUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnSubscribeEvents();
        }

        /// <summary>
        ///   The camera controller_ loaded.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void CameraControllerLoaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeEvents();
        }

        /// <summary>
        ///   The Zoom extents event handler.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void ZoomExtentsHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.StopAnimations();
            this.ZoomExtents();
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
        ///   Invoked when an unhandled MouseDown attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e"> The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state. </param>
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
        ///   The reset camera event handler.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void ResetCameraHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.IsPanEnabled && this.IsZoomEnabled && this.CameraMode != CameraMode.FixedPosition)
            {
                this.StopAnimations();
                this.ResetCamera();
            }
        }

        /// <summary>
        ///   Gets or sets ModelUpDirection.
        /// </summary>
        public Vector3D ModelUpDirection
        {
            get
            {
                return (Vector3D)this.GetValue(UpDirectionProperty);
            }

            set
            {
                this.SetValue(UpDirectionProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the rotation sensitivity (degrees/pixel).
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
        ///   Gets or sets InertiaFactor.
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
        ///   Max duration of mouse drag to activate spin
        /// </summary>
        /// <remarks>
        ///   If the time between mouse down and mouse up is less than this value, spin is activated.
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
        ///   Gets or sets a value indicating whether InfiniteSpin.
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
        ///   Gets or sets a value indicating whether IsRotationEnabled.
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
        ///   The is rotation enabled property.
        /// </summary>
        public static readonly DependencyProperty IsRotationEnabledProperty =
            DependencyProperty.Register(
                "IsRotationEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        ///   Gets or sets a value indicating whether IsPanEnabled.
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
        ///   Gets or sets a value indicating whether IsZoomEnabled.
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
        ///   Show a target adorner when manipulating the camera.
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
        ///   Gets or sets a value indicating whether to rotate around the mouse down point.
        /// </summary>
        /// <value> <c>true</c> if rotatation around the mouse down point is enabled; otherwise, <c>false</c> . </value>
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
        ///   Gets or sets a value indicating whether to zoom around mouse down point.
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
        ///   Zoom around the mouse down point property.
        /// </summary>
        public static readonly DependencyProperty ZoomAroundMouseDownPointProperty =
            DependencyProperty.Register(
                "ZoomAroundMouseDownPoint", typeof(bool), typeof(CameraController), new UIPropertyMetadata(false));

        /// <summary>
        ///   Gets a value indicating whether IsPerspectiveCamera.
        /// </summary>
        protected bool IsPerspectiveCamera
        {
            get
            {
                return this.ActualCamera is PerspectiveCamera;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether IsOrthographicCamera.
        /// </summary>
        protected bool IsOrthographicCamera
        {
            get
            {
                return this.ActualCamera is OrthographicCamera;
            }
        }

        /// <summary>
        ///   Gets PerspectiveCamera.
        /// </summary>
        protected PerspectiveCamera PerspectiveCamera
        {
            get
            {
                return this.ActualCamera as PerspectiveCamera;
            }
        }

        /// <summary>
        ///   Gets OrthographicCamera.
        /// </summary>
        protected OrthographicCamera OrthographicCamera
        {
            get
            {
                return this.ActualCamera as OrthographicCamera;
            }
        }

        /// <summary>
        ///   Gets or sets Camera.
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
        ///   Gets or sets CameraLookDirection.
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
        ///   Gets or sets CameraUpDirection.
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
        ///   Gets or sets CameraPosition.
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
        ///   Gets or sets CameraTarget.
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
        ///   Gets or sets ZoomSensitivity.
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
        ///   The zoom sensitivity property.
        /// </summary>
        public static readonly DependencyProperty ZoomSensitivityProperty =
            DependencyProperty.Register(
                "ZoomSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        ///   Gets or sets CameraRotationMode.
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
        ///   Gets or sets CameraMode.
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
        ///   Gets or sets Viewport.
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
        ///   Gets or sets a value indicating whether Enabled.
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
        ///   Gets a value indicating whether IsActive.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.Enabled && this.Viewport != null && this.ActualCamera != null;
            }
        }

        /// <summary>
        ///   The viewport changed.
        /// </summary>
        /// <param name="d"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private static void ViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CameraController)d).OnViewportChanged();
        }

        /// <summary>
        ///   The on viewport changed.
        /// </summary>
        private void OnViewportChanged()
        {
        }

        /// <summary>
        ///   Gets ActualCamera.
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

        private readonly RenderingEventListener renderingEventListener;

        /// <summary>
        ///   The subscribe events.
        /// </summary>
        private void SubscribeEvents()
        {
            this.MouseWheel += this.OnMouseWheel;
            this.KeyDown += this.OnKeyDown;
            RenderingEventManager.AddListener(renderingEventListener);
        }

        /// <summary>
        ///   The un subscribe events.
        /// </summary>
        private void UnSubscribeEvents()
        {
            this.MouseWheel -= this.OnMouseWheel;
            this.KeyDown -= this.OnKeyDown;
            RenderingEventManager.RemoveListener(renderingEventListener);
        }

        /// <summary>
        ///   The rendering event handler.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void OnCompositionTargetRendering(object sender, RenderingEventArgs e)
        {
            var ticks = e.RenderingTime.Ticks;
            double time = 100e-9 * (ticks - this.lastTick);

            if (this.lastTick != 0)
            {
                this.OnTimeStep(time);
            }

            this.lastTick = ticks;
        }

        /// <summary>
        ///   The on time step.
        /// </summary>
        /// <param name="time"> The time. </param>
        private void OnTimeStep(double time)
        {
            // should be independent of time
            double factor = Math.Pow(this.InertiaFactor, time / 0.012);
            factor = this.Clamp(factor, 0.2, 1);

            if (this.isSpinning && this.spinningSpeed.LengthSquared > 0)
            {
                this.rotateHandler.Rotate(
                    this.spinningPosition, this.spinningPosition + this.spinningSpeed * time, this.spinningPoint3D);

                if (!this.InfiniteSpin)
                {
                    this.spinningSpeed *= factor;
                }

            }

            if (this.rotationSpeed.LengthSquared > 0.1)
            {
                this.rotateHandler.Rotate(
                    this.rotationPosition, this.rotationPosition + this.rotationSpeed * time, this.rotationPoint3D);
                this.rotationSpeed *= factor;
            }

            if (Math.Abs(this.panSpeed.LengthSquared) > 0.0001)
            {
                this.panHandler.Pan(this.panSpeed * time);
                this.panSpeed *= factor;
            }

            if (Math.Abs(this.zoomSpeed) > 0.1)
            {
                this.zoomHandler.Zoom(this.zoomSpeed * time, this.zoomPoint3D);
                this.zoomSpeed *= factor;
            }
        }

        /// <summary>
        ///   The clamp.
        /// </summary>
        /// <param name="factor"> The factor. </param>
        /// <param name="min"> The min. </param>
        /// <param name="max"> The max. </param>
        /// <returns> The clamp. </returns>
        private double Clamp(double factor, double min, double max)
        {
            if (factor < min)
            {
                return min;
            }

            if (factor > max)
            {
                return max;
            }

            return factor;
        }

        /// <summary>
        ///   The camera history stack.
        /// </summary>
        /// <remarks>
        ///   Implemented as a list since we want to remove items at the bottom of the stack.
        /// </remarks>
        private readonly List<CameraSetting> cameraHistory = new List<CameraSetting>();

        /// <summary>
        ///   Push the current camera settings on an internal stack.
        /// </summary>
        public void PushCameraSetting()
        {
            this.cameraHistory.Add(new CameraSetting(this.ActualCamera));
            if (this.cameraHistory.Count > 100)
            {
                this.cameraHistory.RemoveAt(0);
            }
        }

        /// <summary>
        ///   Restores the most recent camera setting from the internal stack.
        /// </summary>
        /// <returns> The restore camera setting. </returns>
        public bool RestoreCameraSetting()
        {
            if (this.cameraHistory.Count > 0)
            {
                CameraSetting cs = this.cameraHistory[this.cameraHistory.Count - 1];
                this.cameraHistory.RemoveAt(this.cameraHistory.Count - 1);
                cs.UpdateCamera(this.ActualCamera);
                return true;
            }

            return false;
        }

        /// <summary>
        ///   Called when a key is pressed.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The <see cref="System.Windows.Input.KeyEventArgs" /> instance containing the event data. </param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
            bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool control = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            double f = control ? 0.25 : 1;

            if (!shift)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        this.AddRotateForce(-1 * f, 0);
                        e.Handled = true;
                        break;
                    case Key.Right:
                        this.AddRotateForce(1 * f, 0);
                        e.Handled = true;
                        break;
                    case Key.Up:
                        this.AddRotateForce(0, -1 * f);
                        e.Handled = true;
                        break;
                    case Key.Down:
                        this.AddRotateForce(0, 1 * f);
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Left:
                        this.AddPanForce(-5 * f, 0);
                        e.Handled = true;
                        break;
                    case Key.Right:
                        this.AddPanForce(5 * f, 0);
                        e.Handled = true;
                        break;
                    case Key.Up:
                        this.AddPanForce(0, -5 * f);
                        e.Handled = true;
                        break;
                    case Key.Down:
                        this.AddPanForce(0, 5 * f);
                        e.Handled = true;
                        break;
                }
            }

            switch (e.Key)
            {
                case Key.PageUp:
                    this.AddZoomForce(-0.1 * f);
                    e.Handled = true;
                    break;
                case Key.PageDown:
                    this.AddZoomForce(0.1 * f);
                    e.Handled = true;
                    break;
                case Key.Back:
                    if (this.RestoreCameraSetting())
                    {
                        e.Handled = true;
                    }

                    break;
            }
        }

        /// <summary>
        ///   The 3D point to spin around.
        /// </summary>
        private Point3D spinningPoint3D;

        /// <summary>
        ///   The spinning position.
        /// </summary>
        private Point spinningPosition;

        /// <summary>
        ///   Stops the spin.
        /// </summary>
        public void StopSpin()
        {
            this.isSpinning = false;
        }

        /// <summary>
        ///   Starts the spin.
        /// </summary>
        /// <param name="speed"> The speed. </param>
        /// <param name="position"> The position. </param>
        /// <param name="aroundPoint"> The spin around point. </param>
        public void StartSpin(Vector speed, Point position, Point3D aroundPoint)
        {
            this.spinningSpeed = speed;
            this.spinningPosition = position;
            this.spinningPoint3D = aroundPoint;
            this.isSpinning = true;
        }

        /// <summary>
        ///   Called when the mouse wheel is moved.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The <see cref="System.Windows.Input.MouseWheelEventArgs" /> instance containing the event data. </param>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            if (this.ZoomAroundMouseDownPoint)
            {
                Point point = e.GetPosition(this);
                Point3D nearestPoint;
                Vector3D normal;
                DependencyObject visual;
                if (Viewport3DHelper.FindNearest(this.Viewport, point, out nearestPoint, out normal, out visual))
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
        ///   The reset camera.
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
                CameraHelper.Copy(this.DefaultCamera, this.ActualCamera);
            }
            else
            {
                CameraHelper.Reset(this.ActualCamera);
                CameraHelper.ZoomExtents(this.ActualCamera, this.Viewport);
            }
        }

        /// <summary>
        ///   The Zoom extents.
        /// </summary>
        /// <param name="animationTime"> The animation time (milliseconds). </param>
        public void ZoomExtents(double animationTime = 200)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            CameraHelper.ZoomExtents(this.ActualCamera, this.Viewport, animationTime);
        }

        /// <summary>
        ///   The add pan force.
        /// </summary>
        /// <param name="dx"> The dx. </param>
        /// <param name="dy"> The dy. </param>
        public void AddPanForce(double dx, double dy)
        {
            this.AddPanForce(this.FindPanVector(dx, dy));
        }

        /// <summary>
        ///   The find pan vector.
        /// </summary>
        /// <param name="dx"> The dx. </param>
        /// <param name="dy"> The dy. </param>
        /// <returns> </returns>
        private Vector3D FindPanVector(double dx, double dy)
        {
            Vector3D axis1 = Vector3D.CrossProduct(this.CameraLookDirection, this.CameraUpDirection);
            Vector3D axis2 = Vector3D.CrossProduct(axis1, this.CameraLookDirection);
            axis1.Normalize();
            axis2.Normalize();
            double l = this.CameraLookDirection.Length;
            double f = l * 0.001;
            Vector3D move = -axis1 * f * dx + axis2 * f * dy;

            // this should be dependent on distance to target?           
            return move;
        }

        /// <summary>
        ///   The add pan force.
        /// </summary>
        /// <param name="pan"> The pan. </param>
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
        ///   The rotation position.
        /// </summary>
        private Point rotationPosition;

        /// <summary>
        ///   The add rotate force.
        /// </summary>
        /// <param name="dx"> The dx. </param>
        /// <param name="dy"> The dy. </param>
        public void AddRotateForce(double dx, double dy)
        {
            if (!this.IsRotationEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            this.rotationPoint3D = this.CameraTarget;
            this.rotationPosition = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            this.rotationSpeed.X += dx * 40;
            this.rotationSpeed.Y += dy * 40;
        }

        /// <summary>
        ///   The point to zoom around.
        /// </summary>
        private Point3D zoomPoint3D;

        /// <summary>
        ///   Adds the zoom force.
        /// </summary>
        /// <param name="dx"> The dx. </param>
        public void AddZoomForce(double dx)
        {
            this.AddZoomForce(dx, this.CameraTarget);
        }

        /// <summary>
        ///   Adds the zoom force.
        /// </summary>
        /// <param name="dx"> The dx. </param>
        /// <param name="zoomOrigin"> The zoom origin. </param>
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
        ///   Shows the target adorner.
        /// </summary>
        /// <param name="position"> The position. </param>
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

            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            this.targetAdorner = new TargetSymbolAdorner(this.Viewport, position);
            myAdornerLayer.Add(this.targetAdorner);
        }

        /// <summary>
        ///   Hides the target adorner.
        /// </summary>
        public void HideTargetAdorner()
        {
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            if (this.targetAdorner != null)
            {
                myAdornerLayer.Remove(this.targetAdorner);
            }

            this.targetAdorner = null;

            // the adorner sometimes leaves some 'dust', so refresh the viewport
            this.RefreshViewport();
        }

        /// <summary>
        ///   The refresh viewport.
        /// </summary>
        private void RefreshViewport()
        {
            // todo: this is a hack, should be improved

            // var mg = new ModelVisual3D { Content = new AmbientLight(Colors.White) };
            // Viewport.Children.Add(mg);
            // Viewport.Children.Remove(mg);
            Camera c = this.Viewport.Camera;
            this.Viewport.Camera = null;
            this.Viewport.Camera = c;

            // var w = Viewport.Width;
            // Viewport.Width = w-1;
            // Viewport.Width = w;

            // Viewport.InvalidateVisual();
        }

#if USE_RECTANGLE_SHAPE
        private Canvas rectangleCanvas;
        private Rectangle rectangleShape;

        public void RectangleShow(Rect rect)
        {
            this.rectangleShape = new Rectangle();
            this.rectangleShape.Width = rect.Width;
            this.rectangleShape.Height = rect.Height;
            this.rectangleShape.Stroke = Brushes.Red;
            this.rectangleShape.StrokeThickness = 1;
            Canvas.SetLeft(this.rectangleShape, rect.Left);
            Canvas.SetTop(this.rectangleShape, rect.Top);
            this.rectangleCanvas = new Canvas();
            this.rectangleCanvas.Children.Add(this.rectangleShape);
            this.Children.Add(this.rectangleCanvas);
        }
        public void RectangleUpdate(Rect rect)
        {
            this.rectangleShape.Width = rect.Width;
            this.rectangleShape.Height = rect.Height;
            Canvas.SetLeft(this.rectangleShape, rect.Left);
            Canvas.SetTop(this.rectangleShape, rect.Top);
        }
        public void RectangleHide()
        {
            this.rectangleCanvas.Children.Remove(this.rectangleShape);
            this.rectangleShape = null;
            Children.Remove(this.rectangleCanvas);
            this.rectangleCanvas = null;
        }
#else

        /// <summary>
        ///   Shows the rectangle.
        /// </summary>
        /// <param name="rect"> The rect. </param>
        /// <param name="color1"> The color 1. </param>
        /// <param name="color2"> The color 2. </param>
        public void ShowRectangle(Rect rect, Color color1, Color color2)
        {
            if (this.rectangleAdorner != null)
            {
                return;
            }

            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            this.rectangleAdorner = new RectangleAdorner(
                this.Viewport, rect, color1, color2, 3, 1, 10, DashStyles.Solid);
            myAdornerLayer.Add(this.rectangleAdorner);
        }

        /// <summary>
        ///   Updates the rectangle.
        /// </summary>
        /// <param name="rect"> The rectangle. </param>
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
        ///   Hides the rectangle.
        /// </summary>
        public void HideRectangle()
        {
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            if (this.rectangleAdorner != null)
            {
                myAdornerLayer.Remove(this.rectangleAdorner);
            }

            this.rectangleAdorner = null;

            this.Viewport.InvalidateVisual();
        }

#endif

        /// <summary>
        ///   Change the "look-at" point.
        /// </summary>
        /// <param name="target"> The target. </param>
        /// <param name="animationTime"> The animation time. </param>
        [Obsolete]
        public void LookAt(Point3D target, double animationTime)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            CameraHelper.LookAt(this.Camera, target, animationTime);
        }

        /// <summary>
        ///   Resets the camera up direction.
        /// </summary>
        public void ResetCameraUpDirection()
        {
            this.CameraUpDirection = this.ModelUpDirection;
        }

        /// <summary>
        ///   Zooms by the specified delta value.
        /// </summary>
        /// <param name="delta"> The delta value. </param>
        public void Zoom(double delta)
        {
            this.zoomHandler.Zoom(delta);
        }
    }
}