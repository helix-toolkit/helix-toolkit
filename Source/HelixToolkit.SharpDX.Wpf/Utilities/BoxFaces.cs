namespace HelixToolkit.SharpDX.Wpf
{
    using System;

    [Flags]
    public enum BoxFaces
    {
        /// <summary>
        ///   The top.
        /// </summary>
        Top = 0x1,

        /// <summary>
        ///   The bottom.
        /// </summary>
        Bottom = 0x2,

        /// <summary>
        ///   The left side.
        /// </summary>
        Left = 0x4,

        /// <summary>
        ///   The right side.
        /// </summary>
        Right = 0x8,

        /// <summary>
        ///   The front side.
        /// </summary>
        Front = 0x10,

        /// <summary>
        ///   The back side.
        /// </summary>
        Back = 0x20,

        /// <summary>
        ///   All sides.
        /// </summary>
        All = Top | Bottom | Left | Right | Front | Back
    }
}