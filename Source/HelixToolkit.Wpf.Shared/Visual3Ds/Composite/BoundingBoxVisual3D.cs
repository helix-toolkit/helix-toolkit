// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoundingBoxVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a wireframe for the specified bounding box.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a wireframe for the specified bounding box.
    /// </summary>
    public class BoundingBoxVisual3D : ModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="BoundingBox"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BoundingBoxProperty = DependencyProperty.Register(
            "BoundingBox", typeof(Rect3D), typeof(BoundingBoxVisual3D), new UIPropertyMetadata(new Rect3D(), BoxChanged));

        /// <summary>
        /// Identifies the <see cref="Diameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(BoundingBoxVisual3D), new UIPropertyMetadata(0.1, BoxChanged));

        /// <summary>
        /// Identifies the <see cref="Fill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", typeof(Brush), typeof(BoundingBoxVisual3D), new UIPropertyMetadata(Brushes.Yellow, FillChanged));

        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value> The bounding box. </value>
        public Rect3D BoundingBox
        {
            get
            {
                return (Rect3D)this.GetValue(BoundingBoxProperty);
            }

            set
            {
                this.SetValue(BoundingBoxProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the diameter.
        /// </summary>
        /// <value> The diameter. </value>
        public double Diameter
        {
            get
            {
                return (double)this.GetValue(DiameterProperty);
            }

            set
            {
                this.SetValue(DiameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brush of the bounding box.
        /// </summary>
        /// <value> The brush. </value>
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
        /// Updates the box.
        /// </summary>
        protected virtual void OnBoxChanged()
        {
            this.Children.Clear();
            if (this.BoundingBox.IsEmpty)
            {
                return;
            }

            Rect3D bb = this.BoundingBox;

            var p0 = new Point3D(bb.X, bb.Y, bb.Z);
            var p1 = new Point3D(bb.X, bb.Y + bb.SizeY, bb.Z);
            var p2 = new Point3D(bb.X + bb.SizeX, bb.Y + bb.SizeY, bb.Z);
            var p3 = new Point3D(bb.X + bb.SizeX, bb.Y, bb.Z);
            var p4 = new Point3D(bb.X, bb.Y, bb.Z + bb.SizeZ);
            var p5 = new Point3D(bb.X, bb.Y + bb.SizeY, bb.Z + bb.SizeZ);
            var p6 = new Point3D(bb.X + bb.SizeX, bb.Y + bb.SizeY, bb.Z + bb.SizeZ);
            var p7 = new Point3D(bb.X + bb.SizeX, bb.Y, bb.Z + bb.SizeZ);

            this.AddEdge(p0, p1);
            this.AddEdge(p1, p2);
            this.AddEdge(p2, p3);
            this.AddEdge(p3, p0);

            this.AddEdge(p4, p5);
            this.AddEdge(p5, p6);
            this.AddEdge(p6, p7);
            this.AddEdge(p7, p4);

            this.AddEdge(p0, p4);
            this.AddEdge(p1, p5);
            this.AddEdge(p2, p6);
            this.AddEdge(p3, p7);
        }

        /// <summary>
        /// Called when the fill changed.
        /// </summary>
        protected virtual void OnFillChanged()
        {
            foreach (MeshElement3D item in this.Children)
            {
                if (item != null)
                {
                    item.Fill = this.Fill;
                }
            }
        }

        /// <summary>
        /// Called when the box dimensions changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void BoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BoundingBoxVisual3D)d).OnBoxChanged();
        }

        /// <summary>
        /// Called when the fill changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        private static void FillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BoundingBoxVisual3D)d).OnFillChanged();
        }

        /// <summary>
        /// Adds an edge.
        /// </summary>
        /// <param name="p1">
        /// The start point.
        /// </param>
        /// <param name="p2">
        /// The end point.
        /// </param>
        private void AddEdge(Point3D p1, Point3D p2)
        {
            var fv = new PipeVisual3D();
            fv.BeginEdit();
            fv.Diameter = this.Diameter;
            fv.ThetaDiv = 10;
            fv.Fill = this.Fill;
            fv.Point1 = p1;
            fv.Point2 = p2;
            fv.EndEdit();
            this.Children.Add(fv);
        }

    }
}