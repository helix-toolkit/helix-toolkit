using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Maths
{
    public static class RectangleHelper
    {
        /// <summary>
        /// To the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Rectangle rect)
        {
            return new Vector2(rect.Width, rect.Height);
        }
        /// <summary>
        /// To the rectangleF.
        /// </summary>
        /// <param name="rect">The rectangleF.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this RectangleF rect)
        {
            return new Vector2(rect.Width, rect.Height);
        }

        public static RectangleF Translate(this RectangleF rect, Vector2 translation)
        {
            return new RectangleF(rect.Left + translation.X, rect.Top + translation.Y, rect.Width, rect.Height);
        }
    }
}
