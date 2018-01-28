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
            var totalSize = new RectangleF() { Left = finalSize.Left, Top = finalSize.Top };
            foreach(var child in Items)
            {
                if(child is Element2DCore c)
                {
                    switch (Orientation)
                    {
                        case Orientation.Horizontal:
                            finalSize.Left += lastSize;
                            lastSize = c.DesiredSize.X;
                            finalSize.Right = finalSize.Left + lastSize;
                            finalSize.Bottom = finalSize.Top + Math.Min(finalSize.Height, c.DesiredSize.Y);
                            totalSize.Width += lastSize;
                            totalSize.Bottom = finalSize.Bottom;
                            break;
                        case Orientation.Vertical:
                            finalSize.Top += lastSize;
                            lastSize = c.DesiredSize.Y;
                            finalSize.Right = finalSize.Left + Math.Min(finalSize.Width, c.DesiredSize.X);
                            finalSize.Bottom = finalSize.Top + lastSize;
                            totalSize.Bottom += lastSize;
                            totalSize.Right = finalSize.Right;
                            break;
                    }
                    c.Arrange(finalSize);
                }
            }
            
            return totalSize;
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            if (!LayoutBoundWithTransform.Contains(mousePoint))
            {
                return false;
            }
            foreach (var item in Items.Reverse())
            {
                if (item is IHitable2D && (item as IHitable2D).HitTest(mousePoint, out hitResult))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
