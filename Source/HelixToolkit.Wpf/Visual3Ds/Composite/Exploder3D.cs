using System.Windows.Media.Media3D;
using System.Windows;

namespace HelixToolkit.Wpf;

/// <summary>
/// A visual element that transforms all child elements as an explosion.
/// </summary>
public class Exploder3D : ModelVisual3D
{
    /// <summary>
    /// Identifies the <see cref="IsExploding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsExplodingProperty = DependencyProperty.Register(
        "IsExploding", typeof(bool), typeof(Exploder3D), new UIPropertyMetadata(false, IsExplodingChanged));

    /// <summary>
    /// Called when the exploding state changed.
    /// </summary>
    private static void IsExplodingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Exploder3D)d).OnIsExplodingChanged();
    }

    /// <summary>
    /// Called when the exploding state changed.
    /// </summary>
    protected virtual void OnIsExplodingChanged()
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is exploding.
    /// </summary>
    /// <value>
    ///  <c>true</c> if this instance is exploding; otherwise, <c>false</c>.
    /// </value>
    public bool IsExploding
    {
        get
        {
            return (bool)this.GetValue(IsExplodingProperty);
        }

        set
        {
            this.SetValue(IsExplodingProperty, value);
        }
    }

    // todo
}
