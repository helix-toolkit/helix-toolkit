/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;

namespace SharpDX.Toolkit.Graphics
{
    [Flags]
    internal enum WICFlags
    {
        None = 0x0,

        ForceRgb = 0x1,
        // Loads DXGI 1.1 BGR formats as DXGI_FORMAT_R8G8B8A8_UNORM to avoid use of optional WDDM 1.1 formats

        NoX2Bias = 0x2,
        // Loads DXGI 1.1 X2 10:10:10:2 format as DXGI_FORMAT_R10G10B10A2_UNORM

        No16Bpp = 0x4,
        // Loads 565, 5551, and 4444 formats as 8888 to avoid use of optional WDDM 1.2 formats

        FlagsAllowMono = 0x8,
        // Loads 1-bit monochrome (black & white) as R1_UNORM rather than 8-bit greyscale

        AllFrames = 0x10,
        // Loads all images in a multi-frame file, converting/resizing to match the first frame as needed, defaults to 0th frame otherwise

        Dither = 0x10000,
        // Use ordered 4x4 dithering for any required conversions

        DitherDiffusion = 0x20000,
        // Use error-diffusion dithering for any required conversions

        FilterPoint = 0x100000,
        FilterLinear = 0x200000,
        FilterCubic = 0x300000,
        FilterFant = 0x400000, // Combination of Linear and Box filter
        // Filtering mode to use for any required image resizing (only needed when loading arrays of differently sized images; defaults to Fant)
    };
}