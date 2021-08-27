using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Threading;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace TT.HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="StateType">The type of the tate type.</typeparam>
        public abstract class StateProxy<StateType> : ReferenceCountDisposeObject where StateType : ComObject
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
                this.state = Collect(state);
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
