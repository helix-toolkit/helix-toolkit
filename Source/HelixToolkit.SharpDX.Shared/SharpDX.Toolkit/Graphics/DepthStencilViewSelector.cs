/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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