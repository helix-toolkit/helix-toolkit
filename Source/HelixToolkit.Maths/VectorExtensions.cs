using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Maths
{
    public static class VectorExtensions
    {
        #region System.Numerics.Vector2

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Numerics.Vector2"/> to <see cref="System.Numerics.Vector3"/>.
        /// </summary>
        /// <param name="vector">The <see cref="System.Numerics.Vector2"/> value.</param>
        /// <param name="z">The z value.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 ToVector3(this System.Numerics.Vector2 vector, float z = 0f)
        {
            return new System.Numerics.Vector3(vector, z);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Numerics.Vector2"/> to <see cref="System.Numerics.Vector4"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="z">The z value.</param>
        /// <param name="w">The w value.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector4 ToVector4(this System.Numerics.Vector2 value, float z = 0f, float w = 0f)
        {
            return new System.Numerics.Vector4(value, z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HelixToolkit.Maths.Color3 ToColor3(this System.Numerics.Vector2 vector, float blue = 0f)
        {
            return new HelixToolkit.Maths.Color3(vector.X, vector.Y, blue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HelixToolkit.Maths.Color4 ToColor4(this System.Numerics.Vector2 vector, float blue = 0f, float alpha = 0f)
        {
            return new HelixToolkit.Maths.Color4(vector.X, vector.Y, blue, alpha);
        }

        /// <summary>
        /// To the rectangleF.
        /// </summary>
        /// <param name="vector">The <see cref=" System.Numerics.Vector2"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HelixToolkit.Maths.RectangleF ToRectangleF(this System.Numerics.Vector2 vector)
        {
            return new HelixToolkit.Maths.RectangleF(0f, 0f, vector.X, vector.Y);
        }
        /// <summary>
        /// To the rectangle.
        /// </summary>
        /// <param name="vector">The <see cref=" System.Numerics.Vector2"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HelixToolkit.Maths.Rectangle ToRectangle(this System.Numerics.Vector2 vector)
        {
            return new HelixToolkit.Maths.Rectangle(0, 0, (int)vector.X, (int)vector.Y);
        }
        #endregion

        #region System.Numerics.Vector3
        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Numerics.Vector3"/> to <see cref="System.Numerics.Vector2"/>.
        /// </summary>
        /// <param name="vector">The <see cref="System.Numerics.Vector3"/> value.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector2 ToVector2(this System.Numerics.Vector3 vector)
        {
            return new System.Numerics.Vector2(vector.X, vector.Y);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Numerics.Vector3"/> to <see cref="System.Numerics.Vector4"/>.
        /// </summary>
        /// <param name="vector">The <see cref="System.Numerics.Vector3"/> value.</param>
        /// <param name="w">The w value.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector4 ToVector4(this System.Numerics.Vector3 vector, float w = 0f)
        {
            return new System.Numerics.Vector4(vector, w);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HelixToolkit.Maths.Color3 ToColor3(this System.Numerics.Vector3 vector)
        {
            return new HelixToolkit.Maths.Color3(vector);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HelixToolkit.Maths.Color4 ToColor4(this System.Numerics.Vector3 vector, float alpha = 0f)
        {
            return new HelixToolkit.Maths.Color4(vector, alpha);
        }
        #endregion

        #region System.Numerics.Vector4

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Numerics.Vector4"/> to <see cref="System.Numerics.Vector2"/>.
        /// </summary>
        /// <param name="vector">The <see cref="System.Numerics.Vector4"/> value.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector2 ToVector2(this System.Numerics.Vector4 vector)
        {
            return new System.Numerics.Vector2(vector.X, vector.Y);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector4"/> to <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vector">The <see cref="System.Numerics.Vector4"/> value.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 ToVector3(this System.Numerics.Vector4 vector)
        {
            return new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 ToHomogeneousVector3(this System.Numerics.Vector4 vector)
        {
            return new System.Numerics.Vector3(vector.X / vector.W, vector.Y / vector.W, vector.Z / vector.W);
        }
     
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HelixToolkit.Maths.Color4 ToColor4(this System.Numerics.Vector4 vector)
        {
            return new HelixToolkit.Maths.Color4(vector);
        }

        #endregion
    }
}
