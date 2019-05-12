// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoordinateSystemVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a coordinate system with arrows in the X, Y and Z directions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a coordinate system with arrows in the X, Y and Z directions.
    /// </summary>
    public class CoordinateSystemVisual3D : ModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="ArrowLengths"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ArrowLengthsProperty = DependencyProperty.Register(
            "ArrowLengths",
            typeof(double),
            typeof(CoordinateSystemVisual3D),
            new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="XAxisColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty XAxisColorProperty = DependencyProperty.Register(
            "XAxisColor",
            typeof(Color),
            typeof(CoordinateSystemVisual3D),
            new UIPropertyMetadata(Color.FromRgb(150, 75, 75)));

        /// <summary>
        /// Identifies the <see cref="YAxisColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YAxisColorProperty = DependencyProperty.Register(
            "YAxisColor",
            typeof(Color),
            typeof(CoordinateSystemVisual3D),
            new UIPropertyMetadata(Color.FromRgb(75, 150, 75)));

        /// <summary>
        /// Identifies the <see cref="ZAxisColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZAxisColorProperty = DependencyProperty.Register(
            "ZAxisColor",
            typeof(Color),
            typeof(CoordinateSystemVisual3D),
            new UIPropertyMetadata(Color.FromRgb(75, 75, 150)));

        /// <summary>
        /// Initializes a new instance of the <see cref = "CoordinateSystemVisual3D" /> class.
        /// </summary>
        public CoordinateSystemVisual3D()
        {
            this.OnGeometryChanged();
        }

        /// <summary>
        /// Gets or sets the arrow lengths.
        /// </summary>
        /// <value>The arrow lengths.</value>
        public double ArrowLengths
        {
            get
            {
                return (double)this.GetValue(ArrowLengthsProperty);
            }

            set
            {
                this.SetValue(ArrowLengthsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the X axis.
        /// </summary>
        /// <value>The color of the X axis.</value>
        public Color XAxisColor
        {
            get
            {
                return (Color)this.GetValue(XAxisColorProperty);
            }

            set
            {
                this.SetValue(XAxisColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the Y axis.
        /// </summary>
        /// <value>The color of the Y axis.</value>
        public Color YAxisColor
        {
            get
            {
                return (Color)this.GetValue(YAxisColorProperty);
            }

            set
            {
                this.SetValue(YAxisColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the Z axis.
        /// </summary>
        /// <value>The color of the Z axis.</value>
        public Color ZAxisColor
        {
            get
            {
                return (Color)this.GetValue(ZAxisColorProperty);
            }

            set
            {
                this.SetValue(ZAxisColorProperty, value);
            }
        }

        /// <summary>
        /// The geometry changed.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected static void GeometryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((CoordinateSystemVisual3D)obj).OnGeometryChanged();
        }

        /// <summary>
        /// Called when the geometry has changed.
        /// </summary>
        protected virtual void OnGeometryChanged()
        {
            this.Children.Clear();
            double l = this.ArrowLengths;
            double d = l * 0.1;

            var xaxis = new ArrowVisual3D();
            xaxis.BeginEdit();
            xaxis.Point2 = new Point3D(l, 0, 0);
            xaxis.Diameter = d;
            xaxis.Fill = new SolidColorBrush(this.XAxisColor);
            xaxis.EndEdit();
            this.Children.Add(xaxis);

            var yaxis = new ArrowVisual3D();
            yaxis.BeginEdit();
            yaxis.Point2 = new Point3D(0, l, 0);
            yaxis.Diameter = d;
            yaxis.Fill = new SolidColorBrush(this.YAxisColor);
            yaxis.EndEdit();
            this.Children.Add(yaxis);

            var zaxis = new ArrowVisual3D();
            zaxis.BeginEdit();
            zaxis.Point2 = new Point3D(0, 0, l);
            zaxis.Diameter = d;
            zaxis.Fill = new SolidColorBrush(this.ZAxisColor);
            zaxis.EndEdit();
            this.Children.Add(zaxis);

            this.Children.Add(new CubeVisual3D { SideLength = d, Fill = Brushes.Black });
        }

    }
}