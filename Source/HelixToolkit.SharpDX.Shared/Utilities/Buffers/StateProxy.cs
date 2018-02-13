using SharpDX;
using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    using ShaderManager;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="StateType">The type of the tate type.</typeparam>
    /// <typeparam name="StateTypeDescription">The type of the tate type description.</typeparam>
    public abstract class StateProxy<StateType, StateTypeDescription> : IDisposable where StateType : ComObject where StateTypeDescription : struct
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { set; get; }
        private StateType state;
        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public StateType State { get { return state; } }
        private StateTypeDescription description;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public StateTypeDescription Description
        {
            set
            {
                if(description.Equals(value)) { return; }
                description = value;
                Disposer.RemoveAndDispose(ref state);
                state = CreateState(poolManager, ref description);
            }
            get
            {
                return description;
            }
        }
        private readonly IStatePoolManager poolManager;
        /// <summary>
        /// Initializes a new instance of the <see cref="StateProxy{StateType, StateTypeDescription}"/> class.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="name">The name.</param>
        public StateProxy(IStatePoolManager poolManager, string name = "")
        {
            this.poolManager = poolManager;
            Name = name;
        }
        /// <summary>
        /// Creates the state.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        protected abstract StateType CreateState(IStatePoolManager poolManager, ref StateTypeDescription description);
        /// <summary>
        /// Performs an implicit conversion from <see cref="StateProxy{StateType, StateTypeDescription}"/> to <see cref="StateType"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator StateType(StateProxy<StateType, StateTypeDescription> proxy)
        {
            return proxy.State;
        }
        /// <summary>
        /// Performs an implicit conversion from <see cref="StateProxy{StateType, StateTypeDescription}"/> to <see cref="StateTypeDescription"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator StateTypeDescription(StateProxy<StateType, StateTypeDescription> proxy)
        {
            return proxy.Description;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disposer.RemoveAndDispose(ref state);
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StateProxy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class SamplerProxy : StateProxy<SamplerState, SamplerStateDescription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SamplerProxy"/> class.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="name">The name.</param>
        public SamplerProxy(IStatePoolManager poolManager, string name = "") : base(poolManager, name)
        {
        }

        /// <summary>
        /// Creates the state.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="desc">The desc.</param>
        /// <returns></returns>
        protected override SamplerState CreateState(IStatePoolManager poolManager, ref SamplerStateDescription desc)
        {
            return poolManager.Register(desc);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class RasterizerStateProxy : StateProxy<RasterizerState, RasterizerStateDescription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RasterizerStateProxy"/> class.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="name">The name.</param>
        public RasterizerStateProxy(IStatePoolManager poolManager, string name = "") : base(poolManager, name)
        {
        }
        /// <summary>
        /// Creates the state.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="desc">The desc.</param>
        /// <returns></returns>
        protected override RasterizerState CreateState(IStatePoolManager poolManager, ref RasterizerStateDescription desc)
        {
            return poolManager.Register(desc);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class BlendStateProxy : StateProxy<BlendState, BlendStateDescription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlendStateProxy"/> class.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="name">The name.</param>
        public BlendStateProxy(IStatePoolManager poolManager, string name = "") : base(poolManager, name)
        {
        }
        /// <summary>
        /// Creates the state.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="desc">The desc.</param>
        /// <returns></returns>
        protected override BlendState CreateState(IStatePoolManager poolManager, ref BlendStateDescription desc)
        {
            return poolManager.Register(desc);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class DepthStencilStateProxy : StateProxy<DepthStencilState, DepthStencilStateDescription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepthStencilStateProxy"/> class.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="name">The name.</param>
        public DepthStencilStateProxy(IStatePoolManager poolManager, string name = "") : base(poolManager, name)
        {
        }
        /// <summary>
        /// Creates the state.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="desc">The desc.</param>
        /// <returns></returns>
        protected override DepthStencilState CreateState(IStatePoolManager poolManager, ref DepthStencilStateDescription desc)
        {
            return poolManager.Register(desc);
        }
    }
}
