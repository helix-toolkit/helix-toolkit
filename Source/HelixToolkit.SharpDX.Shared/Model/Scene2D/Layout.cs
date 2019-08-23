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
    namespace Model.Scene2D
    {
        public enum HorizontalAlignment
        {
            Left, Right, Center, Stretch
        }

        public enum VerticalAlignment
        {
            Top, Bottom, Center, Stretch
        }

        public enum Visibility
        {
            Visible, Collapsed, Hidden
        }

        public enum Orientation
        {
            Horizontal, Vertical
        }

        public struct Thickness : IEquatable<Thickness>
        {
            public float Left;
            public float Right;
            public float Top;
            public float Bottom;

            public Thickness(float size)
            {
                Left = size;
                Right = size;
                Top = size;
                Bottom = size;
            }

            public Thickness(float left, float right, float top, float bottom)
            {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }

            public bool Equals(Thickness other)
            {
                return this.Left == other.Left && this.Right == other.Right && this.Top == other.Top && this.Bottom == other.Bottom;
            }

            public static implicit operator Vector4(Thickness t)
            {
                return new Vector4(t.Left, t.Top, t.Right, t.Bottom);
            }
        }
    }

}
