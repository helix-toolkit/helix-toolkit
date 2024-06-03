using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;
using System.Reflection;

namespace HelixToolkit.Wpf;

/// <summary>
/// A visual element that contains a text billboard.
/// </summary>
public class BillboardTextVisual3D : BillboardVisual3D, IBoundsIgnoredVisual3D
{
    /// <summary>
    /// Identifies the <see cref="Background"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        "Background", typeof(Brush), typeof(BillboardTextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="BorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
        "BorderBrush", typeof(Brush), typeof(BillboardTextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="BorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderThicknessProperty =
        DependencyProperty.Register(
            "BorderThickness",
            typeof(Thickness),
            typeof(BillboardTextVisual3D),
            new UIPropertyMetadata(new Thickness(1), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontFamily"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
        "FontFamily", typeof(FontFamily), typeof(BillboardTextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        "FontSize", typeof(double), typeof(BillboardTextVisual3D), new UIPropertyMetadata(0.0, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
        "FontWeight",
        typeof(FontWeight),
        typeof(BillboardTextVisual3D),
        new UIPropertyMetadata(FontWeights.Normal, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
        "Foreground",
        typeof(Brush),
        typeof(BillboardTextVisual3D),
        new UIPropertyMetadata(Brushes.Black, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="HeightFactor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightFactorProperty = DependencyProperty.Register(
        "HeightFactor", typeof(double), typeof(BillboardTextVisual3D), new PropertyMetadata(1.0, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Padding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
        "Padding",
        typeof(Thickness),
        typeof(BillboardTextVisual3D),
        new UIPropertyMetadata(new Thickness(0), VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Text"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(BillboardTextVisual3D), new UIPropertyMetadata(null, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="MaterialType"/> dependency property.
    /// </summary>        
    public static readonly DependencyProperty MaterialTypeProperty =
        DependencyProperty.Register("MaterialType", typeof(MaterialType), typeof(BillboardTextVisual3D), new PropertyMetadata(MaterialType.Diffuse, VisualChanged));

    /// <summary>
    /// Identifies the <see cref="Angle"/> dependency property.
    /// </summary>        
    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register("AngleProperty", typeof(double), typeof(BillboardTextVisual3D), new UIPropertyMetadata(0.0, VisualChanged));

    /// <summary>
    /// Gets or sets the background.
    /// </summary>
    /// <value>The background.</value>
    public Brush Background
    {
        get
        {
            return (Brush)this.GetValue(BackgroundProperty);
        }

        set
        {
            this.SetValue(BackgroundProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the border brush.
    /// </summary>
    /// <value>The border brush.</value>
    public Brush BorderBrush
    {
        get
        {
            return (Brush)this.GetValue(BorderBrushProperty);
        }

        set
        {
            this.SetValue(BorderBrushProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the border thickness.
    /// </summary>
    /// <value>The border thickness.</value>
    public Thickness BorderThickness
    {
        get
        {
            return (Thickness)this.GetValue(BorderThicknessProperty);
        }

        set
        {
            this.SetValue(BorderThicknessProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    /// <value>The font family.</value>
    public FontFamily FontFamily
    {
        get
        {
            return (FontFamily)this.GetValue(FontFamilyProperty);
        }

        set
        {
            this.SetValue(FontFamilyProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the size of the font.
    /// </summary>
    /// <value>The size of the font.</value>
    public double FontSize
    {
        get
        {
            return (double)this.GetValue(FontSizeProperty);
        }

        set
        {
            this.SetValue(FontSizeProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the font weight.
    /// </summary>
    /// <value>The font weight.</value>
    public FontWeight FontWeight
    {
        get
        {
            return (FontWeight)this.GetValue(FontWeightProperty);
        }

        set
        {
            this.SetValue(FontWeightProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the foreground brush.
    /// </summary>
    /// <value>The foreground.</value>
    public Brush Foreground
    {
        get
        {
            return (Brush)this.GetValue(ForegroundProperty);
        }

        set
        {
            this.SetValue(ForegroundProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the height factor.
    /// </summary>
    /// <value>
    /// The height factor.
    /// </value>
    public double HeightFactor
    {
        get
        {
            return (double)this.GetValue(HeightFactorProperty);
        }

        set
        {
            this.SetValue(HeightFactorProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the type of the material.
    /// </summary>
    /// <value>The type of the material.</value>
    public MaterialType MaterialType
    {
        get
        {
            return (MaterialType)this.GetValue(MaterialTypeProperty);
        }

        set
        {
            this.SetValue(MaterialTypeProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    /// <value>The padding.</value>
    public Thickness Padding
    {
        get
        {
            return (Thickness)this.GetValue(PaddingProperty);
        }

        set
        {
            this.SetValue(PaddingProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
        get
        {
            return (string)this.GetValue(TextProperty);
        }

        set
        {
            this.SetValue(TextProperty, value);
        }
    }

    /// <summary>
    /// The rotation angle of the text in counter-clockwise, in degrees.
    /// </summary>
    public double Angle
    {
        get
        {
            return (double)this.GetValue(AngleProperty);
        }
        set
        {
            this.SetValue(AngleProperty, value);
        }
    }

    private RotateTransform? rotateTransform = null;

    /// <summary>
    /// The visual appearance changed.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
    private static void VisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((BillboardTextVisual3D)d).VisualChanged();
    }

    /// <summary>
    /// Updates the text block when the visual appearance changed.
    /// </summary>
    private void VisualChanged()
    {
        if (string.IsNullOrEmpty(this.Text))
        {
            this.Material = null;
            return;
        }

        var textBlock = new TextBlock(new Run(this.Text))
        {
            Foreground = this.Foreground,
            Background = this.Background,
            FontWeight = this.FontWeight,
            Padding = this.Padding
        };

        if (this.FontFamily != null)
        {
            textBlock.FontFamily = this.FontFamily;
        }

        if (this.FontSize > 0)
        {
            textBlock.FontSize = this.FontSize;
        }

        var element = this.BorderBrush != null
            ? (FrameworkElement)
            new Border
            {
                BorderBrush = this.BorderBrush,
                BorderThickness = this.BorderThickness,
                Child = textBlock
            }
            : textBlock;

        /*
         * In WPF 2D, rotates an object clockwise about a specified point in a 2-D x-y coordinate system.
         * In WPF 3D is a right-handed system, which means that a positive angle value for a rotation results in a counter-clockwise rotation about the axis.
         * So, to make consistent behavior with the default 3D coordinate system, a positive angle value for a rotation will rotate in a counter-clockwise
         * 
         * Only prevent assign when angle == 0, it is equal origin value 
         * https://stackoverflow.com/questions/10329298/performance-impact-of-applying-either-layouttransform-vs-rendertransform
         */
        double angle = -this.Angle;
        if (angle != 0 || (rotateTransform is not null && rotateTransform.Angle != angle))
        {
            rotateTransform ??= new();
            rotateTransform.Angle = angle;
            element.LayoutTransform = rotateTransform;
        }

        element.Measure(new Size(1000, 1000));
        element.Arrange(new Rect(element.DesiredSize));
        element.RenderSize = element.DesiredSize;

        var rtb = new RenderTargetBitmap(
            (int)element.ActualWidth + 1, (int)element.ActualHeight + 1, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(element);
        rtb.Freeze();
        (rtb.GetType().GetField("_renderTargetBitmap", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(rtb) as IDisposable)?.Dispose(); //https://github.com/dotnet/wpf/issues/3067
        var brush = new ImageBrush(rtb);

        if (this.MaterialType == MaterialType.Diffuse)
        {
            this.Material = new DiffuseMaterial(brush);
        }

        if (this.MaterialType == MaterialType.Emissive)
        {
            this.Material = new EmissiveMaterial(brush);
        }

        this.Width = element.ActualWidth;
        this.Height = element.ActualHeight * this.HeightFactor;
    }
}
