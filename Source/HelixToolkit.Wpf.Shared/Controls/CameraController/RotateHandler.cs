// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotateHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Handles rotation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

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
        private Vector3D rotationAxisX;

        /// <summary>
        /// The y rotation axis.
        /// </summary>
        private Vector3D rotationAxisY;

        /// <summary>
        /// The rotation point.
        /// </summary>
        private Point rotationPoint;

        /// <summary>
        /// The 3D rotation point.
        /// </summary>
        private Point3D rotationPoint3D;

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
        /// Occurs when the manipulation is completed.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Completed(ManipulationEventArgs e)
        {
            base.Completed(e);
            this.Controller.HideTargetAdorner();
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Delta(ManipulationEventArgs e)
        {
            base.Delta(e);
            this.Rotate(this.LastPoint, e.CurrentPosition, this.rotationPoint3D);
            this.LastPoint = e.CurrentPosition;
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
        public void LookAt(Point3D target, double animationTime)
        {
            if (!this.Controller.IsPanEnabled)
            {
                return;
            }

            CameraHelper.LookAt(this.Camera, target, animationTime);

            this.Controller.OnLookAtChanged();
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
        public void Rotate(Point p0, Point p1, Point3D rotateAround)
        {
            if (!this.Controller.IsRotationEnabled)
            {
                return;
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

            if (Math.Abs(this.Controller.CameraUpDirection.Length - 1) > 1e-8)
            {
                this.Controller.CameraUpDirection.Normalize();
            }
        }

        /// <summary>
        /// The rotate.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        public void Rotate(Vector delta)
        {
            var p0 = this.LastPoint;
            var p1 = p0 + delta;
            if (this.MouseDownPoint3D != null)
            {
                this.Rotate(p0, p1, this.MouseDownPoint3D.Value);
            }

            this.LastPoint = p0;
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
        public void RotateTurnball(Point p1, Point p2, Point3D rotateAround)
        {
            this.InitTurnballRotationAxes(p1);

            Vector delta = p2 - p1;

            Vector3D relativeTarget = rotateAround - this.CameraTarget;
            Vector3D relativePosition = rotateAround - this.CameraPosition;

            double d = -1;
            if (this.CameraMode != CameraMode.Inspect)
            {
                d = 0.2;
            }

            d *= this.RotationSensitivity;

            var q1 = new Quaternion(this.rotationAxisX, d * delta.X);
            var q2 = new Quaternion(this.rotationAxisY, d * delta.Y);
            Quaternion q = q1 * q2;

            var m = new Matrix3D();
            m.Rotate(q);

            Vector3D newLookDir = m.Transform(this.CameraLookDirection);
            Vector3D newUpDirection = m.Transform(this.CameraUpDirection);

            Vector3D newRelativeTarget = m.Transform(relativeTarget);
            Vector3D newRelativePosition = m.Transform(relativePosition);

            Vector3D newRightVector = Vector3D.CrossProduct(newLookDir, newUpDirection);
            newRightVector.Normalize();
            Vector3D modUpDir = Vector3D.CrossProduct(newRightVector, newLookDir);
            modUpDir.Normalize();
            if ((newUpDirection - modUpDir).Length > 1e-8)
            {
                newUpDirection = modUpDir;
            }

            Point3D newTarget = rotateAround - newRelativeTarget;
            Point3D newPosition = rotateAround - newRelativePosition;
            Vector3D newLookDirection = newTarget - newPosition;

            this.CameraLookDirection = newLookDirection;
            if (CameraMode == CameraMode.Inspect)
            {
                this.CameraPosition = newPosition;
            }
            this.CameraUpDirection = newUpDirection;
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
        public void RotateTurntable(Vector delta, Point3D rotateAround)
        {
            Vector3D relativeTarget = rotateAround - this.CameraTarget;
            Vector3D relativePosition = rotateAround - this.CameraPosition;

            Vector3D up = this.ModelUpDirection;
            Vector3D dir = this.CameraLookDirection;
            dir.Normalize();

            Vector3D right = Vector3D.CrossProduct(dir, this.CameraUpDirection);
            right.Normalize();

            double d = -0.5;
            if (this.CameraMode != CameraMode.Inspect)
            {
                d *= -0.2;
            }

            d *= this.RotationSensitivity;

            var q1 = new Quaternion(up, d * delta.X);
            var q2 = new Quaternion(right, d * delta.Y);
            Quaternion q = q1 * q2;

            var m = new Matrix3D();
            m.Rotate(q);

            Vector3D newUpDirection = m.Transform(this.CameraUpDirection);

            Vector3D newRelativeTarget = m.Transform(relativeTarget);
            Vector3D newRelativePosition = m.Transform(relativePosition);

            Point3D newTarget = rotateAround - newRelativeTarget;
            Point3D newPosition = rotateAround - newRelativePosition;

            this.CameraLookDirection = newTarget - newPosition;
            if (CameraMode == CameraMode.Inspect)
            {
                this.CameraPosition = newPosition;
            }

            this.CameraUpDirection = newUpDirection;
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Started(ManipulationEventArgs e)
        {
            base.Started(e);

            this.rotationPoint = new Point(
                this.Controller.Viewport.ActualWidth / 2, this.Controller.Viewport.ActualHeight / 2);
            this.rotationPoint3D = this.Controller.CameraTarget;

            switch (this.Controller.CameraMode)
            {
                case CameraMode.WalkAround:
                    this.rotationPoint = this.MouseDownPoint;
                    this.rotationPoint3D = this.Controller.CameraPosition;
                    break;
                default:
                    if (this.Controller.FixedRotationPointEnabled)
                    {
                        this.rotationPoint = this.Viewport.Point3DtoPoint2D(this.Controller.FixedRotationPoint);
                        this.rotationPoint3D = this.Controller.FixedRotationPoint;
                    }
                    else if (this.changeLookAt && this.MouseDownNearestPoint3D != null)
                    {
                        this.LookAt(this.MouseDownNearestPoint3D.Value, 0);
                        this.rotationPoint3D = this.Controller.CameraTarget;
                    }
                    else if (this.Controller.RotateAroundMouseDownPoint && this.MouseDownNearestPoint3D != null)
                    {
                        this.rotationPoint = this.MouseDownPoint;
                        this.rotationPoint3D = this.MouseDownNearestPoint3D.Value;
                    }

                    break;
            }

            if (this.Controller.CameraMode == CameraMode.Inspect)
            {
                if (this.Controller.ZoomAroundMouseDownPoint)
                    this.Controller.ShowTargetAdorner(this.MouseDownNearestPoint2D);
                else
                    this.Controller.ShowTargetAdorner(this.MouseDownPoint);
            }

            switch (this.Controller.CameraRotationMode)
            {
                case CameraRotationMode.Trackball:
                    break;
                case CameraRotationMode.Turntable:
                    break;
                case CameraRotationMode.Turnball:
                    this.InitTurnballRotationAxes(e.CurrentPosition);
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
                return this.Controller.CameraMode != CameraMode.FixedPosition && this.Controller.IsPanEnabled;
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
        protected override void OnInertiaStarting(int elapsedTime)
        {
            Vector delta = this.LastPoint - this.MouseDownPoint;

            // Debug.WriteLine("SpinInertiaStarting: " + elapsedTime + "ms " + delta.Length + "px");
            this.Controller.StartSpin(
                4 * delta * ((double)this.Controller.SpinReleaseTime / elapsedTime),
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
        private static Vector3D ProjectToTrackball(Point point, double w, double h)
        {
            // Use the diagonal for scaling, making sure that the whole client area is inside the trackball
            double r = Math.Sqrt(w * w + h * h) / 2;
            double x = (point.X - w / 2) / r;
            double y = (h / 2 - point.Y) / r;
            double z2 = 1 - x * x - y * y;
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// The init turnball rotation axes.
        /// </summary>
        /// <param name="p1">
        /// The p 1.
        /// </param>
        private void InitTurnballRotationAxes(Point p1)
        {
            double fx = p1.X / this.ViewportWidth;
            double fy = p1.Y / this.ViewportHeight;

            Vector3D up = this.CameraUpDirection;
            Vector3D dir = this.CameraLookDirection;
            dir.Normalize();

            Vector3D right = Vector3D.CrossProduct(dir, this.CameraUpDirection);
            right.Normalize();

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
        /// Rotates around the camera up and right axes.
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
        private void RotateAroundUpAndRight(Point p1, Point p2, Point3D rotateAround)
        {
            var dp = p2 - p1;

            // Rotate around the camera up direction
            var delta1 = new Quaternion(this.CameraUpDirection, -dp.X * this.RotationSensitivity);

            // Rotate around the camera right direction
            var delta2 = new Quaternion(
                Vector3D.CrossProduct(this.CameraUpDirection, this.CameraLookDirection), dp.Y * this.RotationSensitivity);

            var delta = delta1 * delta2;
            var rotate = new RotateTransform3D(new QuaternionRotation3D(delta));
            var relativeTarget = rotateAround - this.CameraTarget;
            var relativePosition = rotateAround - this.CameraPosition;

            var newRelativeTarget = rotate.Transform(relativeTarget);
            var newRelativePosition = rotate.Transform(relativePosition);
            var newUpDirection = rotate.Transform(this.CameraUpDirection);

            var newTarget = rotateAround - newRelativeTarget;
            var newPosition = rotateAround - newRelativePosition;

            this.CameraLookDirection = newTarget - newPosition;
            if (CameraMode == CameraMode.Inspect)
            {
                this.CameraPosition = newPosition;
            }
            this.CameraUpDirection = newUpDirection;
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
        private void RotateTrackball(Point p1, Point p2, Point3D rotateAround)
        {
            // http://viewport3d.com/trackball.htm
            // http://www.codeplex.com/3DTools/Thread/View.aspx?ThreadId=22310
            Vector3D v1 = ProjectToTrackball(p1, this.ViewportWidth, this.ViewportHeight);
            Vector3D v2 = ProjectToTrackball(p2, this.ViewportWidth, this.ViewportHeight);

            // transform the trackball coordinates to view space
            Vector3D viewZ = this.CameraLookDirection;
            Vector3D viewX = Vector3D.CrossProduct(this.CameraUpDirection, viewZ);
            Vector3D viewY = Vector3D.CrossProduct(viewX, viewZ);
            viewX.Normalize();
            viewY.Normalize();
            viewZ.Normalize();
            Vector3D u1 = viewZ * v1.Z + viewX * v1.X + viewY * v1.Y;
            Vector3D u2 = viewZ * v2.Z + viewX * v2.X + viewY * v2.Y;

            // Could also use the Camera ViewMatrix
            // var vm = Viewport3DHelper.GetViewMatrix(this.ActualCamera);
            // vm.Invert();
            // var ct = new MatrixTransform3D(vm);
            // var u1 = ct.Transform(v1);
            // var u2 = ct.Transform(v2);

            // Find the rotation axis and angle
            Vector3D axis = Vector3D.CrossProduct(u1, u2);
            if (axis.LengthSquared < 1e-8)
            {
                return;
            }

            double angle = Vector3D.AngleBetween(u1, u2);

            // Create the transform
            var delta = new Quaternion(axis, -angle * this.RotationSensitivity * 5);
            var rotate = new RotateTransform3D(new QuaternionRotation3D(delta));

            // Find vectors relative to the rotate-around point
            Vector3D relativeTarget = rotateAround - this.CameraTarget;
            Vector3D relativePosition = rotateAround - this.CameraPosition;

            // Rotate the relative vectors
            Vector3D newRelativeTarget = rotate.Transform(relativeTarget);
            Vector3D newRelativePosition = rotate.Transform(relativePosition);
            Vector3D newUpDirection = rotate.Transform(this.CameraUpDirection);

            // Find new camera position
            var newTarget = rotateAround - newRelativeTarget;
            var newPosition = rotateAround - newRelativePosition;

            this.CameraLookDirection = newTarget - newPosition;
            if (CameraMode == CameraMode.Inspect)
            {
                this.CameraPosition = newPosition;
            }
            this.CameraUpDirection = newUpDirection;
        }

    }
}