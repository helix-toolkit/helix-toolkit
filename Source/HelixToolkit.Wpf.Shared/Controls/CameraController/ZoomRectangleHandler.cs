// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomRectangleHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Handles rectangle zooming.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Handles rectangle zooming.
    /// </summary>
    internal class ZoomRectangleHandler : MouseGestureHandler
    {
        /// <summary>
        /// The zoom rectangle.
        /// </summary>
        private Rect zoomRectangle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomRectangleHandler"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public ZoomRectangleHandler(CameraController controller)
            : base(controller)
        {
        }

        /// <summary>
        /// Occurs when the manipulation is completed.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Completed(ManipulationEventArgs e)
        {
            base.Completed(e);
            this.Controller.HideRectangle();
            this.ZoomRectangle(this.zoomRectangle);
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Delta(ManipulationEventArgs e)
        {
            base.Delta(e);

            double ar = this.Controller.ActualHeight / this.Controller.ActualWidth;
            var delta = this.MouseDownPoint - e.CurrentPosition;

            if (Math.Abs(delta.Y / delta.X) < ar)
            {
                delta.Y = Math.Sign(delta.Y) * Math.Abs(delta.X * ar);
            }
            else
            {
                delta.X = Math.Sign(delta.X) * Math.Abs(delta.Y / ar);
            }

            this.zoomRectangle = new Rect(this.MouseDownPoint, this.MouseDownPoint - delta);

            this.Controller.UpdateRectangle(this.zoomRectangle);
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Started(ManipulationEventArgs e)
        {
            base.Started(e);
            this.zoomRectangle = new Rect(this.MouseDownPoint, this.MouseDownPoint);
            this.Controller.ShowRectangle(this.zoomRectangle, Colors.LightGray, Colors.Black);
        }

        /// <summary>
        /// Zooms to the specified rectangle.
        /// </summary>
        /// <param name="rectangle">
        /// The zoom rectangle.
        /// </param>
        public void ZoomRectangle(Rect rectangle)
        {
            if (!this.Controller.IsZoomEnabled)
            {
                return;
            }

            if (rectangle.Width < 10 || rectangle.Height < 10)
            {
                return;
            }

            CameraHelper.ZoomToRectangle(this.Camera, this.Viewport, rectangle);
            this.Controller.OnZoomedByRectangle();
        }

        /// <summary>
        /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
        /// </summary>
        /// <returns>True if the execution can continue.</returns>
        protected override bool CanExecute()
        {
            return this.Controller.IsZoomEnabled;
        }

        /// <summary>
        /// Gets the cursor for the gesture.
        /// </summary>
        /// <returns>A cursor.</returns>
        protected override Cursor GetCursor()
        {
            return this.Controller.ZoomRectangleCursor;
        }

    }
}