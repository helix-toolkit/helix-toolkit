/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public class BorderRenderCore2D : RenderCore2DBase
    {
        private D2D.Brush background = null;
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public D2D.Brush Background
        {
            set
            {
                var old = background;
                if(SetAffectsRender(ref background, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
            }
            get { return background; }
        }

        private D2D.Brush strokeBrush = null;
        /// <summary>
        /// Gets or sets the stroke brush.
        /// </summary>
        /// <value>
        /// The stroke brush.
        /// </value>
        public D2D.Brush StrokeBrush
        {
            set
            {
                var old = strokeBrush;
                if (SetAffectsRender(ref strokeBrush, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
            }
            get
            {
                return strokeBrush;
            }
        }

        private float strokeThickness = 0;
        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// The stroke thickness.
        /// </value>
        public float StrokeThickness
        {
            set { SetAffectsRender(ref strokeThickness, value); }
            get { return strokeThickness; }
        }

        private D2D.StrokeStyle strokeStyle = null;
        /// <summary>
        /// Gets or sets the stroke style.
        /// </summary>
        /// <value>
        /// The stroke style.
        /// </value>
        public D2D.StrokeStyle StrokeStyle
        {
            set
            {
                var old = strokeStyle;
                if (SetAffectsRender(ref strokeStyle, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
            }
            get
            {
                return strokeStyle;
            }
        }

        private float cornerRadius = 0;
        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>
        /// The corner radius.
        /// </value>
        public float CornerRadius
        {
            set
            {
                SetAffectsRender(ref cornerRadius, value);
            }
            get { return cornerRadius; }
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void OnRender(IRenderContext2D context)
        {
            var roundRect = new D2D.RoundedRectangle() { Rect = LayoutBound, RadiusX = CornerRadius, RadiusY = CornerRadius };
            if(Background != null)
            {
                context.DeviceContext.FillRoundedRectangle(roundRect, Background);
            }
            if (StrokeThickness > 0 && StrokeBrush != null && StrokeStyle != null)
            {
                context.DeviceContext.DrawRoundedRectangle(roundRect, StrokeBrush, StrokeThickness, StrokeStyle);
            }
        }
    }
}
