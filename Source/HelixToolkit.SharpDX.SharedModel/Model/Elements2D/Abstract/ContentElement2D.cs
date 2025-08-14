using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene2D;
using UIBindable = System.ComponentModel.BindableAttribute;
#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
using HelixToolkit.WinUI.SharpDX.Extensions;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Core2D;
using HelixToolkit.Avalonia.SharpDX.Extensions;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;
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
[ContentProperty(Name = "Content2D")]
#elif WPF
[ContentProperty("Content2D")]
#elif AVALONIA
#else
#error Unknown framework
#endif
public abstract class ContentElement2D : Element2D
{
    public static readonly DependencyProperty Content2DProperty =
        HelixProperty.Register<ContentElement2D, object?>("Content2D",
            null, (d, e) =>
        {
            if (d is not ContentElement2D model)
                return;
            if (model.SceneNode is not ContentNode2D node)
                return;
            if (e.OldValue is Element2D old)
            {
                model.RemoveLogicalChild(old);
                node.Content = null;
            }

            if (e.NewValue is Element2D newElement)
            {
                model.AddLogicalChild(newElement);
                node.Content = newElement;
                model.SetupBindings(newElement);
            }
            else
            {
                var element = new TextModel2D()
                {
                    Text = e.NewValue?.ToString() ?? string.Empty
                };

                model.AddLogicalChild(element);
                node.Content = element;
                model.SetupBindings(element);
            }

            model.InvalidateMeasure();
        });

    [UIBindable(true)]
#if AVALONIA
    [Content]
#endif
    public object? Content2D
    {
        set
        {
            SetValue(Content2DProperty, value);
        }
        get
        {
            return GetValue(Content2DProperty);
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
        HelixProperty.Register<ContentElement2D, Brush>("Background",
            new SolidColorBrush(UIColors.Transparent),
            (d, e) =>
            {
                if (d is ContentElement2D m)
                {
                    m.backgroundChanged = true;
                    m.InvalidateRender();
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
    public Brush Background
    {
        set
        {
            SetValue(BackgroundProperty, value);
        }
        get
        {
            return (Brush)GetValue(BackgroundProperty)!;
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
        HelixProperty.Register<ContentElement2D, Brush>("Foreground",
            new SolidColorBrush(UIColors.Black));

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
    public UIHorizontalAlignment HorizontalContentAlignment
    {
        get
        {
            return (UIHorizontalAlignment)GetValue(HorizontalContentAlignmentProperty)!;
        }
        set
        {
            SetValue(HorizontalContentAlignmentProperty, value);
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
    public static readonly DependencyProperty HorizontalContentAlignmentProperty =
        HelixProperty.Register<ContentElement2D, UIHorizontalAlignment>("HorizontalContentAlignment",
            UIHorizontalAlignment.Center, (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ContentNode2D node })
                {
                    node.HorizontalContentAlignment = ((UIHorizontalAlignment)e.NewValue!).ToD2DHorizontalAlignment();
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
    public UIVerticalAlignment VerticalContentAlignment
    {
        get
        {
            return (UIVerticalAlignment)GetValue(VerticalContentAlignmentProperty)!;
        }
        set
        {
            SetValue(VerticalContentAlignmentProperty, value);
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
    public static readonly DependencyProperty VerticalContentAlignmentProperty =
        HelixProperty.Register<ContentElement2D, UIVerticalAlignment>("VerticalContentAlignment",
            UIVerticalAlignment.Center, (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ContentNode2D node })
                {
                    node.VerticalContentAlignment = ((UIVerticalAlignment)e.NewValue!).ToD2DVerticalAlignment();
                }
            });

    private bool backgroundChanged = true;

    protected override void OnUpdate(RenderContext2D context)
    {
        base.OnUpdate(context);
        if (backgroundChanged)
        {
            if (SceneNode is ContentNode2D node)
            {
                node.Background = Background.ToD2DBrush(context.DeviceContext);
            }

            backgroundChanged = false;
        }
    }

    protected override void OnAttached()
    {
        backgroundChanged = true;
        base.OnAttached();
    }

    protected void SetupBindings(Element2D content)
    {
        if (content is TextModel2D)
        {
            var binding = new Binding
            {
                Source = this,
                Mode = BindingMode.OneWay,
#if false
#elif WINUI || WPF
                Path = new PropertyPath(nameof(Foreground))
#elif AVALONIA
                Path = nameof(Foreground)
#else
#error Unknown framework
#endif
            };

#if false

#elif WINUI || WPF
            BindingOperations.SetBinding(content, TextModel2D.ForegroundProperty, binding);
#elif AVALONIA
            content.Bind(TextModel2D.ForegroundProperty, binding);
#else
#error Unknown framework
#endif
        }
    }
}
