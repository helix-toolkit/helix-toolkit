using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            NVOptimusEnabler nvEnabler = new NVOptimusEnabler();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RenderForm form = new RenderForm();
            CoreTestApp app = new CoreTestApp(form);
            Application.Run(form);
        }
    }

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
}
