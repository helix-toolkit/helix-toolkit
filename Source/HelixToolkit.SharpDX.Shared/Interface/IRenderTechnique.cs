using SharpDX.Direct3D11;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public interface IRenderTechnique
    {
        string Name { get; }

        Effect Effect { set; get; }

        EffectTechnique EffectTechnique { set; get; }

        Device Device { set; get; }

        InputLayout InputLayout { set; get; }
    }

    public interface IRenderTechniquesManager
    {
        void AddRenderTechnique(string techniqueName, byte[] techniqueSource);
        IDictionary<string, IRenderTechnique> RenderTechniques { get; }
    }

    public interface IEffectsManager
    {
        IRenderTechniquesManager RenderTechniquesManager { get; }
        InputLayout GetLayout(IRenderTechnique technique);
        Effect GetEffect(IRenderTechnique technique);
        global::SharpDX.Direct3D11.Device Device { get; }
        int AdapterIndex { get; }
    }
}
