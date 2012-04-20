// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows text.
    /// </summary>
    public class TextVisual3D : ModelVisual3D
    {
        #region Constants and Fields

        /// <summary>
        /// The center property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center", typeof(Point3D), typeof(TextVisual3D), new UIPropertyMetadata(new Point3D(0, 0, 0), VisualChanged));

        /// <summary>
        /// The direction property.
        /// </summary>
        public static readonly DependencyProperty TextDirectionProperty = DependencyProperty.Register(
            "TextDirection",
            typeof(Vector3D),
            typeof(TextVisual3D),
            new UIPropertyMetadata(new Vector3D(1, 0, 0), VisualChanged));

        /// <summary>
        /// The fill property.
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", typeof(Brush), typeof(TextVisual3D), new UIPropertyMetadata(Brushes.Black, VisualChanged));

        /// <summary>
        /// The height property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height", typeof(double), typeof(TextVisual3D), new UIPropertyMetadata(11.0, VisualChanged));

        /// <summary>
        /// The text property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(TextVisual3D), new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// The up property.
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register(
            "UpDirection", typeof(Vector3D), typeof(TextVisual3D), new UIPropertyMetadata(new Vector3D(0, 0, 1), VisualChanged));

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the center of the text.
        /// </summary>
        /// <value>The center.</value>
        public Point3D Center
        {
            get
            {
                return (Point3D)this.GetValue(CenterProperty);
            }

            set
            {
                this.SetValue(CenterProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the text direction.
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
        ///   Gets or sets the fill brush.
        /// </summary>
        /// <value>The fill brush.</value>
        public Brush Fill
        {
            get
            {
                return (Brush)this.GetValue(FillProperty);
            }

            set
            {
                this.SetValue(FillProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the height of the text.
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
        ///   Gets or sets the text.
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a ModelVisual3D containing a text label.
        /// </summary>
        /// <param name="text">
        /// The string
        /// </param>
        /// <param name="textColor">
        /// The color of the text.
        /// </param>
        /// <param name="isDoubleSided">
        /// Visible from both sides?
        /// </param>
        /// <param name="height">
        /// Height of the characters
        /// </param>
        /// <param name="center">
        /// The center of the label
        /// </param>
        /// <param name="textDirection">
        /// Horizontal direction of the label
        /// </param>
        /// <param name="updirection">
        /// Vertical direction of the label
        /// </param>
        /// <returns>
        /// Suitable for adding to your Viewport3D
        /// </returns>
        public static GeometryModel3D CreateTextLabel3D(
            string text,
            Brush textColor,
            bool isDoubleSided,
            double height,
            Point3D center,
            Vector3D textDirection,
            Vector3D updirection)
        {
            // First we need a textblock containing the text of our label
            var tb = new TextBlock(new Run(text)) { Foreground = textColor, FontFamily = new FontFamily("Arial") };

            // Now use that TextBlock as the brush for a material
            var mat = new DiffuseMaterial { Brush = new VisualBrush(tb) };

            // We just assume the characters are square
            double width = text.Length * height;

            // Since the parameter coming in was the center of the label,
            // we need to find the four corners
            // p0 is the lower left corner
            // p1 is the upper left
            // p2 is the lower right
            // p3 is the upper right
            Point3D p0 = center - width / 2 * textDirection - height / 2 * updirection;
            Point3D p1 = p0 + updirection * 1 * height;
            Point3D p2 = p0 + textDirection * width;
            Point3D p3 = p0 + updirection * 1 * height + textDirection * width;

            // Now build the geometry for the sign.  It's just a
            // rectangle made of two triangles, on each side.
            var mg = new MeshGeometry3D { Positions = new Point3DCollection { p0, p1, p2, p3 } };

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

            // These texture coordinates basically stretch the
            // TextBox brush to cover the full side of the label.
            mg.TextureCoordinates.Add(new Point(0, 1));
            mg.TextureCoordinates.Add(new Point(0, 0));
            mg.TextureCoordinates.Add(new Point(1, 1));
            mg.TextureCoordinates.Add(new Point(1, 0));

            if (isDoubleSided)
            {
                mg.TextureCoordinates.Add(new Point(1, 1));
                mg.TextureCoordinates.Add(new Point(1, 0));
                mg.TextureCoordinates.Add(new Point(0, 1));
                mg.TextureCoordinates.Add(new Point(0, 0));
            }

            return new GeometryModel3D(mg, mat);
        }

        #endregion

        #region Methods

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
            ((TextVisual3D)d).OnVisualChanged();
        }

        /// <summary>
        /// Called when the visual changed.
        /// </summary>
        private void OnVisualChanged()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Content = null;
            }
            else
            {
                this.Content = CreateTextLabel3D(
                    this.Text, this.Fill, true, this.Height, this.Center, this.TextDirection, this.UpDirection);
            }
        }

        #endregion

        // http://www.ericsink.com/wpf3d/4_Text.html
    }
}