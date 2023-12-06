namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// Enable dedicated graphics card for rendering. https://stackoverflow.com/questions/17270429/forcing-hardware-accelerated-rendering
/// </summary>
public sealed class NVOptimusEnabler
{
    static NVOptimusEnabler()
    {
        try
        {

            if (Environment.Is64BitProcess)
            {
                NativeMethods.LoadNvApi64();
            }
            else
            {
                NativeMethods.LoadNvApi32();
            }
        }
        catch { } // will always fail since 'fake' entry point doesn't exists
    }
};
