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
    public interface IShaderPass
    {
        string Name { get; }
        IEnumerable<IShader> Shaders { get; }
        BlendState BlendState { get; }
        /// <summary>
        /// 
        /// </summary>
        DepthStencilState DepthStencilState { get; }
        /// <summary>
        /// 
        /// </summary>
        RasterizerState RasterState { get; }

        void BindShader(DeviceContext context);

        IShader GetShader(ShaderStage type);

        void BindStates(DeviceContext context, StateType type);
    }
    public interface IRenderTechnique
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        Device Device { get; }
        /// <summary>
        /// Input layout for all passes
        /// </summary>
        InputLayout Layout { get; }

        IEnumerable<IShaderPass> Passes { get; }

        IConstantBufferPool ConstantBufferPool { get; }
        /// <summary>
        /// Get pass by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IShaderPass GetPass(string name);
        /// <summary>
        /// Get pass by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IShaderPass GetPass(int index);

        IEffectsManager EffectsManager { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IShaderPass this[int index] { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IShaderPass this[string name] { get; }
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
        /// <summary>
        /// 
        /// </summary>
        Device Device { get; }
        /// <summary>
        /// 
        /// </summary>
        DriverType DriverType { get; }
        /// <summary>
        /// 
        /// </summary>
        IShaderPoolManager ShaderManager { get; }
        /// <summary>
        /// Get list of existing technique names
        /// </summary>
        IEnumerable<string> RenderTechniques { get; }
        /// <summary>
        /// 
        /// </summary>
        IStatePoolManager StateManager { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IRenderTechnique GetTechnique(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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
