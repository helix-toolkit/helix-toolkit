/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Flags used by <see cref="DDSHelper.LoadFromDDSMemory"/>.
    /// </summary>
    [Flags]
    internal enum DDSFlags
    {
        None = 0x0,
        LegacyDword = 0x1, // Assume pitch is DWORD aligned instead of BYTE aligned (used by some legacy DDS files)
        NoLegacyExpansion = 0x2, // Do not implicitly convert legacy formats that result in larger pixel sizes (24 bpp, 3:3:2, A8L8, A4L4, P8, A8P8) 
        NoR10B10G10A2Fixup = 0x4, // Do not use work-around for long-standing D3DX DDS file format issue which reversed the 10:10:10:2 color order masks
        ForceRgb = 0x8, // Convert DXGI 1.1 BGR formats to Format.R8G8B8A8_UNorm to avoid use of optional WDDM 1.1 formats
        No16Bpp = 0x10, // Conversions avoid use of 565, 5551, and 4444 formats and instead expand to 8888 to avoid use of optional WDDM 1.2 formats
        CopyMemory = 0x20, // The content of the memory passed to the DDS Loader is copied to another internal buffer.
        ForceDX10Ext = 0x10000, // Always use the 'DX10' header extension for DDS writer (i.e. don't try to write DX9 compatible DDS files)
    };
}