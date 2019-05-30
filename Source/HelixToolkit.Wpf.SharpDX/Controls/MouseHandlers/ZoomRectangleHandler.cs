// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomRectangleHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Handles rectangle zooming.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows;
    using System.Windows.Input;

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
        /// The camera controller.
        /// </param>
        public ZoomRectangleHandler(CameraController controller)
            : base(controller)
        {
        }

        /// <summary>
        /// Occurs when the manipulation is completed.
        /// </summary>
        /// <param name="e">The <see cref="Point"/> instance containing the event data.</param>
        public override void Completed(Point e)
        {
            base.Completed(e);
            this.Viewport.HideZoomRectangle();
            this.ZoomRectangle(this.zoomRectangle);
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="Point"/> instance containing the event data.</param>
        public override void Delta(Point e)
        {
            base.Delta(e);

            double ar = this.Viewport.ActualHeight / this.Viewport.ActualWidth;
            var delta = this.MouseDownPoint - e;

            if (Math.Abs(delta.Y / delta.X) < ar)
            {
                delta.Y = Math.Sign(delta.Y) * Math.Abs(delta.X * ar);
            }
            else
            {
                delta.X = Math.Sign(delta.X) * Math.Abs(delta.Y / ar);
            }

            this.zoomRectangle = new Rect(this.MouseDownPoint, this.MouseDownPoint - delta);

            this.Viewport.ShowZoomRectangle(this.zoomRectangle);
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="Point"/> instance containing the event data.</param>
        public override void Started(Point e)
        {
            base.Started(e);
        }

        /// <summary>
        /// Zooms to the specified rectangle.
        /// </summary>
        /// <param name="rectangle">
        /// The zoom rectangle.
        /// </param>
        public void ZoomRectangle(Rect rectangle)
        {
            if (!this.Viewport.IsZoomEnabled)
            {
                return;
            }

            if (rectangle.Width < 10 || rectangle.Height < 10)
            {
                return;
            }
            this.Viewport.ZoomToRectangle(rectangle);
        }

        /// <summary>
        /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
        /// </summary>
        /// <returns>True if the execution can continue.</returns>
        protected override bool CanExecute()
        {
            return this.Viewport.IsZoomEnabled;
        }

        /// <summary>
        /// Gets the cursor for the gesture.
        /// </summary>
        /// <returns>A cursor.</returns>
        protected override Cursor GetCursor()
        {
            return this.Viewport.ZoomRectangleCursor;
        }
    }
}