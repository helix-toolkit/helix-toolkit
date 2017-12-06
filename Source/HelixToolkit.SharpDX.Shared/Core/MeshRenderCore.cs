using System;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class MeshRenderCore : MaterialGeometryRenderCore
    {
        public bool InvertNormal { set; get; } = false;
        private BlendState blendState;
        private DepthStencilState depthState;

       // private EffectScalarVariable bInvertNormalVar;

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                //      bInvertNormalVar = Collect(Effect.GetVariableByName(ShaderVariableNames.InvertNormal).AsScalar());
                var desc = new BlendStateDescription();
                desc.RenderTarget[0] = new RenderTargetBlendDescription()
                {
                    AlphaBlendOperation = BlendOperation.Add,
                    BlendOperation = BlendOperation.Add,
                    DestinationBlend = BlendOption.InverseSourceAlpha,
                    SourceBlend = BlendOption.SourceAlpha,
                    DestinationAlphaBlend = BlendOption.DestinationAlpha,
                    SourceAlphaBlend = BlendOption.SourceAlpha,
                    IsBlendEnabled = true,
                    RenderTargetWriteMask = ColorWriteMaskFlags.All
                };
                blendState = Collect(new BlendState(technique.Device, desc));

                var depthDesc = new DepthStencilStateDescription() { IsDepthEnabled = true, DepthWriteMask = DepthWriteMask.All, DepthComparison = Comparison.Less, IsStencilEnabled = false };
                depthState = Collect(new DepthStencilState(technique.Device, depthDesc));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnUpdateModelStruct(IRenderMatrices context)
        {
            base.OnUpdateModelStruct(context);
            modelStruct.InvertNormal = InvertNormal ? 1u : 0;
        }

        protected override void OnRender(IRenderMatrices context)
        {
            //EffectTechnique.GetPassByIndex(0).Apply(context.DeviceContext);
            context.DeviceContext.OutputMerger.SetBlendState(blendState);
            context.DeviceContext.OutputMerger.SetDepthStencilState(depthState);                      
            SetMaterialVariables(GeometryBuffer.Geometry as MeshGeometry3D, context);
            SetModelConstantBuffer(context.DeviceContext);
            EffectTechnique.BindShader(context.DeviceContext);
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
