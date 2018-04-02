using SharpDX;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using Model.Scene2D;
    /// <summary>
    /// Position content using relative position. 
    /// <para>Relative position (0,0) is the center of the canvas. LeftTop = (-1,1); RightTop = (1,1); LeftBottom = (-1,-1); RightBottom = (1, -1)</para>
    /// </summary>
    /// <seealso cref="HelixToolkit.Wpf.SharpDX.Elements2D.Panel2D" />
    public class RelativePositionCanvas2D : Panel2D
    {
        #region Attached Properties        
        /// <summary>
        /// The relative x property
        /// </summary>
        public static readonly DependencyProperty RelativeXProperty = DependencyProperty.RegisterAttached("RelativeX", typeof(double), typeof(RelativePositionCanvas2D),
            new PropertyMetadata(0.0, (d, e) => { (d as Element2DCore).InvalidateMeasure(); }));

        /// <summary>
        /// Sets the relative x.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetRelativeX(Element2DCore element, double value)
        {
            element.SetValue(RelativeXProperty, value);
        }
        /// <summary>
        /// Gets the relative x.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static double GetRelativeX(Element2DCore element)
        {
            return (double)element.GetValue(RelativeXProperty);
        }
        /// <summary>
        /// The relative y property
        /// </summary>
        public static readonly DependencyProperty RelativeYProperty = DependencyProperty.RegisterAttached("RelativeY", typeof(double), typeof(RelativePositionCanvas2D),
            new PropertyMetadata(0.0, (d, e) => { (d as Element2DCore).InvalidateMeasure(); }));
        /// <summary>
        /// Sets the relative y.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetRelativeY(Element2DCore element, double value)
        {
            element.SetValue(RelativeYProperty, value);
        }
        /// <summary>
        /// Gets the relative y.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static double GetRelativeY(Element2DCore element)
        {
            return (double)element.GetValue(RelativeYProperty);
        }
        #endregion

        protected override SceneNode2D OnCreateSceneNode()
        {
            return new Node2DRelativePositionCanvas();
        }


        protected class Node2DRelativePositionCanvas : PanelNode2D
        {
            /// <summary>
            /// Measures the override.
            /// </summary>
            /// <param name="availableSize">Size of the available.</param>
            /// <returns></returns>
            protected override Size2F MeasureOverride(Size2F availableSize)
            {
                var childConstraint = new Size2F(float.PositiveInfinity, float.PositiveInfinity);
                foreach (var child in Items)
                {
                    child.Measure(childConstraint);
                }
                return new Size2F();
            }
            /// <summary>
            /// Arranges the override.
            /// </summary>
            /// <param name="finalSize">The final size.</param>
            /// <returns></returns>
            protected override RectangleF ArrangeOverride(RectangleF finalSize)
            {
                foreach (var child in Items)
                {
                    if (child is SceneNode2D c)
                    {
                        float xPos = finalSize.Width/2 * (1+(float)GetRelativeX(c));
                        float yPos = finalSize.Height/2 * (1-(float)GetRelativeY(c));
                        var desired = c.DesiredSize;
                        c.Arrange(new RectangleF(xPos - desired.X/2, yPos - desired.Y/2, desired.X, desired.Y));
                    }
                }
                return finalSize;
            }
        }
    }
}
