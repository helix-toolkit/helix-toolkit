/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using HelixToolkit.Wpf.SharpDX.Shaders;
using SharpDX;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class ShadowMapCore : RenderCoreBase<ShadowMapParamStruct>
    {
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

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ShadowParamCB, ShadowMapParamStruct.SizeInBytes);
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
