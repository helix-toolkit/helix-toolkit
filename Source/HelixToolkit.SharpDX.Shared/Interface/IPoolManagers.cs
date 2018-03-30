/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
#if NETFX_CORE
namespace HelixToolkit.UWP.ShaderManager
#else
namespace HelixToolkit.Wpf.SharpDX.ShaderManager
#endif
{
    using Shaders;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public interface IShaderPoolManager
    {
        /// <summary>
        /// Registers the shader.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        ShaderBase RegisterShader(ShaderDescription description);
        /// <summary>
        /// Registers the input layout.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        InputLayout RegisterInputLayout(InputLayoutDescription description);
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IStatePoolManager
    {
        /// <summary>
        /// Registers the specified desc.
        /// </summary>
        /// <param name="desc">The desc.</param>
        /// <returns></returns>
        BlendStateProxy Register(BlendStateDescription desc);

        /// <summary>
        /// Registers the specified desc.
        /// </summary>
        /// <param name="desc">The desc.</param>
        /// <returns></returns>
        RasterizerStateProxy Register(RasterizerStateDescription desc);
        /// <summary>
        /// Registers the specified desc.
        /// </summary>
        /// <param name="desc">The desc.</param>
        /// <returns></returns>
        DepthStencilStateProxy Register(DepthStencilStateDescription desc);
        /// <summary>
        /// Registers the specified desc.
        /// </summary>
        /// <param name="desc">The desc.</param>
        /// <returns></returns>
        SamplerStateProxy Register(SamplerStateDescription desc);
    }
}
