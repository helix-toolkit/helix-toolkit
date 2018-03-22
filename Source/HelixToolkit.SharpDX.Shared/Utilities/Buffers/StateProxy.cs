using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Threading;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="StateType">The type of the tate type.</typeparam>
    public abstract class StateProxy<StateType> : IDisposable where StateType : ComObject
    {       
        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public StateType State { get { return state; } }
        private StateType state;

        public StateProxy(StateType state)
        {
            this.state = state;
        }

        /// <summary>
        /// Performs an implicit conversion
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator StateType(StateProxy<StateType> proxy)
        {
            return proxy.State;
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
                //Disposed?.Invoke(this, EventArgs.Empty);
                //Disposed = null;
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
    public sealed class RasterizerStateProxy : StateProxy<RasterizerState>
    {
        public static readonly RasterizerStateProxy Empty = new RasterizerStateProxy(null);
        public RasterizerStateProxy(RasterizerState state) : base(state) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class BlendStateProxy : StateProxy<BlendState>
    {
        public static readonly BlendStateProxy Empty = new BlendStateProxy(null);
        public BlendStateProxy(BlendState state) : base(state) { }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class DepthStencilStateProxy : StateProxy<DepthStencilState>
    {
        public static readonly DepthStencilStateProxy Empty = new DepthStencilStateProxy(null);
        public DepthStencilStateProxy(DepthStencilState state) : base(state) { }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class SamplerStateProxy : StateProxy<SamplerState>
    {
        public static readonly SamplerStateProxy Empty = new SamplerStateProxy(null);
        public SamplerStateProxy(SamplerState state) : base(state) { }
    }
}
