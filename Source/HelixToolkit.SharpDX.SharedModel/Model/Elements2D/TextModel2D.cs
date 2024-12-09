using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene2D;
#if WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
using HelixToolkit.WinUI.SharpDX.Extensions;
using Microsoft.UI;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#else
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

#if WINUI
[ContentProperty(Name = "Text")]
#else
[ContentProperty("Text")]
#endif
public class TextModel2D : Element2D, ITextBlock
{
    public static readonly string DefaultFont = "Arial";

    public static readonly DependencyProperty TextProperty
        = DependencyProperty.Register("Text", typeof(string), typeof(TextModel2D),
            new PropertyMetadata("Text", (d, e) =>
            {
                if (d is Element2DCore { SceneNode: TextNode2D node })
                {
                    node.Text = e.NewValue == null ? string.Empty : (string)e.NewValue;
                }
            }));

    public string Text
    {
        set
        {
            SetValue(TextProperty, value);
        }
        get
        {
            return (string)GetValue(TextProperty);
        }
    }


#if WINUI
    new
#endif
    public static readonly DependencyProperty ForegroundProperty
        = DependencyProperty.Register("Foreground", typeof(Brush), typeof(TextModel2D),
            new PropertyMetadata(new SolidColorBrush(Colors.Black), (d, e) =>
            {
                if (d is TextModel2D model)
                {
                    model.foregroundChanged = true;
                }
            }));

#if WINUI
    new
#endif
    public Brush Foreground
    {
        set
        {
            SetValue(ForegroundProperty, value);
        }
        get
        {
            return (Brush)GetValue(ForegroundProperty);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty BackgroundProperty
        = DependencyProperty.Register("Background", typeof(Brush), typeof(TextModel2D),
            new PropertyMetadata(null, (d, e) =>
            {
                if (d is TextModel2D model)
                {
                    model.backgroundChanged = true;
                }
            }));

#if WINUI
    new
#endif
    public Brush Background
    {
        set
        {
            SetValue(BackgroundProperty, value);
        }
        get
        {
            return (Brush)GetValue(BackgroundProperty);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty FontSizeProperty
        = DependencyProperty.Register("FontSize", typeof(int), typeof(TextModel2D),
            new PropertyMetadata(12, (d, e) =>
            {
                if (d is Element2DCore { SceneNode: TextNode2D node })
                {
                    node.FontSize = Math.Max(1, (int)e.NewValue);
                }
            }));

#if WINUI
    new
#endif
    public int FontSize
    {
        set
        {
            SetValue(FontSizeProperty, value);
        }
        get
        {
            return (int)GetValue(FontSizeProperty);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty FontWeightProperty
        = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(TextModel2D),
            new PropertyMetadata(FontWeights.Normal, (d, e) =>
            {
                if (d is Element2DCore { SceneNode: TextNode2D node })
                {
                    node.FontWeight = ((FontWeight)e.NewValue).ToDXFontWeight();
                }
            }));

#if WINUI
    new
#endif
    public FontWeight FontWeight
    {
        set
        {
            SetValue(FontWeightProperty, value);
        }
        get
        {
            return (FontWeight)GetValue(FontWeightProperty);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty FontStyleProperty
        = DependencyProperty.Register("FontStyle", typeof(UIFontStyle), typeof(TextModel2D),
            new PropertyMetadata(UIFontStyles.Normal, (d, e) =>
            {
                if (d is Element2DCore { SceneNode: TextNode2D node })
                {
                    node.FontStyle = ((FontStyle)e.NewValue).ToDXFontStyle();
                }
            }));

#if WINUI
    new
#endif
    public FontStyle FontStyle
    {
        set
        {
            SetValue(FontStyleProperty, value);
        }
        get
        {
            return (FontStyle)GetValue(FontStyleProperty);
        }
    }


    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    /// <value>
    /// The text alignment.
    /// </value>
    public TextAlignment TextAlignment
    {
        get
        {
            return (TextAlignment)GetValue(TextAlignmentProperty);
        }
        set
        {
            SetValue(TextAlignmentProperty, value);
        }
    }

    /// <summary>
    /// The text alignment property
    /// </summary>
    public static readonly DependencyProperty TextAlignmentProperty =
        DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(TextModel2D), new PropertyMetadata(TextAlignment.Left, (d, e) =>
        {
            if (d is Element2DCore { SceneNode: TextNode2D node })
            {
                node.TextAlignment = ((TextAlignment)e.NewValue).ToD2DTextAlignment();
            }
        }));

    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    /// <value>
    /// The text alignment.
    /// </value>
#if WINUI
    public new FlowDirection FlowDirection
#else
    public FlowDirection FlowDirection
#endif
    {
        get
        {
            return (FlowDirection)GetValue(FlowDirectionProperty);
        }
        set
        {
            SetValue(FlowDirectionProperty, value);
        }
    }

    /// <summary>
    /// The text alignment property
    /// </summary>
#if WINUI
    public static readonly new DependencyProperty FlowDirectionProperty =
#else
    public static readonly DependencyProperty FlowDirectionProperty =
#endif
        DependencyProperty.Register("FlowDirection", typeof(FlowDirection), typeof(TextModel2D), new PropertyMetadata(FlowDirection.LeftToRight, (d, e) =>
        {
            if (d is Element2DCore { SceneNode: TextNode2D node })
            {
                node.FlowDirection = ((FlowDirection)e.NewValue).ToD2DFlowDir();
            }
        }));


    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    /// <value>
    /// The font family.
    /// </value>
#if WINUI
    new
#endif
    public string? FontFamily
    {
        get
        {
            return (string?)GetValue(FontFamilyProperty);
        }
        set
        {
            SetValue(FontFamilyProperty, value);
        }
    }
    /// <summary>
    /// The font family property
    /// </summary>
#if WINUI
    new
#endif
    public static readonly DependencyProperty FontFamilyProperty =
        DependencyProperty.Register("FontFamily", typeof(string), typeof(TextModel2D), new PropertyMetadata(DefaultFont, (d, e) =>
        {
            if (d is Element2DCore { SceneNode: TextNode2D node })
            {
                node.FontFamily = e.NewValue == null ? DefaultFont : (string)e.NewValue;
            }
        }));

    private bool foregroundChanged = true;
    private bool backgroundChanged = true;

    protected override SceneNode2D OnCreateSceneNode()
    {
        return new TextNode2D();
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        foregroundChanged = true;
        backgroundChanged = true;
    }

    protected override void OnUpdate(RenderContext2D context)
    {
        base.OnUpdate(context);

        if (foregroundChanged)
        {
            if (SceneNode is TextNode2D node)
            {
                node.Foreground = Foreground?.ToD2DBrush(context.DeviceContext);
            }

            foregroundChanged = false;
        }

        if (backgroundChanged)
        {
            if (SceneNode is TextNode2D node)
            {
                node.Background = Background?.ToD2DBrush(context.DeviceContext);
            }

            backgroundChanged = false;
        }
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode2D node)
    {
        if (node is not TextNode2D t)
        {
            return;
        }

        t.Text = Text ?? string.Empty;
        t.FontFamily = FontFamily ?? DefaultFont;
        t.FontWeight = FontWeight.ToDXFontWeight();
        t.FontStyle = FontStyle.ToDXFontStyle();
        t.FontSize = FontSize;
        t.TextAlignment = TextAlignment.ToD2DTextAlignment();
        t.FlowDirection = FlowDirection.ToD2DFlowDir();
    }
}
