#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core.Components
#else
namespace HelixToolkit.UWP.Core.Components
#endif
{
    using Render;
    using Shaders;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public sealed class ConstantBufferComponent : CoreComponent
    {
        /// <summary>
        /// Gets or sets the model constant buffer.
        /// </summary>
        /// <value>
        /// The model constant buffer.
        /// </value>
        public ConstantBufferProxy ModelConstBuffer { private set; get; }
        private readonly ConstantBufferDescription bufferDesc;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantBufferComponent"/> class.
        /// </summary>
        /// <param name="desc">The desc.</param>
        public ConstantBufferComponent(ConstantBufferDescription desc)
        {
            bufferDesc = desc;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantBufferComponent"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="structSize">Size of the structure.</param>
        public ConstantBufferComponent(string name, int structSize)
        {
            bufferDesc = new ConstantBufferDescription(name, structSize);
        }

        protected override void OnAttach(IRenderTechnique technique)
        {
            ModelConstBuffer = technique.ConstantBufferPool.Register(bufferDesc);
        }

        protected override void OnDetach()
        {
            ModelConstBuffer = null;
        }
        /// <summary>
        /// Uploads the specified device context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="data">The data.</param>
        public void Upload<T>(DeviceContextProxy deviceContext, ref T data) where T : struct
        {
            ModelConstBuffer.UploadDataToBuffer(deviceContext, ref data);
        }
    }
}
