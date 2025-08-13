using Avalonia.Input;
using Avalonia.Interactivity;
using System.ComponentModel;

namespace HelixToolkit.Avalonia.SharpDX;

[TypeConverter(typeof(ManipulationGestureConverter))]
public partial class ManipulationGesture : InputGesture
{
    public override bool Matches(object targetElement, RoutedEventArgs inputEventArgs)
    {
        if (inputEventArgs is PointerEventArgs mdea)
        {
            // mdea.Container.PointerCaptures.Count
            var manipulatorsCount = mdea.Pointer.Captured is null ? 0 : 1;
            return manipulatorsCount == this.FingerCount;
        }

        return false;
    }
}
