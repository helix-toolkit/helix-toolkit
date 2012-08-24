// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManipulationEventArgs.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;

    /// <summary>
    /// Provides data for the manipulation events.
    /// </summary>
    public class ManipulationEventArgs
    {
        #region Constructors and Destructors

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

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the current position.
        /// </summary>
        /// <value>The current position.</value>
        public Point CurrentPosition { get; private set; }

        #endregion
    }
}