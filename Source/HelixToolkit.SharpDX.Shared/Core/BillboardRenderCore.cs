using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class BillboardRenderCore : GeometryRenderCore
    {
        private EffectScalarVariable bHasTextureVar;
        private EffectScalarVariable bHasAlphaTextureVar;
        private EffectScalarVariable bFixedSizeVar;

        private EffectShaderResourceVariable textureVar;
        private EffectShaderResourceVariable alphaTextureVar;

        public bool FixedSize = true;
        

        protected override bool OnAttach(IRenderHost host, RenderTechnique technique)
        {
            if(base.OnAttach(host, technique))
            {
                bHasTextureVar = Collect(Effect.GetVariableByName(ShaderVariableNames.HasTextureVariable).AsScalar());
                textureVar = Collect(Effect.GetVariableByName(ShaderVariableNames.BillboardTextureVariable).AsShaderResource());
                alphaTextureVar = Collect(Effect.GetVariableByName(ShaderVariableNames.BillboardAlphaTextureVariable).AsShaderResource());
                bHasAlphaTextureVar = Collect(Effect.GetVariableByName(ShaderVariableNames.HasAlphaTextureVariable).AsScalar());
                bFixedSizeVar = Collect(Effect.GetVariableByName(ShaderVariableNames.BillboardFixedSizeVariable).AsScalar());
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void SetShaderVariables(IRenderMatrices matrices)
        {
            base.SetShaderVariables(matrices);
            var buffer = GeometryBuffer as IBillboardBufferModel;
            bHasTextureVar.Set(buffer.TextureView != null);
            bHasAlphaTextureVar.Set(buffer.AlphaTextureView != null);
            textureVar.SetResource(buffer.TextureView);
            alphaTextureVar.SetResource(buffer.AlphaTextureView);
            bFixedSizeVar.Set(FixedSize);
        }

        protected override void OnRender(IRenderMatrices context)
        {
            OnDraw(context.DeviceContext, InstanceBuffer);
        }

        protected override void OnDraw(DeviceContext context, IElementsBufferModel instanceModel)
        {
            var vertexCount = GeometryBuffer.Geometry.Positions.Count;
            var type = (GeometryBuffer as IBillboardBufferModel).Type;
            if (instanceModel == null || !instanceModel.HasElements)
            {
                switch (type)
                {
                    case BillboardType.MultipleText:
                        // Use foreground shader to draw text
                        EffectTechnique.GetPassByIndex(0).Apply(context);

                        // --- draw text, foreground vertex is beginning from 0.
                        context.Draw(vertexCount, 0);
                        break;
                    case BillboardType.SingleText:
                        if (vertexCount == 8)
                        {
                            var half = vertexCount / 2;
                            // Use background shader to draw background first
                            EffectTechnique.GetPassByIndex(1).Apply(context);
                            // --- draw background, background vertex is beginning from middle. <see cref="BillboardSingleText3D"/>
                            context.Draw(half, half);

                            // Use foreground shader to draw text
                            EffectTechnique.GetPassByIndex(0).Apply(context);

                            // --- draw text, foreground vertex is beginning from 0.
                            context.Draw(half, 0);
                        }
                        break;
                    case BillboardType.SingleImage:
                        // Use foreground shader to draw text
                        EffectTechnique.GetPassByIndex(2).Apply(context);
                        // --- draw text, foreground vertex is beginning from 0.
                        context.Draw(vertexCount, 0);
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case BillboardType.MultipleText:
                        // Use foreground shader to draw text
                        EffectTechnique.GetPassByIndex(0).Apply(context);

                        // --- draw text, foreground vertex is beginning from 0.
                        context.DrawInstanced(vertexCount, instanceModel.Buffer.Count, 0, 0);
                        break;
                    case BillboardType.SingleText:
                        if (vertexCount == 8)
                        {
                            var half = vertexCount / 2;
                            // Use background shader to draw background first
                            EffectTechnique.GetPassByIndex(1).Apply(context);
                            // --- draw background, background vertex is beginning from middle. <see cref="BillboardSingleText3D"/>
                            context.DrawInstanced(half, instanceModel.Buffer.Count, half, 0);

                            // Use foreground shader to draw text
                            EffectTechnique.GetPassByIndex(0).Apply(context);

                            // --- draw text, foreground vertex is beginning from 0.
                            context.DrawInstanced(half, instanceModel.Buffer.Count, 0, 0);
                        }
                        break;
                    case BillboardType.SingleImage:
                        // Use foreground shader to draw text
                        EffectTechnique.GetPassByIndex(2).Apply(context);
                        // --- draw text, foreground vertex is beginning from 0.
                        context.DrawInstanced(vertexCount, instanceModel.Buffer.Count, 0, 0);
                        break;
                }
            }
        }
    }
}
