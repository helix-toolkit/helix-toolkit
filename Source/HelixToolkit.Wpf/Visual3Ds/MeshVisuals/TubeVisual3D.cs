// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TubeVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// A visual element that shows a tube along a specified path.
    /// </summary>
    /// <remarks>
    /// The implementation will not work well if there are sharp bends in the path.
    /// </remarks>
    public class TubeVisual3D : ExtrudedVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="Diameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(TubeVisual3D), new UIPropertyMetadata(1.0, SectionChanged));

        /// <summary>
        /// Identifies the <see cref="ThetaDiv"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
            "ThetaDiv", typeof(int), typeof(TubeVisual3D), new UIPropertyMetadata(36, SectionChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref = "TubeVisual3D" /> class.
        /// </summary>
        public TubeVisual3D()
        {
            this.OnSectionChanged();
        }

        /// <summary>
        /// Gets or sets the diameter of the tube.
        /// </summary>
        /// <value>The diameter of the tube.</value>
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
        /// Gets or sets the number of divsions around the tube.
        /// </summary>
        /// <value>The theta div.</value>
        public int ThetaDiv
        {
            get
            {
                return (int)this.GetValue(ThetaDivProperty);
            }

            set
            {
                this.SetValue(ThetaDivProperty, value);
            }
        }

        /// <summary>
        /// The section changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void SectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TubeVisual3D)d).OnSectionChanged();
        }

        /// <summary>
        /// Updates the section.
        /// </summary>
        protected virtual void OnSectionChanged()
        {
            var pc = new PointCollection();
            var circle = MeshBuilder.GetCircle(this.ThetaDiv);
            double r = this.Diameter / 2;
            for (int j = 0; j < this.ThetaDiv; j++)
            {
                pc.Add(new Point(circle[j].X * r, circle[j].Y * r));
            }

            this.Section = pc;

            this.OnGeometryChanged();
        }

    }
}