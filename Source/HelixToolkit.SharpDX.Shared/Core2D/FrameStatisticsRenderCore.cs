/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX.DirectWrite;
using System;
using System.Numerics;
using D2D = SharpDX.Direct2D1;
#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    using global::SharpDX.Mathematics.Interop;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public class FrameStatisticsRenderCore : RenderCore2DBase
    {
        private IRenderStatistics statistics;

        private D2D.Brush foreground = null;
        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>
        /// The foreground.
        /// </value>
        public D2D.Brush Foreground
        {
            set
            {
                var old = foreground;
                if (SetAffectsRender(ref foreground, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
            }
            get
            {
                return foreground;
            }
        }

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
                if (SetAffectsRender(ref background, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
            }
            get
            {
                return background;
            }
        }

        private TextLayout textLayout;
        private Factory factory;
        private TextFormat format;
        private RectangleF renderBound = new RectangleF(0, 0, 100, 0);
        private string previousStr = "";
        /// <summary>
        /// Called when [attach].
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderHost target)
        {
            factory = Collect(new Factory(FactoryType.Isolated));
            format = Collect(new TextFormat(factory, "Arial", 12));
            previousStr = "";
            this.statistics = target.RenderStatistics;
            return base.OnAttach(target);
        }
        /// <summary>
        /// Determines whether this instance can render the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance can render the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanRender(RenderContext2D context)
        {
            return base.CanRender(context) && statistics != null && statistics.FrameDetail != RenderDetail.None;
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void OnRender(RenderContext2D context)
        {
            if(background == null)
            {
                Background = new D2D.SolidColorBrush(context.DeviceContext, new Color4(0.8f, 0.8f, 0.8f, 0.6f));
            }
            if(foreground == null)
            {
                Foreground = new D2D.SolidColorBrush(context.DeviceContext, Color.Blue);
            }
            var str = statistics.GetDetailString();
            if (str != previousStr)
            {
                previousStr = str;
                RemoveAndDispose(ref textLayout);
                textLayout = Collect(new TextLayout(factory, str, format, float.MaxValue, float.MaxValue));
            }
            var metrices = textLayout.Metrics;
            renderBound.Width = Math.Max(metrices.Width, renderBound.Width);
            renderBound.Height = metrices.Height;
            context.DeviceContext.Transform = Matrix3x2.CreateTranslation((float)context.ActualWidth - renderBound.Width, 0).ToRaw();                                     
            context.DeviceContext.FillRectangle(renderBound, background);
            context.DeviceContext.DrawTextLayout(new RawVector2(), textLayout, foreground);
        }
    }
}
