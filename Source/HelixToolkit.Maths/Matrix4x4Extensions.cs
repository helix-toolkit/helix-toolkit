using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Maths
{
    public static class Matrix4x4Extensions
    {
        private static readonly ILogger logger = Logger.LogManager.Create(nameof(Matrix4x4Extensions));
        /// <summary>
        /// Return inverted matrix if the operation succeeded.
        /// Otherwise, return <see cref="Matrix4x4.Identity"/>
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 Inverted(this Matrix4x4 matrix)
        {
            if (Matrix4x4.Invert(matrix, out var result))
            {
                return result;
            }
            logger.LogError("Matrix inversion has failed");
            return Matrix4x4.Identity;
        }
    }
}
