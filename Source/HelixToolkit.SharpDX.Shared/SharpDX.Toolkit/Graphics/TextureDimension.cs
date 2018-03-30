/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Defines the dimension of a texture.
    /// </summary>
    public enum TextureDimension
    {
        /// <summary>
        /// The texture dimension is 1D.
        /// </summary>
        Texture1D,

        /// <summary>
        /// The texture dimension is 2D.
        /// </summary>
        Texture2D,

        /// <summary>
        /// The texture dimension is 3D.
        /// </summary>
        Texture3D,

        /// <summary>
        /// The texture dimension is a CubeMap.
        /// </summary>
        TextureCube,
    }
}