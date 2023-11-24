using HelixToolkit.SharpDX.Model.Scene2D;
using HelixToolkit.Wpf.SharpDX.Core2D;
using System.IO;

namespace HelixToolkit.Wpf.SharpDX.Elements2D;

/// <summary>
/// 
/// </summary>
/// <seealso cref="HelixToolkit.Wpf.SharpDX.Elements2D.Element2D" />
public class ImageModel2D : Element2D
{
    /// <summary>
    /// Gets or sets the image stream.
    /// </summary>
    /// <value>
    /// The image stream.
    /// </value>
    public Stream? ImageStream
    {
        get
        {
            return (Stream?)GetValue(ImageStreamProperty);
        }
        set
        {
            SetValue(ImageStreamProperty, value);
        }
    }

    /// <summary>
    /// The image stream property
    /// </summary>
    public static readonly DependencyProperty ImageStreamProperty =
        DependencyProperty.Register("ImageStream", typeof(Stream), typeof(ImageModel2D), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ImageNode2D node })
                {
                    node.ImageStream = e.NewValue as Stream;
                }
            }));


    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    /// <value>
    /// The opacity.
    /// </value>
    public double Opacity
    {
        get
        {
            return (double)GetValue(OpacityProperty);
        }
        set
        {
            SetValue(OpacityProperty, value);
        }
    }

    /// <summary>
    /// The opacity property
    /// </summary>
    public static readonly DependencyProperty OpacityProperty =
        DependencyProperty.Register("Opacity", typeof(double), typeof(ImageModel2D), new PropertyMetadata(1.0, (d, e) =>
        {
            if (d is Element2DCore { SceneNode: ImageNode2D node })
            {
                node.Opacity = (float)(double)e.NewValue;
            }
        }));

    protected override SceneNode2D OnCreateSceneNode()
    {
        return new ImageNode2D();
    }
}
