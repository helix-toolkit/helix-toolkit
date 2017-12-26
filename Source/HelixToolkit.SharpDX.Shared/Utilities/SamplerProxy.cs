using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    public sealed class SamplerProxy
    {
        private SamplerStateDescription description;
        public SamplerStateDescription Description
        {
            set
            {
                description = value;
                SamplerState = pool.Register(value);
            }
            get { return description; }
        }
        public string SamplerName { set; get; }
        public SamplerState SamplerState { private set; get; }
        private IStatePoolManager pool;
        public SamplerProxy(IEffectsManager manager, string name = "")
        {
            pool = manager.StateManager;
            SamplerName = name;
        }

        public static implicit operator SamplerState(SamplerProxy proxy)
        {
            return proxy.SamplerState;
        }

        public static implicit operator SamplerStateDescription(SamplerProxy proxy)
        {
            return proxy.Description;
        }
    }
}
