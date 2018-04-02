using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using Extensions;
    using Model.Scene2D;
    using System.Windows.Data;

    [ContentProperty("Content2D")]
    public abstract class ContentElement2D : Element2D
    {
        public static readonly DependencyProperty Content2DProperty = DependencyProperty.Register("Content2D", typeof(object), typeof(ContentElement2D), 
            new PropertyMetadata(null, (d,e)=>
            {
                var model = d as ContentElement2D;
                var node = model.SceneNode as ContentNode2D;
                if(e.OldValue is Element2D old)
                {
                    model.RemoveLogicalChild(old);                    
                    node.Content = null;
                }
                if(e.NewValue is Element2D newElement)
                {
                    model.AddLogicalChild(newElement);
                    node.Content = newElement;
                    model.SetupBindings(newElement);
                }
            },
                (d, e)=>
            {
                if(e is Element2D)
                {
                    return e;
                }
                else
                {
                    return new TextModel2D() { Text = e.ToString() };
                }
            }));

        [Bindable(true)]
        public object Content2D
        {
            set
            {
                SetValue(Content2DProperty, value);
            }
            get
            {
                return (Element2D)GetValue(Content2DProperty);
            }
        }

        public static readonly DependencyProperty BackgroundProperty
            = DependencyProperty.Register("Background", typeof(Brush), typeof(ContentElement2D),
                new PropertyMetadata(new SolidColorBrush(Colors.Transparent), 
                (d,e)=>
                {
                    var m = d as ContentElement2D;
                    m.backgroundChanged = true;
                    m.InvalidateRender();
                }));

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

        public static readonly DependencyProperty ForegroundProperty
            = DependencyProperty.Register("Foreground", typeof(Brush), typeof(ContentElement2D),
        new PropertyMetadata(new SolidColorBrush(Colors.Black)));

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

        public System.Windows.HorizontalAlignment HorizontalContentAlignment
        {
            get { return (System.Windows.HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register("HorizontalContentAlignment", typeof(System.Windows.HorizontalAlignment), typeof(ContentElement2D), 
                new PropertyMetadata(System.Windows.HorizontalAlignment.Center, (d,e)=>
                {
                    ((d as Element2DCore).SceneNode as ContentNode2D).HorizontalContentAlignment = ((System.Windows.HorizontalAlignment)e.NewValue).ToD2DHorizontalAlignment();
                }));


        public System.Windows.VerticalAlignment VerticalContentAlignment
        {
            get { return (System.Windows.VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register("VerticalContentAlignment", typeof(System.Windows.VerticalAlignment), typeof(ContentElement2D), 
                new PropertyMetadata(System.Windows.VerticalAlignment.Center, (d, e) => 
                {
                    ((d as Element2DCore).SceneNode as ContentNode2D).VerticalContentAlignment = ((System.Windows.VerticalAlignment)e.NewValue).ToD2DVerticalAlignment();
                }));

        private bool backgroundChanged = true;

        protected override void OnUpdate(IRenderContext2D context)
        {
            base.OnUpdate(context);
            if (backgroundChanged)
            {
                (SceneNode as ContentNode2D).Background = Background.ToD2DBrush(context.DeviceContext);
                backgroundChanged = false;
            }
        }

        protected void SetupBindings(Element2D content)
        {
            if (content is TextModel2D)
            {
                var binding = new Binding(nameof(Foreground));
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.Path = new PropertyPath(nameof(Foreground));
                BindingOperations.SetBinding(content, TextModel2D.ForegroundProperty, binding);
            }
        }
    }
}
