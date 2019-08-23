// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows text.
    /// </summary>
    /// <remarks>
    /// Set the Text property last to avoid multiple updates.
    /// </remarks>
    public class TextVisual3D : ModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(TextVisual3D), new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(TextVisual3D), new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                "BorderThickness", typeof(Thickness), typeof(TextVisual3D), new UIPropertyMetadata(new Thickness(1), VisualChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the text should be flipped (mirrored horizontally).
        /// </summary>
        /// <remarks>
        /// This may be useful when using a mirror transform on the text visual.
        /// </remarks>
        /// <value>
        ///   <c>true</c> if text is flipped; otherwise, <c>false</c>.
        /// </value>
        public bool IsFlipped
        {
            get { return (bool)GetValue(IsFlippedProperty); }
            set { SetValue(IsFlippedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsFlipped"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFlippedProperty =
            DependencyProperty.Register("IsFlipped", typeof(bool), typeof(TextVisual3D), new PropertyMetadata(false, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
            "FontFamily", typeof(FontFamily), typeof(TextVisual3D), new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(TextVisual3D), new UIPropertyMetadata(0.0, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
            "FontWeight", typeof(FontWeight), typeof(TextVisual3D), new UIPropertyMetadata(FontWeights.Normal, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Foreground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(Brush), typeof(TextVisual3D), new UIPropertyMetadata(Brushes.Black, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Height"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height", typeof(double), typeof(TextVisual3D), new UIPropertyMetadata(11.0, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="HorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(TextVisual3D),
                new UIPropertyMetadata(HorizontalAlignment.Center, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="IsDoubleSided"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDoubleSidedProperty = DependencyProperty.Register(
            "IsDoubleSided", typeof(bool), typeof(TextVisual3D), new UIPropertyMetadata(true, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            "Padding", typeof(Thickness), typeof(TextVisual3D), new UIPropertyMetadata(new Thickness(0), VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position",
            typeof(Point3D),
            typeof(TextVisual3D),
            new UIPropertyMetadata(new Point3D(0, 0, 0), VisualChanged));

        /// <summary>
        /// Identifies the <see cref="TextDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDirectionProperty = DependencyProperty.Register(
            "TextDirection",
            typeof(Vector3D),
            typeof(TextVisual3D),
            new UIPropertyMetadata(new Vector3D(1, 0, 0), VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(TextVisual3D), new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="UpDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register(
            "UpDirection",
            typeof(Vector3D),
            typeof(TextVisual3D),
            new UIPropertyMetadata(new Vector3D(0, 0, 1), VisualChanged));

        /// <summary>
        /// Identifies the <see cref="VerticalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(
                "VerticalAlignment",
                typeof(VerticalAlignment),
                typeof(TextVisual3D),
                new UIPropertyMetadata(VerticalAlignment.Center, VisualChanged));

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
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal alignment.</value>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(HorizontalAlignmentProperty);
            }
            set
            {
                this.SetValue(HorizontalAlignmentProperty, value);
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
        /// Gets or sets the position of the text.
        /// </summary>
        /// <value>The position.</value>
        public Point3D Position
        {
            get
            {
                return (Point3D)this.GetValue(PositionProperty);
            }

            set
            {
                this.SetValue(PositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }

            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text direction.
        /// </summary>
        /// <value>The direction.</value>
        public Vector3D TextDirection
        {
            get
            {
                return (Vector3D)this.GetValue(TextDirectionProperty);
            }

            set
            {
                this.SetValue(TextDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the up direction of the text.
        /// </summary>
        /// <value>The up direction.</value>
        public Vector3D UpDirection
        {
            get
            {
                return (Vector3D)this.GetValue(UpDirectionProperty);
            }

            set
            {
                this.SetValue(UpDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <value>The vertical alignment.</value>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(VerticalAlignmentProperty);
            }
            set
            {
                this.SetValue(VerticalAlignmentProperty, value);
            }
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
            ((TextVisual3D)d).VisualChanged();
        }

        /// <summary>
        /// Called when the visual changed.
        /// </summary>
        private void VisualChanged()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Content = null;
                return;
            }

            // First we need a textblock containing the text of our label
            var textBlock = new TextBlock(new Run(this.Text))
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

            var element = this.BorderBrush != null
                              ? (FrameworkElement)
                                new Border
                                    {
                                        BorderBrush = this.BorderBrush,
                                        BorderThickness = this.BorderThickness,
                                        Child = textBlock
                                    }
                              : textBlock;

            element.Measure(new Size(1000, 1000));
            element.Arrange(new Rect(element.DesiredSize));

            Material material;
            if (this.FontSize > 0)
            {
                var rtb = new RenderTargetBitmap(
                    (int)element.ActualWidth + 1, (int)element.ActualHeight + 1, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(element);
                rtb.Freeze();
                material = new DiffuseMaterial(new ImageBrush(rtb));
            }
            else
            {
                material = new DiffuseMaterial { Brush = new VisualBrush(element) };
            }

            double width = element.ActualWidth / element.ActualHeight * this.Height;

            var position = this.Position;
            var textDirection = this.TextDirection;
            var updirection = this.UpDirection;
            var height = this.Height;

            // Set horizontal alignment factor
            var xa = -0.5;
            if (this.HorizontalAlignment == HorizontalAlignment.Left)
            {
                xa = 0;
            }
            if (this.HorizontalAlignment == HorizontalAlignment.Right)
            {
                xa = -1;
            }

            // Set vertical alignment factor
            var ya = -0.5;
            if (this.VerticalAlignment == VerticalAlignment.Top)
            {
                ya = -1;
            }
            if (this.VerticalAlignment == VerticalAlignment.Bottom)
            {
                ya = 0;
            }

            // Since the parameter coming in was the center of the label,
            // we need to find the four corners
            // p0 is the lower left corner
            // p1 is the upper left
            // p2 is the lower right
            // p3 is the upper right
            Point3D p0 = position + (xa * width) * textDirection + (ya * height) * updirection;
            Point3D p1 = p0 + updirection * height;
            Point3D p2 = p0 + textDirection * width;
            Point3D p3 = p0 + updirection * height + textDirection * width;

            // Now build the geometry for the sign.  It's just a
            // rectangle made of two triangles, on each side.
            var mg = new MeshGeometry3D { Positions = new Point3DCollection { p0, p1, p2, p3 } };

            bool isDoubleSided = this.IsDoubleSided;
            if (isDoubleSided)
            {
                mg.Positions.Add(p0); // 4
                mg.Positions.Add(p1); // 5
                mg.Positions.Add(p2); // 6
                mg.Positions.Add(p3); // 7
            }

            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(3);
            mg.TriangleIndices.Add(1);
            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(2);
            mg.TriangleIndices.Add(3);

            if (isDoubleSided)
            {
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(5);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(6);
            }

            double u0 = this.IsFlipped ? 1 : 0;
            double u1 = this.IsFlipped ? 0 : 1;

            // These texture coordinates basically stretch the
            // TextBox brush to cover the full side of the label.
            mg.TextureCoordinates.Add(new Point(u0, 1));
            mg.TextureCoordinates.Add(new Point(u0, 0));
            mg.TextureCoordinates.Add(new Point(u1, 1));
            mg.TextureCoordinates.Add(new Point(u1, 0));

            if (isDoubleSided)
            {
                mg.TextureCoordinates.Add(new Point(u1, 1));
                mg.TextureCoordinates.Add(new Point(u1, 0));
                mg.TextureCoordinates.Add(new Point(u0, 1));
                mg.TextureCoordinates.Add(new Point(u0, 0));
            }

            this.Content = new GeometryModel3D(mg, material);
        }

        // http://www.ericsink.com/wpf3d/4_Text.html
    }
}