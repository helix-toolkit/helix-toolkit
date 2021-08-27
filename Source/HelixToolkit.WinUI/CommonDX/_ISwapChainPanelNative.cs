using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.WinUI.CommonDX
{
    [ComImport, Guid("63aad0b8-7c24-40ff-85a8-640d944cc325"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISwapChainPanelNative
    {
        [PreserveSig]
        long SetSwapChain(IDXGISwapChain swapChain);
    }


    [ComImport, Guid("310d36a0-d2e7-4c0a-aa04-6a9d23b8886a"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IDXGISwapChain
    {
        // IDXGIObject
        [PreserveSig]
        long SetPrivateData(/* [annotation][in] _In_ */ [MarshalAs(UnmanagedType.LPStruct)] Guid Name, /* [in] */ uint DataSize, /* [annotation][in] _In_reads_bytes_(DataSize) */ IntPtr pData);

        [PreserveSig]
        long SetPrivateDataInterface(/* [annotation][in] _In_ */ [MarshalAs(UnmanagedType.LPStruct)] Guid Name, /* optional(IUnknown) */ IntPtr pUnknown);

        [PreserveSig]
        long GetPrivateData(/* [annotation][in] _In_ */ [MarshalAs(UnmanagedType.LPStruct)] Guid Name, /* [annotation][out][in] _Inout_ */ ref uint pDataSize, /* [annotation][out] _Out_writes_bytes_(*pDataSize) */ IntPtr pData);

        [PreserveSig]
        long GetParent(/* [annotation][in] _In_ */ [MarshalAs(UnmanagedType.LPStruct)] Guid riid, /* [annotation][retval][out] _COM_Outptr_ */ [MarshalAs(UnmanagedType.IUnknown)] out object ppParent);

        // IDXGIDeviceSubObject
        [PreserveSig]
        long GetDevice(/* [annotation][in] _In_ */ [MarshalAs(UnmanagedType.LPStruct)] Guid riid, /* [annotation][retval][out] _COM_Outptr_ */ [MarshalAs(UnmanagedType.IUnknown)] out object ppDevice);

        // IDXGISwapChain
        [PreserveSig]
        long Present(/* [in] */ uint SyncInterval, /* [in] */ uint Flags);

        [PreserveSig]
        long GetBuffer(/* [in] */ uint Buffer, /* [annotation][in] _In_ */ [MarshalAs(UnmanagedType.LPStruct)] Guid riid, /* [annotation][out][in] _COM_Outptr_ */ [MarshalAs(UnmanagedType.IUnknown)] out object ppSurface);

        [PreserveSig]
        long SetFullscreenState(/* [in] */ bool Fullscreen, /* [annotation][in] _In_opt_ */ IDXGIOutput pTarget);

        [PreserveSig]
        long GetFullscreenState(/* optional(BOOL) */ IntPtr pFullscreen, /* [annotation][out] _COM_Outptr_opt_result_maybenull_ */ out IDXGIOutput ppTarget);

        [PreserveSig]
        long GetDesc(/* [annotation][out] _Out_ */ out DXGI_SWAP_CHAIN_DESC pDesc);

        [PreserveSig]
        long ResizeBuffers(/* [in] */ uint BufferCount, /* [in] */ uint Width, /* [in] */ uint Height, /* [in] */ DXGI_FORMAT NewFormat, /* [in] */ uint SwapChainFlags);

        [PreserveSig]
        long ResizeTarget(/* [annotation][in] _In_ */ ref DXGI_MODE_DESC pNewTargetParameters);

        [PreserveSig]
        long GetContainingOutput(/* [annotation][out] _COM_Outptr_ */ out IDXGIOutput ppOutput);

        [PreserveSig]
        long GetFrameStatistics(/* [annotation][out] _Out_ */ out DXGI_FRAME_STATISTICS pStats);

        [PreserveSig]
        long GetLastPresentCount(/* [annotation][out] _Out_ */ out uint pLastPresentCount);
    }
}
