using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using Render;
    using Shaders;
    using System.Runtime.CompilerServices;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public static class VertexShaderExtensions
    {
        /// <summary>
        /// Binds shader to pipeline
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Bind(this VertexShader shader, DeviceContextProxy context, bool bindConstantBuffer = true)
        {
            context.SetShader(shader, bindConstantBuffer);
        }

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTexture(this VertexShader shader, DeviceContextProxy context, int slot, ShaderResourceViewProxy texture)
        {
            context.BindTexture(shader, slot, texture);
        }

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTexture(this VertexShader shader, DeviceContextProxy context, string name, ShaderResourceViewProxy texture)
        {
            int slot = shader.ShaderResourceViewMapping.TryGetBindSlot(name);
            context.BindTexture(shader, slot, texture);
        }
        /// <summary>
        /// Binds the textures.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="textures">The textures.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTextures(this VertexShader shader, DeviceContextProxy context, IEnumerable<KeyValuePair<int, ShaderResourceViewProxy>> textures)
        {
            foreach (var texture in textures)
            {
                context.BindTexture(shader, texture.Key, texture.Value);
            }
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSampler(this VertexShader shader, DeviceContextProxy context, int slot, SamplerStateProxy sampler)
        {
            context.BindSampler(shader, slot, sampler);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSampler(this VertexShader shader, DeviceContextProxy context, string name, SamplerStateProxy sampler)
        {
            int slot = shader.SamplerMapping.TryGetBindSlot(name);
            context.BindSampler(shader, slot, sampler);
        }

        /// <summary>
        /// Binds the samplers.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="samplers">The samplers.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSamplers(this VertexShader shader, DeviceContextProxy context, IEnumerable<KeyValuePair<int, SamplerStateProxy>> samplers)
        {
            foreach (var sampler in samplers)
            {
                context.BindSampler(shader, sampler.Key, sampler.Value);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class HullShaderExtensions
    {
        /// <summary>
        /// Binds shader to pipeline
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Bind(this HullShader shader, DeviceContextProxy context, bool bindConstantBuffer = true)
        {
            context.SetShader(shader);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DomainShaderExtensions
    {
        /// <summary>
        /// Binds shader to pipeline
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Bind(this DomainShader shader, DeviceContextProxy context, bool bindConstantBuffer = true)
        {
            context.SetShader(shader);
        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTexture(this DomainShader shader, DeviceContextProxy context, string name, ShaderResourceViewProxy texture)
        {
            int slot = shader.ShaderResourceViewMapping.TryGetBindSlot(name);
            context.BindTexture(shader, slot, texture);
        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTexture(this DomainShader shader, DeviceContextProxy context, int slot, ShaderResourceViewProxy texture)
        {
            context.BindTexture(shader, slot, texture);
        }
        /// <summary>
        /// Binds the textures.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="textures">The textures.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTextures(this DomainShader shader, DeviceContextProxy context, IEnumerable<KeyValuePair<int, ShaderResourceViewProxy>> textures)
        {
            foreach (var texture in textures)
            {
                context.BindTexture(shader, texture.Key, texture.Value);
            }
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSampler(this DomainShader shader, DeviceContextProxy context, int slot, SamplerStateProxy sampler)
        {
            context.BindSampler(shader, slot, sampler);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSampler(this DomainShader shader, DeviceContextProxy context, string name, SamplerStateProxy sampler)
        {
            int slot = shader.SamplerMapping.TryGetBindSlot(name);
            context.BindSampler(shader, slot, sampler);
        }

        /// <summary>
        /// Binds the samplers.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="samplers">The samplers.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSamplers(this DomainShader shader, DeviceContextProxy context, IEnumerable<KeyValuePair<int, SamplerStateProxy>> samplers)
        {
            foreach (var sampler in samplers)
            {
                context.BindSampler(shader, sampler.Key, sampler.Value);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class GeometryShaderExtensions
    {
        /// <summary>
        /// Binds shader to pipeline
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Bind(this GeometryShader shader, DeviceContextProxy context, bool bindConstantBuffer = true)
        {
            context.SetShader(shader);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class PixelShaderExtensions
    {
        /// <summary>
        /// Binds shader to pipeline
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Bind(this PixelShader shader, DeviceContextProxy context, bool bindConstantBuffer = true)
        {
            context.SetShader(shader);
        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTexture(this PixelShader shader, DeviceContextProxy context, string name, ShaderResourceViewProxy texture)
        {
            int slot = shader.ShaderResourceViewMapping.TryGetBindSlot(name);
            context.BindTexture(shader, slot, texture);
        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTexture(this PixelShader shader, DeviceContextProxy context, int slot, ShaderResourceViewProxy texture)
        {
            context.BindTexture(shader, slot, texture);
        }
        /// <summary>
        /// Binds the textures.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="textures">The textures.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTextures(this PixelShader shader, DeviceContextProxy context, IEnumerable<KeyValuePair<int, ShaderResourceViewProxy>> textures)
        {
            foreach (var texture in textures)
            {
                context.BindTexture(shader, texture.Key, texture.Value);
            }
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSampler(this PixelShader shader, DeviceContextProxy context, int slot, SamplerStateProxy sampler)
        {
            context.BindSampler(shader, slot, sampler);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSampler(this PixelShader shader, DeviceContextProxy context, string name, SamplerStateProxy sampler)
        {
            int slot = shader.SamplerMapping.TryGetBindSlot(name);
            context.BindSampler(shader, slot, sampler);
        }

        /// <summary>
        /// Binds the samplers.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="samplers">The samplers.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSamplers(this PixelShader shader, DeviceContextProxy context, IEnumerable<KeyValuePair<int, SamplerStateProxy>> samplers)
        {
            foreach (var sampler in samplers)
            {
                context.BindSampler(shader, sampler.Key, sampler.Value);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ComputeShaderExtensions
    {
        /// <summary>
        /// Binds shader to pipeline
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Bind(this ComputeShader shader, DeviceContextProxy context, bool bindConstantBuffer = true)
        {
            context.SetShader(shader);
        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTexture(this ComputeShader shader, DeviceContextProxy context, string name, ShaderResourceViewProxy texture)
        {
            int slot = shader.ShaderResourceViewMapping.TryGetBindSlot(name);
            context.BindTexture(shader, slot, texture);
        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTexture(this ComputeShader shader, DeviceContextProxy context, int slot, ShaderResourceViewProxy texture)
        {
            context.BindTexture(shader, slot, texture);
        }
        /// <summary>
        /// Binds the textures.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="textures">The textures.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTextures(this ComputeShader shader, DeviceContextProxy context, IEnumerable<KeyValuePair<int, ShaderResourceViewProxy>> textures)
        {
            foreach (var texture in textures)
            {
                context.BindTexture(shader, texture.Key, texture.Value);
            }
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSampler(this ComputeShader shader, DeviceContextProxy context, int slot, SamplerStateProxy sampler)
        {
            context.BindSampler(shader, slot, sampler);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSampler(this ComputeShader shader, DeviceContextProxy context, string name, SamplerStateProxy sampler)
        {
            int slot = shader.SamplerMapping.TryGetBindSlot(name);
            context.BindSampler(shader, slot, sampler);
        }

        /// <summary>
        /// Binds the samplers.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="samplers">The samplers.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindSamplers(this ComputeShader shader, DeviceContextProxy context, IEnumerable<KeyValuePair<int, SamplerStateProxy>> samplers)
        {
            foreach (var sampler in samplers)
            {
                context.BindSampler(shader, sampler.Key, sampler.Value);
            }
        }
        /// <summary>
        /// Binds the UnorderedAccessView.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="uav">The uav.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindUAV(this ComputeShader shader, DeviceContextProxy context, int slot, UAVBufferViewProxy uav)
        {
            context.BindUnorderedAccessView(shader, slot, uav);
        }
        /// <summary>
        /// Binds the UnorderedAccessView.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="uav">The uav.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindUAV(this ComputeShader shader, DeviceContextProxy context, string name, UAVBufferViewProxy uav)
        {
            int slot = shader.UnorderedAccessViewMapping.TryGetBindSlot(name);
            context.BindUnorderedAccessView(shader, slot, uav);
        }
        /// <summary>
        /// Binds the UnorderedAccessViews.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="context">The context.</param>
        /// <param name="uavs">The uavs.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindUAVs(this ComputeShader shader, DeviceContextProxy context, IEnumerable<KeyValuePair<int, UAVBufferViewProxy>> uavs)
        {
            foreach (var uav in uavs)
            {
                context.BindUnorderedAccessView(shader, uav.Key, uav.Value);
            }
        }
    }
}
