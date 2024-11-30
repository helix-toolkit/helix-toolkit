using HelixToolkit.SharpDX.Model.Scene2D;
using System.IO;
#if WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
#else
using HelixToolkit.Wpf.SharpDX.Core2D;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

/// <summary>
/// 
/// </summary>
/// <seealso cref="Element2D" />
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
#if WINUI
    public new double Opacity
#else
    public double Opacity
#endif
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
#if WINUI
    public static readonly new DependencyProperty OpacityProperty =
#else
    public static readonly DependencyProperty OpacityProperty =
#endif
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
