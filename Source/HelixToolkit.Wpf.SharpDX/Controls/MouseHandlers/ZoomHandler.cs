// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Handles zooming.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Handles zooming.
    /// </summary>
    internal class ZoomHandler : MouseGestureHandler
    {
        /// <summary>
        /// The change field of view.
        /// </summary>
        private readonly bool changeFieldOfView;

        /// <summary>
        /// The zoom point.
        /// </summary>
        private Point zoomPoint;

        /// <summary>
        /// The 3D zoom point.
        /// </summary>
        private Point3D zoomPoint3D;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomHandler"/> class.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="changeFieldOfView">
        /// if set to <c>true</c> [change field of view].
        /// </param>
        public ZoomHandler(Viewport3DX viewport, bool changeFieldOfView = false)
            : base(viewport)
        {
            this.changeFieldOfView = changeFieldOfView;
        }

        /// <summary>
        /// Occurs when the manipulation is completed.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Completed(ManipulationEventArgs e)
        {
            base.Completed(e);
            this.Viewport.HideTargetAdorner();
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Delta(ManipulationEventArgs e)
        {
            var delta = e.CurrentPosition - this.LastPoint;
            this.LastPoint = e.CurrentPosition;
            this.Zoom(delta.Y * 0.01, this.zoomPoint3D);
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Started(ManipulationEventArgs e)
        {
            base.Started(e);
            this.zoomPoint = new Point(this.Viewport.ActualWidth / 2, this.Viewport.ActualHeight / 2);
            this.zoomPoint3D = this.Camera.Target;

            if (this.Viewport.ZoomAroundMouseDownPoint && this.MouseDownNearestPoint3D != null)
            {
                this.zoomPoint = this.MouseDownPoint;
                this.zoomPoint3D = this.MouseDownNearestPoint3D.Value;
            }

            if (!this.changeFieldOfView)
            {
                this.Viewport.ShowTargetAdorner(this.zoomPoint);
            }
        }

        /// <summary>
        /// Zooms the view.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        public void Zoom(double delta)
        {
            this.Zoom(delta, this.Camera.Target);
        }

        /// <summary>
        /// Zooms the view around the specified point.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        /// <param name="zoomAround">
        /// The zoom around.
        /// </param>
        public void Zoom(double delta, Point3D zoomAround)
        {
            if (!this.Viewport.IsZoomEnabled)
            {
                return;
            }

            if (this.Camera is PerspectiveCamera)
            {
                if (delta < -0.5)
                {
                    delta = -0.5;
                }

                delta *= this.ZoomSensitivity;

                if (this.CameraMode == CameraMode.FixedPosition || this.changeFieldOfView)
                {
                    this.Viewport.ZoomByChangingFieldOfView(delta);
                }
                else
                {
                    switch (this.CameraMode)
                    {
                        case CameraMode.Inspect:
                            this.ChangeCameraDistance(delta, zoomAround);
                            break;
                        case CameraMode.WalkAround:
                            this.Camera.Position -= this.Camera.LookDirection * delta;
                            break;
                    }
                }

                return;
            }

            if (this.Camera is OrthographicCamera)
            {
                this.ZoomByChangingCameraWidth(delta, zoomAround);
            }
        }

        /// <summary>
        /// Changes the camera position by the specified vector.
        /// </summary>
        /// <param name="delta">The translation vector in camera space (z in look direction, y in up direction, and x perpendicular to the two others)</param>
        public void MoveCameraPosition(Vector3D delta)
        {
            var z = this.Camera.LookDirection;
            z.Normalize();
            var x = Vector3D.CrossProduct(this.Camera.LookDirection, this.Camera.UpDirection);
            var y = Vector3D.CrossProduct(x, z);
            y.Normalize();
            x = Vector3D.CrossProduct(z, y);

            // delta *= this.ZoomSensitivity;
            switch (this.CameraMode)
            {
                case CameraMode.Inspect:
                case CameraMode.WalkAround:
                    this.Camera.Position += (x * delta.X) + (y * delta.Y) + (z * delta.Z);
                    break;
            }
        }

        /// <summary>
        /// The change camera width.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        /// <param name="zoomAround">
        /// The zoom around.
        /// </param>
        public void ZoomByChangingCameraWidth(double delta, Point3D zoomAround)
        {
            if (delta < -0.5)
            {
                delta = -0.5;
            }

            switch (this.CameraMode)
            {
                case CameraMode.WalkAround:
                case CameraMode.Inspect:
                case CameraMode.FixedPosition:
                    this.ChangeCameraDistance(delta, zoomAround);

                    // Modify the camera width
                    var ocamera = this.Camera as OrthographicCamera;
                    if (ocamera != null)
                    {
                        ocamera.Width *= 1 + delta;
                    }

                    break;
            }
        }

        /// <summary>
        /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
        /// </summary>
        /// <returns>
        /// True if the execution can continue.
        /// </returns>
        protected override bool CanExecute()
        {
            if (this.changeFieldOfView)
            {
                return this.Viewport.IsChangeFieldOfViewEnabled && this.Camera is PerspectiveCamera;
            }

            return this.Viewport.IsZoomEnabled;
        }

        /// <summary>
        /// Gets the cursor for the gesture.
        /// </summary>
        /// <returns>
        /// A cursor.
        /// </returns>
        protected override Cursor GetCursor()
        {
            return this.Viewport.ZoomCursor;
        }

        /// <summary>
        /// Changes the camera distance.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="zoomAround">The zoom around point.</param>
        private void ChangeCameraDistance(double delta, Point3D zoomAround)
        {
            // Handle the 'zoomAround' point
            var target = this.Camera.Position + this.Camera.LookDirection;
            var relativeTarget = zoomAround - target;
            var relativePosition = zoomAround - this.Camera.Position;
            if (relativePosition.Length < 1e-4)
            {
                if (delta > 0) //If Zoom out from very close distance, increase the initial relativePosition
                {
                    relativePosition.Normalize();
                    relativePosition /= 10;
                }
                else//If Zoom in too close, stop it.
                {
                    return;
                }
            }
            var newRelativePosition = relativePosition * (1 + delta);
            var newRelativeTarget = relativeTarget * (1 + delta);
           
            var newTarget = zoomAround - newRelativeTarget;
            var newPosition = zoomAround - newRelativePosition;

            var newDistance = (newPosition - zoomAround).Length;
            var oldDistance = (this.Camera.Position - zoomAround).Length;

            if (newDistance > this.Viewport.ZoomDistanceLimitFar && (oldDistance < this.Viewport.ZoomDistanceLimitFar || newDistance > oldDistance))
            {
                return;
            }

            if (newDistance < this.Viewport.ZoomDistanceLimitNear && (oldDistance > this.Viewport.ZoomDistanceLimitNear || newDistance < oldDistance))
            {
                return;
            }

            var newLookDirection = newTarget - newPosition;
            this.Camera.LookDirection = newLookDirection;
            this.Camera.Position = newPosition;
        }
    }
}