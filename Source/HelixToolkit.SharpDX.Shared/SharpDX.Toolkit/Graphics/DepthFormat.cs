// Copyright (c) 2010 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Defines the format of data in a depth-stencil buffer.
    /// </summary>
    public enum DepthFormat
    {
        /// <summary>
        /// No depth stencil buffer.
        /// </summary>
        None = 0,

        /// <summary>
        /// A buffer that contains 16-bits of depth data.
        /// </summary>
        /// <msdn-id>bb173059</msdn-id>	
        /// <unmanaged>DXGI_FORMAT_D16_UNORM</unmanaged>	
        /// <unmanaged-short>DXGI_FORMAT_D16_UNORM</unmanaged-short>	
        Depth16 = SharpDX.DXGI.Format.D16_UNorm,

        /// <summary>
        /// A 32 bit buffer that contains 24 bits of depth data and 8 bits of stencil data.
        /// </summary>
        /// <msdn-id>bb173059</msdn-id>	
        /// <unmanaged>DXGI_FORMAT_D24_UNORM_S8_UINT</unmanaged>	
        /// <unmanaged-short>DXGI_FORMAT_D24_UNORM_S8_UINT</unmanaged-short>	
        Depth24Stencil8 = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,

        /// <summary>
        /// A buffer that contains 32-bits of depth data.
        /// </summary>
        /// <msdn-id>bb173059</msdn-id>	
        /// <unmanaged>DXGI_FORMAT_D32_FLOAT</unmanaged>	
        /// <unmanaged-short>DXGI_FORMAT_D32_FLOAT</unmanaged-short>	
        Depth32 = SharpDX.DXGI.Format.D32_Float,

        /// <summary>
        /// A double 32 bit buffer that contains 32 bits of depth data and 8 bits padded with 24 zero bits of stencil data.
        /// </summary>
        /// <msdn-id>bb173059</msdn-id>	
        /// <unmanaged>DXGI_FORMAT_D32_FLOAT_S8X24_UINT</unmanaged>	
        /// <unmanaged-short>DXGI_FORMAT_D32_FLOAT_S8X24_UINT</unmanaged-short>	
        Depth32Stencil8X24 = SharpDX.DXGI.Format.D32_Float_S8X24_UInt,
    }
}
