namespace HelixToolkit.WinUI.SharpDX;

public static class ManipulationActionExtensions
{
    public static int FingerCount(this ManipulationAction manipulationAction)
    {
        return manipulationAction switch
        {
            ManipulationAction.Pan => 1,
            ManipulationAction.Pinch or ManipulationAction.TwoFingerPan => 2,
            ManipulationAction.ThreeFingerPan => 3,
            _ => 0,
        };
    }
}
