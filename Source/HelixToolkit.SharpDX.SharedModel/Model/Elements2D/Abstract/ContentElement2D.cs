using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene2D;
using UIBindable = System.ComponentModel.BindableAttribute;
#if WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
using HelixToolkit.WinUI.SharpDX.Extensions;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
using VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment;
#else
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using VerticalAlignment = System.Windows.VerticalAlignment;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

#if WINUI
[ContentProperty(Name = "Content2D")]
#else
[ContentProperty("Content2D")]
#endif
public abstract class ContentElement2D : Element2D
{
    public static readonly DependencyProperty Content2DProperty = DependencyProperty.Register("Content2D",
        typeof(object), typeof(ContentElement2D), new PropertyMetadata(null, (d, e) =>
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
        }));

    [UIBindable(true)]
    public object Content2D
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

#if WINUI
    new
#endif
    public static readonly DependencyProperty BackgroundProperty
        = DependencyProperty.Register("Background", typeof(Brush), typeof(ContentElement2D),
            new PropertyMetadata(new SolidColorBrush(UIColors.Transparent),
            (d, e) =>
            {
                if (d is ContentElement2D m)
                {
                    m.backgroundChanged = true;
                    m.InvalidateRender();
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
    public static readonly DependencyProperty ForegroundProperty
        = DependencyProperty.Register("Foreground", typeof(Brush), typeof(ContentElement2D),
    new PropertyMetadata(new SolidColorBrush(UIColors.Black)));

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
    public HorizontalAlignment HorizontalContentAlignment
    {
        get
        {
            return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty);
        }
        set
        {
            SetValue(HorizontalContentAlignmentProperty, value);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty HorizontalContentAlignmentProperty =
        DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ContentElement2D),
            new PropertyMetadata(HorizontalAlignment.Center, (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ContentNode2D node })
                {
                    node.HorizontalContentAlignment = ((HorizontalAlignment)e.NewValue).ToD2DHorizontalAlignment();
                }
            }));

#if WINUI
    new
#endif
    public VerticalAlignment VerticalContentAlignment
    {
        get
        {
            return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty);
        }
        set
        {
            SetValue(VerticalContentAlignmentProperty, value);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty VerticalContentAlignmentProperty =
        DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(ContentElement2D),
            new PropertyMetadata(VerticalAlignment.Center, (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ContentNode2D node })
                {
                    node.VerticalContentAlignment = ((VerticalAlignment)e.NewValue).ToD2DVerticalAlignment();
                }
            }));

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
                Path = new PropertyPath(nameof(Foreground))
            };
            BindingOperations.SetBinding(content, TextModel2D.ForegroundProperty, binding);
        }
    }
}
