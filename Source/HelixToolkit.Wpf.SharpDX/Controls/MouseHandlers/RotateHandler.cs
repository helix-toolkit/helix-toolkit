// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotateHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Handles rotation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Vector3 = System.Numerics.Vector3;
    using Vector2 = System.Numerics.Vector2;
    using Quaternion = System.Numerics.Quaternion;
    using Matrix = System.Numerics.Matrix4x4;

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
            switch (this.Controller.CameraRotationMode)
            {
                case CameraRotationMode.Trackball:
                    this.RotateTrackball(p0, p1, rotateAround);
                    break;
                case CameraRotationMode.Turntable:
                    this.RotateTurntable(p1 - p0, rotateAround);
                    break;
                case CameraRotationMode.Turnball:
                    this.RotateTurnball(p0, p1, rotateAround);
                    break;
            }

            if (Math.Abs(this.Camera.UpDirection.Length - 1) > 1e-8)
            {
                this.Camera.UpDirection.Normalize();
            }
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
        /// Rotate around three axes.
        /// </summary>
        /// <param name="p1">
        /// The previous mouse position.
        /// </param>
        /// <param name="p2">
        /// The current mouse position.
        /// </param>
        /// <param name="rotateAround">
        /// The point to rotate around.
        /// </param>
        public void RotateTurnball(Vector2 p1, Vector2 p2, Vector3 rotateAround)
        {
            this.InitTurnballRotationAxes(p1);

            Vector2 delta = p2 - p1;

            var relativeTarget = rotateAround - this.Camera.CameraInternal.Target;
            var relativePosition = rotateAround - this.Camera.CameraInternal.Position;

            float d = -1;
            if (this.CameraMode != CameraMode.Inspect)
            {
                d = 0.2f;
            }

            d *= (float)RotationSensitivity;

            var q1 = Quaternion.CreateFromAxisAngle(this.rotationAxisX, d * Inv * delta.X / 180 * (float)Math.PI);
            var q2 = Quaternion.CreateFromAxisAngle(this.rotationAxisY, d * delta.Y / 180 * (float)Math.PI);
            Quaternion q = q1 * q2;

            var m = Matrix.CreateFromQuaternion(q);
            Vector3 newLookDir = Vector3.TransformNormal(this.Camera.CameraInternal.LookDirection, m);
            Vector3 newUpDirection = Vector3.TransformNormal(this.Camera.CameraInternal.UpDirection, m);

            Vector3 newRelativeTarget = Mathematics.Vector3Helper.TransformCoordinate(relativeTarget, m);
            Vector3 newRelativePosition = Mathematics.Vector3Helper.TransformCoordinate(relativePosition, m);

            var newRightVector = Vector3.Normalize(Vector3.Cross(newLookDir, newUpDirection));
            var modUpDir = Vector3.Normalize(Vector3.Cross(newRightVector, newLookDir));
            if ((newUpDirection - modUpDir).Length() > 1e-8)
            {
                newUpDirection = modUpDir;
            }

            var newTarget = rotateAround - newRelativeTarget;
            var newPosition = rotateAround - newRelativePosition;
            var newLookDirection = newTarget - newPosition;

            this.Camera.LookDirection = newLookDirection.ToVector3D();
            if (this.CameraMode == CameraMode.Inspect)
            {
                this.Camera.Position = newPosition.ToPoint3D();
            }

            this.Camera.UpDirection = newUpDirection.ToVector3D();
        }

        /// <summary>
        /// Rotate camera using 'Turntable' rotation.
        /// </summary>
        /// <param name="delta">
        /// The relative change in position.
        /// </param>
        /// <param name="rotateAround">
        /// The point to rotate around.
        /// </param>
        public void RotateTurntable(Vector2 delta, Vector3 rotateAround)
        {
            var relativeTarget = rotateAround - this.Camera.CameraInternal.Target;
            var relativePosition = rotateAround - this.Camera.CameraInternal.Position;
            var cUp = Camera.CameraInternal.UpDirection;
            var up = this.ModelUpDirection;
            var dir = Vector3.Normalize(Camera.CameraInternal.LookDirection);
            var right = Vector3.Normalize(Vector3.Cross(dir, cUp));

            float d = -0.5f;
            if (this.CameraMode != CameraMode.Inspect)
            {
                d *= -0.2f;
            }

            d *= (float)this.RotationSensitivity;

            var q1 = Quaternion.CreateFromAxisAngle(up, d * Inv * delta.X / 180 * (float)Math.PI);
            var q2 = Quaternion.CreateFromAxisAngle(right, d * delta.Y / 180 * (float)Math.PI);
            Quaternion q = q1 * q2;

            var m = Matrix.CreateFromQuaternion(q);

            var newUpDirection = Vector3.TransformNormal(cUp, m);

            var newRelativeTarget = Mathematics.Vector3Helper.TransformCoordinate(relativeTarget, m);
            var newRelativePosition = Mathematics.Vector3Helper.TransformCoordinate(relativePosition, m);

            var newTarget = rotateAround - newRelativeTarget;
            var newPosition = rotateAround - newRelativePosition;

            this.Camera.LookDirection = (newTarget - newPosition).ToVector3D();
            if (this.CameraMode == CameraMode.Inspect)
            {
                this.Camera.Position = newPosition.ToPoint3D();
            }

            this.Camera.UpDirection = newUpDirection.ToVector3D();
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
                    this.InitTurnballRotationAxes(e.ToVector2());
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

        /// <summary>
        /// Projects a screen position to the trackball unit sphere.
        /// </summary>
        /// <param name="point">
        /// The screen position.
        /// </param>
        /// <param name="w">
        /// The width of the viewport.
        /// </param>
        /// <param name="h">
        /// The height of the viewport.
        /// </param>
        /// <returns>
        /// A trackball coordinate.
        /// </returns>
        private static Vector3 ProjectToTrackball(Vector2 point, double w, double h)
        {
            // Use the diagonal for scaling, making sure that the whole client area is inside the trackball
            double r = Math.Sqrt((w * w) + (h * h)) / 2;
            double x = (point.X - (w / 2)) / r;
            double y = ((h / 2) - point.Y) / r;
            double z2 = 1 - (x * x) - (y * y);
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Vector3((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Initializes the 'turn-ball' rotation axes from the specified point.
        /// </summary>
        /// <param name="p1">
        /// The point.
        /// </param>
        private void InitTurnballRotationAxes(Vector2 p1)
        {
            double fx = p1.X / this.Viewport.ActualWidth;
            double fy = p1.Y / this.Viewport.ActualHeight;

            var up = Vector3.Normalize(Camera.CameraInternal.UpDirection);
            var dir = Vector3.Normalize(Camera.CameraInternal.LookDirection);

            var right = Vector3.Normalize(Vector3.Cross(dir, up));

            this.rotationAxisX = up;
            this.rotationAxisY = right;
            if (fy > 0.8 || fy < 0.2)
            {
                // delta.Y = 0;
            }

            if (fx > 0.8)
            {
                // delta.X = 0;
                this.rotationAxisY = dir;
            }

            if (fx < 0.2)
            {
                // delta.X = 0;
                this.rotationAxisY = -dir;
            }
        }

        /// <summary>
        /// The rotate trackball.
        /// </summary>
        /// <param name="p1">
        /// The previous mouse position.
        /// </param>
        /// <param name="p2">
        /// The current mouse position.
        /// </param>
        /// <param name="rotateAround">
        /// The point to rotate around.
        /// </param>
        private void RotateTrackball(Vector2 p1, Vector2 p2, Vector3 rotateAround)
        {
            // http://viewport3d.com/trackball.htm
            // http://www.codeplex.com/3DTools/Thread/View.aspx?ThreadId=22310
            var v1 = ProjectToTrackball(p1, this.Viewport.ActualWidth, this.Viewport.ActualHeight);
            var v2 = ProjectToTrackball(p2, this.Viewport.ActualWidth, this.Viewport.ActualHeight);
            var cUP = Camera.CameraInternal.UpDirection;
            // transform the trackball coordinates to view space
            var viewZ = Vector3.Normalize(Camera.CameraInternal.LookDirection * Inv);
            var viewX = Vector3.Normalize(Vector3.Cross(cUP, viewZ) * Inv);
            var viewY = Vector3.Normalize(Vector3.Cross(viewX, viewZ));
            var u1 = (viewZ * v1.Z) + (viewX * v1.X) + (viewY * v1.Y);
            var u2 = (viewZ * v2.Z) + (viewX * v2.X) + (viewY * v2.Y);

            // Could also use the Camera ViewMatrix
            // var vm = Viewport3DHelper.GetViewMatrix(this.ActualCamera);
            // vm.Invert();
            // var ct = new MatrixTransform3D(vm);
            // var u1 = ct.Transform(v1);
            // var u2 = ct.Transform(v2);

            // Find the rotation axis and angle
            var axis = Vector3.Cross(u1, u2);
            if (axis.LengthSquared() < 1e-8)
            {
                return;
            }

            var angle = VectorExtensions.AngleBetween(u1, u2);

            // Create the transform
            var rotate = Matrix.CreateFromAxisAngle(Vector3.Normalize(axis), -angle * (float)RotationSensitivity * 5);

            // Find vectors relative to the rotate-around point
            var relativeTarget = rotateAround - this.Camera.CameraInternal.Target;
            var relativePosition = rotateAround - this.Camera.CameraInternal.Position;

            // Rotate the relative vectors
            var newRelativeTarget = Mathematics.Vector3Helper.TransformCoordinate(relativeTarget, rotate);
            var newRelativePosition = Mathematics.Vector3Helper.TransformCoordinate(relativePosition, rotate);
            var newUpDirection = Mathematics.Vector3Helper.TransformCoordinate(cUP, rotate);

            // Find new camera position
            var newTarget = rotateAround - newRelativeTarget;
            var newPosition = rotateAround - newRelativePosition;

            this.Camera.LookDirection = (newTarget - newPosition).ToVector3D();
            if (this.CameraMode == CameraMode.Inspect)
            {
                this.Camera.Position = newPosition.ToPoint3D();
            }

            this.Camera.UpDirection = newUpDirection.ToVector3D();
        }
    }
}