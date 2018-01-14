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

        SamplerState Register(SamplerStateDescription desc);
    }
}
