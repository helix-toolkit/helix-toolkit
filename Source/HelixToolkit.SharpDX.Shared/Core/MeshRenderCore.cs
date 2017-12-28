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
    public class MeshRenderCore : MaterialGeometryRenderCore
    {
        public bool InvertNormal { set; get; } = false;       

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.InvertNormal = InvertNormal ? 1 : 0;
        }

        protected override void OnRender(IRenderContext context)
        {                  
            DefaultShaderPass.BindShader(context.DeviceContext);
            DefaultShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            if(!BindMaterialTextures(context.DeviceContext, DefaultShaderPass.Shaders))
            {
                return;
            }
            if (context.RenderHost.IsShadowMapEnabled)
            {
                DefaultShaderPass.GetShader(ShaderStage.Pixel).BindTexture(context.DeviceContext, DefaultBufferNames.ShadowMapTB, context.SharedResource.ShadowView);
            }
            OnDraw(context.DeviceContext, InstanceBuffer);
        }

        protected override void OnRenderShadow(IRenderContext context)
        {
            if (!IsThrowingShadow)
            {
                return;
            }
            ShadowPass.BindShader(context.DeviceContext);
            ShadowPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
