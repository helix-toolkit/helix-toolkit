// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionCommand.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   Provides an abstract base class for selection commands.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides an abstract base class for selection commands.
    /// </summary>
    public abstract class SelectionCommand : ICommand
    {
        /// <summary>
        /// The viewport of the command.
        /// </summary>
        protected readonly Viewport3D Viewport;

        /// <summary>
        /// Keeps track of the old cursor.
        /// </summary>
        private Cursor oldCursor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionCommand"/> class.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="eventHandler">The selection event handler.</param>
        protected SelectionCommand(Viewport3D viewport, EventHandler<ModelsSelectedEventArgs> eventHandler)
        {
            this.Viewport = viewport;
            this.ModelsSelected = eventHandler;
        }

        /// <summary>
        /// Occurs when <see cref="CanExecute" /> is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Occurs when models are selected.
        /// </summary>
        private event EventHandler<ModelsSelectedEventArgs> ModelsSelected;

        /// <summary>
        /// Gets or sets the selection hit mode.
        /// </summary>
        public SelectionHitMode SelectionHitMode { get; set; }

        /// <summary>
        /// Gets the mouse down point (2D screen coordinates).
        /// </summary>
        protected Point MouseDownPoint { get; private set; }

        /// <summary>
        /// Executes the command.
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
        /// Checks whether the command can be executed.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// <c>true</c> if the command can be executed. Otherwise, it returns <c>false</c>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Gets the selected models.
        /// </summary>
        /// <returns>The selected models.</returns>
        protected abstract IList<Model3D> GetSelectedModels();

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void Started(ManipulationEventArgs e)
        {
            this.MouseDownPoint = e.CurrentPosition;
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void Delta(ManipulationEventArgs e)
        {
        }

        /// <summary>
        /// Occurs when the manipulation is completed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void Completed(ManipulationEventArgs e)
        {
            var selectedModels = this.GetSelectedModels();

            var handler = this.ModelsSelected;
            if (handler != null)
            {
                handler(this.Viewport, new ModelsSelectedEventArgs(selectedModels));
            }
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

            this.oldCursor = this.Viewport.Cursor;
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
            this.Viewport.Cursor = this.oldCursor;
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
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
