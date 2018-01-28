using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using global::SharpDX;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderContext2D : IDisposable
    {
        /// <summary>
        /// Gets the device context.
        /// </summary>
        /// <value>
        /// The device context.
        /// </value>
        DeviceContext DeviceContext { get; }
        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        double ActualWidth { get; }
        /// <summary>
        /// Gets the actual height.
        /// </summary>
        /// <value>
        /// The actual height.
        /// </value>
        double ActualHeight { get; }

        /// <summary>
        /// Pushes the render target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="clear">if set to <c>true</c> [clear].</param>
        void PushRenderTarget(BitmapProxy target, bool clear);
        /// <summary>
        /// Pops the render target.
        /// </summary>
        void PopRenderTarget();

        Matrix3x2 LastBitmapTransform { get; }

        Matrix3x2 PushLastBitmapTransform(Matrix3x2 transform);

        Matrix3x2 PopLastBitmapTransform();
    }
    /// <summary>
    /// 
    /// </summary>
    public class RenderContext2D : DisposeObject, IRenderContext2D
    {
        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        public double ActualWidth { get { return renderHost.ActualWidth; } }
        /// <summary>
        /// Gets the actual height.
        /// </summary>
        /// <value>
        /// The actual height.
        /// </value>
        public double ActualHeight { get { return renderHost.ActualHeight; } }
        /// <summary>
        /// Gets or sets the device context.
        /// </summary>
        /// <value>
        /// The device context.
        /// </value>
        public DeviceContext DeviceContext { private set; get; }
        /// <summary>
        /// Gets or sets the last bitmap transform.
        /// </summary>
        /// <value>
        /// The last bitmap transform.
        /// </value>
        public Matrix3x2 LastBitmapTransform { private set; get; } = Matrix3x2.Identity;

        private Stack<Matrix3x2> lastBitmapTransformStack = new Stack<Matrix3x2>();

        public Matrix3x2 PushLastBitmapTransform(Matrix3x2 transform)
        {
            lastBitmapTransformStack.Push(LastBitmapTransform);
            LastBitmapTransform *= transform;
            return LastBitmapTransform;
        }

        public Matrix3x2 PopLastBitmapTransform()
        {
            LastBitmapTransform = lastBitmapTransformStack.Pop();
            return LastBitmapTransform;
        }
        /// <summary>
        /// The render host
        /// </summary>
        private IRenderHost renderHost;
        /// <summary>
        /// The target stack
        /// </summary>
        private readonly Stack<BitmapProxy> targetStack = new Stack<BitmapProxy>();
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderContext2D"/> class.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="host">The host.</param>
        public RenderContext2D(DeviceContext deviceContext, IRenderHost host)
        {
            DeviceContext = deviceContext;
            renderHost = host;
        }
        /// <summary>
        /// Pushes the render target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="clear">if set to <c>true</c> [clear].</param>
        public void PushRenderTarget(BitmapProxy target, bool clear)
        {
            if (targetStack.Count > 0)
            {
                DeviceContext.EndDraw();                
            }
            targetStack.Push(target);
            DeviceContext.Target = targetStack.Peek();
            DeviceContext.BeginDraw();
            if (clear)
            {
                DeviceContext.Clear(Color.Transparent);
            }
        }
        /// <summary>
        /// Pops the render target.
        /// </summary>
        public void PopRenderTarget()
        {
            DeviceContext.EndDraw();
            DeviceContext.Target = null;
            targetStack.Pop();
            if (targetStack.Count > 0)
            {
                DeviceContext.Target = targetStack.Peek();
                DeviceContext.BeginDraw();
            }
        }
    }
}
