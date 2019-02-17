// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManipulationGesture.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Defines a touch input gesture that can be used to invoke a command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Defines a touch input gesture that can be used to invoke a command.
    /// </summary>
    [TypeConverter(typeof(ManipulationGestureConverter))]
    public class ManipulationGesture : InputGesture
    {
        public ManipulationAction ManipulationAction { get; }

        public int FingerCount { get; }

        public ManipulationGesture(ManipulationAction manipulationAction)
        {
            this.ManipulationAction = manipulationAction;
            this.FingerCount = manipulationAction.FingerCount();
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (inputEventArgs is ManipulationDeltaEventArgs mdea)
            {
                // mdea.CumulativeManipulation.Translation.Length ...
                var manipulatorsCount = mdea.Manipulators.Count();
                return manipulatorsCount == this.FingerCount;
            }

            return false;
        }
    }
}