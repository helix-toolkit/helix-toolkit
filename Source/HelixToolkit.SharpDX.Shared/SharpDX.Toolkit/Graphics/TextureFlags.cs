/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Specifies usage of a texture.
    /// </summary>
    [Flags]
    public enum TextureFlags
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// The texture will be used as a <see cref="SharpDX.Direct3D11.ShaderResourceView"/>.
        /// </summary>
        ShaderResource = 1,

        /// <summary>
        /// The texture will be used as a <see cref="SharpDX.Direct3D11.RenderTargetView"/>.
        /// </summary>
        RenderTarget = 2,

        /// <summary>
        /// The texture will be used as an <see cref="SharpDX.Direct3D11.UnorderedAccessView"/>.
        /// </summary>
        UnorderedAccess = 4,
    }
}