// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionCommand.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   The basic class of selection command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// The basic class of selection command.
    /// </summary>
    public abstract class SelectionCommand : ICommand
    {
        /// <summary>
        /// The viewport of the command.
        /// </summary>
        protected readonly Viewport3D Viewport;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionCommand"/> class.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="mode">
        /// The mode of selection.
        /// </param>
        protected SelectionCommand(Viewport3D viewport, SelectionHitMode mode)
        {
            this.Viewport = viewport;
            this.SelectionHitMode = mode;
            this.SpinReleaseTime = 200;
            this.ManipulationWatch = new Stopwatch();
        }

        /// <summary>
        /// The event occurs after the models are selected.
        /// </summary>
        public event EventHandler<RangeSelectionEventArgs> ModelsSelected;

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets or sets the selection hit mode.
        /// </summary>
        public SelectionHitMode SelectionHitMode { get; set; }

        /// <summary>
        /// Gets or sets the max duration of mouse drag to activate spin.
        /// </summary>
        /// <remarks>
        /// If the time between mouse down and mouse up is less than this value, spin is activated.
        /// </remarks>
        public int SpinReleaseTime { get; set; }

        /// <summary>
        /// Gets or sets the mouse down point (2D screen coordinates).
        /// </summary>
        protected Point MouseDownPoint { get; set; }

        /// <summary>
        /// Gets or sets the last point (2D screen coordinates).
        /// </summary>
        protected Point LastPoint { get; set; }

        /// <summary>
        /// Gets or sets the manipulation watch.
        /// </summary>
        protected Stopwatch ManipulationWatch { get; set; }

        /// <summary>
        /// Gets or sets the selected models.
        /// </summary>
        protected IList<Model3D> SelectedModels { get; set; }

        /// <summary>
        /// Gets or sets the old cursor.
        /// </summary>
        private Cursor OldCursor { get; set; }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        public virtual void Delta(ManipulationEventArgs e)
        {
        }

        /// <summary>
        /// The customized complete operation when the manipulation is completed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        public virtual void CompletedImpl(ManipulationEventArgs e)
        {
        }

        /// <summary>
        /// Select the models.
        /// </summary>
        public abstract void SelectModels();

        /// <summary>
        /// Occurs when the manipulation is completed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        public void Completed(ManipulationEventArgs e)
        {
            var elapsed = this.ManipulationWatch.ElapsedMilliseconds;
            if (elapsed > 0 && elapsed < this.SpinReleaseTime)
            {
                this.OnInertiaStarting((int)this.ManipulationWatch.ElapsedMilliseconds);
            }

            this.CompletedImpl(e);
            this.LastPoint = e.CurrentPosition;
            this.SelectedModels = new List<Model3D>();
            this.SelectModels();

            if (this.ModelsSelected != null)
            {
                this.ModelsSelected(this.Viewport, new RangeSelectionEventArgs(this.SelectedModels, this.MouseDownPoint, this.LastPoint));
            }
        }
        
        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        public virtual void Started(ManipulationEventArgs e)
        {
            this.MouseDownPoint = e.CurrentPosition;
            this.LastPoint = this.MouseDownPoint;
            this.ManipulationWatch.Restart();
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            this.Viewport.MouseMove += this.OnMouseMove;
            this.Viewport.MouseUp += this.OnMouseUp;

            this.OnMouseDown(this.Viewport, null);
            this.Viewport.Focus();
            this.Viewport.CaptureMouse();
        }

        /// <summary>
        /// Check whether the command can executed.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// True if the command can be executed. Otherwise, it returns false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Called when inertia is starting.
        /// </summary>
        /// <param name="elapsedTime">
        /// The elapsed time (milliseconds).
        /// </param>
        protected virtual void OnInertiaStarting(int elapsedTime)
        {
        }

        /// <summary>
        /// Gets the cursor for the gesture.
        /// </summary>
        /// <returns>
        /// A cursor.
        /// </returns>
        protected abstract Cursor GetCursor();
        
        /// <summary>
        /// Called when the mouse button is pressed down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.Started(new ManipulationEventArgs(Mouse.GetPosition(this.Viewport)));

            this.OldCursor = this.Viewport.Cursor;
            this.Viewport.Cursor = this.GetCursor();
        }

        /// <summary>
        /// Called when the mouse button is released.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Viewport.MouseMove -= this.OnMouseMove;
            this.Viewport.MouseUp -= this.OnMouseUp;
            this.Viewport.ReleaseMouseCapture();
            this.Viewport.Cursor = this.OldCursor;
            this.Completed(new ManipulationEventArgs(Mouse.GetPosition(this.Viewport)));
        }

        /// <summary>
        /// Called when the mouse is move on the control.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMouseMove(object sender, MouseEventArgs e)
        {
            this.Delta(new ManipulationEventArgs(Mouse.GetPosition(this.Viewport)));
        }

        /// <summary>
        /// Called when the condition of execution is changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnCanExecutedChanged(object sender, EventArgs e)
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(sender, e);
            }
        }
    }
}
