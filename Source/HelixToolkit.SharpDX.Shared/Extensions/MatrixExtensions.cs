using System;
using System.Collections.Generic;
using System.Text;
using Media = System.Windows.Media;
using global::SharpDX;

namespace HelixToolkit.Wpf.SharpDX
{
    public static class MatrixExtensions
    {
        public static Matrix3x3 ToMatrix3x3(this Media.Matrix m)
        {
            return new Matrix3x3((float)m.M11, (float)m.M12, 0, (float)m.M21, (float)m.M22, 0f, (float)m.OffsetX, (float)m.OffsetY, 1f);
        }
        public static Matrix3x2 ToMatrix3x2(this Media.Matrix m)
        {
            return new Matrix3x2((float)m.M11, (float)m.M12, (float)m.M21, (float)m.M22, (float)m.OffsetX, (float)m.OffsetY);
        }
    }
}
