/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;


#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public static class DeviceContextExtension
    {
        public static void AttachConstantBuffer(this DeviceContext context, ShaderStage stage, int slot, Buffer buffer)
        {
            if (slot < 0)
            {
                return;
            }
            switch (stage)
            {
                case ShaderStage.None:
                    break;
                case ShaderStage.Vertex:
                    context.VertexShader.SetConstantBuffer(slot, buffer);
                    break;
                case ShaderStage.Domain:
                    context.DomainShader.SetConstantBuffer(slot, buffer);
                    break;
                case ShaderStage.Geometry:
                    context.GeometryShader.SetConstantBuffer(slot, buffer);
                    break;
                case ShaderStage.Hull:
                    context.HullShader.SetConstantBuffer(slot, buffer);
                    break;
                case ShaderStage.Pixel:
                    context.PixelShader.SetConstantBuffer(slot, buffer);
                    break;
                case ShaderStage.Compute:
                    context.ComputeShader.SetConstantBuffer(slot, buffer);
                    break;
                default:
                    break;
            }
        }

        public static void AttachConstantBuffer(this DeviceContext context, ShaderStage stage, int slot, params Buffer[] buffer)
        {
            if (slot < 0)
            {
                return;
            }
            switch (stage)
            {
                case ShaderStage.None:
                    break;
                case ShaderStage.Vertex:
                    context.VertexShader.SetConstantBuffers(slot, buffer);
                    break;
                case ShaderStage.Domain:
                    context.DomainShader.SetConstantBuffers(slot, buffer);
                    break;
                case ShaderStage.Geometry:
                    context.GeometryShader.SetConstantBuffers(slot, buffer);
                    break;
                case ShaderStage.Hull:
                    context.HullShader.SetConstantBuffers(slot, buffer);
                    break;
                case ShaderStage.Pixel:
                    context.PixelShader.SetConstantBuffers(slot, buffer);
                    break;
                case ShaderStage.Compute:
                    context.ComputeShader.SetConstantBuffers(slot, buffer);
                    break;
                default:
                    break;
            }
        }

        public static void AttachShaderResources(this DeviceContext context, ShaderStage stage, int slot, ShaderResourceView SRV)
        {
            if (slot < 0)
            { return; }
            switch (stage)
            {
                case ShaderStage.None:
                    break;
                case ShaderStage.Vertex:
                    context.VertexShader.SetShaderResource(slot, SRV);
                    break;
                case ShaderStage.Domain:
                    context.DomainShader.SetShaderResource(slot, SRV);
                    break;
                case ShaderStage.Geometry:
                    context.GeometryShader.SetShaderResource(slot, SRV);
                    break;
                case ShaderStage.Hull:
                    context.HullShader.SetShaderResource(slot, SRV);
                    break;
                case ShaderStage.Pixel:
                    context.PixelShader.SetShaderResource(slot, SRV);
                    break;
                case ShaderStage.Compute:
                    context.ComputeShader.SetShaderResource(slot, SRV);
                    break;
                default:
                    break;
            }
        }

        public static void AttachShaderResources(this DeviceContext context, ShaderStage stage, int slot, params ShaderResourceView[] SRV)
        {
            if (slot < 0)
            {
                return;
            }
            switch (stage)
            {
                case ShaderStage.None:
                    break;
                case ShaderStage.Vertex:
                    context.VertexShader.SetShaderResources(slot, SRV);
                    break;
                case ShaderStage.Domain:
                    context.DomainShader.SetShaderResources(slot, SRV);
                    break;
                case ShaderStage.Geometry:
                    context.GeometryShader.SetShaderResources(slot, SRV);
                    break;
                case ShaderStage.Hull:
                    context.HullShader.SetShaderResources(slot, SRV);
                    break;
                case ShaderStage.Pixel:
                    context.PixelShader.SetShaderResources(slot, SRV);
                    break;
                case ShaderStage.Compute:
                    context.ComputeShader.SetShaderResources(slot, SRV);
                    break;
                default:
                    break;
            }
        }

        public static void AttachUnorderedAccessViews(this DeviceContext context, ShaderStage stage, int slot, UnorderedAccessView UAV)
        {
            if (slot < 0) { return; }
            switch (stage)
            {
                case ShaderStage.Compute:
                    context.ComputeShader.SetUnorderedAccessView(slot, UAV);
                    break;
                default:
                    break;
            }
        }

        public static void AttachUnorderedAccessViews(this DeviceContext context, ShaderStage stage, int slot, params UnorderedAccessView[] UAVs)
        {
            if (slot < 0) { return; }
            switch (stage)
            {
                case ShaderStage.Compute:
                    context.ComputeShader.SetUnorderedAccessViews(slot, UAVs);
                    break;
                default:
                    break;
            }
        }
    }
}
