/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System;
using System.Diagnostics;
namespace HelixToolkit.SharpDX.Core.Controls
{   
    using Cameras;

    public sealed class CameraController
    {
        /// <summary>
        /// Gets or sets CameraMode.
        /// </summary>
        public CameraMode CameraMode = CameraMode.Inspect;

        /// <summary>
        /// Gets or sets CameraRotationMode.
        /// </summary>
        public CameraRotationMode CameraRotationMode = CameraRotationMode.Turntable;

        /// <summary>
        /// Gets or sets InertiaFactor.
        /// </summary>
        public float InertiaFactor = 0.85f;

        /// <summary>
        /// Gets or sets a value indicating whether InfiniteSpin.
        /// </summary>
        public bool InfiniteSpin = false;

        /// <summary>
        /// Gets or sets a value indicating whether field of view can be changed.
        /// </summary>
        public bool IsChangeFieldOfViewEnabled = true;

        /// <summary>
        /// Gets or sets a value indicating whether inertia is enabled for the camera manipulations.
        /// </summary>
        /// <value><c>true</c> if inertia is enabled; otherwise, <c>false</c>.</value>
        public bool IsInertiaEnabled = true;

        /// <summary>
        /// Gets or sets a value indicating whether move is enabled.
        /// </summary>
        /// <value> <c>true</c> if move is enabled; otherwise, <c>false</c> . </value>
        public bool IsMoveEnabled = true;

        /// <summary>
        /// Gets or sets a value indicating whether pan is enabled.
        /// </summary>
        public bool IsPanEnabled = true;

        /// <summary>
        /// Gets or sets a value indicating whether IsRotationEnabled.
        /// </summary>
        public bool IsRotationEnabled = true;

        /// <summary>
        /// Gets or sets a value indicating whether IsZoomEnabled.
        /// </summary>
        public bool IsZoomEnabled = true;

        /// <summary>
        /// Gets or sets the sensitivity for pan by the left and right keys.
        /// </summary>
        /// <value> The pan sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the pan direction.
        /// </remarks>
        public float LeftRightPanSensitivity = 1.0f;

        /// <summary>
        /// Gets or sets the sensitivity for rotation by the left and right keys.
        /// </summary>
        /// <value> The rotation sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the rotation direction.
        /// </remarks>
        public float LeftRightRotationSensitivity = 1.0f;

        /// <summary>
        /// Gets or sets the maximum field of view.
        /// </summary>
        /// <value> The maximum field of view. </value>
        public float MaximumFieldOfView = 120.0f;

        /// <summary>
        /// Gets or sets the minimum field of view.
        /// </summary>
        /// <value> The minimum field of view. </value>
        public float MinimumFieldOfView = 10.0f;

        private Vector3 modelUpDirection = Vector3.UnitY;
        /// <summary>
        /// Gets or sets the model up direction.
        /// </summary>
        public Vector3 ModelUpDirection
        {
            set
            {
                modelUpDirection = value;
                if (Viewport != null)
                {
                    Viewport.ModelUpDirection = value;
                }
            }
            get => modelUpDirection;
        }

        /// <summary>
        /// Gets or sets the move sensitivity.
        /// </summary>
        /// <value> The move sensitivity. </value>
        public float MoveSensitivity = 1.0f;

        /// <summary>
        /// Gets or sets the sensitivity for zoom by the page up and page down keys.
        /// </summary>
        /// <value> The zoom sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the zoom direction.
        /// </remarks>
        public float PageUpDownZoomSensitivity = 1.0f;

        /// <summary>
        /// Gets or sets a value indicating whether to rotate around the mouse down point.
        /// </summary>
        /// <value> <c>true</c> if rotation around the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool RotateAroundMouseDownPoint = false;

        /// <summary>
        /// Gets or sets the rotation sensitivity (degrees/pixel).
        /// </summary>
        /// <value> The rotation sensitivity. </value>
        public float RotationSensitivity = 1.0f;

        /// <summary>
        /// Gets or sets a value indicating whether to show a target adorner when manipulating the camera.
        /// </summary>
        public bool ShowCameraTarget = true;

        /// <summary>
        /// Gets or sets the max duration of mouse drag to activate spin.
        /// </summary>
        /// <remarks>
        /// If the time between mouse down and mouse up is less than this value, spin is activated.
        /// </remarks>
        public int SpinReleaseTime = 200;

        /// <summary>
        /// Gets or sets the sensitivity for pan by the up and down keys.
        /// </summary>
        /// <value> The pan sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the pan direction.
        /// </remarks>
        public float UpDownPanSensitivity = 1.0f;

        /// <summary>
        /// Gets or sets the sensitivity for rotation by the up and down keys.
        /// </summary>
        /// <value> The rotation sensitivity. </value>
        /// <remarks>
        /// Use -1 to invert the rotation direction.
        /// </remarks>
        public float UpDownRotationSensitivity = 1.0f;

        /// <summary>
        /// Gets or sets a value indicating whether to zoom around mouse down point.
        /// </summary>
        /// <value> <c>true</c> if zooming around the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool ZoomAroundMouseDownPoint = false;

        /// <summary>
        /// Gets or sets the zoom distance limit far.
        /// </summary>
        /// <value>
        /// The zoom distance limit far.
        /// </value>
        public float ZoomDistanceLimitFar = float.PositiveInfinity;

        /// <summary>
        /// Gets or sets the zoom distance limit near.
        /// </summary>
        /// <value>
        /// The zoom distance limit near.
        /// </value>
        public float ZoomDistanceLimitNear = 0.001f;

        /// <summary>
        /// Gets or sets ZoomSensitivity.
        /// </summary>
        public float ZoomSensitivity = 1.0f;

        private static readonly Point PointZero = new Point(0, 0);

        private static readonly Vector3 Vector3DZero = new Vector3();

        private static readonly Vector2 VectorZero = new Vector2();

        /// <summary>
        /// The pan event handler.
        /// </summary>
        private readonly PanHandler panHandler;

        /// <summary>
        /// The rotation event handler.
        /// </summary>
        private readonly RotateHandler rotateHandler;

        /// <summary>
        /// The zoom event handler.
        /// </summary>
        private readonly ZoomHandler zoomHandler;

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
        /// The pan speed.
        /// </summary>
        private Vector3 panSpeed;
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
        /// The point to zoom around.
        /// </summary>
        private Vector3 zoomPoint3D;

        /// <summary>
        /// The zoom speed.
        /// </summary>
        private float zoomSpeed;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is panning.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is panning; otherwise, <c>false</c>.
        /// </value>
        public bool IsPanning { private set; get; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is rotating.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is rotating; otherwise, <c>false</c>.
        /// </value>
        public bool IsRotating { private set; get; } = false;
        /// <summary>
        /// Gets a value indicating whether this instance is mouse captured.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mouse captured; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseCaptured { get => IsPanning || IsRotating; }

        public CameraController(ViewportCore viewport)
        {
            this.Viewport = viewport;
            this.zoomHandler = new ZoomHandler(this);
            this.panHandler = new PanHandler(this);
            this.rotateHandler = new RotateHandler(this);
            rotateHandler.MouseCaptureRequested += (s, e) => { IsRotating = true; };
            panHandler.MouseCaptureRequested += (s, e) => { IsPanning = true; };
            rotateHandler.MouseReleaseRequested += (s, e) => { IsRotating = false; };
            panHandler.MouseReleaseRequested += (s, e) => { IsPanning = false; };
        }

        /// <summary>
        /// Gets ActualCamera.
        /// </summary>
        public CameraCore ActualCamera
        {
            get => Viewport.CameraCore;
        }

        /// <summary>
        /// Gets or sets CameraLookDirection.
        /// </summary>
        public Vector3 CameraLookDirection
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
        public Vector3 CameraPosition
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
                return this.ActualCamera.UpDirection;
            }

            set
            {
                this.ActualCamera.UpDirection = value;
            }
        }
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
        /// Gets a value indicating whether IsOrthographicCamera.
        /// </summary>
        public bool IsOrthographicCamera
        {
            get
            {
                return this.ActualCamera is OrthographicCameraCore;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsPerspectiveCamera.
        /// </summary>
        public bool IsPerspectiveCamera
        {
            get
            {
                return this.ActualCamera is PerspectiveCameraCore;
            }
        }

        /// <summary>
        /// Gets OrthographicCamera.
        /// </summary>
        public OrthographicCameraCore OrthographicCamera
        {
            get
            {
                return this.ActualCamera as OrthographicCameraCore;
            }
        }

        /// <summary>
        /// Gets PerspectiveCamera.
        /// </summary>
        public PerspectiveCameraCore PerspectiveCamera
        {
            get
            {
                return this.ActualCamera as PerspectiveCameraCore;
            }
        }

        /// <summary>
        /// Gets or sets to allow rotate x direction and y direction globally. X, Y is screen space.
        /// <para>X = 1: Allow left/right rotation. Y = 1: Allow up/down rotation</para>
        /// <para>Default is (1, 1)</para>
        /// </summary>
        /// <value>
        /// The allow rotate xy.
        /// </value>
        public Vector2 AllowRotateXY
        {
            set; get;
        } = Vector2.One;

        /// <summary>
        /// Gets or sets Viewport.
        /// </summary>
        public ViewportCore Viewport
        {
            get;
        }
        internal int Height { get => Viewport.ViewportRectangle.Height; }
        internal int Width { get => Viewport.ViewportRectangle.Width; }

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

            if (this.IsInertiaEnabled)
            {
                this.panSpeed += pan;
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

            if (this.IsInertiaEnabled)
            {
                this.rotationPoint3D = this.CameraTarget;
                this.rotationPosition = new Vector2(Width / 2f, Height / 2f);
                this.rotationSpeed.X += dx * 40;
                this.rotationSpeed.Y += dy * 40;
            }
            else if (FixedRotationPointEnabled)
            {
                this.rotationPosition = new Vector2(Width / 2f, Height / 2f);
                this.rotateHandler.Rotate(
                    this.rotationPosition, this.rotationPosition + new Vector2(dx, dy), FixedRotationPoint);
            }
            else
            {
                this.rotationPosition = new Vector2(Width / 2f, Height / 2f);
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
        /// Changes the direction.
        /// </summary>
        /// <param name="lookDir">The look dir.</param>
        /// <param name="upDir">Up dir.</param>
        /// <param name="animationTime">The animation time.</param>
        public void ChangeDirection(Vector3 lookDir, Vector3 upDir, float animationTime = 500)
        {
            if (!this.IsRotationEnabled)
            {
                return;
            }

            this.StopAnimations();
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
        public void ChangeDirection(Vector3 lookDir, float animationTime = 500)
        {
            if (!this.IsRotationEnabled)
            {
                return;
            }

            this.StopAnimations();
            this.ActualCamera.ChangeDirection(lookDir, ActualCamera.UpDirection, animationTime);
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
        public void LookAt(Vector3 target, float animationTime)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }
            this.ActualCamera.LookAt(target, animationTime);
        }

        /// <summary>
        /// Pass mouse wheel scrolling for zooming.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public bool MouseWheel(float delta, Vector2 position)
        {
            if (!this.IsZoomEnabled)
            {
                return false;
            }
            if (this.ZoomAroundMouseDownPoint)
            {
                if (this.Viewport.FindNearest(position, out Vector3 nearestPoint, out Vector3 normal, out object model))
                {
                    this.AddZoomForce(-delta * 0.001f, nearestPoint);
                    return true;
                }
            }

            this.AddZoomForce(-delta * 0.001f);
            return true;
        }

        /// <summary>
        /// The refresh viewport.
        /// </summary>
        public void RefreshViewport()
        {
            Viewport.InvalidateRender();
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
            this.ActualCamera.Reset();
            this.ActualCamera.ZoomExtents(this.Viewport);
        }

        /// <summary>
        /// Resets the camera up direction.
        /// </summary>
        public void ResetCameraUpDirection()
        {
            this.CameraUpDirection = this.ModelUpDirection;
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
        public void StartSpin(Vector2 speed, Vector2 position, Vector3 aroundPoint)
        {
            this.spinningSpeed = speed;
            this.spinningPosition = position;
            this.spinningPoint3D = aroundPoint;
            this.isSpinning = true;
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
        /// Stops the panning.
        /// </summary>
        public void StopPanning()
        {
            panSpeed = Vector3.Zero;
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
        /// Views the back.
        /// </summary>
        public void ViewBack()
        {
            this.ChangeDirection(new Vector3(1, 0, 0), new Vector3(0, 0, 1));
        }

        public void ViewFront()
        {
            this.ChangeDirection(new Vector3(-1, 0, 0), new Vector3(0, 0, 1));
        }

        /// <summary>
        /// Views the right.
        /// </summary>
        public void ViewRight()
        {
            this.ChangeDirection(new Vector3(0, -1, 0), new Vector3(0, 0, 1));
        }

        /// <summary>
        /// Views the top.
        /// </summary>
        public void ViewTop()
        {
            this.ChangeDirection(new Vector3(0, 0, -1), new Vector3(0, 1, 0));
        }

        /// <summary>
        /// Zooms by the specified delta value.
        /// </summary>
        /// <param name="delta">
        /// The delta value.
        /// </param>
        public void Zoom(float delta)
        {
            this.zoomHandler.Zoom(delta);
        }

        /// <summary>
        /// Zooms to the extents of the model.
        /// </summary>
        /// <param name="animationTime">
        /// The animation time (milliseconds).
        /// </param>
        public void ZoomExtents(float animationTime = 200)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            this.ActualCamera.ZoomExtents(this.Viewport, animationTime);
        }
        /// <summary>
        /// Zooms the extents handler.
        /// </summary>
        public void ZoomExtentsHandler()
        {
            this.StopAnimations();
            this.ZoomExtents();
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
        private float Clamp(float value, float min, float max)
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
            float l = 0;
            if (ActualCamera is PerspectiveCameraCore)
            {
                // this should be dependent on distance to target?
                l = this.CameraLookDirection.Length();
            }
            else if (ActualCamera is OrthographicCameraCore orth)
            {
                // this should be dependent on width
                l = orth.Width;
            }
            var f = l * 0.001f;
            var move = (-axis1 * f * dx) + (axis2 * f * dy);
            // this should be dependent on distance to target?
            return move;
        }

        /// <summary>
        /// The on time step.
        /// </summary>
        public void OnTimeStep()
        {
            long ticks = Stopwatch.GetTimestamp();
            if (lastTick == 0 || lastTick > ticks)
            {
                lastTick = ticks;
            }
            var time = (float)(ticks - this.lastTick) / Stopwatch.Frequency;
            time = time == 0 ? 0.016f : time;
            time = Math.Min(time, 0.05f); // Clamp the maximum time elapse to prevent over shooting
            // should be independent of time
            var factor = this.IsInertiaEnabled ? Clamp((float)Math.Pow(this.InertiaFactor, time / 0.02f), 0.1f, 1) : 0;
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
        /// Views the bottom.
        /// </summary>
        private void ViewBottom()
        {
            this.ChangeDirection(new Vector3(0, 0, 1), new Vector3(0, -1, 0));
        }
        /// <summary>
        /// Views the left.
        /// </summary>
        private void ViewLeft()
        {
            this.ChangeDirection(new Vector3(0, 1, 0), new Vector3(0, 0, 1));
        }
        /// <summary>
        /// Starts the rotation.
        /// </summary>
        /// <param name="p">The p.</param>
        public void StartRotate(Vector2 p)
        {
            rotateHandler.Start(p);
        }
        /// <summary>
        /// Ends the rotation.
        /// </summary>
        /// <param name="p">The p.</param>
        public void EndRotate(Vector2 p)
        {
            rotateHandler.End(p);
        }
        /// <summary>
        /// Starts the panning.
        /// </summary>
        /// <param name="p">The p.</param>
        public void StartPan(Vector2 p)
        {
            panHandler.Start(p);
        }
        /// <summary>
        /// Ends the panning.
        /// </summary>
        /// <param name="p">The p.</param>
        public void EndPan(Vector2 p)
        {
            panHandler.End(p);
        }
        /// <summary>
        /// Mouses move position.
        /// </summary>
        /// <param name="p">The p.</param>
        public void MouseMove(Vector2 p)
        {
            if (IsRotating)
            {
                rotateHandler.MouseMove(p);
            }
            if (IsPanning)
            {
                panHandler.MouseMove(p);
            }
        }
    }
}