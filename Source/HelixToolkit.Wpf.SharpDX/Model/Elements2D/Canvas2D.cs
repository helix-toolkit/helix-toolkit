// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupElement3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using global::SharpDX;
    using System.Windows;
    using Model.Scene2D;

    /// <summary>
    /// Supports both ItemsSource binding and Xaml children. Binds with ObservableElement2DCollection 
    /// </summary>
    public class Canvas2D : Panel2D
    {
        #region Attached Properties        
        /// <summary>
        /// The left property
        /// </summary>
        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached("Left", typeof(double), typeof(Canvas2D),
            new PropertyMetadata(double.PositiveInfinity, (d, e) => { (d as Element2DCore).InvalidateMeasure(); }));
        /// <summary>
        /// Sets the left.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetLeft(Element2DCore element, double value)
        {
            element.SetValue(LeftProperty, value);
        }
        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static double GetLeft(Element2DCore element)
        {
            return (double)element.GetValue(LeftProperty);
        }
        /// <summary>
        /// The top property
        /// </summary>
        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached("Top", typeof(double), typeof(Canvas2D),
            new PropertyMetadata(double.PositiveInfinity, (d, e) => { (d as Element2DCore).InvalidateMeasure(); }));
        /// <summary>
        /// Sets the top.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetTop(Element2DCore element, double value)
        {
            element.SetValue(TopProperty, value);
        }
        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static double GetTop(Element2DCore element)
        {
            return (double)element.GetValue(TopProperty);
        }
        /// <summary>
        /// The right property
        /// </summary>
        public static readonly DependencyProperty RightProperty = DependencyProperty.RegisterAttached("Right", typeof(double), typeof(Canvas2D),
            new PropertyMetadata(double.PositiveInfinity, (d, e) => { (d as Element2DCore).InvalidateMeasure(); }));
        /// <summary>
        /// Sets the right.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetRight(Element2DCore element, double value)
        {
            element.SetValue(RightProperty, value);
        }
        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static double GetRight(Element2DCore element)
        {
            return (double)element.GetValue(RightProperty);
        }
        /// <summary>
        /// The bottom property
        /// </summary>
        public static readonly DependencyProperty BottomProperty = DependencyProperty.RegisterAttached("Bottom", typeof(double), typeof(Canvas2D),
            new PropertyMetadata(double.PositiveInfinity, (d, e) => { (d as Element2DCore).InvalidateMeasure(); }));
        /// <summary>
        /// Sets the bottom.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetBottom(Element2DCore element, double value)
        {
            element.SetValue(BottomProperty, value);
        }
        /// <summary>
        /// Gets the bottom.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static double GetBottom(Element2DCore element)
        {
            return (double)element.GetValue(BottomProperty);
        }
        #endregion

        protected override SceneNode2D OnCreateSceneNode()
        {
            return new Node2DCanvas();
        }

        protected class Node2DCanvas : PanelNode2D
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
                        float xPos = 0;
                        float yPos = 0;
                        var left = GetLeft(c);
                        var desired = c.DesiredSize;
                        if (left != double.PositiveInfinity)
                        {
                            xPos = (float)left;
                        }
                        else
                        {
                            var right = GetRight(c);
                            if (right != double.PositiveInfinity)
                            {
                                xPos = finalSize.Width - desired.X - (float)right;
                            }
                        }

                        var top = GetTop(c);
                        if (top != double.PositiveInfinity)
                        {
                            yPos = (float)top;
                        }
                        else
                        {
                            var bottom = GetBottom(c);
                            if (bottom != double.PositiveInfinity)
                            {
                                yPos = finalSize.Height - desired.Y - (float)bottom;
                            }
                        }
                        c.Arrange(new RectangleF(xPos, yPos, desired.X, desired.Y));
                        //c.Arrange(new RectangleF(xPos, yPos, desired.X + xPos, desired.Y + yPos));
                    }
                }
                return finalSize;
            }
        }
    }
}