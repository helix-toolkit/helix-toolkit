// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectByRectangleCommand.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   Provides a command that shows a rectangle when the mouse is dragged and raises an event returning the models contained in the rectangle
//   when the mouse button is released.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides a command that shows a rectangle when the mouse is dragged and raises an event returning the models contained in the rectangle
    /// when the mouse button is released.
    /// </summary>
    public class SelectByRectangleCommand : SelectionCommand
    {
        /// <summary>
        /// The selection rectangle.
        /// </summary>
        private Rect selectionRect;

        /// <summary>
        /// The rectangle adorner.
        /// </summary>
        private RectangleAdorner rectangleAdorner;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectByRectangleCommand" /> class.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="eventHandler">The selection event handler.</param>
        public SelectByRectangleCommand(Viewport3D viewport, EventHandler<ModelsSelectedEventArgs> eventHandler)
            : base(viewport, eventHandler)
        {
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        protected override void Started(ManipulationEventArgs e)
        {
            base.Started(e);
            this.selectionRect = new Rect(this.MouseDownPoint, this.MouseDownPoint);
            this.ShowRectangle();
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        protected override void Delta(ManipulationEventArgs e)
        {
            base.Delta(e);
            this.selectionRect = new Rect(this.MouseDownPoint, e.CurrentPosition);
            this.UpdateRectangle();
        }

        /// <summary>
        /// The customized complete operation when the manipulation is completed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        protected override void Completed(ManipulationEventArgs e)
        {
            this.HideRectangle();
            base.Completed(e);
        }

        /// <summary>
        /// Gets the selected models.
        /// </summary>
        /// <returns>The selected models.</returns>
        protected override IList<Model3D> GetSelectedModels()
        {
            return this.Viewport.FindHits(this.selectionRect, this.SelectionHitMode).Select(hit => hit.Model).ToList();
        }

        /// <summary>
        /// Gets the cursor for the gesture.
        /// </summary>
        /// <returns>
        /// A cursor.
        /// </returns>
        protected override Cursor GetCursor()
        {
            return Cursors.Arrow;
        }

        /// <summary>
        /// Hides the selection rectangle.
        /// </summary>
        private void HideRectangle()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            if (this.rectangleAdorner != null)
            {
                myAdornerLayer.Remove(this.rectangleAdorner);
            }

            this.rectangleAdorner = null;

            this.Viewport.InvalidateVisual();
        }

        /// <summary>
        /// Updates the selection rectangle.
        /// </summary>
        private void UpdateRectangle()
        {
            if (this.rectangleAdorner == null)
            {
                return;
            }

            this.rectangleAdorner.Rectangle = this.selectionRect;
            this.rectangleAdorner.InvalidateVisual();
        }

        /// <summary>
        /// Shows the selection rectangle.
        /// </summary>
        private void ShowRectangle()
        {
            if (this.rectangleAdorner != null)
            {
                return;
            }

            var adornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            this.rectangleAdorner = new RectangleAdorner(this.Viewport, this.selectionRect, Colors.LightGray, Colors.Black, 1, 1, 0, DashStyles.Dash);
            adornerLayer.Add(this.rectangleAdorner);
        }
    }
}
