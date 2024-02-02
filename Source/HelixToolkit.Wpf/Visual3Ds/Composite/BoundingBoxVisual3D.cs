using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Geometry;

namespace HelixToolkit.Wpf;

/// <summary>
/// A visual element that shows the edges of the specified bounding box.
/// </summary>
public class BoundingBoxVisual3D : ModelVisual3D
{
    /// <summary>
    /// Identifies the <see cref="BoundingBox"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BoundingBoxProperty = DependencyProperty.Register(
        "BoundingBox", typeof(Rect3D), typeof(BoundingBoxVisual3D), new UIPropertyMetadata(Rect3D.Empty, BoxChanged));

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
        if (this.BoundingBox.IsEmpty)
        {
            this.Content = null;
            return;
        }
        var meshBuilder = new MeshBuilder(false, false);
        meshBuilder.AddBoundingBox(this.BoundingBox.ToBoundingBox(), (float)Diameter);
        GeometryModel3D geoBoundingBox = new GeometryModel3D(meshBuilder.ToMesh().ToWndMeshGeometry3D(), MaterialHelper.CreateMaterial(Fill));
        this.Content = geoBoundingBox;
    }

    /// <summary>
    /// Called when the fill changed.
    /// </summary>
    protected virtual void OnFillChanged()
    {
        GeometryModel3D? geoBoundingBox = Content as GeometryModel3D;
        if (geoBoundingBox is null)
        {
            return;
        }

        geoBoundingBox.Material = MaterialHelper.CreateMaterial(Fill);
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
}
