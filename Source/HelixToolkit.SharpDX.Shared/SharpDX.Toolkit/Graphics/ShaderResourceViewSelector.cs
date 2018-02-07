/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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