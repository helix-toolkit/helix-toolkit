using System;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Markup;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using Utilities;    
    using Extensions;

    [ContentProperty("Text")]
    [TypeConverter(typeof(StringToModel2DConverter))]
    public class TextModel2D : Element2D, ITextBlock
    {
        public static readonly string DefaultFont = "Arial";

        public static readonly DependencyProperty TextProperty 
            = DependencyProperty.Register("Text", typeof(string), typeof(TextModel2D), 
                new PropertyMetadata("Text", (d,e)=>
                {
                    var model = (d as TextModel2D);
                    if (model.textRenderable == null) { return; }
                    model.textRenderable.Text = e.NewValue == null ? "" : (string)e.NewValue;
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


        public static readonly DependencyProperty ForegroundProperty
            = DependencyProperty.Register("Foreground", typeof(Brush), typeof(TextModel2D),
                new PropertyMetadata(new SolidColorBrush(Colors.Black), (d, e) =>
                {
                    var model = (d as TextModel2D);
                    model.foregroundChanged = true;
                }));

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
            = DependencyProperty.Register("Background", typeof(Brush), typeof(TextModel2D),
                new PropertyMetadata(null, (d, e) =>
            {
                var model = (d as TextModel2D);
                model.backgroundChanged = true;
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

        public static readonly DependencyProperty FontSizeProperty
            = DependencyProperty.Register("FontSize", typeof(int), typeof(TextModel2D),
                new PropertyMetadata(12, (d, e) =>
                {
                    var model = (d as TextModel2D);
                    if (model.textRenderable == null) { return; }
                    model.textRenderable.FontSize = Math.Max(1, (int)e.NewValue);
                }));

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

        public static readonly DependencyProperty FontProperty
            = DependencyProperty.Register("Font", typeof(string), typeof(TextModel2D),
                new PropertyMetadata(DefaultFont, (d, e) =>
                {
                    var model = (d as TextModel2D);
                    if (model.textRenderable == null) { return; }
                    model.textRenderable.FontFamily = e.NewValue == null ? "Arial" : (string)e.NewValue;
                }));

        public string Font
        {
            set
            {
                SetValue(FontProperty, value);
            }
            get
            {
                return (string)GetValue(FontProperty);
            }
        }

        public static readonly DependencyProperty FontWeightProperty
            = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(TextModel2D),
                new PropertyMetadata(FontWeights.Normal, (d, e) =>
                {
                    var model = (d as TextModel2D);
                    if (model.textRenderable == null) { return; }
                    model.textRenderable.FontWeight = ((FontWeight)e.NewValue).ToDXFontWeight();
                }));

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

        public static readonly DependencyProperty FontStyleProperty
            = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(TextModel2D),
                new PropertyMetadata(FontStyles.Normal, (d, e) =>
                {
                    var model = (d as TextModel2D);
                    if (model.textRenderable == null) { return; }
                    model.textRenderable.FontStyle = ((FontStyle)e.NewValue).ToDXFontStyle();
                }));

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

        private TextRenderCore2D textRenderable;
        private bool foregroundChanged = true;
        private bool backgroundChanged = true;

        protected override IRenderCore2D CreateRenderCore()
        {
            textRenderable = new TextRenderCore2D();
            AssignProperties();
            return textRenderable;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                foregroundChanged = true;
                backgroundChanged = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Update(IRenderContext2D context)
        {
            base.Update(context);
            if (foregroundChanged)
            {
                textRenderable.Foreground = Foreground != null ? Foreground.ToD2DBrush(context.DeviceContext) : null;
                foregroundChanged = false;
            }
            if (backgroundChanged)
            {
                textRenderable.Background = Background != null ? Background.ToD2DBrush(context.DeviceContext) : null;
                backgroundChanged = false;
            }
        }

        protected virtual void AssignProperties()
        {
            if (textRenderable == null) { return; }
            textRenderable.Text = Text == null ? "" : Text;
            textRenderable.FontFamily = Font == null ? DefaultFont : Font;
            textRenderable.FontWeight = FontWeight.ToDXFontWeight();
            textRenderable.FontStyle = FontStyle.ToDXFontStyle();
            textRenderable.FontSize = FontSize;
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            if (LayoutBoundWithTransform.Contains(mousePoint))
            {
                hitResult = new HitTest2DResult(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            textRenderable.MaxWidth = availableSize.Width;
            textRenderable.MaxHeight = availableSize.Height;
            var metrices = textRenderable.Metrices;
            return new Size2F(metrices.WidthIncludingTrailingWhitespace, metrices.Height);
        }

        protected override RectangleF ArrangeOverride(RectangleF finalSize)
        {
            textRenderable.MaxWidth = finalSize.Width;
            textRenderable.MaxHeight = finalSize.Height;
            var metrices = textRenderable.Metrices;
            return finalSize;
        }
    }
}
