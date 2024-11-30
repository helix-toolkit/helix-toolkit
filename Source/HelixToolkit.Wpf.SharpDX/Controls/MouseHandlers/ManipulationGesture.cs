using System.ComponentModel;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX;

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
