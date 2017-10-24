using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    public class TextModel2D : Model2D
    {
        public static readonly string DefaultFont = "Arial";

        public static readonly DependencyProperty TextProperty 
            = DependencyProperty.Register("Text", typeof(string), typeof(TextModel2D), 
                new AffectsRenderPropertyMetadata("Text", (d,e)=>
                {
                    (d as TextModel2D).OnTextPropertyChanged();
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

        public static readonly DependencyProperty FontSizeProperty
            = DependencyProperty.Register("FontSize", typeof(int), typeof(TextModel2D),
                new AffectsRenderPropertyMetadata(12, (d, e) =>
                {
                    (d as TextModel2D).OnTextPropertyChanged();
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
                new AffectsRenderPropertyMetadata(DefaultFont, (d, e) =>
                {
                    (d as TextModel2D).OnTextPropertyChanged();
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
                new AffectsRenderPropertyMetadata(FontWeights.Normal, (d, e) =>
                {
                    (d as TextModel2D).OnTextPropertyChanged();
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
                new AffectsRenderPropertyMetadata(FontStyles.Normal, (d, e) =>
                {
                    (d as TextModel2D).OnTextPropertyChanged();
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

        private TextRenderable textRenderable;

        protected override IRenderable2D CreateRenderCore(IRenderHost host)
        {
            textRenderable = new TextRenderable();
            OnTextPropertyChanged();
            return textRenderable;
        }

        protected override void OnRender(RenderContext context)
        {
            textRenderable.Rect = this.Bound;
            textRenderable.Transform = transformMatrix;         
            renderCore.Render(context, RenderHost.D2DControls.D2DTarget);
        }

        protected virtual void OnTextPropertyChanged()
        {
            if (textRenderable == null) { return; }
            textRenderable.Text = Text == null ? "" : Text;
            textRenderable.Font = Font == null ? DefaultFont : Font;
            textRenderable.FontWeight = FontWeight.ToDXFontWeight();
            textRenderable.FontStyle = FontStyle.ToDXFontStyle();
            textRenderable.FontSize = FontSize;
        }
    }
}
