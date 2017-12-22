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
    public class MeshRenderCore : MaterialGeometryRenderCore
    {
        public bool InvertNormal { set; get; } = false;       

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.InvertNormal = InvertNormal ? 1 : 0;
        }

        protected override void OnRender(IRenderMatrices context)
        {                  
            DefaultShaderPass.BindShader(context.DeviceContext);
            DefaultShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            if(!BindMaterialTextures(context.DeviceContext, DefaultShaderPass.GetShader(ShaderStage.Pixel)))
            {
                return;
            }
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
