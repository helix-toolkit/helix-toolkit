/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System;

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
        public class StackPanelNode2D : PanelNode2D
        {
            private Orientation orientation = Orientation.Horizontal;

            public Orientation Orientation
            {
                set
                {
                    SetAffectsMeasure(ref orientation, value);
                }
                get
                {
                    return orientation;
                }
            }

            public StackPanelNode2D()
            {
                EnableBitmapCache = true;
            }

            protected override Size2F MeasureOverride(Size2F availableSize)
            {
                var constraint = availableSize;

                var size = new Size2F();
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        availableSize.Width = float.PositiveInfinity;
                        break;

                    case Orientation.Vertical:
                        availableSize.Height = float.PositiveInfinity;
                        break;
                }

                foreach (var child in Items)
                {
                    if (child is SceneNode2D c)
                    {
                        child.Measure(availableSize);
                        switch (Orientation)
                        {
                            case Orientation.Horizontal:
                                size.Width += c.DesiredSize.X;
                                size.Height = Math.Max(size.Height, c.DesiredSize.Y);
                                break;

                            case Orientation.Vertical:
                                size.Width = Math.Max(c.DesiredSize.X, size.Width);
                                size.Height += c.DesiredSize.Y;
                                break;
                        }
                    }
                }

                return size;
            }

            protected override RectangleF ArrangeOverride(RectangleF finalSize)
            {
                float lastSize = 0;
                var totalSize = finalSize;
                foreach (var child in Items)
                {
                    if (child is SceneNode2D c)
                    {
                        switch (Orientation)
                        {
                            case Orientation.Horizontal:
                                totalSize.Left += lastSize;
                                lastSize = c.DesiredSize.X;
                                totalSize.Right = totalSize.Left + lastSize;
                                //totalSize.Bottom = totalSize.Top + Math.Min(finalSize.Height, c.DesiredSize.Y);
                                break;

                            case Orientation.Vertical:
                                totalSize.Top += lastSize;
                                lastSize = c.DesiredSize.Y;
                                //totalSize.Right = totalSize.Left + Math.Min(finalSize.Width, c.DesiredSize.X);
                                totalSize.Bottom = totalSize.Top + lastSize;
                                break;
                        }
                        c.Arrange(totalSize);
                    }
                }

                return finalSize;
            }
        }
    }

}