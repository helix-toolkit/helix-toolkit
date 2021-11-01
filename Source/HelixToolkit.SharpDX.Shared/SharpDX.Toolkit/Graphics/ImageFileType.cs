/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Image file format
    /// </summary>
    public enum ImageFileType
    {
        /// <summary>
        /// A DDS file.
        /// </summary>
        Dds,

        /// <summary>
        /// A PNG file.
        /// </summary>
        Png,

        /// <summary>
        /// A GIF file.
        /// </summary>
        Gif,

        /// <summary>
        /// A JPG file.
        /// </summary>
        Jpg,

        /// <summary>
        /// A BMP file.
        /// </summary>
        Bmp,

        /// <summary>
        /// A TIFF file.
        /// </summary>
        Tiff,

        /// <summary>
        /// A WMP file.
        /// </summary>
        Wmp,

        /// <summary>
        /// A TGA File.
        /// </summary>
        Tga,

        /// <summary>
        /// A TKTX File.
        /// </summary>
        /// <remarks>
        /// This is a format available with this toolkit, similar to DDS, but It doesn't require any conversion and is a straight dump of the memory pixel buffers.
        /// </remarks>
        Tktx,
    }
}