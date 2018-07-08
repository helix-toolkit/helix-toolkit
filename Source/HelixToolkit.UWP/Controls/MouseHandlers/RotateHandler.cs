// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotateHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Handles rotation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    using System;
    using System.Numerics;
    using HelixToolkit.Mathematics;
    using Point3D = System.Numerics.Vector3;
    using Vector3D = System.Numerics.Vector3;
    using Point = Windows.Foundation.Point;
    using Matrix = System.Numerics.Matrix4x4;
    using Windows.UI.Core;
    using System.Diagnostics;

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
        public void LookAt(Point3D target, double animationTime)
        {
            if (!this.Controller.IsPanEnabled)
            {
                return;
            }

            this.Camera.LookAt(target, animationTime);
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
        /// <param name="stopOther">Stop other manipulation</param>
        public void Rotate(Point p0, Point p1, Point3D rotateAround, bool stopOther = true)
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
                    this.RotateTurntable(p1.ToVector2() - p0.ToVector2(), rotateAround);
                    break;
                case CameraRotationMode.Turnball:
                    this.RotateTurnball(p0, p1, rotateAround);
                    break;
            }

            if (Math.Abs(this.Camera.UpDirection.Length() - 1) > 1e-8)
            {
                Camera.UpDirection = Vector3.Normalize(this.Camera.UpDirection);
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
            var p0 = this.LastPoint;
            var p1 = p0.ToVector2() + delta;
            if (this.MouseDownPoint3D != null)
            {
                this.Rotate(p0, p1.ToPoint(), this.MouseDownPoint3D.Value);
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

            Vector2 delta = p2.ToVector2() - p1.ToVector2();

            var relativeTarget = rotateAround - this.Camera.CameraInternal.Target;
            var relativePosition = rotateAround - this.Camera.CameraInternal.Position;

            float d = -1f;
            if (this.CameraMode != CameraMode.Inspect)
            {
                d = 0.2f;
            }

            d *= (float)this.RotationSensitivity;

            var q1 = QuaternionHelper.RotationAxis(this.rotationAxisX, (float)(d * inv * delta.X / 180 * Math.PI));
            var q2 = QuaternionHelper.RotationAxis(this.rotationAxisY, (float)(d * delta.Y / 180 * Math.PI));
            Quaternion q = q1 * q2;

            var m = Matrix.CreateFromQuaternion(q);

            var newLookDir = Vector3Helper.TransformCoordinate(this.Camera.LookDirection, m);
            var newUpDirection = Vector3Helper.TransformCoordinate(this.Camera.UpDirection, m);

            var newRelativeTarget = Vector3Helper.TransformCoordinate(relativeTarget, m);
            var newRelativePosition = Vector3Helper.TransformCoordinate(relativePosition, m);

            var newRightVector = Vector3D.Normalize(Vector3D.Cross(newLookDir, newUpDirection));
            
            var modUpDir = Vector3D.Normalize( Vector3D.Cross(newRightVector, newLookDir));
            
            if ((newUpDirection - modUpDir).Length() > 1e-8)
            {
                newUpDirection = modUpDir;
            }

            var newTarget = rotateAround - newRelativeTarget;
            var newPosition = rotateAround - newRelativePosition;
            var newLookDirection = newTarget - newPosition;

            this.Camera.LookDirection = newLookDirection;
            if (this.CameraMode == CameraMode.Inspect)
            {
                this.Camera.Position = newPosition;
            }

            this.Camera.UpDirection = newUpDirection;
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
        public void RotateTurntable(Vector2 delta, Point3D rotateAround)
        {
            var relativeTarget = rotateAround - this.Camera.CameraInternal.Target;
            var relativePosition = rotateAround - this.Camera.CameraInternal.Position;

            var up = this.ModelUpDirection;
            var dir = Vector3D.Normalize(this.Camera.LookDirection);

            var right = Vector3D.Normalize(Vector3D.Cross(dir, this.Camera.UpDirection));

            var d = -0.5f;
            if (this.CameraMode != CameraMode.Inspect)
            {
                d *= -0.2f;
            }

            d *= (float)this.RotationSensitivity;

            var q1 = Quaternion.CreateFromAxisAngle(up, (float)(d * inv * delta.X / 180 * Math.PI));
            var q2 = Quaternion.CreateFromAxisAngle(right, (float)(d * delta.Y / 180 * Math.PI));
            Quaternion q = q1 * q2;

            var m = Matrix.CreateFromQuaternion(q);

            var newUpDirection = Vector3Helper.TransformCoordinate(this.Camera.UpDirection, m);
            var newRelativeTarget = Vector3Helper.TransformCoordinate(relativeTarget, m);
            var newRelativePosition = Vector3Helper.TransformCoordinate(relativePosition, m);
            var newTarget = rotateAround - newRelativeTarget;
            var newPosition = rotateAround - newRelativePosition;

            this.Camera.LookDirection = newTarget - newPosition;
            if (this.CameraMode == CameraMode.Inspect)
            {
                this.Camera.Position = newPosition;
            }

            this.Camera.UpDirection = newUpDirection;
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Started(Point e)
        {
            base.Started(e);

            this.rotationPoint = new Point(
                this.Controller.Viewport.ActualWidth / 2, this.Controller.Viewport.ActualHeight / 2);
            this.rotationPoint3D = this.Camera.CameraInternal.Target;

            switch (this.CameraMode)
            {
                case CameraMode.WalkAround:
                    this.rotationPoint = this.MouseDownPoint;
                    this.rotationPoint3D = this.Camera.CameraInternal.Position;
                    break;
                default:
                    if (Controller.Viewport.FixedRotationPointEnabled)
                    {
                        this.rotationPoint3D = Controller.Viewport.FixedRotationPoint;
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
                //this.Viewport.ShowTargetAdorner(this.rotationPoint);
            }

            switch (this.CameraRotationMode)
            {
                case CameraRotationMode.Trackball:
                    break;
                case CameraRotationMode.Turntable:
                    break;
                case CameraRotationMode.Turnball:
                    this.InitTurnballRotationAxes(e);
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
        protected override CoreCursorType GetCursor()
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
            Vector2 delta = this.LastPoint.ToVector2() - this.MouseDownPoint.ToVector2();

            // Debug.WriteLine("SpinInertiaStarting: " + elapsedTime + "ms " + delta.Length + "px");
            this.Controller.StartSpin(
                4 * delta * (float)(this.Controller.SpinReleaseTime / elapsedTime),
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
        private static Point3D ProjectToTrackball(Point point, double w, double h)
        {
            // Use the diagonal for scaling, making sure that the whole client area is inside the trackball
            double r = Math.Sqrt((w * w) + (h * h)) / 2;
            double x = (point.X - (w / 2)) / r;
            double y = ((h / 2) - point.Y) / r;
            double z2 = 1 - (x * x) - (y * y);
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Point3D((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Initializes the 'turn-ball' rotation axes from the specified point.
        /// </summary>
        /// <param name="p1">
        /// The point.
        /// </param>
        private void InitTurnballRotationAxes(Point p1)
        {
            double fx = p1.X / this.Controller.Viewport.ActualWidth;
            double fy = p1.Y / this.Controller.Viewport.ActualHeight;

            var up = Vector3D.Normalize(this.Camera.UpDirection);
            var dir = Vector3D.Normalize(this.Camera.LookDirection);           

            var right = Vector3D.Normalize(Vector3D.Cross(dir, this.Camera.UpDirection));            

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
        private void RotateTrackball(Point p1, Point p2, Point3D rotateAround)
        {
            // http://viewport3d.com/trackball.htm
            // http://www.codeplex.com/3DTools/Thread/View.aspx?ThreadId=22310
            var v1 = ProjectToTrackball(p1, this.Controller.Viewport.ActualWidth, this.Controller.Viewport.ActualHeight);
            var v2 = ProjectToTrackball(p2, this.Controller.Viewport.ActualWidth, this.Controller.Viewport.ActualHeight);

            // transform the trackball coordinates to view space
            var viewZ = this.Camera.LookDirection * inv;
            var viewX = Vector3D.Cross(this.Camera.UpDirection, viewZ) * inv;
            var viewY = Vector3D.Normalize(Vector3D.Cross(viewX, viewZ));
            viewZ = Vector3D.Normalize(viewZ);
            viewX = Vector3D.Normalize(viewX);

            var u1 = (viewZ * v1.Z) + (viewX * v1.X) + (viewY * v1.Y);
            var u2 = (viewZ * v2.Z) + (viewX * v2.X) + (viewY * v2.Y);

            // Could also use the Camera ViewMatrix
            // var vm = Viewport3DHelper.GetViewMatrix(this.ActualCamera);
            // vm.Invert();
            // var ct = new MatrixTransform3D(vm);
            // var u1 = ct.Transform(v1);
            // var u2 = ct.Transform(v2);

            // Find the rotation axis and angle
            var axis = Vector3D.Cross(u1, u2);
            if (axis.LengthSquared() < 1e-8)
            {
                return;
            }

            var angle = u1.AngleBetween(u2);
            
            // Create the transform
            var delta = Quaternion.CreateFromAxisAngle(Vector3D.Normalize(axis), -(float)(angle * this.RotationSensitivity * 5));
            var rotate = Matrix.CreateFromQuaternion(delta);

            // Find vectors relative to the rotate-around point
            var relativeTarget = rotateAround - this.Camera.CameraInternal.Target;
            var relativePosition = rotateAround - this.Camera.CameraInternal.Position;

            // Rotate the relative vectors
            var newRelativeTarget = Vector3D.Transform(relativeTarget, rotate);
            var newRelativePosition = Vector3D.Transform(relativePosition, rotate);
            var newUpDirection = Vector3D.Transform(this.Camera.UpDirection, rotate);

            // Find new camera position
            var newTarget = rotateAround - newRelativeTarget;
            var newPosition = rotateAround - newRelativePosition;

            this.Camera.LookDirection = newTarget - newPosition;
            if (this.CameraMode == CameraMode.Inspect)
            {
                this.Camera.Position = newPosition;
            }

            this.Camera.UpDirection = newUpDirection;
        }
    }
}