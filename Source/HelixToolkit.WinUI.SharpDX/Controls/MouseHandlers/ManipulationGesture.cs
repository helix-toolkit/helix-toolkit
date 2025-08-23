using Windows.Foundation.Metadata;

namespace HelixToolkit.WinUI.SharpDX;

[CreateFromString(MethodName = "CreateFromString")]
public partial class ManipulationGesture : InputGesture
{
    public override bool Matches(object targetElement, RoutedEventArgs inputEventArgs)
    {
        if (inputEventArgs is ManipulationStartedRoutedEventArgs mdea)
        {
            var manipulatorsCount = mdea.Container.PointerCaptures.Count;
            return manipulatorsCount == this.FingerCount;
        }

        return false;
    }

    public static ManipulationGesture? CreateFromString(string value)
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
