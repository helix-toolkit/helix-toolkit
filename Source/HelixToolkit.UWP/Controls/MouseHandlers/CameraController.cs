using System.Numerics;
using HelixToolkit.Mathematics;
using System;
using System.Linq;
using Point = Windows.Foundation.Point;
using Point3D = System.Numerics.Vector3;
using Vector3D = System.Numerics.Vector3;

namespace HelixToolkit.UWP
{
    using System.Diagnostics;
    using Utilities;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Input;

    public class CameraController
    {
        private InputController inputController;
        public InputController InputController
        {
            set
            {
                if(inputController == value)
                {
                    return;
                }
                if(inputController != null)
                {
                    inputController.Viewport = null;
                    inputController.OnStartPan -= InputController_OnStartPan;
                    inputController.OnStartRotate -= InputController_OnStartRotate;
                    inputController.OnStartZoom -= InputController_OnStartZoom;
                    inputController.OnStartZoomExtends -= InputController_OnStartZoomExtends;
                    inputController.OnStartZoomRectangle -= InputController_OnStartZoomRectangle;
                    inputController.OnResetCamera -= InputController_OnResetCamera;
                    inputController.OnChangeFieldOfView -= InputController_OnChangeFieldOfView;
                    inputController.OnChangeLookAt -= InputController_OnChangeLookAt;
                    inputController.OnTopView -= InputController_OnTopView;
                    inputController.OnBottomView -= InputController_OnBottomView;
                    inputController.OnLeftView -= InputController_OnLeftView;
                    inputController.OnRightView -= InputController_OnRightView;
                    inputController.OnFrontView -= InputController_OnFrontView;
                    inputController.OnBackView -= InputController_OnBackView;
                    inputController.OnAddMoveForce -= InputController_OnAddMoveForce;
                    inputController.OnAddPanForce -= InputController_OnAddPanForce;
                    inputController.OnAddRotationForce -= InputController_OnAddRotationForce;
                    inputController.OnAddZoomForce -= InputController_OnAddZoomForce;
                    inputController.OnRestoreCameraSettings -= InputController_OnRestoreCameraSettings;
                }
                inputController = value;
                if(inputController != null)
                {
                    inputController.Viewport = this.Viewport;
                    inputController.OnStartPan += InputController_OnStartPan;
                    inputController.OnStartRotate += InputController_OnStartRotate;
                    inputController.OnStartZoom += InputController_OnStartZoom;
                    inputController.OnStartZoomExtends += InputController_OnStartZoomExtends;
                    inputController.OnStartZoomRectangle += InputController_OnStartZoomRectangle;
                    inputController.OnResetCamera += InputController_OnResetCamera;
                    inputController.OnChangeFieldOfView += InputController_OnChangeFieldOfView;
                    inputController.OnChangeLookAt += InputController_OnChangeLookAt;
                    inputController.OnTopView += InputController_OnTopView;
                    inputController.OnBottomView += InputController_OnBottomView;
                    inputController.OnLeftView += InputController_OnLeftView;
                    inputController.OnRightView += InputController_OnRightView;
                    inputController.OnFrontView += InputController_OnFrontView;
                    inputController.OnBackView += InputController_OnBackView;
                    inputController.OnAddMoveForce += InputController_OnAddMoveForce;
                    inputController.OnAddPanForce += InputController_OnAddPanForce;
                    inputController.OnAddRotationForce += InputController_OnAddRotationForce;
                    inputController.OnAddZoomForce += InputController_OnAddZoomForce;
                    inputController.OnRestoreCameraSettings += InputController_OnRestoreCameraSettings;
                }
            }
            get
            {
                return inputController;
            }
        }

        /// <summary>
        /// Gets ActualCamera.
        /// </summary>
        public ProjectionCamera ActualCamera
        {
            private set;get;
        }

        /// <summary>
        /// Gets or sets the default camera (used when resetting the view).
        /// </summary>
        /// <value> The default camera. </value>
        public ProjectionCamera DefaultCamera
        {
            private set;get;
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

        #region Properties
        /// <summary>
        /// Gets or sets CameraMode.
        /// </summary>
        public CameraMode CameraMode
        {
            set; get;
        } = CameraMode.Inspect;

        /// <summary>
        /// Gets or sets CameraRotationMode.
        /// </summary>
        public CameraRotationMode CameraRotationMode
        {
            set; get;
        } = CameraRotationMode.Turntable;

        /// <summary>
        /// Gets or sets the change field of view Cursor
        /// </summary>
        /// <value> The change field of view Cursor. </value>
        public CoreCursorType ChangeFieldOfViewCursor
        {
            set; get;
        } = CoreCursorType.SizeNorthSouth;

        /// <summary>
        /// Gets or sets InertiaFactor.
        /// </summary>
        public double InertiaFactor
        {
            set; get;
        } = 0.93;

        /// <summary>
        /// Gets or sets a value indicating whether InfiniteSpin.
        /// </summary>
        public bool InfiniteSpin
        {
            set; get;
        } = false;

        /// <summary>
        /// Gets or sets a value indicating whether field of view can be changed.
        /// </summary>
        public bool IsChangeFieldOfViewEnabled
        {
            set; get;
        } = true;

        /// <summary>
        /// Gets or sets a value indicating whether inertia is enabled for the camera manipulations.
        /// </summary>
        /// <value><c>true</c> if inertia is enabled; otherwise, <c>false</c>.</value>
        public bool IsInertiaEnabled
        {
            set; get;
        } = true;

        /// <summary>
        /// Gets or sets a value indicating whether move is enabled.
        /// </summary>
        /// <value> <c>true</c> if move is enabled; otherwise, <c>false</c> . </value>
        public bool IsMoveEnabled
        {
            set; get;
        } = true;

        /// <summary>
        /// Gets or sets a value indicating whether pan is enabled.
        /// </summary>
        public bool IsPanEnabled
        {
            set; get;
        } = true;

        /// <summary>
        /// Gets or sets a value indicating whether IsRotationEnabled.
        /// </summary>
        public bool IsRotationEnabled
        {
            set; get;
        } = true;

        /// <summary>
        /// Gets or sets a value indicating whether IsZoomEnabled.
        /// </summary>
        public bool IsZoomEnabled
        {
            set; get;
        } = true;

        /// <summary>
        /// Gets or sets the sensitivity for pan by the left and right keys.
        /// </summary>
        /// <value> The pan sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the pan direction.
        /// </remarks>
        public double LeftRightPanSensitivity
        {
            set; get;
        } = 1.0;

        /// <summary>
        /// Gets or sets the sensitivity for rotation by the left and right keys.
        /// </summary>
        /// <value> The rotation sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the rotation direction.
        /// </remarks>
        public double LeftRightRotationSensitivity
        {
            set; get;
        } = 1.0;

        /// <summary>
        /// Gets or sets the maximum field of view.
        /// </summary>
        /// <value> The maximum field of view. </value>
        public double MaximumFieldOfView
        {
            set; get;
        } = 120.0;

        /// <summary>
        /// Gets or sets the minimum field of view.
        /// </summary>
        /// <value> The minimum field of view. </value>
        public double MinimumFieldOfView
        {
            set; get;
        } = 10.0;

        /// <summary>
        /// Gets or sets the model up direction.
        /// </summary>
        public Vector3D ModelUpDirection
        {
            set; get;
        } = new Vector3D(0, 1, 0);

        /// <summary>
        /// Gets or sets the move sensitivity.
        /// </summary>
        /// <value> The move sensitivity. </value>
        public double MoveSensitivity
        {
            set; get;
        } = 1.0;

        /// <summary>
        /// Gets or sets the sensitivity for zoom by the page up and page down keys.
        /// </summary>
        /// <value> The zoom sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the zoom direction.
        /// </remarks>
        public double PageUpDownZoomSensitivity
        {
            set; get;
        } = 1.0;

        /// <summary>
        /// Gets or sets the pan Cursor.
        /// </summary>
        /// <value> The pan Cursor. </value>
        public CoreCursorType PanCursor
        {
            set; get;
        } = CoreCursorType.Hand;

        /// <summary>
        /// Gets or sets a value indicating whether to rotate around the mouse down point.
        /// </summary>
        /// <value> <c>true</c> if rotation around the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool RotateAroundMouseDownPoint
        {
            set; get;
        } = false;

        /// <summary>
        /// Gets or sets the rotate Cursor.
        /// </summary>
        /// <value> The rotate Cursor. </value>
        public CoreCursorType RotateCursor
        {
            set; get;
        } = CoreCursorType.SizeAll;

        /// <summary>
        /// Gets or sets the rotation sensitivity (degrees/pixel).
        /// </summary>
        /// <value> The rotation sensitivity. </value>
        public double RotationSensitivity
        {
            set; get;
        } = 1.0;

        /// <summary>
        /// Gets or sets a value indicating whether to show a target adorner when manipulating the camera.
        /// </summary>
        public bool ShowCameraTarget
        {
            set; get;
        } = true;

        /// <summary>
        /// Gets or sets the max duration of mouse drag to activate spin.
        /// </summary>
        /// <remarks>
        /// If the time between mouse down and mouse up is less than this value, spin is activated.
        /// </remarks>
        public int SpinReleaseTime
        {
            set; get;
        } = 200;

        /// <summary>
        /// Gets or sets the sensitivity for pan by the up and down keys.
        /// </summary>
        /// <value> The pan sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the pan direction.
        /// </remarks>
        public double UpDownPanSensitivity
        {
            set; get;
        } = 1.0;

        /// <summary>
        /// Gets or sets the sensitivity for rotation by the up and down keys.
        /// </summary>
        /// <value> The rotation sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the rotation direction.
        /// </remarks>
        public double UpDownRotationSensitivity
        {
            set; get;
        } = 1.0;

        /// <summary>
        /// Gets or sets a value indicating whether to zoom around mouse down point.
        /// </summary>
        /// <value> <c>true</c> if zooming around the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool ZoomAroundMouseDownPoint
        {
            set; get;
        } = false;

        /// <summary>
        /// Gets or sets the zoom Cursor.
        /// </summary>
        /// <value> The zoom CursorType. </value>
        public CoreCursorType ZoomCursor
        {
            set; get;
        } = CoreCursorType.SizeNorthSouth;

        /// <summary>
        /// Gets or sets the zoom rectangle Cursor.
        /// </summary>
        /// <value> The zoom rectangle Cursor. </value>
        public CoreCursorType ZoomRectangleCursor
        {
            set; get;
        } = CoreCursorType.SizeNorthwestSoutheast;

        /// <summary>
        /// Gets or sets ZoomSensitivity.
        /// </summary>
        public double ZoomSensitivity
        {
            set; get;
        } = 1.0;
        /// <summary>
        /// Gets or sets the zoom distance limit far.
        /// </summary>
        /// <value>
        /// The zoom distance limit far.
        /// </value>
        public double ZoomDistanceLimitFar { set; get; } = double.PositiveInfinity;
        /// <summary>
        /// Gets or sets the zoom distance limit near.
        /// </summary>
        /// <value>
        /// The zoom distance limit near.
        /// </value>
        public double ZoomDistanceLimitNear { set; get; } = 0.001;

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
        /// Gets or sets a value indicating whether [fixed rotation point enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixed rotation point enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool FixedRotationPointEnabled
        {
            set; get;
        } = false;
        /// <summary>
        /// Gets or sets the fixed rotation point.
        /// </summary>
        /// <value>
        /// The fixed rotation point.
        /// </value>
        public Point3D FixedRotationPoint
        {
            set; get;
        } = new Point3D();
        #endregion

        #region TouchGesture
        public bool EnableTouchRotate { set; get; } = true;
        public bool EnablePinchZoom { set; get; } = true;
        public bool EnableThreeFingerPan { set; get; } = true;
        #endregion
        /// <summary>
        /// The camera history stack.
        /// </summary>
        /// <remarks>
        /// Implemented as a list since we want to remove items at the bottom of the stack.
        /// </remarks>
        private readonly SimpleRingBuffer<CameraSetting> cameraHistory = new SimpleRingBuffer<CameraSetting>(100);

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

        ///// <summary>
        ///// The rectangle adorner.
        ///// </summary>
        //private RectangleAdorner rectangleAdorner;

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
        private Vector2 rotationSpeed;

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
        private Vector2 spinningSpeed;

        ///// <summary>
        ///// The target adorner.
        ///// </summary>
        //private Adorner targetAdorner;

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
        private readonly ZoomHandler zoomHandler;

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

        private static readonly Point PointZero = new Point(0, 0);
        private static readonly Vector2 VectorZero = new Vector2();
        private static readonly Vector3D Vector3DZero = new Vector3D();
        public Viewport3DX Viewport { private set; get; }

        public CameraController(Viewport3DX viewport)
        {
            this.Viewport = viewport;
            this.changeLookAtHandler = new RotateHandler(this, true);
            this.rotateHandler = new RotateHandler(this);
            this.zoomRectangleHandler = new ZoomRectangleHandler(this);
            this.zoomHandler = new ZoomHandler(this);
            this.panHandler = new PanHandler(this);
            this.changeFieldOfViewHandler = new ZoomHandler(this, true);
            this.Viewport.RegisterPropertyChangedCallback(Viewport3DX.CameraProperty, (d, e) => { ActualCamera = d.GetValue(e) as ProjectionCamera; });
            this.Viewport.RegisterPropertyChangedCallback(Viewport3DX.DefaultCameraProperty, (d, e) => { DefaultCamera = d.GetValue(e) as ProjectionCamera; });
        }
        #region Input Events
        private void InputController_OnBackView(object sender, EventArgs e)
        {
            ExecuteBackView();
        }

        private void InputController_OnFrontView(object sender, EventArgs e)
        {
            ExecuteFrontView();
        }

        private void InputController_OnRightView(object sender, EventArgs e)
        {
            ExecuteRightView();
        }

        private void InputController_OnLeftView(object sender, EventArgs e)
        {
            ExecuteLeftView();
        }

        private void InputController_OnBottomView(object sender, EventArgs e)
        {
            ExecuteBottomView();
        }

        private void InputController_OnTopView(object sender, EventArgs e)
        {
            ExecuteTopView();
        }

        private void InputController_OnChangeLookAt(object sender, PointerRoutedEventArgs e)
        {
            changeLookAtHandler.Execute(Viewport, e);
        }

        private void InputController_OnChangeFieldOfView(object sender, PointerRoutedEventArgs e)
        {
            changeFieldOfViewHandler.Execute(Viewport, e);
        }

        private void InputController_OnResetCamera(object sender, EventArgs e)
        {
            ExecuteResetCamera();
        }

        private void InputController_OnStartZoomRectangle(object sender, PointerRoutedEventArgs e)
        {
            zoomRectangleHandler.Execute(Viewport, e);
        }

        private void InputController_OnStartZoomExtends(object sender, PointerRoutedEventArgs e)
        {
            ExecuteZoomExtents();
        }

        private void InputController_OnStartZoom(object sender, PointerRoutedEventArgs e)
        {
            OnMouseWheel(e);
        }

        private void InputController_OnStartRotate(object sender, PointerRoutedEventArgs e)
        {
            rotateHandler.Execute(Viewport, e);
        }

        private void InputController_OnStartPan(object sender, PointerRoutedEventArgs e)
        {
            panHandler.Execute(Viewport, e);
        }

        private void InputController_OnAddZoomForce(object sender, InputController.AddForceEventArgs e)
        {
            AddZoomForce(e.Force.X);
        }

        private void InputController_OnAddRotationForce(object sender, InputController.AddForceEventArgs e)
        {
            AddRotateForce(e.Force.X, e.Force.Y);
        }

        private void InputController_OnAddPanForce(object sender, InputController.AddForceEventArgs e)
        {
            AddPanForce(e.Force.X, e.Force.Y);
        }

        private void InputController_OnAddMoveForce(object sender, InputController.AddForceEventArgs e)
        {
            AddMoveForce(e.Move);
        }

        private void InputController_OnRestoreCameraSettings(object sender, EventArgs e)
        {
            RestoreCameraSetting();
        }
        #endregion
        /// <summary>
        /// Adds the specified move force.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        public void AddMoveForce(Vector3D delta)
        {
            if (!this.Viewport.IsMoveEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            this.moveSpeed += delta * 40;
            Viewport.InvalidateRender();
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
            if (!this.Viewport.IsPanEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            if (this.Viewport.IsInertiaEnabled)
            {
                this.panSpeed += pan * 40;
            }
            else
            {
                this.panHandler.Pan(pan);
            }
            Viewport.InvalidateRender();
        }

        /// <summary>
        /// Adds the rotate force.
        /// </summary>
        /// <param name="force">The force.</param>
        public void AddRotateForce(Vector2 force)
        {
            AddRotateForce(force.X, force.Y);
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
            if (!this.Viewport.IsRotationEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            if (this.Viewport.IsInertiaEnabled)
            {
                this.rotationPoint3D = this.CameraTarget;
                this.rotationPosition = new Point(Viewport.ActualWidth / 2, Viewport.ActualHeight / 2);
                this.rotationSpeed.X += (float)dx * 40;
                this.rotationSpeed.Y += (float)dy * 40;
            }
            else if (FixedRotationPointEnabled)
            {
                this.rotationPosition = new Point(Viewport.ActualWidth / 2, Viewport.ActualHeight / 2);
                this.rotateHandler.Rotate(
                    this.rotationPosition, new Point(this.rotationPosition.X + dx, this.rotationPosition.Y + dy), 
                    FixedRotationPoint);
            }
            else
            {
                this.rotationPosition = new Point(this.Viewport.ActualWidth / 2, this.Viewport.ActualHeight / 2);
                this.rotateHandler.Rotate(
                    this.rotationPosition, new Point(this.rotationPosition.X + dx, this.rotationPosition.Y + dy), 
                    this.CameraTarget);
            }
            Viewport.InvalidateRender();
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
            if (!this.Viewport.IsZoomEnabled)
            {
                return;
            }
            this.PushCameraSetting();

            if (this.Viewport.IsInertiaEnabled)
            {
                this.zoomPoint3D = zoomOrigin;
                this.zoomSpeed += delta * 8;
            }
            else
            {
                this.zoomHandler.Zoom(delta, zoomOrigin);
            }
            Viewport.InvalidateRender();
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
            if (!Viewport.IsRotationEnabled)
            {
                return;
            }

            this.StopAnimations();
            this.PushCameraSetting();
            this.ActualCamera.ChangeDirection(lookDir, upDir, animationTime);
        }

        /// <summary>
        /// Push the current camera settings on an internal stack.
        /// </summary>
        public void PushCameraSetting()
        {
            if (ActualCamera == null)
            {
                return;
            }
            this.cameraHistory.Add(new CameraSetting(this.ActualCamera));
        }

        /// <summary>
        /// Resets the camera.
        /// </summary>
        public void ResetCamera()
        {
            if (!Viewport.IsZoomEnabled || !Viewport.IsRotationEnabled || !Viewport.IsPanEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            if (this.DefaultCamera != null)
            {
                this.DefaultCamera.CopyTo(this.ActualCamera);
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
            this.CameraUpDirection = Viewport.ModelUpDirection;
        }

        /// <summary>
        /// Restores the most recent camera setting from the internal stack.
        /// </summary>
        /// <returns> The restore camera setting. </returns>
        public bool RestoreCameraSetting()
        {
            if (this.cameraHistory.Count > 0)
            {
                var cs = this.cameraHistory.Last;
                this.cameraHistory.RemoveLast();
                cs.UpdateCamera(this.ActualCamera);
                return true;
            }

            return false;
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
        public void StartSpin(Vector2 speed, Point position, Point3D aroundPoint)
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
            this.spinningSpeed = new Vector2();
        }
        /// <summary>
        /// Stops the zooming inertia
        /// </summary>
        public void StopZooming()
        {
            this.zoomSpeed = 0;
        }
        /// <summary>
        /// Stops the panning.
        /// </summary>
        public void StopPanning()
        {
            this.panSpeed = Vector3.Zero;
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
            if (!Viewport.IsZoomEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            this.ActualCamera.ZoomExtents(this.Viewport, animationTime);
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.UIElement.ManipulationCompleted"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        public void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            var p = new Point(e.Position.X + e.Cumulative.Translation.X, e.Position.Y + e.Cumulative.Translation.Y);

            if (this.manipulatorCount == 1)
            {
                this.rotateHandler.Completed(p);
            }

            if (this.manipulatorCount == 2)
            {
                this.panHandler.Completed(p);
                this.zoomHandler.Completed(p);
            }
        }

        private float prevScale = 1;
        /// <summary>
        /// Called when the <see cref="E:System.Windows.UIElement.ManipulationDelta"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        public void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            // number of manipulators (fingers)
            if (Viewport.PointerCaptures == null)
            { return; }
            int n = Viewport.PointerCaptures.Count();
            var position = new Point(touchPreviousPoint.X + e.Cumulative.Translation.X, touchPreviousPoint.Y + e.Cumulative.Translation.Y);

            // http://msdn.microsoft.com/en-us/library/system.windows.uielement.manipulationdelta.aspx

            //System.Diagnostics.Debug.WriteLine($"OnManipulationDelta: T={e.Cumulative.Translation}, S={e.Cumulative.Scale}, R={e.Cumulative.Rotation}, E={e.Cumulative.Expansion}");
            //// System.Diagnostics.Debug.WriteLine(n + " Delta:" + e.DeltaManipulation.Translation + " Origin:" + e.ManipulationOrigin + " pos:" + position);
            
            if (this.manipulatorCount != n)
            {
                // the number of manipulators has changed
                switch (this.manipulatorCount)
                {
                    case 1:
                        this.rotateHandler.Completed(position);
                        break;
                    case 2:
                        this.zoomHandler.Completed(e.Position);
                        break;
                    case 3:
                        this.panHandler.Completed(position);
                        break;
                }

                switch (n)
                {
                    case 1:
                        if (EnableTouchRotate)
                        {
                            this.rotateHandler.Started(position);
                            e.Handled = true;
                        }
                        break;
                    case 2:
                        if (EnablePinchZoom)
                        {
                            this.zoomHandler.Started(e.Position);
                            e.Handled = true;
                        }
                        break;
                    case 3:
                        if (EnableThreeFingerPan)
                        {
                            this.panHandler.Started(position);
                            e.Handled = true;
                        }
                        break;
                }

                // skip this event, the origin may have changed
                this.manipulatorCount = n;
                
                return;
            }
            else
            {
                switch (n)
                {
                    case 1:
                        if (EnableTouchRotate)
                        {
                            this.rotateHandler.Delta(position);
                            e.Handled = true;
                        }
                        break;
                    case 2:
                        if (EnablePinchZoom)
                        {
                            if(prevScale == 1)
                            {
                                prevScale = e.Cumulative.Scale;
                            }
                            else
                            {
                                var zoomAroundPoint = this.zoomHandler.UnProject(
                                    e.Position, this.zoomHandler.Origin, this.CameraLookDirection);
                                if (zoomAroundPoint != null)
                                {
                                    float s = e.Cumulative.Scale;
                                    this.zoomHandler.Zoom((prevScale - s), zoomAroundPoint.Value, true);
                                    prevScale = s;
                                }
                            }
                        }
                        e.Handled = true;
                        break;
                    case 3:
                        if (EnableThreeFingerPan)
                        {
                            this.panHandler.Delta(position);
                            e.Handled = true;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Called when the <see cref="E:System.Windows.UIElement.ManipulationStarted"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        public void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            this.touchPreviousPoint = e.Position;
            this.manipulatorCount = 0;
            this.prevScale = 1;
        }

        /// <summary>
        /// Invoked when an unhandled MouseDown attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.
        /// </param>
        public void OnMouseDown(PointerRoutedEventArgs e)
        {
            if(e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
            {
                OnStylusSystemGesture(e);
            }
            else if(e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                Viewport.CapturePointer(e.Pointer);
            }
            else
            {
                inputController.OnPointerPressed(e);
            }
        }

        public void OnMouseUp(PointerRoutedEventArgs e)
        {
            if(e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                Viewport.ReleasePointerCapture(e.Pointer);
            }
        }

        /// <summary>
        /// Invoked when an unhandled StylusSystemGesture attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.StylusSystemGestureEventArgs"/> that contains the event data.
        /// </param>
        private void OnStylusSystemGesture(PointerRoutedEventArgs e)
        {
            // Debug.WriteLine("OnStylusSystemGesture: " + e.SystemGesture);
            if (e.Pointer.IsInContact)
            {
                var p = e.GetCurrentPoint(Viewport).Position;
                this.changeLookAtHandler.Started(p);
                this.changeLookAtHandler.Completed(p);
                e.Handled = true;
            }

            //if (e.SystemGesture == SystemGesture.TwoFingerTap)
            //{
            //    this.ZoomExtents();
            //    e.Handled = true;
            //}
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
            var axis1 =Vector3D.Normalize(Vector3D.Cross(this.CameraLookDirection, this.CameraUpDirection));
            var axis2 =Vector3D.Normalize(Vector3D.Cross(axis1, this.CameraLookDirection));
            axis1 *= (ActualCamera.CreateLeftHandSystem ? -1 : 1);
            var l = this.CameraLookDirection.Length();
            var f = l * 0.001f;
            var move = (-axis1 * f * (float)dx) + (axis2 * f * (float)dy);

            // this should be dependent on distance to target?
            return move;
        }

        /// <summary>
        /// The on time step.
        /// </summary>
        /// <param name="ticks">
        /// The time.
        /// </param>
        public void OnTimeStep(long ticks)
        {
            if (lastTick == 0)
            {
                lastTick = ticks;
            }
            var time = (float)(ticks - this.lastTick) / Stopwatch.Frequency;
            time = time == 0 ? 0.016f : time;
            // should be independent of time
            var factor = Viewport.IsInertiaEnabled ? (float)Clamp(Math.Pow(Viewport.CameraInertiaFactor, time / 0.02f), 0.1f, 1) : 0;
            bool needUpdate = false;

            if (this.rotationSpeed.LengthSquared() > 0.1)
            {
                this.rotateHandler.Rotate(
                    this.rotationPosition, (rotationPosition.ToVector2() + (rotationSpeed * time)).ToPoint(), this.rotationPoint3D, false);
                this.rotationSpeed *= factor;
                needUpdate = true;
                this.spinningSpeed = VectorZero;
            }
            else
            {
                this.rotationSpeed = VectorZero;
                if (this.isSpinning && this.spinningSpeed.LengthSquared() > 0.1)
                {
                    this.rotateHandler.Rotate(
                        this.spinningPosition, (spinningPosition.ToVector2() + (spinningSpeed * time)).ToPoint(), this.spinningPoint3D, false);
                    if (!Viewport.InfiniteSpin)
                    {
                        this.spinningSpeed *= factor;
                    }
                    needUpdate = true;
                }
                else
                {
                    this.spinningSpeed = VectorZero;
                }
            }

            if (this.panSpeed.LengthSquared() > 0.0001)
            {
                this.panHandler.Pan(this.panSpeed * time, false);
                this.panSpeed *= factor;
                needUpdate = true;
            }
            else
            {
                this.panSpeed = Vector3DZero;
            }

            if (this.moveSpeed.LengthSquared() > 0.0001)
            {
                this.zoomHandler.MoveCameraPosition(this.moveSpeed * time, false);
                this.moveSpeed *= factor;
                needUpdate = true;
            }
            else
            {
                this.moveSpeed = Vector3DZero;
            }

            if (Math.Abs(this.zoomSpeed) > 0.001)
            {
                this.zoomHandler.Zoom(this.zoomSpeed * time, this.zoomPoint3D, false, false);
                this.zoomSpeed *= factor;
                needUpdate = true;
            }
            else
            {
                zoomSpeed = 0;
            }

            if (ActualCamera != null && ActualCamera.OnTimeStep())
            {
                needUpdate = true;
            }
            if (needUpdate)
            {
                lastTick = ticks;
                Viewport.InvalidateRender();
            }
            else
            {
                lastTick = 0;
            }
        }

        public void OnMouseWheel(PointerRoutedEventArgs e)
        {
            if (!Viewport.IsZoomEnabled)
            {
                return;
            }
            var delta = e.GetCurrentPoint(Viewport).Properties.MouseWheelDelta;
            if (Viewport.ZoomAroundMouseDownPoint)
            {
                var point = e.GetCurrentPoint(Viewport).Position;
                if (this.Viewport.FindNearest(point, out Point3D nearestPoint, out Vector3D normal, out Element3D visual))
                {
                    this.AddZoomForce(-delta * 0.001, nearestPoint);
                    e.Handled = true;
                    return;
                }
            }

            this.AddZoomForce(-delta * 0.001);
            e.Handled = true;
        }

        /// <summary>
        /// The stop animations.
        /// </summary>
        public void StopAnimations()
        {
            StopPanning();
            StopZooming();
            StopSpin();
        }

        private void ExecuteZoomExtents()
        {
            this.StopAnimations();
            this.ZoomExtents();
        }

        /// <summary>
        /// Determines whether the model up direction is (0,1,0).
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the up direction is (0,1,0); otherwise, <c>false</c>.
        /// </returns>
        public bool IsModelUpDirectionY()
        {
            return Viewport.ModelUpDirection.Y.Equals(1);
        }

        private void ExecuteTopView()
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

        private void ExecuteBottomView()
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

        private void ExecuteLeftView()
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

        private void ExecuteRightView()
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

        private void ExecuteFrontView()
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

        private void ExecuteBackView()
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

        private void ExecuteResetCamera()
        {
            if (Viewport.IsPanEnabled && Viewport.IsZoomEnabled && Viewport.CameraMode != CameraMode.FixedPosition)
            {
                this.StopAnimations();
                this.ResetCamera();
            }
        }
    }
}
