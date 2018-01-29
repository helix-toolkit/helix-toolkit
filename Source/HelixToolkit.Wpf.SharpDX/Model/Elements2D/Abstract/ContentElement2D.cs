using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    [ContentProperty("Content2D")]
    public abstract class ContentElement2D : Clickable2D
    {
        public static readonly DependencyProperty Content2DProperty = DependencyProperty.Register("Content2D", typeof(object), typeof(ContentElement2D), 
            new PropertyMetadata(null, (d,e)=>
            {
                var model = d as ContentElement2D;
                if(e.NewValue is Element2D element)
                {
                    if (model.contentInternal != null)
                    {
                        model.RemoveLogicalChild(model.contentInternal);
                        model.contentInternal.Detach();
                        BindingOperations.ClearAllBindings(model.contentInternal);
                    }
                    model.contentInternal = element;
                }
                else
                {
                    if(!(model.contentInternal is TextModel2D) && model.contentInternal != null)
                    {
                        model.RemoveLogicalChild(model.contentInternal);
                        model.contentInternal.Detach();
                        BindingOperations.ClearAllBindings(model.contentInternal);
                    }
                    string  s = e.NewValue.ToString();
                    TextModel2D txtModel;
                    if (model.contentInternal is TextModel2D m)
                    {
                        txtModel = m;
                    }
                    else
                    {
                        txtModel =  new TextModel2D();
                        model.contentInternal = txtModel;
                    }
                    txtModel.Text = s;
                    model.SetupBindings(model.contentInternal);
                }
                if (model.contentInternal != null)
                {
                    model.AddLogicalChild(model.contentInternal);
                    if (model.IsAttached)
                    {
                        model.contentInternal.Attach(model.RenderHost);
                        model.contentInternal.Measure(model.RenderSize);
                    }
                }
            }));

        [BindableAttribute(true)]
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

        public static readonly DependencyProperty BackgroundProperty
            = DependencyProperty.Register("Background", typeof(Brush), typeof(ContentElement2D),
                new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

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


        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ContentElement2D), 
                new FrameworkPropertyMetadata(HorizontalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsMeasure));


        public VerticalAlignment VerticalContentAlighment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlighmentProperty); }
            set { SetValue(VerticalContentAlighmentProperty, value); }
        }

        public static readonly DependencyProperty VerticalContentAlighmentProperty =
            DependencyProperty.Register("VerticalContentAlighment", typeof(VerticalAlignment), typeof(ContentElement2D), 
                new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsMeasure));

        private Element2D contentInternal;

        public override IEnumerable<IRenderable2D> Items
        {
            get
            {
                return contentInternal == null ? Enumerable.Empty<IRenderable2D>() : Enumerable.Repeat<IRenderable2D>(contentInternal, 1);
            }
        }

        protected void SetupBindings(Element2D content)
        {
            if(contentInternal is TextModel2D)
            {
                var binding = new Binding(nameof(Foreground));
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.Path = new PropertyPath(nameof(Foreground));
                BindingOperations.SetBinding(content, TextModel2D.ForegroundProperty, binding);
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                contentInternal?.Attach(host);
                if (contentInternal.Parent == null)
                {
                    this.AddLogicalChild(contentInternal);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            contentInternal?.Detach();
            if (contentInternal.Parent == this)
            {
                this.RemoveLogicalChild(contentInternal);
            }
            base.OnDetach();
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            if (contentInternal != null)
            {
                return contentInternal.HitTest(mousePoint, out hitResult);
            }
            else
            {
                hitResult = null;
                return false;
            }
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            foreach(var item in Items)
            {
                if(item is Element2D e)
                {
                    e.HorizontalAlignment = HorizontalContentAlignment;
                    e.VerticalAlignment = VerticalContentAlighment;
                }
            }
            return base.MeasureOverride(availableSize);
        }
    }
}
