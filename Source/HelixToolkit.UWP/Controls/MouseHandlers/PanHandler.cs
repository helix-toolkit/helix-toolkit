// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PanHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Handles panning.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    using SharpDX;
    using Point3D = SharpDX.Vector3;
    using Vector3D = SharpDX.Vector3;
    using Point = Windows.Foundation.Point;
    using Windows.UI.Core;

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
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        public PanHandler(Viewport3DX viewport)
            : base(viewport)
        {
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="Point"/> instance containing the event data.</param>
        public override void Delta(Point e)
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
        public void Pan(Vector3D delta)
        {
            if (!this.Viewport.IsPanEnabled)
            {
                return;
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
        public void Pan(Vector2 delta)
        {
            var mousePoint = (this.LastPoint.ToVector2() + delta).ToPoint();

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
        /// <param name="e">The <see cref="Point"/> instance containing the event data.</param>
        public override void Started(Point e)
        {
            base.Started(e);
            this.panPoint3D = this.Camera.Target;
            if (this.MouseDownNearestPoint3D != null)
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
        protected override bool CanExecute()
        {
            return this.Viewport.IsPanEnabled && this.Viewport.CameraMode != CameraMode.FixedPosition;
        }

        /// <summary>
        /// Gets the cursor for the gesture.
        /// </summary>
        /// <returns>
        /// A cursor.
        /// </returns>
        protected override CoreCursorType GetCursor()
        {
            return this.Viewport.PanCursor;
        }

        /// <summary>
        /// Called when inertia is starting.
        /// </summary>
        /// <param name="elapsedTime">
        /// The elapsed time (milliseconds).
        /// </param>
        protected override void OnInertiaStarting(double elapsedTime)
        {
            var speed = (this.LastPoint.ToVector2() - this.MouseDownPoint.ToVector2()) * (40.0f / (float)elapsedTime);
            this.Viewport.AddPanForce(speed.X, speed.Y);
        }
    }
}