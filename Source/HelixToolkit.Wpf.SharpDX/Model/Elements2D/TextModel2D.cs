using System;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Markup;
using SharpDX;
using System.Linq;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using Utilities;
    using Extensions;
    using HelixToolkit.Wpf.SharpDX.Model.Scene2D;

    [ContentProperty("Text")]
    public class TextModel2D : Element2D, ITextBlock
    {
        public static readonly string DefaultFont = "Arial";

        public static readonly DependencyProperty TextProperty 
            = DependencyProperty.Register("Text", typeof(string), typeof(TextModel2D), 
                new PropertyMetadata("Text", (d,e)=>
                {
                    ((d as Element2DCore).SceneNode as TextNode2D).Text = e.NewValue == null ? "" : (string)e.NewValue;
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
                    ((d as Element2DCore).SceneNode as TextNode2D).FontSize = Math.Max(1, (int)e.NewValue);
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

        public static readonly DependencyProperty FontWeightProperty
            = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(TextModel2D),
                new PropertyMetadata(FontWeights.Normal, (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as TextNode2D).FontWeight = ((FontWeight)e.NewValue).ToDXFontWeight();
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
                    ((d as Element2DCore).SceneNode as TextNode2D).FontStyle = ((FontStyle)e.NewValue).ToDXFontStyle();
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


        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>
        /// The text alignment.
        /// </value>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>
        /// The text alignment property
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(TextModel2D), new PropertyMetadata(TextAlignment.Left, (d,e)=> 
            {
                ((d as Element2DCore).SceneNode as TextNode2D).TextAlignment = ((TextAlignment)e.NewValue).ToD2DTextAlignment();
            }));

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>
        /// The text alignment.
        /// </value>
        public FlowDirection FlowDirection
        {
            get { return (FlowDirection)GetValue(FlowDirectionProperty); }
            set { SetValue(FlowDirectionProperty, value); }
        }

        /// <summary>
        /// The text alignment property
        /// </summary>
        public static readonly DependencyProperty FlowDirectionProperty =
            DependencyProperty.Register("FlowDirection", typeof(FlowDirection), typeof(TextModel2D), new PropertyMetadata(FlowDirection.LeftToRight, (d, e) =>
            {
                ((d as Element2DCore).SceneNode as TextNode2D).FlowDirection = ((FlowDirection)e.NewValue).ToD2DFlowDir();
            }));


        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }
        /// <summary>
        /// The font family property
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(string), typeof(TextModel2D), new PropertyMetadata(DefaultFont, (d,e)=>
            {
                ((d as Element2DCore).SceneNode as TextNode2D).FontFamily = e.NewValue == null ? "Arial" : (string)e.NewValue;
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

        protected override void OnUpdate(IRenderContext2D context)
        {
            base.OnUpdate(context);
            if (foregroundChanged)
            {
                (SceneNode as TextNode2D).Foreground = Foreground != null ? Foreground.ToD2DBrush(context.DeviceContext) : null;
                foregroundChanged = false;
            }
            if (backgroundChanged)
            {
                (SceneNode as TextNode2D).Background = Background != null ? Background.ToD2DBrush(context.DeviceContext) : null;
                backgroundChanged = false;
            }
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode2D node)
        {
            var t = node as TextNode2D;
            t.Text = Text == null ? "" : Text;
            t.FontFamily = FontFamily == null ? DefaultFont : FontFamily;
            t.FontWeight = FontWeight.ToDXFontWeight();
            t.FontStyle = FontStyle.ToDXFontStyle();
            t.FontSize = FontSize;
            t.TextAlignment = TextAlignment.ToD2DTextAlignment();
            t.FlowDirection = FlowDirection.ToD2DFlowDir();
        }
    }
}
