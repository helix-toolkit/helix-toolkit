/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Text;
#if NETFX_CORE
using Media = Windows.UI.Xaml.Media;
#else
using Media = System.Windows.Media;
#endif
using global::SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
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
