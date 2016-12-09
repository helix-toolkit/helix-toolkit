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

using System;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Used by <see cref="Texture"/> to provide a selector to a <see cref="ShaderResourceView"/>.
    /// </summary>
    public sealed class ShaderResourceViewSelector
    {
        private readonly Texture texture;

        internal ShaderResourceViewSelector(Texture thisTexture)
        {
            this.texture = thisTexture;
        }

        /// <summary>
        /// Gets a specific <see cref="ShaderResourceView" /> from this texture.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="arrayOrDepthSlice">The array or depth slice.</param>
        /// <param name="mipIndex">Index of the mip.</param>
        /// <returns>An <see cref="ShaderResourceView" /></returns>
        public TextureView this[ViewType viewType, int arrayOrDepthSlice, int mipIndex]
        {
            get
            {
                if(FormatHelper.IsTypeless(texture.Format))
                {
                    throw new InvalidOperationException(string.Format("Cannot create a SRV on a TypeLess texture format [{0}]", texture.Format));
                }
                return texture.GetShaderResourceView(texture.Format, viewType, arrayOrDepthSlice, mipIndex);
            }
        }

        /// <summary>
        /// Gets a specific <see cref="ShaderResourceView" /> from this texture.
        /// </summary>
        /// <param name="viewFormat">The view format.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="arrayOrDepthSlice">The array or depth slice.</param>
        /// <param name="mipIndex">Index of the mip.</param>
        /// <returns>An <see cref="ShaderResourceView" /></returns>
        public TextureView this[DXGI.Format viewFormat, ViewType viewType, int arrayOrDepthSlice, int mipIndex] { get { return this.texture.GetShaderResourceView(viewFormat, viewType, arrayOrDepthSlice, mipIndex); } }
    }
}