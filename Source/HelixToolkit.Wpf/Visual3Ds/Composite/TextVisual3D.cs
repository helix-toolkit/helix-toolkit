// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
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
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction", 
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
        public static readonly DependencyProperty UpProperty = DependencyProperty.Register(
            "Up", typeof(Vector3D), typeof(TextVisual3D), new UIPropertyMetadata(new Vector3D(0, 0, 1), VisualChanged));

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the center.
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
        ///   Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public Vector3D Direction
        {
            get
            {
                return (Vector3D)this.GetValue(DirectionProperty);
            }

            set
            {
                this.SetValue(DirectionProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the fill.
        /// </summary>
        /// <value>The fill.</value>
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
        ///   Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
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
        ///   Gets or sets up.
        /// </summary>
        /// <value>Up.</value>
        public Vector3D Up
        {
            get
            {
                return (Vector3D)this.GetValue(UpProperty);
            }

            set
            {
                this.SetValue(UpProperty, value);
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
        /// <param name="bDoubleSided">
        /// Visible from both sides?
        /// </param>
        /// <param name="height">
        /// Height of the characters
        /// </param>
        /// <param name="center">
        /// The center of the label
        /// </param>
        /// <param name="over">
        /// Horizontal direction of the label
        /// </param>
        /// <param name="up">
        /// Vertical direction of the label
        /// </param>
        /// <returns>
        /// Suitable for adding to your Viewport3D
        /// </returns>
        public static GeometryModel3D CreateTextLabel3D(
            string text, Brush textColor, bool bDoubleSided, double height, Point3D center, Vector3D over, Vector3D up)
        {
            // First we need a textblock containing the text of our label
            var tb = new TextBlock(new Run(text));
            tb.Foreground = textColor;
            tb.FontFamily = new FontFamily("Arial");

            // Now use that TextBlock as the brush for a material
            var mat = new DiffuseMaterial();
            mat.Brush = new VisualBrush(tb);

            // We just assume the characters are square
            double width = text.Length * height;

            // Since the parameter coming in was the center of the label,
            // we need to find the four corners
            // p0 is the lower left corner
            // p1 is the upper left
            // p2 is the lower right
            // p3 is the upper right
            Point3D p0 = center - width / 2 * over - height / 2 * up;
            Point3D p1 = p0 + up * 1 * height;
            Point3D p2 = p0 + over * width;
            Point3D p3 = p0 + up * 1 * height + over * width;

            // Now build the geometry for the sign.  It's just a
            // rectangle made of two triangles, on each side.
            var mg = new MeshGeometry3D { Positions = new Point3DCollection { p0, p1, p2, p3 } };

            if (bDoubleSided)
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

            if (bDoubleSided)
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

            if (bDoubleSided)
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
                    this.Text, this.Fill, true, this.Height, this.Center, this.Direction, this.Up);
            }
        }

        #endregion

        // http://www.ericsink.com/wpf3d/4_Text.html
    }
}