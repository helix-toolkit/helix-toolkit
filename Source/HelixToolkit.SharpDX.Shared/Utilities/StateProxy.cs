using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    public abstract class StateProxy<StateType, StateTypeDescription> : IDisposable where StateType : class, IDisposable where StateTypeDescription : struct
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
                State?.Dispose();
                State = CreateState(device, ref description);
            }
            get
            {
                return description;
            }
        }
        private readonly Device device;
        public StateProxy(Device device, string name = "")
        {
            this.device = device;
            Name = name;
        }

        public StateProxy(Device device, StateTypeDescription description, string name = "") : this(device)
        {
            Description = description;
            Name = name;
        }

        protected abstract StateType CreateState(Device device, ref StateTypeDescription description);

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
                    State?.Dispose();
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
        public SamplerProxy(Device device, string name = "") : base(device, name)
        {
        }
        public SamplerProxy(Device device, SamplerStateDescription desc, string name = "") : base(device, desc, name)
        {
        }
        protected override SamplerState CreateState(Device device, ref SamplerStateDescription desc)
        {
            return new SamplerState(device, desc);
        }
    }

    public sealed class RasterizerStateProxy : StateProxy<RasterizerState, RasterizerStateDescription>
    {
        public RasterizerStateProxy(Device device, string name = "") : base(device, name)
        {
        }
        public RasterizerStateProxy(Device device, RasterizerStateDescription desc, string name = "") : base(device, desc, name)
        {
        }
        protected override RasterizerState CreateState(Device device, ref RasterizerStateDescription desc)
        {
            return new RasterizerState(device, desc);
        }
    }

    public sealed class BlendStateProxy : StateProxy<BlendState, BlendStateDescription>
    {
        public BlendStateProxy(Device device, string name = "") : base(device, name)
        {
        }
        public BlendStateProxy(Device device, BlendStateDescription desc, string name = "") : base(device, desc, name)
        {
        }

        protected override BlendState CreateState(Device device, ref BlendStateDescription desc)
        {
            return new BlendState(device, desc);
        }
    }

    public sealed class DepthStencilStateProxy : StateProxy<DepthStencilState, DepthStencilStateDescription>
    {
        public DepthStencilStateProxy(Device device, string name = "") : base(device, name)
        {
        }

        public DepthStencilStateProxy(Device device, DepthStencilStateDescription desc, string name = "") : base(device, desc, name)
        {
        }

        protected override DepthStencilState CreateState(Device device, ref DepthStencilStateDescription desc)
        {
            return new DepthStencilState(device, desc);
        }
    }
}
