using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.SharpDX.Core2D;
using SharpDX;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    public class TextModel2D : Model2D
    {
        public static readonly DependencyProperty TextProperty 
            = DependencyProperty.Register("Text", typeof(string), typeof(TextModel2D), 
                new AffectsRenderPropertyMetadata("Text", (d,e)=>
                {
                    (d as TextModel2D).text = e.NewValue == null ? "" : (string)e.NewValue;
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

        private TextRenderable textRenderable;
        private string text = "Text";

        protected override IRenderable2D CreateRenderCore(IRenderHost host)
        {
            textRenderable = new TextRenderable();
            return textRenderable;
        }

        protected override void OnRender(RenderContext context)
        {
            textRenderable.Rect = this.Bound;
            textRenderable.Transform = transformMatrix;
            textRenderable.Text = text;
            renderCore.Render(context, RenderHost.D2DControls.D2DTarget);
        }
    }
}
