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

        IEnumerable<ShaderBase> Shaders { get; }

        void BindShader(DeviceContext context);

        ShaderBase GetShader(ShaderStage type);

        IConstantBufferPool ConstantBufferPool { get; }
    }

    public interface IRenderTechniquesManager
    {
        void AddRenderTechnique(string techniqueName, byte[] techniqueSource);
        IDictionary<string, IRenderTechnique> RenderTechniques { get; }
    }

    public interface IEffectsManager
    {
        //IRenderTechniquesManager RenderTechniquesManager { get; }
        //InputLayout GetLayout(IRenderTechnique technique);
        //Effect GetEffect(IRenderTechnique technique);
        //global::SharpDX.Direct3D11.Device Device { get; }
        int AdapterIndex { get; }
        IConstantBufferPool ConstantBufferPool { get; }
        Device Device { get; }
        DriverType DriverType { get; }
        IDictionary<string, Technique> Techniques { get; }
        IShaderPoolManager ShaderManager { get; }
        IStatePoolManager StateManager { get; }
        void Initialize();
    }

    public interface IShaderPoolManager
    {
        ShaderBase RegisterShader(ShaderDescription description);

        InputLayout RegisterInputLayout(InputLayoutDescription description);
    }

    public interface IStatePoolManager
    {
        BlendState Register(BlendStateDescription desc);

        RasterizerState Register(RasterizerStateDescription desc);

        DepthStencilState Register(DepthStencilStateDescription desc);
    }
}
