/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct2D1;
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransform2D
    {
        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>
        /// The transform.
        /// </value>
        Matrix3x2 Transform { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderable2D : IDisposable
    {
        /// <summary>
        /// Gets or sets the bound.
        /// </summary>
        /// <value>
        /// The bound.
        /// </value>
        RectangleF Bound { set; get; }

        //Matrix3x2 Transform { set; get; }        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is rendering.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is rendering; otherwise, <c>false</c>.
        /// </value>
        bool IsRendering { set; get; }
        /// <summary>
        /// Attaches the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Attach(IRenderHost target);
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        void Detach();
        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void Render(IRenderContext2D context);
        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse over.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
        /// </value>
        bool IsMouseOver { set; get; }
    }
}
