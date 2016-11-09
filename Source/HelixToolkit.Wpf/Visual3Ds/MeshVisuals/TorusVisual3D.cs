namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;
    
    /// <summary>
    /// A visual element that shows a torus defined by two diameters (torus and it's tube).
    /// </summary>
    public class TorusVisual3D : MeshElement3D
    {
        /// <summary>
        /// Identifies the <see cref="TorusDiameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TorusDiameterProperty = DependencyProperty.Register(
            "TorusDiameter", typeof(double), typeof(TorusVisual3D), new UIPropertyMetadata(3.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="TubeDiameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TubeDiameterProperty = DependencyProperty.Register(
            "TubeDiameter", typeof(double), typeof(TorusVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="ThetaDiv"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
            "ThetaDiv", typeof(int), typeof(TorusVisual3D), new UIPropertyMetadata(36, GeometryChanged));
        
        /// <summary>
        /// Identifies the <see cref="PhiDiv"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PhiDivProperty = DependencyProperty.Register(
            "PhiDiv", typeof(int), typeof(TorusVisual3D), new UIPropertyMetadata(24, GeometryChanged));

        /// <summary>
        /// Gets or sets the (torus) diameter.
        /// </summary>
        /// <value>The diameter. The default value is <c>3</c>.</value>
        public double TorusDiameter
        {
            get
            {
                return (double)this.GetValue(TorusDiameterProperty);
            }

            set
            {
                if (value >= 0.0)
                    this.SetValue(TorusDiameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tube diameter.
        /// </summary>
        /// <value>The tube diameter. The default value is <c>1</c>.</value>
        public double TubeDiameter
        {
            get
            {
                return (double)this.GetValue(TubeDiameterProperty);
            }

            set
            {
                if (value >= 0.0)
                    this.SetValue(TubeDiameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the UDiv.
        /// </summary>
        /// <value>The UDiv. The default value is <c>36</c>.</value>
        public int ThetaDiv
        {
            get
            {
                return (int)this.GetValue(ThetaDivProperty);
            }

            set
            {
                if (value >= 3)
                    this.SetValue(ThetaDivProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the PhiDiv.
        /// </summary>
        /// <value>The PhiDiv. The default value is <c>24</c>.</value>
        public int PhiDiv
        {
            get
            {
                return (int)this.GetValue(PhiDivProperty);
            }

            set
            {
                if (value >= 3)
                    this.SetValue(PhiDivProperty, value);
            }
        }

        /// <summary>
        /// Do the tessellation and return the <see cref="MeshGeometry3D" />.
        /// </summary>
        /// <returns>
        /// A triangular mesh geometry.
        /// </returns>
        protected override MeshGeometry3D Tessellate()
        {
            var builder = new MeshBuilder(false, true);
            builder.AddTorus(this.TorusDiameter, this.TubeDiameter, this.ThetaDiv, this.PhiDiv);
            return builder.ToMesh();
        }
    }
}