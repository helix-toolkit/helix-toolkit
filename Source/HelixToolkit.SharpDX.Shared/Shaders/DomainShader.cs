/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Render;
    using Utilities;   

    /// <summary>
    /// 
    /// </summary>
    public sealed class DomainShader : ShaderBase
    {
        internal global::SharpDX.Direct3D11.DomainShader Shader { private set; get; }
        public static readonly DomainShader NullDomainShader = new DomainShader("NULL");
        /// <summary>
        /// Vertex Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public DomainShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Domain)
        {
            Shader = Collect(new global::SharpDX.Direct3D11.DomainShader(device, byteCode));
        }

        private DomainShader(string name)
            :base(name, ShaderStage.Domain, true)
        {

        }

        /// <summary>
        /// Binds shader to pipeline
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Bind(DeviceContextProxy context, bool bindConstantBuffer = true)
        {
            context.SetShader(this);
        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindTexture(DeviceContextProxy context, string name, ShaderResourceViewProxy texture)
        {
            int slot = this.ShaderResourceViewMapping.TryGetBindSlot(name);
            context.BindTexture(this, slot, texture);
        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindTexture(DeviceContextProxy context, int slot, ShaderResourceViewProxy texture)
        {
            context.BindTexture(this, slot, texture);
        }
        /// <summary>
        /// Binds the textures.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="textures">The textures.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindTextures(DeviceContextProxy context, IList<KeyValuePair<int, ShaderResourceViewProxy>> textures)
        {
            foreach (var texture in textures)
            {
                context.BindTexture(this, texture.Key, texture.Value);
            }
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSampler(DeviceContextProxy context, int slot, SamplerStateProxy sampler)
        {
            context.BindSampler(this, slot, sampler);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSampler(DeviceContextProxy context, string name, SamplerStateProxy sampler)
        {
            int slot = this.SamplerMapping.TryGetBindSlot(name);
            context.BindSampler(this, slot, sampler);
        }

        /// <summary>
        /// Binds the samplers.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="samplers">The samplers.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSamplers(DeviceContextProxy context, IList<KeyValuePair<int, SamplerStateProxy>> samplers)
        {
            foreach (var sampler in samplers)
            {
                context.BindSampler(this, sampler.Key, sampler.Value);
            }
        }
    }
}
