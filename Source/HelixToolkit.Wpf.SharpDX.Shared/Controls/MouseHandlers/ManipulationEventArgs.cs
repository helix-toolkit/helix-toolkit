// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManipulationEventArgs.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides data for the manipulation events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// Provides data for the manipulation events.
    /// </summary>
    public sealed class ManipulationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManipulationEventArgs"/> class.
        /// </summary>
        /// <param name="currentPosition">
        /// The current position.
        /// </param>
        public ManipulationEventArgs(Point currentPosition)
        {
            this.CurrentPosition = currentPosition;
        }

        /// <summary>
        /// Gets the current position.
        /// </summary>
        /// <value>The current position.</value>
        public Point CurrentPosition
        {
            get; private set;
        }
    }
}