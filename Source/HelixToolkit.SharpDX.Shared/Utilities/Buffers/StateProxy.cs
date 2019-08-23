using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Threading;

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
    namespace Utilities
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
            private readonly StateType state;

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
            /// <summary>
            /// Occurs when this instance is fully disposed.
            /// </summary>
            public event EventHandler<BoolArgs> Disposed;
            private int refCounter = 1;

            internal int IncRef()
            {
                return Interlocked.Increment(ref refCounter);
            }
            /// <summary>
            /// Forces the dispose.
            /// </summary>
            internal void ForceDispose()
            {
                Interlocked.Exchange(ref refCounter, 1);
                Dispose();
            }
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (Interlocked.Decrement(ref refCounter) == 0 && !disposedValue)
                {
                    if (disposing)
                    {
                        state?.Dispose();
                    }

                    disposedValue = true;
                    Disposed?.Invoke(this, BoolArgs.TrueArgs);
                }
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                Dispose(true);
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

}
