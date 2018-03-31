/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core2D;
    using Model.Scene2D;

    public interface IRenderable2D : ITransform2D, IGUID
    {
        /// <summary>
        /// Gets a value indicating whether this instance is renderable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is renderable; otherwise, <c>false</c>.
        /// </value>
        bool IsRenderable { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is measure dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is measure dirty; otherwise, <c>false</c>.
        /// </value>
        bool IsMeasureDirty { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is arrange dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is arrange dirty; otherwise, <c>false</c>.
        /// </value>
        bool IsArrangeDirty { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is transform dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is transform dirty; otherwise, <c>false</c>.
        /// </value>
        bool IsTransformDirty { get; }
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
        /// Optional for scene graph traverse
        /// </summary>
        IList<SceneNode2D> Items { get; }

        RenderCore2D RenderCore { get; }
        /// <summary>
        /// Update render related parameters such as model matrix by scene graph and bounding boxes
        /// </summary>
        /// <param name="context"></param>
        void Update(IRenderContext2D context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Render(IRenderContext2D context);
        /// <summary>
        /// Renders the bitmap cache to render target. If bitmap cache is disabled, nothing will be rendered
        /// </summary>
        /// <param name="context">The context.</param>
        void RenderBitmapCache(IRenderContext2D context);

        void Measure(Size2F size);
        void Arrange(RectangleF rect);
        void InvalidateArrange();
        void InvalidateMeasure();
        void InvalidateVisual();
        void InvalidateTransform();
        void InvalidateAll();
    }

    public interface IHitable2D
    {
        bool HitTest(Vector2 mousePoint, out HitTest2DResult hitResult);
        bool IsHitTestVisible { set; get; }
    }
}
