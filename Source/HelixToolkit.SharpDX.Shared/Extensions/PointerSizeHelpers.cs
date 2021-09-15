/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    /// <summary>
    /// Helpers methods for <see cref="PointerSize"/>.
    /// </summary>
    public static class PointerSizeHelpers
    {
        /// <summary>
        /// Converts a <see cref="PointerSize"/> to a 64-bit unsigned integer.
        /// </summary>
        /// <param name="ptr">The pointer to convert.</param>
        /// <returns>An <c>unsigned long</c>.</returns>
        public static ulong ToUInt64(this PointerSize ptr)
        {
            if (UIntPtr.Size == 8)
            {
                return (ulong)(long)ptr;
            }
            else
            {
                return (uint)(int)ptr;
            }
        }
    }
}
