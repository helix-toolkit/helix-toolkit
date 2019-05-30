// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputBinding.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Binds a <see cref="InputGesture"/> to an <see cref="ICommand"/> implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    using System.Windows.Input;
    using Windows.UI.Xaml;

    /// <summary>
    /// Binds a <see cref="InputGesture"/> to an <see cref="ICommand"/> implementation.
    /// </summary>
    public class InputBinding : DependencyObject
    {
        /// <summary>
        /// Dependency Property for Command property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InputBinding), new PropertyMetadata(null));

        /// <summary>
        /// Dependency Property for Command Parameter
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InputBinding), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property for command target
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(UIElement), typeof(InputBinding), new PropertyMetadata(null));

        private InputGesture gesture;

        /// <summary>
        /// Command Object associated
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// A parameter for the command.
        /// </summary>
        public object CommandParameter
        {
            get => this.GetValue(CommandParameterProperty);
            set => this.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Where the command should be raised.
        /// </summary>
        public UIElement CommandTarget
        {
            get => (UIElement)this.GetValue(CommandTargetProperty);
            set => this.SetValue(CommandTargetProperty, value);
        }

        public virtual InputGesture Gesture
        {
            get => this.gesture;
            set => this.gesture = value;
        }
    }
}
