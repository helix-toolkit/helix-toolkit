// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManipulationGesture.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Defines a touch input gesture that can be used to invoke a command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    using System;
    using Windows.Foundation.Metadata;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// Defines a touch input gesture that can be used to invoke a command.
    /// </summary>
    [CreateFromString(MethodName = "CreateFromString")]
    public class ManipulationGesture : InputGesture
    {
        public ManipulationAction ManipulationAction { get; }

        public int FingerCount { get; }

        public ManipulationGesture(ManipulationAction manipulationAction)
        {
            this.ManipulationAction = manipulationAction;
            this.FingerCount = manipulationAction.FingerCount();
        }

        public override bool Matches(object targetElement, RoutedEventArgs inputEventArgs)
        {
            if (inputEventArgs is ManipulationStartedRoutedEventArgs mdea)
            {                
                var manipulatorsCount = mdea.Container.PointerCaptures.Count;
                return manipulatorsCount == this.FingerCount;
            }

            return false;
        }

        public static ManipulationGesture CreateFromString(string value)
        {
            if (value is string manipulationActionToken)
            {
                manipulationActionToken = manipulationActionToken.Trim();
                var action = ManipulationAction.None;
                if (manipulationActionToken != string.Empty && 
                    !Enum.TryParse(manipulationActionToken, true, out action))
                {
                    return null;
                }

                return new ManipulationGesture(action);
            }

            return null;
        }
    }
}