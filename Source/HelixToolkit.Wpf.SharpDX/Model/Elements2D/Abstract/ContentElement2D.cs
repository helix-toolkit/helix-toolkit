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
    using Extensions;
    using Core2D;
    using System;

    [ContentProperty("Content2D")]
    public abstract class ContentElement2D : Element2D
    {
        public static readonly DependencyProperty Content2DProperty = DependencyProperty.Register("Content2D", typeof(object), typeof(ContentElement2D), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure,
                (d,e)=>
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
                    }
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

        public static readonly DependencyProperty ForegroundProperty
            = DependencyProperty.Register("Foreground", typeof(Brush), typeof(ContentElement2D),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

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
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), FrameworkPropertyMetadataOptions.AffectsRender,
                (d,e)=>
                {
                    (d as ContentElement2D).backgroundChanged = true;
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


        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ContentElement2D), 
                new FrameworkPropertyMetadata(HorizontalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsMeasure));


        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(ContentElement2D), 
                new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsMeasure));

        protected Element2D contentInternal { private set; get; }

        public override IEnumerable<IRenderable2D> Items
        {
            get
            {
                return contentInternal == null ? Enumerable.Empty<IRenderable2D>() : Enumerable.Repeat<IRenderable2D>(contentInternal, 1);
            }
        }

        private bool backgroundChanged = true;

        protected BorderRenderCore2D borderCore { private set; get; }

        protected override IRenderCore2D CreateRenderCore()
        {
            borderCore = new BorderRenderCore2D();
            return borderCore;
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
                backgroundChanged = true;
                contentInternal?.Attach(host);
                if (contentInternal != null && contentInternal.Parent == null)
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
            if (contentInternal != null && contentInternal.Parent == this)
            {
                this.RemoveLogicalChild(contentInternal);
            }
            base.OnDetach();
        }

        public override void Update(IRenderContext2D context)
        {
            base.Update(context);
            if (backgroundChanged)
            {
                borderCore.Background = Background.ToD2DBrush(context.DeviceContext);
                backgroundChanged = false;
            }
        }

        protected override bool CanRender(IRenderContext2D context)
        {
            return base.CanRender(context);
        }

        protected override bool CanHitTest()
        {
            return base.CanHitTest();
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            if(contentInternal != null && LayoutBoundWithTransform.Contains(mousePoint))
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
            Size2F maxContentSize = new Size2F();
            foreach(var item in Items)
            {
                if(item is Element2D e)
                {
                    e.HorizontalAlignment = HorizontalContentAlignment;
                    e.VerticalAlignment = VerticalContentAlignment;
                    e.Measure(availableSize);
                    maxContentSize.Width = Math.Max(maxContentSize.Width, e.DesiredSize.X);
                    maxContentSize.Height = Math.Max(maxContentSize.Height, e.DesiredSize.Y);
                }
            }
            if(HorizontalAlignmentInternal == HorizontalAlignment.Center)
            {
                availableSize.Width = Math.Min(availableSize.Width, maxContentSize.Width);
            }
            else
            {
                if (float.IsInfinity(availableSize.Width))
                {
                    if (float.IsInfinity(WidthInternal))
                    {
                        availableSize.Width = maxContentSize.Width;
                    }
                    else
                    {
                        availableSize.Width = WidthInternal;
                    }
                }
            }

            if(VerticalAlignmentInternal == VerticalAlignment.Center)
            {
                availableSize.Height = Math.Min(availableSize.Height, maxContentSize.Height);
            }
            else
            {
                if (float.IsInfinity(availableSize.Height))
                {
                    if (float.IsInfinity(HeightInternal))
                    {
                        availableSize.Height = maxContentSize.Height;
                    }
                    else
                    {
                        availableSize.Height = HeightInternal;
                    }
                }
            }

            return availableSize;
        }
    }
}
