using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene2D;
#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
using HelixToolkit.WinUI.SharpDX.Extensions;
using Microsoft.UI;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Core2D;
using HelixToolkit.Avalonia.SharpDX.Extensions;
using Avalonia.Media;
using Avalonia.Metadata;
using FontWeights = Avalonia.Media.FontWeight;
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

#if false
#elif WINUI
[ContentProperty(Name = "Text")]
#elif WPF
[ContentProperty("Text")]
#elif AVALONIA
#else
#error Unknown framework
#endif
public class TextModel2D : Element2D, ITextBlock
{
    public static readonly string DefaultFont = "Arial";

    public static readonly DependencyProperty TextProperty =
        HelixProperty.Register<TextModel2D, string>("Text",
            "", (d, e) =>
            {
                if (d is Element2DCore { SceneNode: TextNode2D node })
                {
                    node.Text = e.NewValue == null ? string.Empty : (string)e.NewValue;
                }
            });

#if AVALONIA
    [Content]
#endif
    public string Text
    {
        set
        {
            SetValue(TextProperty, value);
        }
        get
        {
            return (string)GetValue(TextProperty)!;
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty ForegroundProperty =
        HelixProperty.Register<TextModel2D, Brush>("Foreground",
            new SolidColorBrush(Colors.Black), (d, e) =>
            {
                if (d is TextModel2D model)
                {
                    model.foregroundChanged = true;
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public Brush Foreground
    {
        set
        {
            SetValue(ForegroundProperty, value);
        }
        get
        {
            return (Brush)GetValue(ForegroundProperty)!;
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty BackgroundProperty =
        HelixProperty.Register<TextModel2D, Brush?>("Background",
            null, (d, e) =>
            {
                if (d is TextModel2D model)
                {
                    model.backgroundChanged = true;
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public Brush? Background
    {
        set
        {
            SetValue(BackgroundProperty, value);
        }
        get
        {
            return (Brush?)GetValue(BackgroundProperty);
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty FontSizeProperty =
        HelixProperty.Register<TextModel2D, int>("FontSize",
            12, (d, e) =>
            {
                if (d is Element2DCore { SceneNode: TextNode2D node })
                {
                    node.FontSize = Math.Max(1, (int)e.NewValue!);
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public int FontSize
    {
        set
        {
            SetValue(FontSizeProperty, value);
        }
        get
        {
            return (int)GetValue(FontSizeProperty)!;
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty FontWeightProperty =
        HelixProperty.Register<TextModel2D, FontWeight>("FontWeight",
            FontWeights.Normal, (d, e) =>
            {
                if (d is Element2DCore { SceneNode: TextNode2D node })
                {
                    node.FontWeight = ((FontWeight)e.NewValue!).ToDXFontWeight();
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public FontWeight FontWeight
    {
        set
        {
            SetValue(FontWeightProperty, value);
        }
        get
        {
            return (FontWeight)GetValue(FontWeightProperty)!;
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty FontStyleProperty =
        HelixProperty.Register<TextModel2D, UIFontStyle>("FontStyle",
            UIFontStyles.Normal, (d, e) =>
            {
                if (d is Element2DCore { SceneNode: TextNode2D node })
                {
                    node.FontStyle = ((FontStyle)e.NewValue!).ToDXFontStyle();
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public UIFontStyle FontStyle
    {
        set
        {
            SetValue(FontStyleProperty, value);
        }
        get
        {
            return (UIFontStyle)GetValue(FontStyleProperty)!;
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
            return (TextAlignment)GetValue(TextAlignmentProperty)!;
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
        HelixProperty.Register<TextModel2D, TextAlignment>("TextAlignment",
            TextAlignment.Left, (d, e) =>
        {
            if (d is Element2DCore { SceneNode: TextNode2D node })
            {
                node.TextAlignment = ((TextAlignment)e.NewValue!).ToD2DTextAlignment();
            }
        });

    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    /// <value>
    /// The text alignment.
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
    public FlowDirection FlowDirection
    {
        get
        {
            return (FlowDirection)GetValue(FlowDirectionProperty)!;
        }
        set
        {
            SetValue(FlowDirectionProperty, value);
        }
    }

    /// <summary>
    /// The text alignment property
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
    public static readonly DependencyProperty FlowDirectionProperty =
        HelixProperty.Register<TextModel2D, FlowDirection>("FlowDirection",
            FlowDirection.LeftToRight, (d, e) =>
        {
            if (d is Element2DCore { SceneNode: TextNode2D node })
            {
                node.FlowDirection = ((FlowDirection)e.NewValue!).ToD2DFlowDir();
            }
        });

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    /// <value>
    /// The font family.
    /// </value>
#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
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
#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty FontFamilyProperty =
        HelixProperty.Register<TextModel2D, string?>("FontFamily",
            DefaultFont, (d, e) =>
        {
            if (d is Element2DCore { SceneNode: TextNode2D node })
            {
                node.FontFamily = e.NewValue == null ? DefaultFont : (string)e.NewValue;
            }
        });

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
