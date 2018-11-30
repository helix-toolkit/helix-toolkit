using SharpDX;

namespace HelixToolkit.SharpDX.Core.Controls
{
    public sealed class PanHandler : MouseGestureHandler
    {
        public PanHandler(CameraController cameraController) : base(cameraController)
        {
        }

        /// <summary>
        /// The 3D pan origin.
        /// </summary>
        private Vector3 panPoint3D;

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="Vector2"/> instance containing the event data.</param>
        public override void Delta(Vector2 e)
        {
            base.Delta(e);
            var thisPoint3D = this.UnProject(e, this.panPoint3D, this.Camera.LookDirection);

            if (this.LastPoint3D == null || thisPoint3D == null)
            {
                return;
            }

            var delta3D = this.LastPoint3D.Value - thisPoint3D.Value;
            this.Pan(delta3D);

            this.LastPoint = e;
            this.LastPoint3D = this.UnProject(e, this.panPoint3D, this.Camera.LookDirection);
        }

        /// <summary>
        /// Pans the camera by the specified 3D vector (world coordinates).
        /// </summary>
        /// <param name="delta">
        /// The panning vector.
        /// </param>
        /// <param name="stopOther">Stop other manipulation</param>
        public void Pan(Vector3 delta, bool stopOther = true)
        {
            if (!this.Controller.IsPanEnabled)
            {
                return;
            }
            if (stopOther)
            {
                this.Controller.StopSpin();
                this.Controller.StopZooming();
            }
            if (this.CameraMode == CameraMode.FixedPosition)
            {
                return;
            }
            this.Camera.Position += delta;
        }

        /// <summary>
        /// Pans the camera by the specified 2D vector (screen coordinates).
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        /// <param name="stopOther">Stop other manipulation</param>
        public void Pan(Vector2 delta, bool stopOther = true)
        {
            if (stopOther)
            {
                this.Controller.StopSpin();
                this.Controller.StopZooming();
            }
            var mousePoint = (this.LastPoint + delta);

            var thisPoint3D = this.UnProject(mousePoint, this.panPoint3D, this.Camera.LookDirection);

            if (this.LastPoint3D == null || thisPoint3D == null)
            {
                return;
            }

            var delta3D = this.LastPoint3D.Value - thisPoint3D.Value;
            this.Pan(delta3D);

            this.LastPoint3D = this.UnProject(mousePoint, this.panPoint3D, this.Camera.LookDirection);

            this.LastPoint = mousePoint;
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="Vector2"/> instance containing the event data.</param>
        protected override void Started(Vector2 e)
        {
            base.Started(e);
            this.panPoint3D = this.Camera.Target;
            if (this.MouseDownNearestPoint3D.HasValue)
            {
                this.panPoint3D = this.MouseDownNearestPoint3D.Value;
            }

            this.LastPoint3D = this.UnProject(this.MouseDownPoint, this.panPoint3D, this.Camera.LookDirection);
        }

        /// <summary>
        /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
        /// </summary>
        /// <returns>
        /// True if the execution can continue.
        /// </returns>
        protected override bool CanStart()
        {
            return this.Controller.IsPanEnabled && this.Controller.CameraMode != CameraMode.FixedPosition;
        }

        /// <summary>
        /// Called when inertia is starting.
        /// </summary>
        /// <param name="elapsedTime">
        /// The elapsed time (milliseconds).
        /// </param>
        protected override void OnInertiaStarting(double elapsedTime)
        {
            var speed = (this.LastPoint - this.MouseDownPoint) * (40.0f / (float)elapsedTime);
            this.Controller.AddPanForce(speed.X, speed.Y);
        }
    }
}
