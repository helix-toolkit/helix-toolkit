/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public class TextRenderable : RenderCore2DBase
    {
        public string Text { set; get; } = "Text";

        private D2D.Brush foreground = null;
        public D2D.Brush Foreground
        {
            set
            {
                Set(ref foreground, value);
            }
            get
            {
                return foreground;
            }
        }

        private string fontFamily = "Arial";
        public string FontFamily
        {
            set
            {
                if(Set(ref fontFamily, value) && IsAttached)
                {
                    UpdateFontFormat();
                }
            }
            get
            {
                return fontFamily;
            }
        }

        private int fontSize = 12;
        public int FontSize
        {
            set
            {
                if(Set(ref fontSize, value) && IsAttached)
                {
                    UpdateFontFormat();
                }
            }
            get { return fontSize; }
        }

        private FontWeight fontWeight = FontWeight.Normal;
        public FontWeight FontWeight
        {
            set
            {
                if(Set(ref fontWeight, value) && IsAttached)
                {
                    UpdateFontFormat();
                }
            }
            get { return fontWeight; }
        }

        private FontStyle fontStyle = FontStyle.Normal;
        public FontStyle FontStyle
        {
            set
            {
                if(Set(ref fontStyle, value) && IsAttached)
                {
                    UpdateFontFormat();
                }
            }
            get { return fontStyle; }
        } 

        public D2D.DrawTextOptions DrawingOptions { set; get; } = D2D.DrawTextOptions.None;

        private Factory textFactory;
        private TextFormat textFormat;

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                textFactory = Collect(new Factory(FactoryType.Isolated));
                textFormat = Collect(new TextFormat(textFactory, FontFamily, FontWeight, FontStyle, FontSize));
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdateFontFormat()
        {
            RemoveAndDispose(ref textFormat);
            textFormat = Collect(new TextFormat(textFactory, FontFamily, FontWeight, FontStyle, FontSize));
        }

        protected override bool CanRender(IRenderContext2D context)
        {
            return base.CanRender(context) && Foreground != null;
        }

        protected override void OnRender(IRenderContext2D context)
        {
            context.DeviceContext.DrawText(Text, textFormat,
               Bound, Foreground, DrawingOptions);
        }
    }
}
