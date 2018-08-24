// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraController.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a control that manipulates the camera by mouse and keyboard gestures.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Numerics;
    using Utilities;
    /// <summary>
    /// Provides a control that manipulates the camera by mouse and keyboard gestures.
    /// </summary>
    public class CameraController
    {        
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
        internal ZoomHandler changeFieldOfViewHandler;

        /// <summary>
        /// The change look at event handler.
        /// </summary>
        internal RotateHandler changeLookAtHandler;

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
        private Vector3 moveSpeed;

        /// <summary>
        /// The pan event handler.
        /// </summary>
        internal PanHandler panHandler;
        /// <summary>
        /// The set target handler
        /// </summary>
        internal RotateHandler setTargetHandler;
        /// <summary>
        /// The pan speed.
        /// </summary>
        private Vector3 panSpeed;

        /// <summary>
        /// The rotation event handler.
        /// </summary>
        internal RotateHandler rotateHandler;

        /// <summary>
        /// The 3D rotation point.
        /// </summary>
        private Vector3 rotationPoint3D;

        /// <summary>
        /// The rotation position.
        /// </summary>
        private Vector2 rotationPosition;

        /// <summary>
        /// The rotation speed.
        /// </summary>
        private Vector2 rotationSpeed;

        /// <summary>
        /// The 3D point to spin around.
        /// </summary>
        private Vector3 spinningPoint3D;

        /// <summary>
        /// The spinning position.
        /// </summary>
        private Vector2 spinningPosition;

        /// <summary>
        /// The spinning speed.
        /// </summary>
        private Vector2 spinningSpeed;


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
        internal ZoomHandler zoomHandler;

        /// <summary>
        /// The point to zoom around.
        /// </summary>
        private Vector3 zoomPoint3D;

        private double prevScale = 1;
        /// <summary>
        /// The zoom rectangle event handler.
        /// </summary>
        internal ZoomRectangleHandler zoomRectangleHandler;

        /// <summary>
        /// The zoom speed.
        /// </summary>
        private double zoomSpeed;

        private static readonly Point PointZero = new Point(0, 0);
        private static readonly Vector2 VectorZero = new Vector2();
        private static readonly Vector3 Vector3DZero = new Vector3();

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraController" /> class.
        /// </summary>
        public CameraController(Viewport3DX viewport)
        {
            this.InitializeBindings();
            this.Viewport = viewport;
        }

        private Camera actualCamera;
        /// <summary>
        /// Gets ActualCamera.
        /// </summary>
        public Camera ActualCamera
        {
            set
            {
                if(actualCamera != value)
                {
                    actualCamera = value;
                    OnCameraChanged();
                }
            }
            get { return actualCamera; }
        }

        /// <summary>
        /// Gets or sets CameraLookDirection.
        /// </summary>
        public Vector3 CameraLookDirection
        {
            get
            {
                return this.ActualCamera.CameraInternal.LookDirection;
            }

            set
            {
                this.ActualCamera.LookDirection = value.ToVector3D();
            }
        }

        /// <summary>
        /// Gets or sets CameraMode.
        /// </summary>
        public CameraMode CameraMode
        {
            set; get;
        } = CameraMode.Inspect;

        /// <summary>
        /// Gets or sets CameraPosition.
        /// </summary>
        public Vector3 CameraPosition
        {
            get
            {
                return this.ActualCamera.CameraInternal.Position;
            }

            set
            {
                this.ActualCamera.Position = value.ToPoint3D();
            }
        }

        /// <summary>
        /// Gets or sets CameraRotationMode.
        /// </summary>
        public CameraRotationMode CameraRotationMode
        {
            set; get;
        } = CameraRotationMode.Turntable;

        /// <summary>
        /// Gets or sets CameraTarget.
        /// </summary>
        public Vector3 CameraTarget
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
        public Vector3 CameraUpDirection
        {
            get
            {
                return this.ActualCamera.CameraInternal.UpDirection;
            }

            set
            {
                this.ActualCamera.UpDirection = value.ToVector3D();
            }
        }

        /// <summary>
        /// Gets or sets the change field of view cursor.
        /// </summary>
        /// <value> The change field of view cursor. </value>
        public Cursor ChangeFieldOfViewCursor
        {
            set; get;
        } = Cursors.ScrollNS;

        /// <summary>
        /// Gets or sets the default camera (used when resetting the view).
        /// </summary>
        /// <value> The default camera. </value>
        public ProjectionCamera DefaultCamera
        {
            set;get;
        }

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
        public Vector3 ModelUpDirection
        {
            set; get;
        } = new Vector3(0, 1, 0);

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
        /// Gets or sets the pan cursor.
        /// </summary>
        /// <value> The pan cursor. </value>
        public Cursor PanCursor
        {
            set; get;
        } = Cursors.Hand;

        /// <summary>
        /// Gets or sets a value indicating whether to rotate around the mouse down point.
        /// </summary>
        /// <value> <c>true</c> if rotation around the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool RotateAroundMouseDownPoint
        {
            set; get;
        } = false;

        /// <summary>
        /// Gets or sets the rotate cursor.
        /// </summary>
        /// <value> The rotate cursor. </value>
        public Cursor RotateCursor
        {
            set; get;
        } = Cursors.SizeAll;

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
        /// Gets or sets Viewport.
        /// </summary>
        public Viewport3DX Viewport
        {
            private set;get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to zoom around mouse down point.
        /// </summary>
        /// <value> <c>true</c> if zooming around the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool ZoomAroundMouseDownPoint
        {
            set; get;
        } = false;

        /// <summary>
        /// Gets or sets the zoom cursor.
        /// </summary>
        /// <value> The zoom cursor. </value>
        public Cursor ZoomCursor
        {
            set; get;
        } = Cursors.SizeNS;

        /// <summary>
        /// Gets or sets the zoom rectangle cursor.
        /// </summary>
        /// <value> The zoom rectangle cursor. </value>
        public Cursor ZoomRectangleCursor
        {
            set; get;
        } = Cursors.SizeNWSE;

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
        public Vector3 FixedRotationPoint
        {
            set; get;
        } = new Vector3();
        #region TouchGesture
        public bool EnableTouchRotate { set; get; } = true;
        public bool EnablePinchZoom { set; get; } = true;
        public bool EnableThreeFingerPan { set; get; } = true;
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
        public void AddMoveForce(float dx, float dy, float dz)
        {
            this.AddMoveForce(new Vector3(dx, dy, dz));
        }

        /// <summary>
        /// Adds the specified move force.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        public void AddMoveForce(Vector3 delta)
        {
            if (!this.IsMoveEnabled)
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
        public void AddPanForce(float dx, float dy)
        {
            this.AddPanForce(this.FindPanVector(dx, dy));
        }

        /// <summary>
        /// The add pan force.
        /// </summary>
        /// <param name="pan">
        /// The pan.
        /// </param>
        public void AddPanForce(Vector3 pan)
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
            Viewport.InvalidateRender();
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
        public void AddRotateForce(float dx, float dy)
        {
            if (!this.IsRotationEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            if (this.IsInertiaEnabled)
            {
                this.rotationPoint3D = this.CameraTarget;
                this.rotationPosition = new Vector2((float)Viewport.ActualWidth / 2, (float)Viewport.ActualHeight / 2);
                this.rotationSpeed.X += dx * 40;
                this.rotationSpeed.Y += dy * 40;
            }
            else if (FixedRotationPointEnabled)
            {
                this.rotationPosition = new Vector2((float)Viewport.ActualWidth / 2, (float)Viewport.ActualHeight / 2);
                this.rotateHandler.Rotate(
                    this.rotationPosition, this.rotationPosition + new Vector2(dx, dy), FixedRotationPoint);
            }
            else
            {
                this.rotationPosition = new Vector2((float)Viewport.ActualWidth / 2, (float)Viewport.ActualHeight / 2);
                this.rotateHandler.Rotate(
                    this.rotationPosition, this.rotationPosition + new Vector2(dx, dy), this.CameraTarget);
            }
            Viewport.InvalidateRender();
        }

        /// <summary>
        /// Adds the zoom force.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        public void AddZoomForce(float delta)
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
        public void AddZoomForce(float delta, Vector3 zoomOrigin)
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
        public void ChangeDirection(Vector3 lookDir, Vector3 upDir, double animationTime = 500)
        {
            ChangeDirection(lookDir.ToVector3D(), upDir.ToVector3D(), animationTime);
        }
        /// <summary>
        /// Changes the direction.
        /// </summary>
        /// <param name="lookDir">The look dir.</param>
        /// <param name="upDir">Up dir.</param>
        /// <param name="animationTime">The animation time.</param>
        public void ChangeDirection(System.Windows.Media.Media3D.Vector3D lookDir, System.Windows.Media.Media3D.Vector3D upDir, double animationTime = 500)
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
        public void ChangeDirection(Vector3 lookDir, double animationTime = 500)
        {
            if (!this.IsRotationEnabled)
            {
                return;
            }

            this.StopAnimations();
            this.PushCameraSetting();
            this.ActualCamera.ChangeDirection(lookDir.ToVector3D(), this.ActualCamera.UpDirection, animationTime);
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
        public void LookAt(Vector3 target, double animationTime)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }

            this.PushCameraSetting();
            this.ActualCamera.LookAt(target.ToPoint3D(), animationTime);
        }

        /// <summary>
        /// Push the current camera settings on an internal stack.
        /// </summary>
        public void PushCameraSetting()
        {
            if(ActualCamera == null)
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
            if (!this.IsZoomEnabled || !this.IsRotationEnabled || !this.IsPanEnabled)
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
        public void StartSpin(Vector2 speed, Point position, Vector3 aroundPoint)
        {
            this.spinningSpeed = speed;
            this.spinningPosition = position.ToVector2();
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
        /// Stops the zooming inertia.
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
            panSpeed = Vector3.Zero;
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
        /// Called when the <see cref="E:System.Windows.UIElement.ManipulationCompleted"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        public void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            var p = e.ManipulationOrigin + e.TotalManipulation.Translation;

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

        /// <summary>
        /// Called when the <see cref="E:System.Windows.UIElement.ManipulationDelta"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        public void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            if(!EnablePinchZoom && !EnableThreeFingerPan && !EnableTouchRotate)
            {
                return;
            }
            // number of manipulators (fingers)
            int n = e.Manipulators.Count();
            var p = e.ManipulationOrigin;
            var position = new Point(this.touchPreviousPoint.X + e.DeltaManipulation.Translation.X, this.touchPreviousPoint.Y + e.DeltaManipulation.Translation.Y);
            this.touchPreviousPoint = position;
            
            // http://msdn.microsoft.com/en-us/library/system.windows.uielement.manipulationdelta.aspx

            //// System.Diagnostics.Debug.WriteLine("OnManipulationDelta: T={0}, S={1}, R={2}, O={3}", e.DeltaManipulation.Translation, e.DeltaManipulation.Scale, e.DeltaManipulation.Rotation, e.ManipulationOrigin);
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
                        this.zoomHandler.Completed(p);
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
                            this.zoomHandler.Started(p);
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
                this.manipulatorCount = n;
                // skip this event, the origin may have changed
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
                                prevScale = e.CumulativeManipulation.Scale.Length;
                            }
                            else
                            {
                                var zoomAroundPoint = this.zoomHandler.UnProject(
                                    p, this.zoomHandler.Origin, this.CameraLookDirection);
                                if (zoomAroundPoint != null)
                                {
                                    var s = e.CumulativeManipulation.Scale.Length;
                                    //Debug.WriteLine(s);
                                    this.zoomHandler.Zoom((prevScale - s), zoomAroundPoint.Value, true);
                                    prevScale = s;
                                }
                            }
                            e.Handled = true;
                        }                        
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
        public void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            this.touchPreviousPoint = e.ManipulationOrigin;
            this.manipulatorCount = 0;
            this.prevScale = 1;
        }

        /// <summary>
        /// Invoked when an unhandled MouseDown attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.
        /// </param>
        public void OnMouseDown(MouseButtonEventArgs e)
        {
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
        public void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            // Debug.WriteLine("OnStylusSystemGesture: " + e.SystemGesture);
            if (e.SystemGesture == SystemGesture.HoldEnter)
            {
                var p = e.GetPosition(Viewport);
                this.changeLookAtHandler.Started(p);
                this.changeLookAtHandler.Completed(p);
                e.Handled = true;
            }

            if (e.SystemGesture == SystemGesture.TwoFingerTap)
            {
                this.ZoomExtents();
                e.Handled = true;
            }
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
            this.ChangeDirection(new Vector3(1, 0, 0), new Vector3(0, 0, 1));
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
            this.ChangeDirection(new Vector3(0, 0, 1), new Vector3(0, -1, 0));
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
        /// The <see cref="Vector3"/> .
        /// </returns>
        private Vector3 FindPanVector(float dx, float dy)
        {
            var axis1 = Vector3.Normalize(Vector3.Cross(this.CameraLookDirection, this.CameraUpDirection));
            var axis2 = Vector3.Normalize(Vector3.Cross(axis1, this.CameraLookDirection));
            axis1 *= (ActualCamera.CreateLeftHandSystem ? -1 : 1);
            var l = this.CameraLookDirection.Length();
            var f = l * 0.001f;
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
            this.ChangeDirection(new Vector3(-1, 0, 0), new Vector3(0, 0, 1));
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
            this.setTargetHandler = new RotateHandler(this, true);
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
            this.ChangeDirection(new Vector3(0, 1, 0), new Vector3(0, 0, 1));
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
        /// Called when [composition target rendering].
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        public void OnCompositionTargetRendering(long ticks)
        {
            this.OnTimeStep(ticks);
        }

        /// <summary>
        /// Called when a key is pressed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.
        /// </param>
        public void OnKeyDown(KeyEventArgs e)
        {
            var shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            var control = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            var f = control ? 0.25f : 1;

            if (!shift)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        this.AddRotateForce(-1 * f * (float)this.LeftRightRotationSensitivity, 0);
                        e.Handled = true;
                        break;
                    case Key.Right:
                        this.AddRotateForce(1 * f * (float)this.LeftRightRotationSensitivity, 0);
                        e.Handled = true;
                        break;
                    case Key.Up:
                        this.AddRotateForce(0, -1 * f * (float)this.UpDownRotationSensitivity);
                        e.Handled = true;
                        break;
                    case Key.Down:
                        this.AddRotateForce(0, 1 * f * (float)this.UpDownRotationSensitivity);
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Left:
                        this.AddPanForce(-5 * f * (float)this.LeftRightPanSensitivity, 0);
                        e.Handled = true;
                        break;
                    case Key.Right:
                        this.AddPanForce(5 * f * (float)this.LeftRightPanSensitivity, 0);
                        e.Handled = true;
                        break;
                    case Key.Up:
                        this.AddPanForce(0, -5 * f * (float)this.UpDownPanSensitivity);
                        e.Handled = true;
                        break;
                    case Key.Down:
                        this.AddPanForce(0, 5 * f * (float)this.UpDownPanSensitivity);
                        e.Handled = true;
                        break;
                }
            }

            switch (e.Key)
            {
                case Key.PageUp:
                    this.AddZoomForce(-0.1f * f * (float)this.PageUpDownZoomSensitivity);
                    e.Handled = true;
                    break;
                case Key.PageDown:
                    this.AddZoomForce(0.1f * f * (float)this.PageUpDownZoomSensitivity);
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
                    this.AddMoveForce(0, 0, 0.1f * f * (float)this.MoveSensitivity);
                    break;
                case Key.A:
                    this.AddMoveForce(-0.1f * f * (float)this.LeftRightPanSensitivity, 0, 0);
                    break;
                case Key.S:
                    this.AddMoveForce(0, 0, -0.1f * f * (float)this.MoveSensitivity);
                    break;
                case Key.D:
                    this.AddMoveForce(0.1f * f * (float)this.LeftRightPanSensitivity, 0, 0);
                    break;
                case Key.Z:
                    this.AddMoveForce(0, -0.1f * f * (float)this.LeftRightPanSensitivity, 0);
                    break;
                case Key.Q:
                    this.AddMoveForce(0, 0.1f * f * (float)this.LeftRightPanSensitivity, 0);
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
        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }
            if (this.ZoomAroundMouseDownPoint)
            {
                var point = e.GetPosition(Viewport);
                if (this.Viewport.FindNearest(point, out Vector3 nearestPoint, out Vector3 normal, out Element3D visual))
                {
                    this.AddZoomForce(-e.Delta * 0.001f, nearestPoint);
                    e.Handled = true;
                    return;
                }
            }

            this.AddZoomForce(-e.Delta * 0.001f);
            e.Handled = true;
        }

        /// <summary>
        /// The on time step.
        /// </summary>
        /// <param name="ticks">
        /// The time.
        /// </param>
        private void OnTimeStep(long ticks)
        {
            if (lastTick == 0)
            {
                lastTick = ticks;
            }
            var time = (float)(ticks - this.lastTick) / Stopwatch.Frequency;
            time = time == 0 ? 0.016f : time;
            // should be independent of time
            var factor = this.IsInertiaEnabled ?  (float)Clamp(Math.Pow(this.InertiaFactor, time / 0.02f), 0.1f, 1) : 0;
            bool needUpdate = false;

            if (this.rotationSpeed.LengthSquared() > 0.1f)
            {
                this.rotateHandler.Rotate(
                    this.rotationPosition, this.rotationPosition + (this.rotationSpeed * time), this.rotationPoint3D, false);
                this.rotationSpeed *= factor;
                needUpdate = true;
                this.spinningSpeed = VectorZero;
            }
            else
            {
                this.rotationSpeed = VectorZero;
                if (this.isSpinning && this.spinningSpeed.LengthSquared() > 0.1f)
                {
                    this.rotateHandler.Rotate(
                        this.spinningPosition, this.spinningPosition + (this.spinningSpeed * time), this.spinningPoint3D, false);
                    if (!this.InfiniteSpin)
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

            if (this.panSpeed.LengthSquared() > 0.0001f)
            {
                this.panHandler.Pan(this.panSpeed * time, false);
                this.panSpeed *= factor;
                needUpdate = true;
            }
            else
            {
                this.panSpeed = Vector3DZero;
            }

            if (this.moveSpeed.LengthSquared() > 0.0001f)
            {
                this.zoomHandler.MoveCameraPosition(this.moveSpeed * time, false);
                this.moveSpeed *= factor;
                needUpdate = true;
            }
            else
            {
                this.moveSpeed = Vector3DZero;
            }

            if (Math.Abs(this.zoomSpeed) > 0.001f)
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

        /// <summary>
        /// The on viewport changed.
        /// </summary>
        private void OnViewportChanged()
        {
            this.InitializeBindings();
        }

        /// <summary>
        /// The refresh viewport.
        /// </summary>
        private void RefreshViewport()
        {
            Viewport.InvalidateRender();
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
            this.ChangeDirection(new Vector3(0, -1, 0), new Vector3(0, 0, 1));
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
            this.ChangeDirection(new Vector3(0, 0, -1), new Vector3(0, 1, 0));
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