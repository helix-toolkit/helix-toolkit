/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Used by <see cref="Texture"/> to provide a selector to a <see cref="UnorderedAccessView"/>.
    /// </summary>
    public sealed class UnorderedAccessViewSelector
    {
        private readonly Texture texture;

        internal UnorderedAccessViewSelector(Texture thisTexture)
        {
            this.texture = thisTexture;
        }

        /// <summary>
        /// Gets a specific <see cref="UnorderedAccessView" /> from this texture.
        /// </summary>
        /// <param name="arrayOrDepthSlice">The texture array slice index.</param>
        /// <param name="mipIndex">The mip map slice index.</param>
        /// <returns>An <see cref="UnorderedAccessView" /></returns>
        public UnorderedAccessView this[int arrayOrDepthSlice, int mipIndex] { get { return this.texture.GetUnorderedAccessView(arrayOrDepthSlice, mipIndex); } }
    }
}