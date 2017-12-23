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
        IEnumerable<Tuple<int, UAVMapping>> UAVMapping { get; }
        int TextureMappingCount { get; }
        int UAVMappingCount { get; }
        void AddConstantBuffer(string name, int slot, IBufferProxy buffer);
        void AddTextureMapping(string name, int slot, TextureMapping mapping);
        void AddUAVMapping(string name, int slot, UAVMapping mapping);
        void Bind(DeviceContext context);
        void BindConstantBuffers(DeviceContext context);
        void BindTexture(DeviceContext context, string name, ShaderResourceView texture);
        void BindTexture(DeviceContext context, int slot, ShaderResourceView texture);
        void BindTextures(DeviceContext context, IEnumerable<Tuple<int, ShaderResourceView>> textures);
        void BindUAV(DeviceContext context, string name, UnorderedAccessView uav);
        void BindUAV(DeviceContext context, int slot, UnorderedAccessView uav);
        void BindUAVs(DeviceContext context, IEnumerable<Tuple<int, UnorderedAccessView>> uavs);
        void ClearConstantBuffer();
        void ClearTextureMappings();
        void ClearUAVMappings();
        int TryGetTextureIndex(string name);
        TextureMapping GetTextureMapping(string name);
        TextureMapping GetTextureMapping(int slot);
        UAVMapping GetUAVMapping(string name);
        UAVMapping GetUAVMapping(int slot);
        string TryGetTextureName(int slot);
        string TryGetUAVName(int slot);
        bool RemoveConstantBuffer(string name);
        bool RemoveConstantBuffer(int slot);
        bool RemoveTextureMapping(string name);
        bool RemoveTextureMapping(int slot);
        bool RemoveUAVMapping(int slot);
        bool RemoveUAVMapping(string name);
    }
}