using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.WinUI.CommonDX
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("63aad0b8-7c24-40ff-85a8-640d944cc325")]
    public interface ISwapChainPanelNative
    {
        [PreserveSig] Result SetSwapChain([In] IntPtr swapChain);
        [PreserveSig] ulong Release();
    }
}
