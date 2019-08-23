// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManipulationBinding.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Binds a <see cref="ManipulationGesture"/> to an <see cref="ICommand"/> implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    using System.Windows.Input;
    using Windows.Foundation.Metadata;

    /// <summary>
    /// Binds a <see cref="ManipulationGesture"/> to an <see cref="ICommand"/> implementation.
    /// </summary>
    public class ManipulationBinding : InputBinding
    {
        /// <summary>
        /// Gets the finger count.
        /// </summary>
        public int FingerCount => this.Gesture.FingerCount;

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <remarks>
        /// Makes it possible to assign a string to this property in XAML via <see cref="CreateFromStringAttribute"/>.
        /// </remarks>
        public new ManipulationGesture Gesture
        {
            get => (ManipulationGesture)base.Gesture;
            set => base.Gesture = value;
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <remarks>
        /// Makes it possible to assign a string to this property in XAML via <see cref="CreateFromStringAttribute"/>.
        /// Another way would be the use of <see cref="ViewportCommandExtension"/>, but it 
        /// requires Min Target Platform Version "Windows 10 Fall Creators Update (introduced v10.0.16299.0)".
        /// </remarks>
        public new ViewportCommand Command
        {
            get => (ViewportCommand)base.Command;
            set => base.Command = value;
        }
    }
}