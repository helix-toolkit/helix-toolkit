using HelixToolkit.SharpDX.Model.Scene2D;
using System.IO;
#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Core2D;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Core2D;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX.Elements2D;
#else
#error Unknown framework
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
        HelixProperty.Register<ImageModel2D, Stream?>("ImageStream",
            null,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ImageNode2D node })
                {
                    node.ImageStream = e.NewValue as Stream;
                }
            });

    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    /// <value>
    /// The opacity.
    /// </value>
#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public double Opacity
    {
        get
        {
            return (double)GetValue(OpacityProperty)!;
        }
        set
        {
            SetValue(OpacityProperty, value);
        }
    }

    /// <summary>
    /// The opacity property
    /// </summary>
#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty OpacityProperty =
        HelixProperty.Register<ImageModel2D, double>("Opacity",
            1.0, (d, e) =>
        {
            if (d is Element2DCore { SceneNode: ImageNode2D node })
            {
                node.Opacity = (float)(double)e.NewValue!;
            }
        });

    protected override SceneNode2D OnCreateSceneNode()
    {
        return new ImageNode2D();
    }
}
