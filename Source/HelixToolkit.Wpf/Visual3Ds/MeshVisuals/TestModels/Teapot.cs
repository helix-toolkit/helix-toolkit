using System.Windows.Media.Media3D;
using System.Windows;

namespace HelixToolkit.Wpf;

/// <summary>
/// A visual element that shows the Utah teapot test model.
/// </summary>
public class Teapot : MeshElement3D
{
    /// <summary>
    /// Identifies the <see cref="Position"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        "Position", typeof(Point3D), typeof(Teapot), new UIPropertyMetadata(new Point3D(0, 0, 1), TransformChanged));

    /// <summary>
    /// Identifies the <see cref="SpoutDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SpoutDirectionProperty = DependencyProperty.Register(
        "SpoutDirection",
        typeof(Vector3D),
        typeof(Teapot),
        new UIPropertyMetadata(new Vector3D(1, 0, 0), TransformChanged));

    /// <summary>
    /// Identifies the <see cref="UpDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register(
        "UpDirection",
        typeof(Vector3D),
        typeof(Teapot),
        new UIPropertyMetadata(new Vector3D(0, 0, 1), TransformChanged));

    /// <summary>
    /// Gets or sets the position.
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
    /// Gets or sets the spout direction.
    /// </summary>
    /// <value>The spout direction.</value>
    public Vector3D SpoutDirection
    {
        get
        {
            return (Vector3D)this.GetValue(SpoutDirectionProperty);
        }

        set
        {
            this.SetValue(SpoutDirectionProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets up direction.
    /// </summary>
    /// <value>Up direction.</value>
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
    /// Do the tessellation and return the <see cref="MeshGeometry3D" />.
    /// </summary>
    /// <returns>
    /// A triangular mesh geometry.
    /// </returns>
    protected override MeshGeometry3D? Tessellate()
    {
        var name = System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name;
        name = name[..name.IndexOf(".dll")];

        var component = Application.LoadComponent(new Uri($"{name};component/Resources/TeapotGeometry.xaml", UriKind.Relative));

        if (component is not ResourceDictionary rd)
        {
            return null;
        }

        this.OnTransformChanged();
        return rd["TeapotGeometry"] as MeshGeometry3D;
    }

    /// <summary>
    /// The transform changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void TransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Teapot)d).OnTransformChanged();
    }

    /// <summary>
    /// Called when the transform is changed.
    /// </summary>
    private void OnTransformChanged()
    {
        Vector3D right = this.SpoutDirection;
        Vector3D back = Vector3D.CrossProduct(this.UpDirection, right);
        Vector3D up = this.UpDirection;

        this.Transform =
            new MatrixTransform3D(
                new Matrix3D(
                    right.X,
                    right.Y,
                    right.Z,
                    0,
                    up.X,
                    up.Y,
                    up.Z,
                    0,
                    back.X,
                    back.Y,
                    back.Z,
                    0,
                    this.Position.X,
                    this.Position.Y,
                    this.Position.Z,
                    1));
    }
}
