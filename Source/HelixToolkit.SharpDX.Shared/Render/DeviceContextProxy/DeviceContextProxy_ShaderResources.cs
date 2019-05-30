using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;

#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Shaders;
    public partial class DeviceContextProxy
    {
        public const int ConstantBufferCount = CommonShaderStage.ConstantBufferApiSlotCount;
        public const int SamplerStateCount = CommonShaderStage.SamplerSlotCount;
        public const int ShaderResourceViewCount = CommonShaderStage.InputResourceSlotCount;
        public const int SimultaneousRenderTargetCount = OutputMergerStage.SimultaneousRenderTargetCount;
        public const int StageCount = 6;
        public const int UnorderedAcccesViewCount = ComputeShaderStage.UnorderedAccessViewSlotCount;
        #region Constant Buffer Check
        private readonly object[] ConstantBufferCheck = new object[ConstantBufferCount * StageCount]; //Used to store current bind constant buffers. Reduce API call and state change.
        private readonly object[] SamplerStateCheck = new object[SamplerStateCount * StageCount];//Used to store current bind sampler states. Reduce API Call and state change.
        private const int CBCheckVertexShaderStartIdx = Constants.VertexIdx * ConstantBufferCount;
        private const int CBCheckHullShaderStartIdx = Constants.HullIdx * ConstantBufferCount;
        private const int CBCheckDomainShaderStartIdx = Constants.DomainIdx * ConstantBufferCount;
        private const int CBCheckGeometryShaderStartIdx = Constants.GeometryIdx * ConstantBufferCount;
        private const int CBCheckPixelShaderStartIdx = Constants.PixelIdx * ConstantBufferCount;
        private const int CBCheckComputeShaderStartIdx = Constants.ComputeIdx * ConstantBufferCount;

        private const int SamplerCheckVertexShaderStartIdx = Constants.VertexIdx * SamplerStateCount;
        private const int SamplerCheckHullShaderStartIdx = Constants.HullIdx * SamplerStateCount;
        private const int SamplerCheckDomainShaderStartIdx = Constants.DomainIdx * SamplerStateCount;
        private const int SamplerCheckGeometryShaderStartIdx = Constants.GeometryIdx * SamplerStateCount;
        private const int SamplerCheckPixelShaderStartIdx = Constants.PixelIdx * SamplerStateCount;
        private const int SamplerCheckComputeShaderStartIdx = Constants.ComputeIdx * SamplerStateCount;
        #endregion
        #region Set Shaders and Constant Buffers
        /// <summary>
        /// Sets the vertex shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(VertexShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.VertexShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    int idx = CBCheckVertexShaderStartIdx + buff.Key;
                    if(ConstantBufferCheck[idx] != buff.Value)
                    {
                        ConstantBufferCheck[idx] = buff.Value;
                        deviceContext.VertexShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the hull shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(HullShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.HullShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    int idx = CBCheckHullShaderStartIdx + buff.Key;
                    if(ConstantBufferCheck[idx] != buff.Value)
                    {
                        ConstantBufferCheck[idx] = buff.Value;
                        deviceContext.HullShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the domain shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(DomainShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.DomainShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    int idx = CBCheckDomainShaderStartIdx + buff.Key;
                    if(ConstantBufferCheck[idx] != buff.Value)
                    {
                        ConstantBufferCheck[idx] = buff.Value;
                        deviceContext.DomainShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the geometry shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(GeometryShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.GeometryShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    int idx = CBCheckGeometryShaderStartIdx + buff.Key;
                    if(ConstantBufferCheck[idx] != buff.Value)
                    {
                        ConstantBufferCheck[idx] = buff.Value;
                        deviceContext.GeometryShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the pixel shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(PixelShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.PixelShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    int idx = CBCheckPixelShaderStartIdx + buff.Key;
                    if(ConstantBufferCheck[idx] != buff.Value)
                    {
                        ConstantBufferCheck[idx] = buff.Value;
                        deviceContext.PixelShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the compute shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(ComputeShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.ComputeShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    int idx = CBCheckComputeShaderStartIdx + buff.Key;
                    if(ConstantBufferCheck[idx] != buff.Value)
                    {
                        ConstantBufferCheck[idx] = buff.Value;
                        deviceContext.ComputeShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                    }
                }
            }
        }

        #endregion Set Shaders and Constant Buffers

        #region Set Get ShaderResources

        #region Vertex Shader

        /// <summary>
        /// Binds the texture.  Use <see cref="VertexShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shader type. Use <see cref="VertexShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResource(VertexShaderType shaderType, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.VertexShader.SetShaderResource(slot, texture);
        }

        /// <summary>
        /// Binds the texture. Use <see cref="VertexShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shader. Use <see cref="VertexShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResources(VertexShaderType shaderType, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.VertexShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Gets the texture. Use <see cref="VertexShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shader type.  Use <see cref="VertexShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResources(VertexShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.VertexShader.GetShaderResources(startSlot, num);
        }
        /// <summary>
        /// Binds the sampler.  Use <see cref="VertexShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shader type.  Use <see cref="VertexShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSampler(VertexShaderType shaderType, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckVertexShaderStartIdx + slot;
            if(SamplerStateCheck[idx] != sampler)
            {
                SamplerStateCheck[idx] = sampler;
                deviceContext.VertexShader.SetSampler(slot, sampler);
            }
        }

        /// <summary>
        /// Binds the sampler. Use <see cref="VertexShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shader type. Use <see cref="VertexShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="samplers">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplers(VertexShaderType shaderType, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckVertexShaderStartIdx + slot;
            bool needUpdate = false;
            for(int i = 0; i < samplers.Length; ++i)
            {
                if(SamplerStateCheck[idx+i] != samplers[i])
                {
                    needUpdate = true;
                    break;
                }           
            }
            if (needUpdate)
            {
                for(int i = 0; i < samplers.Length; ++i)
                {
                    SamplerStateCheck[idx + i] = samplers[i];
                }
                deviceContext.VertexShader.SetSamplers(slot, samplers);
            }
        }
        /// <summary>
        /// Gets the sampler. Use <see cref="VertexShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="VertexShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSampler(VertexShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.VertexShader.GetSamplers(startSlot, num);
        }
        #endregion Vertex Shader

        #region Domain Shader

        /// <summary>
        /// Binds the texture. Use <see cref="DomainShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="DomainShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResource(DomainShaderType shaderType, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.DomainShader.SetShaderResource(slot, texture);
        }

        /// <summary>
        /// Binds the texture. Use <see cref="DomainShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="DomainShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResources(DomainShaderType shaderType, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.DomainShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Gets the texture. Use <see cref="DomainShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="DomainShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResources(DomainShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.DomainShader.GetShaderResources(startSlot, num);
        }
        /// <summary>
        /// Binds the sampler. Use <see cref="DomainShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="DomainShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSampler(DomainShaderType shaderType, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckDomainShaderStartIdx + slot;
            if(SamplerStateCheck[idx] != sampler)
            {
                SamplerStateCheck[idx] = sampler;
                deviceContext.DomainShader.SetSampler(slot, sampler);
            }
        }

        /// <summary>
        /// Binds the sampler. Use <see cref="DomainShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="DomainShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="samplers">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplers(DomainShaderType shaderType, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckDomainShaderStartIdx + slot;
            bool needUpdate = false;
            for (int i = 0; i < samplers.Length; ++i)
            {
                if (SamplerStateCheck[idx + i] != samplers[i])
                {
                    needUpdate = true;
                    break;
                }
            }
            if (needUpdate)
            {
                for (int i = 0; i < samplers.Length; ++i)
                {
                    SamplerStateCheck[idx + i] = samplers[i];
                }
                deviceContext.DomainShader.SetSamplers(slot, samplers);
            }
        }
        /// <summary>
        /// Gets the sampler. Use <see cref="DomainShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="DomainShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSampler(DomainShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.DomainShader.GetSamplers(startSlot, num);
        }
        #endregion Domain Shader

        #region Hull Shader

        /// <summary>
        /// Binds the texture. Use <see cref="HullShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="HullShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResource(HullShaderType shaderType, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.HullShader.SetShaderResource(slot, texture);
        }

        /// <summary>
        /// Binds the texture. Use <see cref="HullShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="HullShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResources(HullShaderType shaderType, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.HullShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Gets the texture. Use <see cref="HullShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="HullShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResources(HullShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.HullShader.GetShaderResources(startSlot, num);
        }
        /// <summary>
        /// Binds the sampler. Use <see cref="HullShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="HullShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSampler(HullShaderType shaderType, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckHullShaderStartIdx + slot;
            if (SamplerStateCheck[idx] != sampler)
            {
                SamplerStateCheck[idx] = sampler;
                deviceContext.HullShader.SetSampler(slot, sampler);
            }
        }

        /// <summary>
        /// Binds the sampler. Use <see cref="HullShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="HullShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="samplers">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplers(HullShaderType shaderType, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckHullShaderStartIdx + slot;
            bool needUpdate = false;
            for (int i = 0; i < samplers.Length; ++i)
            {
                if (SamplerStateCheck[idx + i] != samplers[i])
                {
                    needUpdate = true;
                    break;
                }
            }
            if (needUpdate)
            {
                for (int i = 0; i < samplers.Length; ++i)
                {
                    SamplerStateCheck[idx + i] = samplers[i];
                }
                deviceContext.HullShader.SetSamplers(slot, samplers);
            }
        }
        /// <summary>
        /// Gets the sampler. Use <see cref="HullShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="HullShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSampler(HullShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.HullShader.GetSamplers(startSlot, num);
        }
        #endregion Hull Shader

        #region Geometry Shader

        /// <summary>
        /// Binds the texture. Use <see cref="GeometryShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="GeometryShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResource(GeometryShaderType shaderType, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.GeometryShader.SetShaderResource(slot, texture);
        }

        /// <summary>
        /// Binds the texture. Use <see cref="GeometryShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="GeometryShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResources(GeometryShaderType shaderType, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.GeometryShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Gets the texture. Use <see cref="GeometryShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="GeometryShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResources(GeometryShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.GeometryShader.GetShaderResources(startSlot, num);
        }
        /// <summary>
        /// Binds the sampler. Use <see cref="GeometryShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="GeometryShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSampler(GeometryShaderType shaderType, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckGeometryShaderStartIdx + slot;
            if (SamplerStateCheck[idx] != sampler)
            {
                SamplerStateCheck[idx] = sampler;
                deviceContext.GeometryShader.SetSampler(slot, sampler);
            }
        }

        /// <summary>
        /// Binds the sampler. Use <see cref="GeometryShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="GeometryShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="samplers">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplers(GeometryShaderType shaderType, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckGeometryShaderStartIdx + slot;
            bool needUpdate = false;
            for (int i = 0; i < samplers.Length; ++i)
            {
                if (SamplerStateCheck[idx + i] != samplers[i])
                {
                    needUpdate = true;
                    break;
                }
            }
            if (needUpdate)
            {
                for (int i = 0; i < samplers.Length; ++i)
                {
                    SamplerStateCheck[idx + i] = samplers[i];
                }
                deviceContext.GeometryShader.SetSamplers(slot, samplers);
            }
        }
        /// <summary>
        /// Gets the sampler. Use <see cref="GeometryShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="GeometryShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSampler(GeometryShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.GeometryShader.GetSamplers(startSlot, num);
        }
        #endregion Geometry Shader

        #region Pixel Shader

        /// <summary>
        /// Binds the texture. Use <see cref="PixelShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="PixelShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResource(PixelShaderType shaderType, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.PixelShader.SetShaderResource(slot, texture);
        }

        /// <summary>
        /// Binds the texture. Use <see cref="PixelShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="PixelShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResources(PixelShaderType shaderType, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.PixelShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Gets the texture. Use <see cref="PixelShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="PixelShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResources(PixelShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.PixelShader.GetShaderResources(startSlot, num);
        }
        /// <summary>
        /// Binds the sampler. Use <see cref="PixelShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="PixelShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSampler(PixelShaderType shaderType, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckPixelShaderStartIdx + slot;
            if(SamplerStateCheck[idx] != sampler)
            {
                SamplerStateCheck[idx] = sampler;
                deviceContext.PixelShader.SetSampler(slot, sampler);
            }
        }

        /// <summary>
        /// Binds the sampler. Use <see cref="PixelShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="PixelShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="samplers">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplers(PixelShaderType shaderType, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckPixelShaderStartIdx + slot;
            bool needUpdate = false;
            for (int i = 0; i < samplers.Length; ++i)
            {
                if (SamplerStateCheck[idx + i] != samplers[i])
                {
                    needUpdate = true;
                    break;
                }
            }
            if (needUpdate)
            {
                for (int i = 0; i < samplers.Length; ++i)
                {
                    SamplerStateCheck[idx + i] = samplers[i];
                }
                deviceContext.PixelShader.SetSamplers(slot, samplers);
            }           
        }
        /// <summary>
        /// Gets the sampler. Use <see cref="PixelShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="PixelShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSampler(PixelShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.PixelShader.GetSamplers(startSlot, num);
        }
        #endregion Pixel Shader

        #region Compute Shader

        /// <summary>
        /// Binds the texture. Use <see cref="ComputeShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="ComputeShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResource(ComputeShaderType shaderType, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetShaderResource(slot, texture);
        }

        /// <summary>
        /// Binds the texture. Use <see cref="ComputeShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="ComputeShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResources(ComputeShaderType shaderType, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Gets the texture. Use <see cref="ComputeShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="ComputeShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResources(ComputeShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.ComputeShader.GetShaderResources(startSlot, num);
        }
        /// <summary>
        /// Binds the unordered access view. Use <see cref="ComputeShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="ComputeShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="uav">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUnorderedAccessView(ComputeShaderType shaderType, int slot, UnorderedAccessView uav)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetUnorderedAccessView(slot, uav);
        }

        /// <summary>
        /// Binds the unordered access views. Use <see cref="ComputeShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="ComputeShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="UAVs">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUnorderedAccessViews(ComputeShaderType shaderType, int slot, UnorderedAccessView[] UAVs)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetUnorderedAccessViews(slot, UAVs);
        }
        /// <summary>
        /// Gets the unordered access view. Use <see cref="ComputeShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="ComputeShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnorderedAccessView[] GetUnorderedAccessView(ComputeShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.ComputeShader.GetUnorderedAccessViews(startSlot, num);
        }
        /// <summary>
        /// Binds the sampler. Use <see cref="ComputeShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="ComputeShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSampler(ComputeShaderType shaderType, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckComputeShaderStartIdx + slot;
            if(SamplerStateCheck[idx] != sampler)
            {
                SamplerStateCheck[idx] = sampler;
                deviceContext.ComputeShader.SetSampler(slot, sampler);
            }
        }

        /// <summary>
        /// Binds the sampler. Use <see cref="ComputeShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="ComputeShader.Type"/></param>
        /// <param name="slot">The slot.</param>
        /// <param name="samplers">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplers(ComputeShaderType shaderType, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            int idx = SamplerCheckComputeShaderStartIdx + slot;
            bool needUpdate = false;
            for (int i = 0; i < samplers.Length; ++i)
            {
                if (SamplerStateCheck[idx + i] != samplers[i])
                {
                    needUpdate = true;
                    break;
                }
            }
            if (needUpdate)
            {
                for (int i = 0; i < samplers.Length; ++i)
                {
                    SamplerStateCheck[idx + i] = samplers[i];
                }
                deviceContext.ComputeShader.SetSamplers(slot, samplers);
            }            
        }
        /// <summary>
        /// Gets the sampler. Use <see cref="ComputeShader.Type"/>
        /// </summary>
        /// <param name="shaderType">The shaderType. Use <see cref="ComputeShader.Type"/></param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSampler(ComputeShaderType shaderType, int startSlot, int num)
        {
            return deviceContext.ComputeShader.GetSamplers(startSlot, num);
        }
        #endregion Compute Shader

        #endregion Set ShaderResources

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderPass(ShaderPass pass, bool bindConstantBuffer = true)
        {
            if (pass.Topology != global::SharpDX.Direct3D.PrimitiveTopology.Undefined)
            {
                //If specified, set topology
                PrimitiveTopology = pass.Topology;
            }
            if (CurrShaderPass == pass || pass.IsNULL)
            {
                return;
            }
            SetShader(pass.VertexShader, bindConstantBuffer);
            SetShader(pass.PixelShader, bindConstantBuffer);
            SetShader(pass.ComputeShader, bindConstantBuffer);
            SetShader(pass.HullShader, bindConstantBuffer);
            SetShader(pass.DomainShader, bindConstantBuffer);
            SetShader(pass.GeometryShader, bindConstantBuffer);
            CurrShaderPass = pass;
        }
    }
}
