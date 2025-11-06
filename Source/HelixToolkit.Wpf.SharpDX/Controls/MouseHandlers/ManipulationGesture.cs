using System.ComponentModel;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX;

[TypeConverter(typeof(ManipulationGestureConverter))]
public partial class ManipulationGesture : InputGesture
{
    public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
    {
        if (inputEventArgs is ManipulationDeltaEventArgs mdea)
        {
            var manipulatorsCount = mdea.Manipulators.Count();
            return manipulatorsCount == this.FingerCount;
        }

        return false;
    }
}
