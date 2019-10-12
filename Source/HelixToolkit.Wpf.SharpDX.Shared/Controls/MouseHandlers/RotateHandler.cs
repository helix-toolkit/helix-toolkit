// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotateHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Handles rotation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Input;
using Vector3 = global::SharpDX.Vector3;
using Vector2 = global::SharpDX.Vector2;
using Quaternion = global::SharpDX.Quaternion;
using Matrix = global::SharpDX.Matrix;
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Cameras;
#endif
namespace HelixToolkit.Wpf.SharpDX
{
#if !COREWPF
    using Cameras;
#endif
    /// <summary>
    /// Handles rotation.
    /// </summary>
    internal class RotateHandler : MouseGestureHandler
    {
        /// <summary>
        /// The change look at.
        /// </summary>
        private readonly bool changeLookAt;

        /// <summary>
        /// The x rotation axis.
        /// </summary>
        private Vector3 rotationAxisX;

        /// <summary>
        /// The y rotation axis.
        /// </summary>
        private Vector3 rotationAxisY;

        /// <summary>
        /// The rotation point.
        /// </summary>
        private Point rotationPoint;

        /// <summary>
        /// The 3D rotation point.
        /// </summary>
        private Vector3 rotationPoint3D;

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateHandler"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="changeLookAt">
        /// The change look at.
        /// </param>
        public RotateHandler(CameraController controller, bool changeLookAt = false)
            : base(controller)
        {
            this.changeLookAt = changeLookAt;
        }

        /// <summary>
        /// Gets the camera rotation mode.
        /// </summary>
        /// <value>
        /// The camera rotation mode.
        /// </value>
        protected CameraRotationMode CameraRotationMode
        {
            get
            {
                return this.Controller.CameraRotationMode;
            }
        }

        /// <summary>
        /// Occurs when the manipulation is completed.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Completed(Point e)
        {
            base.Completed(e);
            this.Viewport.HideTargetAdorner();
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Delta(Point e)
        {
            base.Delta(e);
            this.Rotate(this.LastPoint, e, this.rotationPoint3D);
            this.LastPoint = e;
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
        public void LookAt(Vector3 target, double animationTime)
        {
            if (!this.Controller.IsPanEnabled)
            {
                return;
            }

            this.Camera.LookAt(target.ToPoint3D(), animationTime);
        }

        /// <summary>
        /// Rotate the camera around the specified point.
        /// </summary>
        /// <param name="p0">
        /// The p 0.
        /// </param>
        /// <param name="p1">
        /// The p 1.
        /// </param>
        /// <param name="rotateAround">
        /// The rotate around.
        /// </param>
        public void Rotate(Point p0, Point p1, Vector3 rotateAround)
        {
            Rotate(p0.ToVector2(), p1.ToVector2(), rotateAround);
        }

        /// <summary>
        /// Rotates the specified p0.
        /// </summary>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="rotateAround">The rotate around.</param>
        /// <param name="stopOther">if set to <c>true</c> [stop other].</param>
        public void Rotate(Vector2 p0, Vector2 p1, Vector3 rotateAround, bool stopOther = true)
        {
            if (!this.Controller.IsRotationEnabled)
            {
                return;
            }
            if (stopOther)
            {
                Controller.StopZooming();
                Controller.StopPanning();
            }
            p0 = Vector2.Multiply(p0, Controller.AllowRotateXY);
            p1 = Vector2.Multiply(p1, Controller.AllowRotateXY);
            Vector3 newPos = Camera.CameraInternal.Position;
            Vector3 newLook = Camera.CameraInternal.LookDirection;
            Vector3 newUp = Vector3.Normalize(Camera.CameraInternal.UpDirection);
            switch (this.Controller.CameraRotationMode)
            {
                case CameraRotationMode.Trackball:
                    CameraMath.RotateTrackball(CameraMode, ref p0, ref p1, ref rotateAround, (float)RotationSensitivity,
                        Controller.Width, Controller.Height, Camera, Inv, out newPos, out newLook, out newUp);                   
                    break;
                case CameraRotationMode.Turntable:
                    var p = p1 - p0;
                    CameraMath.RotateTurntable(CameraMode, ref p, ref rotateAround, (float)RotationSensitivity,
                        Controller.Width, Controller.Height, Camera, Inv, ModelUpDirection, out newPos, out newLook, out newUp);
                    break;
                case CameraRotationMode.Turnball:
                    CameraMath.RotateTurnball(CameraMode, ref p0, ref p1, ref rotateAround, (float)RotationSensitivity,
                        Controller.Width, Controller.Height, Camera, Inv, out newPos, out newLook, out newUp);
                    break;
                default:
                    break;
            }
            Camera.LookDirection = newLook.ToVector3D();
            Camera.Position = newPos.ToPoint3D();
            Camera.UpDirection = newUp.ToVector3D();
        }
        /// <summary>
        /// The rotate.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        public void Rotate(Vector2 delta)
        {
            var p0 = this.LastPoint.ToVector2();
            var p1 = p0 + delta;
            if (this.MouseDownPoint3D != null)
            {
                this.Rotate(p0, p1, this.MouseDownPoint3D.Value);
            }
            this.LastPoint = new Point(p0.X, p0.Y);
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Started(Point e)
        {
            base.Started(e);
            this.rotationPoint = new Point(
                this.Viewport.ActualWidth / 2, this.Viewport.ActualHeight / 2);
            this.rotationPoint3D = this.Camera.CameraInternal.Target;

            switch (this.CameraMode)
            {
                case CameraMode.WalkAround:
                    this.rotationPoint = this.MouseDownPoint;
                    this.rotationPoint3D = this.Camera.CameraInternal.Position;
                    break;
                default:
                    if (Controller.FixedRotationPointEnabled)
                    {
                        this.rotationPoint3D = Controller.FixedRotationPoint;
                    }
                    else if (this.changeLookAt && this.MouseDownNearestPoint3D != null)
                    {
                        this.LookAt(this.MouseDownNearestPoint3D.Value, 0);
                        this.rotationPoint3D = this.Camera.CameraInternal.Target;
                    }
                    else if (this.Controller.RotateAroundMouseDownPoint && this.MouseDownNearestPoint3D != null)
                    {
                        this.rotationPoint = this.MouseDownPoint;
                        this.rotationPoint3D = this.MouseDownNearestPoint3D.Value;
                    }

                    break;
            }

            if (this.CameraMode == CameraMode.Inspect)
            {
                this.Viewport.ShowTargetAdorner(this.rotationPoint);
            }

            switch (this.CameraRotationMode)
            {
                case CameraRotationMode.Trackball:
                    break;
                case CameraRotationMode.Turntable:
                    break;
                case CameraRotationMode.Turnball:
                    CameraMath.InitTurnballRotationAxes(e.ToVector2(), (int)Viewport.ActualWidth, (int)Viewport.ActualHeight, Camera,
                        out rotationAxisX, out rotationAxisY);
                    break;
            }

            this.Controller.StopSpin();
        }

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <returns>
        /// True if the execution can continue.
        /// </returns>
        protected override bool CanExecute()
        {
            if (this.changeLookAt)
            {
                return this.CameraMode != CameraMode.FixedPosition && this.Controller.IsPanEnabled;
            }

            return this.Controller.IsRotationEnabled;
        }

        /// <summary>
        /// Gets the cursor.
        /// </summary>
        /// <returns>
        /// A cursor.
        /// </returns>
        protected override Cursor GetCursor()
        {
            return this.Controller.RotateCursor;
        }

        /// <summary>
        /// Called when inertia is starting.
        /// </summary>
        /// <param name="elapsedTime">
        /// The elapsed time.
        /// </param>
        protected override void OnInertiaStarting(double elapsedTime)
        {
            var delta = this.LastPoint - this.MouseDownPoint;
            var deltaV = new Vector2((float)delta.X, (float)delta.Y);
            // Debug.WriteLine("SpinInertiaStarting: " + elapsedTime + "ms " + delta.Length + "px");
            this.Controller.StartSpin(
                4 * deltaV * (float)(this.Controller.SpinReleaseTime / elapsedTime),
                this.MouseDownPoint,
                this.rotationPoint3D);
        }

    }
}