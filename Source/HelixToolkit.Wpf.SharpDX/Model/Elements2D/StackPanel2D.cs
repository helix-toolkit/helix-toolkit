using System;
using System.Windows;
using System.Windows.Controls;
using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;
using System.Linq;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public class StackPanel2D : Panel2D
    {
        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// The orientation property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackPanel2D), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public StackPanel2D()
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

            foreach(var child in Items)
            {
                if(child is Element2DCore c)
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
            foreach(var child in Items)
            {
                if(child is Element2DCore c)
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
