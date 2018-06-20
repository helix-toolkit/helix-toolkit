using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;

#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Shaders;
    using System.Runtime.CompilerServices;
    using Utilities;

    /// <summary>
    ///
    /// </summary>
    public sealed partial class DeviceContextProxy : DisposeObject
    {
        public static bool AutoSkipRedundantStateSetting = false;
        private readonly DeviceContext deviceContext;
        private readonly Device device;
        private RasterizerStateProxy currRasterState = null;
        private DepthStencilStateProxy currDepthStencilState = null;
        private int currStencilRef;
        private BlendStateProxy currBlendState = null;
        private Color4? currBlendFactor = null;
        private uint currSampleMask = uint.MaxValue;

        public readonly bool IsDeferred = false;

        #region Properties

        /// <summary>
        /// Gets or sets the last shader pass.
        /// </summary>
        /// <value>
        /// The last shader pass.
        /// </value>
        public ShaderPass CurrShaderPass { private set; get; }

        /// <summary>
        /// Gets the number of draw calls.
        /// </summary>
        /// <value>
        /// The number of draw calls.
        /// </value>
        public int NumberOfDrawCalls { private set; get; } = 0;
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Initializes a new deferred context
        /// </summary>
        /// <param name="device">The device.</param>
        public DeviceContextProxy(Device device)
        {
            deviceContext = Collect(new DeviceContext(device));
            this.device = device;
            IsDeferred = true;
        }

        /// <summary>
        /// Muse pass immediate context for this constructor
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="device">device</param>
        public DeviceContextProxy(DeviceContext context, Device device)
        {
            deviceContext = context;
            this.device = device;
            IsDeferred = false;
        }
        #endregion Constructor

        #region Cast

        /// <summary>
        /// Performs an implicit conversion from <see cref="DeviceContextProxy"/> to <see cref="DeviceContext"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator DeviceContext(DeviceContextProxy proxy)
        {
            return proxy.deviceContext;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DeviceContextProxy"/> to <see cref="Device"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Device(DeviceContextProxy proxy)
        {
            return proxy.device;
        }

        #endregion Cast

        /// <summary>
        /// Resets this instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            CurrShaderPass = null;
            currRasterState = null;
            currBlendState = null;
            currDepthStencilState = null;
            currBlendFactor = null;
            currSampleMask = uint.MaxValue;
            currStencilRef = 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if(deviceContext!=null && !deviceContext.IsDisposed)
            {
                deviceContext.ClearState();
                deviceContext.OutputMerger.ResetTargets();
            }
            base.OnDispose(disposeManagedResources);
        }
    }
}