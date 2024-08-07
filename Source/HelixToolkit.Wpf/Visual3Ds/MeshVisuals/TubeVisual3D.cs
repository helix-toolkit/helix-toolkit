using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;
using HelixToolkit.Geometry;

namespace HelixToolkit.Wpf;

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
    /// Identifies the <see cref="AddCaps"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AddCapsProperty = DependencyProperty.Register(
        "AddCaps", typeof(bool), typeof(TubeVisual3D), new UIPropertyMetadata(false, SectionChanged));

    /// <summary>
    /// Initializes static members of the <see cref="TubeVisual3D"/> class.
    /// </summary>
    static TubeVisual3D()
    {
        SectionScalesProperty.OverrideMetadata(typeof(TubeVisual3D), new UIPropertyMetadata(null, SectionChanged));
    }

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
    /// Gets or sets the number of divisions around the tube.
    /// </summary>
    /// <value>The number of divisions.</value>
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
    /// Gets or sets the create Caps indicator.
    /// </summary>
    /// <value>True if Caps should be generated.</value>
    public bool AddCaps
    {
        get
        {
            return (bool)this.GetValue(AddCapsProperty);
        }

        set
        {
            this.SetValue(AddCapsProperty, value);
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
    protected void OnSectionChanged()
    {
        if (this.ThetaDiv < 2)
        {
            this.OnGeometryChanged();
            return;
        }
        var pc = new PointCollection();
        var circle = MeshBuilder.GetCircle(this.ThetaDiv, false);
        // If Diameters is not set, create a unit circle
        // otherwise, create a circle with the specified diameter
        double r = this.SectionScales != null ? 1 : this.Diameter / 2;
        for (int j = 0; j < circle.Count; j++)
        {
            pc.Add(new Point(circle[j].X * r, circle[j].Y * r));
        }
        this.Section = pc;

        this.OnGeometryChanged();
    }

    /// <summary>
    /// Do the tessellation and return the <see cref="MeshGeometry3D"/> .
    /// </summary>
    /// <returns>
    /// A triangular mesh geometry.
    /// </returns>
    protected override MeshGeometry3D? Tessellate()
    {
        if (this.Path is null || this.Path.Count < 2
            || this.Section is null || this.Section.Count < 2
            || this.ThetaDiv < 2)
        {
            return null;
        }

        // See also "The GLE Tubing and Extrusion Library":
        // http://linas.org/gle/
        // http://sharpmap.codeplex.com/Thread/View.aspx?ThreadId=18864
        var builder = new MeshBuilder(false, this.TextureCoordinates != null);

        var sectionXAxis = this.SectionXAxis;
        if (sectionXAxis.Length < 1e-6)
        {
            sectionXAxis = new Vector3D(1, 0, 0);
        }

        var forward = this.Path[1] - this.Path[0];
        var up = Vector3D.CrossProduct(forward, sectionXAxis);
        if (up.LengthSquared < 1e-6)
        {
            sectionXAxis = forward.FindAnyPerpendicular();
        }

        builder.AddTube(
            this.Path.ToVector3Collection()!,
            this.Angles.ToFloatCollection(),
            this.TextureCoordinates?.ToFloatCollection(),
            this.SectionScales.ToFloatCollection(),
            this.Section.ToVector2Collection()!,
            sectionXAxis.ToVector3(),
            this.IsPathClosed,
            this.IsSectionClosed, this.AddCaps, this.AddCaps);

        return builder.ToMesh().ToWndMeshGeometry3D();
    }
}
