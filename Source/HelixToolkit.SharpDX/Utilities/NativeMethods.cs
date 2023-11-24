namespace HelixToolkit.SharpDX.Utilities;

internal static class NativeMethods
{
    [System.Runtime.InteropServices.DllImport("nvapi64.dll", EntryPoint = "fake")]
    internal static extern int LoadNvApi64();

    [System.Runtime.InteropServices.DllImport("nvapi.dll", EntryPoint = "fake")]
    internal static extern int LoadNvApi32();
}
