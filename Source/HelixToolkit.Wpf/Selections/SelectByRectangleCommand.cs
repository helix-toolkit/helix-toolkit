// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectByRectangleCommand.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   The select by rectangle command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Selections
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// The select by rectangle command.
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
        /// Initializes a new instance of the <see cref="SelectByRectangleCommand"/> class.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <param name="selectedHandler">
        /// The selected Handler.
        /// </param>
        public SelectByRectangleCommand(Viewport3D viewport, SelectionHitMode mode, EventHandler<RangeSelectionEventArgs> selectedHandler)
            : base(viewport, mode)
        {
            this.ModelsSelected += selectedHandler;
        }

        /// <summary>
        /// The customized complete operation when the manipulation is completed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        public override void CompletedImpl(ManipulationEventArgs e)
        {
            base.CompletedImpl(e);
            this.HideRectangle();
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Delta(ManipulationEventArgs e)
        {
            base.Delta(e);
            this.selectionRect = new Rect(this.MouseDownPoint, e.CurrentPosition);
            this.UpdateRectangle();
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        public override void Started(ManipulationEventArgs e)
        {
            base.Started(e);
            this.selectionRect = new Rect(this.MouseDownPoint, this.MouseDownPoint);
            this.ShowRectangle();
        }

        /// <summary>
        /// Select the models.
        /// </summary>
        public override void SelectModels()
        {
            this.SelectedModels = this.Viewport.FindHits(this.selectionRect, this.LastPoint, this.SelectionHitMode).Models;
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
        /// Hides the rectangle.
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
        /// Updates the rectangle.
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
        /// Shows the rectangle.
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
