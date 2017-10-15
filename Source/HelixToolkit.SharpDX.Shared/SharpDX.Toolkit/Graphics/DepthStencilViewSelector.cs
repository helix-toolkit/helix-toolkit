// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
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

using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Used by <see cref="DepthStencilBuffer"/> to provide a selector to a <see cref="DepthStencilView"/>.
    /// </summary>
    public sealed class DepthStencilViewSelector
    {
        private readonly DepthStencilBuffer texture;

        internal DepthStencilViewSelector(DepthStencilBuffer texture)
        {
            this.texture = texture;
        }

        /// <summary>
        /// Gets a specific <see cref="DepthStencilView" /> from this texture.
        /// </summary>
        /// <param name="viewType">Type of the view slice.</param>
        /// <param name="arrayOrDepthSlice">The texture array slice index.</param>
        /// <param name="mipIndex">The mip map slice index.</param>
        /// <param name="readOnlyView">Indicates if this view is read-only.</param>
        /// <returns>An <see cref="DepthStencilView" /></returns>
        public TextureView this[ViewType viewType, int arrayOrDepthSlice, int mipIndex, bool readOnlyView = false] { get { return this.texture.GetDepthStencilView(viewType, arrayOrDepthSlice, mipIndex, readOnlyView); } }
    }
}