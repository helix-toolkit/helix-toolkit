/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using SharpDX.Direct3D11;


#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Utilities;
    public interface IShader : IDisposable
    {
        IEnumerable<Tuple<int, IBufferProxy>> CBufferMapping { get; }
        int ConstantBufferCount { get; }
        string Name { get; }
        ShaderStage ShaderType { get; }
        IEnumerable<Tuple<int, TextureMapping>> TextureMapping { get; }
        int TextureMappingCount { get; }

        void AddConstantBuffer(string name, int index, IBufferProxy buffer);
        void AddTextureMapping(string name, int index, TextureMapping mapping);
        void Bind(DeviceContext context);
        void BindConstantBuffers(DeviceContext context);
        void BindTexture(DeviceContext context, string name, ShaderResourceView texture);
        void BindTexture(DeviceContext context, int index, ShaderResourceView texture);
        void BindTextures(DeviceContext context, IEnumerable<Tuple<int, ShaderResourceView>> textures);
        void ClearConstantBuffer();
        void ClearTextureMappings();
        int TryGetTextureIndex(string name);
        TextureMapping GetTextureMapping(string name);
        TextureMapping GetTextureMapping(int slot);
        string TryGetTextureName(int index);
        bool RemoveConstantBuffer(string name);
        bool RemoveConstantBuffer(int index);
        bool RemoveTextureMapping(string name);
        bool RemoveTextureMapping(int index);
    }
}