using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf;

/// <summary>
/// Represents a spatial text item.
/// </summary>
public class SpatialTextItem : TextItem
{
    /// <summary>
    /// Gets or sets the text direction.
    /// </summary>
    /// <value>The text direction.</value>
    public Vector3D TextDirection { get; set; }

    /// <summary>
    /// Gets or sets up direction.
    /// </summary>
    /// <value>Up direction.</value>
    public Vector3D UpDirection { get; set; }

    /// <summary>
    /// The rotation angle of the text in counter-clockwise, in degrees.
    /// </summary>
    public double Angle { get; set; }
}
