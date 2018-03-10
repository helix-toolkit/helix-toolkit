using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
#if !NETFX_CORE
    /// <summary>
    /// Enable dedicated graphics card for rendering. https://stackoverflow.com/questions/17270429/forcing-hardware-accelerated-rendering
    /// </summary>
    public sealed class NVOptimusEnabler
    {
        [System.Runtime.InteropServices.DllImport("nvapi64.dll", EntryPoint = "fake")]
        static extern int LoadNvApi64();

        [System.Runtime.InteropServices.DllImport("nvapi.dll", EntryPoint = "fake")]
        static extern int LoadNvApi32();

        static NVOptimusEnabler()
        {
            try
            {

                if (Environment.Is64BitProcess)
                    LoadNvApi64();
                else
                    LoadNvApi32();
            }
            catch { } // will always fail since 'fake' entry point doesn't exists
        }
    };
#endif
}
