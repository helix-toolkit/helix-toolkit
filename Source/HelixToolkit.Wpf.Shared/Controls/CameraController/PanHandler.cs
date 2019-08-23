// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PanHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Handles panning.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Handles panning.
    /// </summary>
    internal class PanHandler : MouseGestureHandler
    {
        /// <summary>
        /// The 3D pan origin.
        /// </summary>
        private Point3D panPoint3D;

        /// <summary>
        /// Initializes a new instance of the <see cref="PanHandler"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public PanHandler(CameraController controller)
            : base(controller)
        {
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Delta(ManipulationEventArgs e)
        {
            base.Delta(e);
            var thisPoint3D = this.UnProject(e.CurrentPosition, this.panPoint3D, this.Controller.CameraLookDirection);

            if (this.LastPoint3D == null || thisPoint3D == null)
            {
                return;
            }

            Vector3D delta3D = this.LastPoint3D.Value - thisPoint3D.Value;
            this.Pan(delta3D);

            this.LastPoint = e.CurrentPosition;
            this.LastPoint3D = this.UnProject(e.CurrentPosition, this.panPoint3D, this.Controller.CameraLookDirection);
        }

        /// <summary>
        /// Pans the camera by the specified 3D vector (world coordinates).
        /// </summary>
        /// <param name="delta">
        /// The panning vector.
        /// </param>
        public void Pan(Vector3D delta)
        {
            if (!this.Controller.IsPanEnabled)
            {
                return;
            }

            if (this.CameraMode == CameraMode.FixedPosition)
            {
                return;
            }

            this.CameraPosition += delta;
        }

        /// <summary>
        /// Pans the camera by the specified 2D vector (screen coordinates).
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        public void Pan(Vector delta)
        {
            var mousePoint = this.LastPoint + delta;

            var thisPoint3D = this.UnProject(mousePoint, this.panPoint3D, this.Controller.CameraLookDirection);

            if (this.LastPoint3D == null || thisPoint3D == null)
            {
                return;
            }

            Vector3D delta3D = this.LastPoint3D.Value - thisPoint3D.Value;
            this.Pan(delta3D);

            this.LastPoint3D = this.UnProject(mousePoint, this.panPoint3D, this.Controller.CameraLookDirection);

            this.LastPoint = mousePoint;
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Started(ManipulationEventArgs e)
        {
            base.Started(e);
            this.panPoint3D = this.Controller.CameraTarget;
            if (this.MouseDownNearestPoint3D != null)
            {
                this.panPoint3D = this.MouseDownNearestPoint3D.Value;
            }

            this.LastPoint3D = this.UnProject(this.MouseDownPoint, this.panPoint3D, this.Controller.CameraLookDirection);
        }

        /// <summary>
        /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
        /// </summary>
        /// <returns>
        /// True if the execution can continue.
        /// </returns>
        protected override bool CanExecute()
        {
            return this.Controller.IsPanEnabled && this.Controller.CameraMode != CameraMode.FixedPosition;
        }

        /// <summary>
        /// Gets the cursor for the gesture.
        /// </summary>
        /// <returns>
        /// A cursor.
        /// </returns>
        protected override Cursor GetCursor()
        {
            return this.Controller.PanCursor;
        }

        /// <summary>
        /// Called when inertia is starting.
        /// </summary>
        /// <param name="elapsedTime">
        /// The elapsed time (milliseconds).
        /// </param>
        protected override void OnInertiaStarting(int elapsedTime)
        {
            var speed = (this.LastPoint - this.MouseDownPoint) * (40.0 / elapsedTime);
            this.Controller.AddPanForce(speed.X, speed.Y);
        }

    }
}