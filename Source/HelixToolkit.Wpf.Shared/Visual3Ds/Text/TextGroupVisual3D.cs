// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextGroupVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a visual element that shows a collection of text items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides a visual element that shows a collection of text items.
    /// </summary>
    /// <remarks>
    /// Set the <see cref="Items"/> property last to avoid multiple updates.
    /// </remarks>
    public class TextGroupVisual3D : ModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(TextGroupVisual3D), new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(TextGroupVisual3D), new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                "BorderThickness",
                typeof(Thickness),
                typeof(TextGroupVisual3D),
                new UIPropertyMetadata(new Thickness(1), VisualChanged));

        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
            "FontFamily", typeof(FontFamily), typeof(TextGroupVisual3D), new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(TextGroupVisual3D), new UIPropertyMetadata(10.0, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
            "FontWeight",
            typeof(FontWeight),
            typeof(TextGroupVisual3D),
            new UIPropertyMetadata(FontWeights.Normal, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Foreground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(Brush), typeof(TextGroupVisual3D), new UIPropertyMetadata(Brushes.Black, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Height"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height", typeof(double), typeof(TextGroupVisual3D), new UIPropertyMetadata(1.0, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="IsDoubleSided"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDoubleSidedProperty = DependencyProperty.Register(
            "IsDoubleSided", typeof(bool), typeof(TextGroupVisual3D), new UIPropertyMetadata(false, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="IsFlipped"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register(
            "IsFlipped", typeof(bool), typeof(TextGroupVisual3D), new PropertyMetadata(false, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Items"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items",
            typeof(IList<SpatialTextItem>),
            typeof(TextGroupVisual3D),
            new UIPropertyMetadata(new List<SpatialTextItem>(), VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            "Padding",
            typeof(Thickness),
            typeof(TextGroupVisual3D),
            new UIPropertyMetadata(new Thickness(0), VisualChanged));

        /// <summary>
        /// Gets or sets the background brush.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get
            {
                return (Brush)this.GetValue(BackgroundProperty);
            }

            set
            {
                this.SetValue(BackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush.
        /// </summary>
        /// <value>The border brush.</value>
        public Brush BorderBrush
        {
            get
            {
                return (Brush)this.GetValue(BorderBrushProperty);
            }

            set
            {
                this.SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border thickness.
        /// </summary>
        /// <value>The border thickness.</value>
        public Thickness BorderThickness
        {
            get
            {
                return (Thickness)this.GetValue(BorderThicknessProperty);
            }

            set
            {
                this.SetValue(BorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)this.GetValue(FontFamilyProperty);
            }

            set
            {
                this.SetValue(FontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the font (if not set, the Height property is used.
        /// </summary>
        /// <value>The size of the font.</value>
        public double FontSize
        {
            get
            {
                return (double)this.GetValue(FontSizeProperty);
            }

            set
            {
                this.SetValue(FontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>The font weight.</value>
        public FontWeight FontWeight
        {
            get
            {
                return (FontWeight)this.GetValue(FontWeightProperty);
            }

            set
            {
                this.SetValue(FontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the foreground (text) brush.
        /// </summary>
        /// <value>The foreground brush.</value>
        public Brush Foreground
        {
            get
            {
                return (Brush)this.GetValue(ForegroundProperty);
            }

            set
            {
                this.SetValue(ForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the text.
        /// </summary>
        /// <value>The text height.</value>
        public double Height
        {
            get
            {
                return (double)this.GetValue(HeightProperty);
            }

            set
            {
                this.SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this text visual is double sided.
        /// </summary>
        /// <value><c>true</c> if this instance is double sided; otherwise, <c>false</c>.</value>
        public bool IsDoubleSided
        {
            get
            {
                return (bool)this.GetValue(IsDoubleSidedProperty);
            }

            set
            {
                this.SetValue(IsDoubleSidedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text should be flipped (mirrored horizontally).
        /// </summary>
        /// <remarks>
        /// This may be useful when using a mirror transform on the text visual.
        /// </remarks>
        /// <value>
        ///     <c>true</c> if text is flipped; otherwise, <c>false</c>.
        /// </value>
        public bool IsFlipped
        {
            get
            {
                return (bool)this.GetValue(IsFlippedProperty);
            }

            set
            {
                this.SetValue(IsFlippedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<SpatialTextItem> Items
        {
            get
            {
                return (IList<SpatialTextItem>)this.GetValue(ItemsProperty);
            }

            set
            {
                this.SetValue(ItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>The padding.</value>
        public Thickness Padding
        {
            get
            {
                return (Thickness)this.GetValue(PaddingProperty);
            }

            set
            {
                this.SetValue(PaddingProperty, value);
            }
        }

        /// <summary>
        /// Creates the text material.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="createElement">The create element.</param>
        /// <param name="background">The background.</param>
        /// <param name="elementMap">The element map.</param>
        /// <param name="elementPositions">The element positions.</param>
        /// <returns>A text material.</returns>
        public static Material CreateTextMaterial(
            IEnumerable<TextItem> items,
            Func<string, FrameworkElement> createElement,
            Brush background,
            out Dictionary<string, FrameworkElement> elementMap,
            out Dictionary<FrameworkElement, Rect> elementPositions)
        {
            var panel = new WrapPanel();
            elementMap = new Dictionary<string, FrameworkElement>();
            double maxWidth = 16;
            foreach (var item in items)
            {
                if (elementMap.ContainsKey(item.Text))
                {
                    continue;
                }

                var e = createElement(item.Text);
                e.Measure(new Size(2048, 2048));
                maxWidth = Math.Max(maxWidth, e.DesiredSize.Width);
                elementMap[item.Text] = e;
                panel.Children.Add(e);
            }

            var pw = (int)Math.Ceiling(OptimizeSize(panel, maxWidth, 1024));
            var ph = (int)Math.Ceiling(Math.Min(pw, panel.ActualHeight));

            elementPositions = new Dictionary<FrameworkElement, Rect>();
            foreach (FrameworkElement element in panel.Children)
            {
                var loc = element.TranslatePoint(new Point(0, 0), panel);
                double x = (int)Math.Floor(loc.X);
                double y = (int)Math.Floor(loc.Y);
                double x2 = (int)Math.Ceiling(loc.X + element.RenderSize.Width);
                double y2 = (int)Math.Ceiling(loc.Y + element.RenderSize.Height);
                elementPositions[element] = new Rect(x / pw, y / ph, (x2 - x) / pw, (y2 - y) / ph);
            }

            // Create the bitmap
            var rtb = new RenderTargetBitmap(pw, ph, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(panel);
            rtb.Freeze();
            var ib = new ImageBrush(rtb)
                         {
                             Stretch = Stretch.Fill,
                             ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
                             Viewbox = new Rect(0, 0, 1, 1),
                             ViewportUnits = BrushMappingMode.Absolute,
                             Viewport = new Rect(0, 0, 1, 1),
                             TileMode = TileMode.None,
                             AlignmentX = AlignmentX.Left,
                             AlignmentY = AlignmentY.Top
                         };

            if (background != null && !background.Equals(Brushes.Transparent))
            {
                var mg = new MaterialGroup();
                mg.Children.Add(new DiffuseMaterial(Brushes.Black));
                mg.Children.Add(new EmissiveMaterial(ib));
                return mg;
            }

            return new DiffuseMaterial(ib) { Color = Colors.White };
        }

        /// <summary>
        /// Optimizes the size of a panel.
        /// </summary>
        /// <param name="panel">The panel to optimize.</param>
        /// <param name="minWidth">The minimum width.</param>
        /// <param name="maxWidth">The maximum width.</param>
        /// <returns>The desired size.</returns>
        private static double OptimizeSize(UIElement panel, double minWidth, double maxWidth)
        {
            double width;

            // optimize size
            for (width = minWidth; width < maxWidth; width += 50)
            {
                panel.Measure(new Size(width, width + 1));
                if (panel.DesiredSize.Height <= width)
                {
                    break;
                }
            }

            panel.Arrange(new Rect(0, 0, width, width));

            return width;
        }

        /// <summary>
        /// The visual changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void VisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextGroupVisual3D)d).VisualChanged();
        }

        /// <summary>
        /// Creates an element (<see cref="TextBlock" /> or <see cref="FrameworkElement"/> wrapping a <see cref="TextBlock" />) for the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>A text block.</returns>
        private FrameworkElement CreateElement(string text)
        {
            var textBlock = new TextBlock(new Run(text))
                                {
                                    Foreground = this.Foreground,
                                    Background = this.Background,
                                    FontWeight = this.FontWeight,
                                    Padding = this.Padding
                                };

            if (this.FontFamily != null)
            {
                textBlock.FontFamily = this.FontFamily;
            }

            if (this.FontSize > 0)
            {
                textBlock.FontSize = this.FontSize;
            }

            if (this.BorderBrush != null)
            {
                return new Border
                           {
                               BorderBrush = this.BorderBrush,
                               BorderThickness = this.BorderThickness,
                               Child = textBlock
                           };
            }

            return textBlock;
        }

        /// <summary>
        /// Called when the visual changed.
        /// </summary>
        private void VisualChanged()
        {
            if (this.Items == null)
            {
                this.Content = null;
                return;
            }

            var items = this.Items.Where(i => !string.IsNullOrEmpty(i.Text)).ToList();
            var group = new Model3DGroup();
            while (items.Count > 0)
            {
                Dictionary<string, FrameworkElement> elementMap;
                Dictionary<FrameworkElement, Rect> elementPositions;
                var material = CreateTextMaterial(items, this.CreateElement, this.Background, out elementMap, out elementPositions);

                var builder = new MeshBuilder(false, true);
                var addedChildren = new List<SpatialTextItem>();
                foreach (var item in items)
                {
                    var element = elementMap[item.Text];
                    var r = elementPositions[element];
                    double u0 = r.Left;
                    double v0 = r.Top;
                    double u1 = r.Right;
                    double v1 = r.Bottom;
                    if (v1 > 1)
                    {
                        break;
                    }

                    if (this.IsFlipped)
                    {
                        var tmp = u0;
                        u0 = u1;
                        u1 = tmp;
                    }

                    // Set horizontal alignment factor
                    var xa = -0.5;
                    if (item.HorizontalAlignment == HorizontalAlignment.Left)
                    {
                        xa = 0;
                    }

                    if (item.HorizontalAlignment == HorizontalAlignment.Right)
                    {
                        xa = -1;
                    }

                    // Set vertical alignment factor
                    var ya = -0.5;
                    if (item.VerticalAlignment == VerticalAlignment.Top)
                    {
                        ya = -1;
                    }

                    if (item.VerticalAlignment == VerticalAlignment.Bottom)
                    {
                        ya = 0;
                    }

                    var position = item.Position;
                    var textDirection = item.TextDirection;
                    var upDirection = item.UpDirection;
                    var height = this.Height;
                    var width = this.Height / element.ActualHeight * element.ActualWidth;
                    var p0 = position + ((xa * width) * textDirection) + ((ya * height) * upDirection);
                    var p1 = p0 + (textDirection * width);
                    var p2 = p0 + (upDirection * height) + (textDirection * width);
                    var p3 = p0 + (upDirection * height);
                    builder.AddQuad(p0, p1, p2, p3, new Point(u0, v1), new Point(u1, v1), new Point(u1, v0), new Point(u0, v0));

                    if (this.IsDoubleSided)
                    {
                        builder.AddQuad(p1, p0, p3, p2, new Point(u0, v1), new Point(u1, v1), new Point(u1, v0), new Point(u0, v0));
                    }

                    addedChildren.Add(item);
                }

                var mg = builder.ToMesh();
                group.Children.Add(new GeometryModel3D(mg, material));

                foreach (var c in addedChildren)
                {
                    items.Remove(c);
                }
            }

            this.Content = group;
        }
    }
}