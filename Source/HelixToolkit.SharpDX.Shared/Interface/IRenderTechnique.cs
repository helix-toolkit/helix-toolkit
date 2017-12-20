using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using ShaderManager;
    using Shaders;
    public interface IRenderTechnique
    {
        string Name { get; }

        Device Device { get; }

        InputLayout Layout { get; }

        IEnumerable<IShader> Shaders { get; }

        IConstantBufferPool ConstantBufferPool { get; }

        void BindStates(DeviceContext context, StateType type);

        void BindShader(DeviceContext context);

        IShader GetShader(ShaderStage type);

        IEffectsManager EffectsManager { get; }
    }

    public interface IRenderTechniquesManager
    {
        void AddRenderTechnique(string techniqueName, byte[] techniqueSource);
        IDictionary<string, IRenderTechnique> RenderTechniques { get; }
    }

    public interface IEffectsManager
    {
        int AdapterIndex { get; }
        IConstantBufferPool ConstantBufferPool { get; }
        Device Device { get; }
        DriverType DriverType { get; }
        IShaderPoolManager ShaderManager { get; }
        IStatePoolManager StateManager { get; }
        IRenderTechnique GetTechnique(string name);
        IRenderTechnique this[string name] { get; }
        void Initialize();
    }

    public interface IShaderPoolManager
    {
        IShader RegisterShader(ShaderDescription description);

        InputLayout RegisterInputLayout(InputLayoutDescription description);
    }

    public interface IStatePoolManager
    {
        BlendState Register(BlendStateDescription desc);

        RasterizerState Register(RasterizerStateDescription desc);

        DepthStencilState Register(DepthStencilStateDescription desc);
    }
}
