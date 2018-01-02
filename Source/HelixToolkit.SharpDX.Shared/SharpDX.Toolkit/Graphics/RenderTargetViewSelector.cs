/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Used by <see cref="RenderTarget2D"/> to provide a selector to a <see cref="RenderTargetView"/>.
    /// </summary>
    public sealed class RenderTargetViewSelector
    {
        private readonly Texture texture;

        internal RenderTargetViewSelector(Texture texture)
        {
            this.texture = texture;
        }

        /// <summary>
        /// Gets a specific <see cref="RenderTargetView" /> from this texture.
        /// </summary>
        /// <param name="viewType">Type of the view slice.</param>
        /// <param name="arrayOrDepthSlice">The texture array slice index.</param>
        /// <param name="mipIndex">The mip map slice index.</param>
        /// <returns>An <see cref="RenderTargetView" /></returns>
        public TextureView this[ViewType viewType, int arrayOrDepthSlice, int mipIndex] { get { return this.texture.GetRenderTargetView(viewType, arrayOrDepthSlice, mipIndex); } }
    }
}