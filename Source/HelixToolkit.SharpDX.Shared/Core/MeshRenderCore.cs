/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Render;
    public class MeshRenderCore : MaterialGeometryRenderCore, IInvertNormal
    {
        /// <summary>
        /// 
        /// </summary>
        public bool InvertNormal
        {
            set
            {
                SetAffectsRender(ref modelStruct.InvertNormal, (value ? 1 : 0));
            }
            get
            {
                return modelStruct.InvertNormal == 1 ? true : false;
            }
        }

        public string ShaderShadowMapTextureName { set; get; } = DefaultBufferNames.ShadowMapTB;

        private int shadowMapSlot;

        protected override void OnDefaultPassChanged(IShaderPass pass)
        {
            base.OnDefaultPassChanged(pass);
            shadowMapSlot = pass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(ShaderShadowMapTextureName);
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {                  
            DefaultShaderPass.BindShader(deviceContext);
            DefaultShaderPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
            if(!BindMaterialTextures(deviceContext, DefaultShaderPass))
            {
                return;
            }
            if (context.RenderHost.IsShadowMapEnabled)
            {
                DefaultShaderPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, shadowMapSlot, context.SharedResource.ShadowView);
            }
            OnDraw(deviceContext, InstanceBuffer);
        }

        protected override void OnRenderShadow(IRenderContext context, DeviceContextProxy deviceContext)
        {
            if (!IsThrowingShadow)
            {
                return;
            }
            ShadowPass.BindShader(deviceContext);
            ShadowPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
            OnDraw(deviceContext, InstanceBuffer);
        }
    }
}
