using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Maths
{
    public static class Matrix4x4Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 Inverted(this Matrix4x4 m)
        {
            return Matrix4x4.Invert(m, out var result) ? result : Matrix4x4.Identity;
        }
    }
}
