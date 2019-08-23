// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionCommand.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides an abstract base class for selection commands.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Provides an abstract base class for mouse selection commands.
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
        /// <param name="eventHandlerModels">The selection event handler for models.</param>
        /// <param name="eventHandlerVisuals">The selection event handler for visuals.</param>
        protected SelectionCommand(Viewport3D viewport, EventHandler<ModelsSelectedEventArgs> eventHandlerModels, EventHandler<VisualsSelectedEventArgs> eventHandlerVisuals)
        {
            this.Viewport = viewport;
            this.ModelsSelected = eventHandlerModels;
            this.VisualsSelected = eventHandlerVisuals;
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
        /// Occurs when visuals are selected.
        /// </summary>
        private event EventHandler<VisualsSelectedEventArgs> VisualsSelected;

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
            this.OnMouseDown(this.Viewport);
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
        }

        /// <summary>
        /// Raises the <see cref="E:ModelsSelected" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ModelsSelectedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnModelsSelected(ModelsSelectedEventArgs e)
        {
            var handler = this.ModelsSelected;
            if (handler != null)
            {
                handler(this.Viewport, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:VisualsSelected" /> event.
        /// </summary>
        /// <param name="e">The <see cref="VisualsSelectedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnVisualsSelected(VisualsSelectedEventArgs e)
        {
            var handler = this.VisualsSelected;
            if (handler != null)
            {
                handler(this.Viewport, e);
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
        protected virtual void OnMouseDown(object sender)
        {
            this.Viewport.MouseMove += this.OnMouseMove;
            this.Viewport.MouseUp += this.OnMouseUp;

            this.Viewport.Focus();
            this.Viewport.CaptureMouse();

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
            e.Handled = true;
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
            e.Handled = true;
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