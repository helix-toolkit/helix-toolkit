/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX.Direct3D;
    using global::SharpDX.DXGI;
    using Shaders;
    using Utilities;
    public class ShadowMapCore : RenderCoreBase<ShadowMapParamStruct>
    {
        protected ShaderResouceViewProxy viewResource { private set; get; }

        public Vector2 Resolution
        {
            set
            {
                modelStruct.ShadowMapSize = value;
            }
            get
            {
                return modelStruct.ShadowMapSize;
            }
        }

        public float FactorPCF { set; get; } = 1.5f;
        public float Bias { set; get; } = 0.0015f;

        public float Intensity { set; get; } = 0.5f;

        public ShadowMapCore()
        {
            Resolution = new Vector2(1024, 1024);
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ShadowParamCB, ShadowMapParamStruct.SizeInBytes);
        }

        protected virtual Texture2DDescription ShadowMapTextureDesc
        {
            get
            {
                return new Texture2DDescription()
                {
                    Format = Format.R32_Typeless, //!!!! because of depth and shader resource
                                                  //Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = (int)Resolution.X,
                    Height = (int)Resolution.Y,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource, //!!!!
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                };
            }
        }

        protected virtual DepthStencilViewDescription DepthStencilViewDesc
        {
            get
            {
                return new DepthStencilViewDescription()
                {
                    Format = Format.D32_Float,
                    Dimension = DepthStencilViewDimension.Texture2D,
                    Texture2D = new DepthStencilViewDescription.Texture2DResource()
                    {
                        MipSlice = 0
                    }
                };
            }
        }

        protected virtual ShaderResourceViewDescription ShaderResourceViewDesc
        {
            get
            {
                return new ShaderResourceViewDescription()
                {
                    Format = Format.R32_Float,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                    {
                        MipLevels = 1,
                        MostDetailedMip = 0,
                    }
                };
            }
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                viewResource = Collect(new ShaderResouceViewProxy(Device, ShadowMapTextureDesc));
                viewResource.CreateView(DepthStencilViewDesc);
                viewResource.CreateView(ShaderResourceViewDesc);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnRender(IRenderMatrices context)
        {
            
        }

        protected override void OnUpdatePerModelStruct(ref ShadowMapParamStruct model, IRenderMatrices context)
        {
            model.ShadowMapInfo = new Vector4(Intensity, FactorPCF, Bias, 0);
        }
    }
}
