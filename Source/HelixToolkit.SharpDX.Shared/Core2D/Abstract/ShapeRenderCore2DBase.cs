/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using D2D = global::SharpDX.Direct2D1;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core2D
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract class ShapeRenderCore2DBase : RenderCore2DBase
        {
            private D2D.Brush fillBrush = null;
            /// <summary>
            /// Gets or sets the fill brush.
            /// </summary>
            /// <value>
            /// The fill brush.
            /// </value>
            public D2D.Brush FillBrush
            {
                set
                {
                    var old = fillBrush;
                    if(SetAffectsRender(ref fillBrush, value))
                    {
                        RemoveAndDispose(ref old);
                        Collect(value);
                    }
                }
                get
                {
                    return fillBrush;
                }
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
                    if(SetAffectsRender(ref strokeBrush, value))
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
            /// <summary>
            /// Gets or sets the width of the stroke.
            /// </summary>
            /// <value>
            /// The width of the stroke.
            /// </value>
            public float StrokeWidth
            {
                set; get;
            } = 1.0f;

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
                    if(SetAffectsRender(ref strokeStyle, value))
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
        }
    }

}
