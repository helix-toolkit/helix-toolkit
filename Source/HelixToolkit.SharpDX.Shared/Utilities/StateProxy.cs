using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    public abstract class StateProxy<StateType, StateTypeDescription> : IDisposable where StateType : class where StateTypeDescription : struct
    {
        public string Name { set; get; }
        public StateType State { get; private set; }
        private StateTypeDescription description;
        public StateTypeDescription Description
        {
            set
            {
                if(description.Equals(value)) { return; }
                description = value;
                State = CreateState(poolManager, ref description);
            }
            get
            {
                return description;
            }
        }
        private readonly IStatePoolManager poolManager;
        public StateProxy(IStatePoolManager poolManager, string name = "")
        {
            this.poolManager = poolManager;
            Name = name;
        }

        public StateProxy(IStatePoolManager poolManager, StateTypeDescription description, string name = "") : this(poolManager)
        {
            Description = description;
            Name = name;
        }

        protected abstract StateType CreateState(IStatePoolManager poolManager, ref StateTypeDescription description);

        public static implicit operator StateType(StateProxy<StateType, StateTypeDescription> proxy)
        {
            return proxy.State;
        }

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
                    State = null;
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

    public sealed class SamplerProxy : StateProxy<SamplerState, SamplerStateDescription>
    {
        public SamplerProxy(IStatePoolManager poolManager, string name = "") : base(poolManager, name)
        {
        }
        public SamplerProxy(IStatePoolManager poolManager, SamplerStateDescription desc, string name = "") : base(poolManager, desc, name)
        {
        }
        protected override SamplerState CreateState(IStatePoolManager poolManager, ref SamplerStateDescription desc)
        {
            return poolManager.Register(desc);
        }
    }

    public sealed class RasterizerStateProxy : StateProxy<RasterizerState, RasterizerStateDescription>
    {
        public RasterizerStateProxy(IStatePoolManager poolManager, string name = "") : base(poolManager, name)
        {
        }
        public RasterizerStateProxy(IStatePoolManager poolManager, RasterizerStateDescription desc, string name = "") : base(poolManager, desc, name)
        {
        }
        protected override RasterizerState CreateState(IStatePoolManager poolManager, ref RasterizerStateDescription desc)
        {
            return poolManager.Register(desc);
        }
    }

    public sealed class BlendStateProxy : StateProxy<BlendState, BlendStateDescription>
    {
        public BlendStateProxy(IStatePoolManager poolManager, string name = "") : base(poolManager, name)
        {
        }
        public BlendStateProxy(IStatePoolManager poolManager, BlendStateDescription desc, string name = "") : base(poolManager, desc, name)
        {
        }

        protected override BlendState CreateState(IStatePoolManager poolManager, ref BlendStateDescription desc)
        {
            return poolManager.Register(desc);
        }
    }

    public sealed class DepthStencilStateProxy : StateProxy<DepthStencilState, DepthStencilStateDescription>
    {
        public DepthStencilStateProxy(IStatePoolManager poolManager, string name = "") : base(poolManager, name)
        {
        }

        public DepthStencilStateProxy(IStatePoolManager poolManager, DepthStencilStateDescription desc, string name = "") : base(poolManager, desc, name)
        {
        }

        protected override DepthStencilState CreateState(IStatePoolManager poolManager, ref DepthStencilStateDescription desc)
        {
            return poolManager.Register(desc);
        }
    }
}
