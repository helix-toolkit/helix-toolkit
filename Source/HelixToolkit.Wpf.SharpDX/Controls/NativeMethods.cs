using System.Runtime.InteropServices;

namespace HelixToolkit.Wpf.SharpDX;

internal static class NativeMethods
{
    [DllImport("user32.dll", SetLastError = false)]
    private static extern IntPtr GetDesktopWindow();
}
