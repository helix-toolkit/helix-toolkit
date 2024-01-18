using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Maths
{
    public static class VectorComparisonExtensions
    {
        #region VectorComparison
        /// <summary>
        /// Returns whether ALL elements of this are SmallerOrEqual the corresponding element of v2.
        /// ATTENTION: For example (v1.AllSmaller(v2)) is not the same as !(v1.AllGreaterOrEqual(v2)) but !(v1.AnyGreaterOrEqual(v2)).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllSmallerOrEqual(this Vector2 v1, Vector2 v2)
        {
            return (v1.X <= v2.X && v1.Y <= v2.Y);
        }

        /// <summary>
        /// Returns whether ALL elements of this are SmallerOrEqual the corresponding element of v2.
        /// ATTENTION: For example (v1.AllSmaller(v2)) is not the same as !(v1.AllGreaterOrEqual(v2)) but !(v1.AnyGreaterOrEqual(v2)).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllSmallerOrEqual(this Vector3 v1, Vector3 v2)
        {
            return (v1.X <= v2.X && v1.Y <= v2.Y && v1.Z <= v2.Z);
        }

        /// <summary>
        /// Returns whether ALL elements of v are SmallerOrEqual value.
        /// ATTENTION: For example (v.AllSmaller(value)) is not the same as !(v.AllGreaterOrEqual(value)) but !(v.AnyGreaterOrEqual(value)).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllSmallerOrEqual(this Vector3 v, float value)
        {
            return (v.X <= value && v.Y <= value && v.Z <= value);
        }

        /// <summary>
        /// Returns whether ALL elements of this are Smaller the corresponding element of v2.
        /// ATTENTION: For example (v1.AllSmaller(v2)) is not the same as !(v1.AllGreaterOrEqual(v2)) but !(v1.AnyGreaterOrEqual(v2)).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllSmaller(this Vector2 v1, Vector2 v2)
        {
            return (v1.X < v2.X && v1.Y < v2.Y);
        }

        /// <summary>
        /// Returns whether ALL elements of this are Smaller the corresponding element of v2.
        /// ATTENTION: For example (v1.AllSmaller(v2)) is not the same as !(v1.AllGreaterOrEqual(v2)) but !(v1.AnyGreaterOrEqual(v2)).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllSmaller(this Vector3 v1, Vector3 v2)
        {
            return (v1.X < v2.X && v1.Y < v2.Y && v1.Z < v2.Z);
        }

        /// <summary>
        /// Returns whether ALL elements of v are Smaller value.
        /// ATTENTION: For example (v.AllSmaller(value)) is not the same as !(v.AllGreaterOrEqual(value)) but !(v.AnyGreaterOrEqual(value)).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllSmaller(this Vector3 v, float value)
        {
            return (v.X < value && v.Y < value && v.Z < value);
        }

        /// <summary>
        /// Returns whether AT LEAST ONE element of v1 is SmallerOrEqual the corresponding element of v2.
        /// ATTENTION: For example (v1.AllSmaller(v2)) is not the same as !(v1.AllGreaterOrEqual(v2)) but !(v1.AnyGreaterOrEqual(v2)).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnySmallerOrEqual(this Vector3 v1, Vector3 v2)
        {
            return (v1.X <= v2.X || v1.Y <= v2.Y || v1.Z <= v2.Z);
        }

        /// <summary>
        /// Returns whether AT LEAST ONE element of v is SmallerOrEqual value.
        /// ATTENTION: For example (v.AllSmaller(value)) is not the same as !(v.AllGreaterOrEqual(value)) but !(v.AnyGreaterOrEqual(value)).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnySmallerOrEqual(this Vector3 v, float value)
        {
            return (v.X <= value || v.Y <= value || v.Z <= value);
        }

        /// <summary>
        /// Returns whether ALL elements of v1 are GreaterOrEqual the corresponding element of v2.
        /// ATTENTION: For example (v1.AllSmaller(v2)) is not the same as !(v1.AllGreaterOrEqual(v2)) but !(v1.AnyGreaterOrEqual(v2)).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllGreaterOrEqual(this Vector3 v1, Vector3 v2)
        {
            return (v1.X >= v2.X && v1.Y >= v2.Y && v1.Z >= v2.Z);
        }

        /// <summary>
        /// Returns whether ALL elements of v are GreaterOrEqual value.
        /// ATTENTION: For example (v.AllSmaller(value)) is not the same as !(v.AllGreaterOrEqual(value)) but !(v.AnyGreaterOrEqual(value)).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllGreaterOrEqual(this Vector3 v, float value)
        {
            return (v.X >= value && v.Y >= value && v.Z >= value);
        }

        /// <summary>
        /// Returns whether AT LEAST ONE element of v1 is GreaterOrEqual the corresponding element of v2.
        /// ATTENTION: For example (v1.AllSmaller(v2)) is not the same as !(v1.AllGreaterOrEqual(v2)) but !(v1.AnyGreaterOrEqual(v2)).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyGreaterOrEqual(this Vector3 v1, Vector3 v2)
        {
            return (v1.X >= v2.X || v1.Y >= v2.Y || v1.Z >= v2.Z);
        }

        /// <summary>
        /// Returns whether AT LEAST ONE element of v is GreaterOrEqual value.
        /// ATTENTION: For example (v.AllSmaller(a,b)) is not the same as !(v.AllGreaterOrEqual(value)) but !(v.AnyGreaterOrEqual(value)).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyGreaterOrEqual(this Vector3 v, float value)
        {
            return (v.X >= value || v.Y >= value || v.Z >= value);
        }

        /// <summary>
        /// Component-wise min vec
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ComponentMin(this Vector3 v1, Vector3 v2)
        {
            return new Vector3(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y), Math.Min(v1.Z, v2.Z));
        }

        /// <summary>
        /// Component-wise max vec
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ComponentMax(this Vector3 v1, Vector3 v2)
        {
            return new Vector3(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y), Math.Max(v1.Z, v2.Z));
        }
        #endregion
    }
}
