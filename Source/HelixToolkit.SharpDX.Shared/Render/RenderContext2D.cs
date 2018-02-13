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
    public interface IRenderContext2D
    {
        /// <summary>
        /// Gets the device context.
        /// </summary>
        /// <value>
        /// The device context.
        /// </value>
        DeviceContext DeviceContext { get; }
        /// <summary>
        /// 
        /// </summary>
        IDevice2DResources DeviceResources { get; }
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
        /// <summary>
        /// Gets the last bitmap transform. 
        /// If some of controls did not enable the bitmap cache, 
        /// it needs to propagate the relative transform into its children if some children use the bitmap cache to draw onto the parent bitmap cache.
        /// </summary>
        /// <value>
        /// The last bitmap transform.
        /// </value>
        Matrix3x2 RelativeTransform { get; }
        /// <summary>
        /// Pushes the last bitmap transform.
        /// </summary>
        /// <param name="transform">The transform.</param>
        void PushRelativeTransform(Matrix3x2 transform);
        /// <summary>
        /// Pops the last bitmap transform.
        /// </summary>
        void PopRelativeTransform();
        /// <summary>
        /// Gets a value indicating whether this context has render target.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this context has target; otherwise, <c>false</c>.
        /// </value>
        bool HasTarget { get; }
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
        /// Gets the device resources.
        /// </summary>
        /// <value>
        /// The device resources.
        /// </value>
        public IDevice2DResources DeviceResources { private set; get; }
        /// <summary>
        /// Gets or sets the last bitmap transform.
        /// </summary>
        /// <value>
        /// The last bitmap transform.<see cref="IRenderContext2D.RelativeTransform"/>
        /// </value>
        public Matrix3x2 RelativeTransform { private set; get; } = Matrix3x2.Identity;

        private Stack<Matrix3x2> relativeTransformStack = new Stack<Matrix3x2>();
        /// <summary>
        /// Gets or sets a value indicating whether this instance has target.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has target; otherwise, <c>false</c>.
        /// </value>
        public bool HasTarget { private set; get; } = false;
        /// <summary>
        /// Pushes the last bitmap transform.
        /// </summary>
        /// <param name="transform">The transform.</param>
        public void PushRelativeTransform(Matrix3x2 transform)
        {
            relativeTransformStack.Push(RelativeTransform);
            RelativeTransform = transform;
        }
        /// <summary>
        /// Pops the last bitmap transform.
        /// </summary>
        public void PopRelativeTransform()
        {
            RelativeTransform = relativeTransformStack.Pop();
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
            DeviceResources = host.EffectsManager;
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
            HasTarget = true;
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
            HasTarget = false;
            targetStack.Pop();
            if (targetStack.Count > 0)
            {
                DeviceContext.Target = targetStack.Peek();
                HasTarget = true;
                DeviceContext.BeginDraw();
            }
        }
    }
}
