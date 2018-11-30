using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities
    {
    #if !NETFX_CORE
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
                        NativeMethods.LoadNvApi64();
                    else
                        NativeMethods.LoadNvApi32();
                }
                catch { } // will always fail since 'fake' entry point doesn't exists
            }
        };

        internal static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("nvapi64.dll", EntryPoint = "fake")]
            internal static extern int LoadNvApi64();

            [System.Runtime.InteropServices.DllImport("nvapi.dll", EntryPoint = "fake")]
            internal static extern int LoadNvApi32();
        }
    #endif
    }

}
