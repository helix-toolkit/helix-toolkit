using System.Windows;
﻿using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace HelixToolkit.Wpf.SharpDX
{
    public class BillboardTextModel3D : MeshGeometryModel3D
    {
        #region Private Class Data Members

        private EffectVectorVariable vViewport;
        private ShaderResourceView billboardTextureView;
        private EffectShaderResourceVariable billboardTextureVariable;

        #endregion

        #region Overridable Methods

        public override bool HitTest(Ray rayWS, ref List<HitTestResult> hits)
        {
            return false; // No hit testing on text geometry.
        }

        public override void Attach(IRenderHost host)
        {
            // --- attach
            renderTechnique = host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BillboardText];
            effect = host.EffectsManager.GetEffect(renderTechnique);
            renderHost = host;

            // --- get variables
            vertexLayout = renderHost.EffectsManager.GetLayout(renderTechnique);
            effectTechnique = effect.GetTechniqueByName(renderTechnique.Name);

            // --- transformations
            effectTransforms = new EffectTransformVariables(effect);

            // --- shader variables
            vViewport = effect.GetVariableByName("vViewport").AsVector();

            // --- get geometry
            var geometry = Geometry as BillboardText3D;
            if (geometry == null)
            {
                return;
            }

            // --- material 
            // this.AttachMaterial();
            billboardTextureVariable = effect.GetVariableByName("billboardTexture").AsShaderResource();

            var textureBytes = BillboardText3D.Texture.ToByteArray();
            billboardTextureView = ShaderResourceView.FromMemory(Device, textureBytes);
            billboardTextureVariable.SetResource(billboardTextureView);

            // -- set geometry if given
            vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer,
                VertexSizeInBytes, CreateBillboardVertexArray());

            /// --- set rasterstate
            OnRasterStateChanged(DepthBias);

            /// --- flush
            Device.ImmediateContext.Flush();
        }

        public override void Detach()
        {
            Disposer.RemoveAndDispose(ref vViewport);
            Disposer.RemoveAndDispose(ref billboardTextureVariable);
            Disposer.RemoveAndDispose(ref billboardTextureView);
            base.Detach();
        }

        public override void Render(RenderContext renderContext)
        {
            /// --- check to render the model
            {
                if (!IsRendering)
                    return;

                if (Geometry == null)
                    return;

                if (Visibility != System.Windows.Visibility.Visible)
                    return;

                if (renderContext.IsShadowPass)
                    if (!IsThrowingShadow)
                        return;
            }

            if (renderContext.Camera is ProjectionCamera)
            {
                var c = renderContext.Camera as ProjectionCamera;
                var width = ((float)renderContext.Canvas.ActualWidth);
                var height = ((float) renderContext.Canvas.ActualHeight);
                var viewport = new Vector4(width, height, 0, 0);
                vViewport.Set(ref viewport);
            }

            /// --- set constant paramerers             
            var worldMatrix = modelMatrix * renderContext.worldMatrix;
            effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            /// --- check shadowmaps
            //this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            //this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);

            /// --- set context
            Device.ImmediateContext.InputAssembler.InputLayout = vertexLayout;
            Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            /// --- set rasterstate            
            Device.ImmediateContext.Rasterizer.State = rasterState;

            /// --- bind buffer                
            Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, BillboardVertex.SizeInBytes, 0));
            /// --- render the geometry
            effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);

            /// --- draw
            Device.ImmediateContext.Draw(Geometry.Positions.Count, 0);
        }

        #endregion

        #region Private Helper Methdos

        private BillboardVertex[] CreateBillboardVertexArray()
        {
            var billboardGeometry = Geometry as BillboardText3D;

            // Gather all of the textInfo offsets.
            // These should be equal in number to the positions.
            foreach (var ti in billboardGeometry.TextInfo)
            {
                billboardGeometry.DrawText(ti);
            }

            var position = billboardGeometry.Positions.Array;
            var vertexCount = billboardGeometry.Positions.Count;
            var result = new List<BillboardVertex>();

            var allOffsets = billboardGeometry.TextInfo.SelectMany(ti => ti.Offsets).ToArray();

            for (var i = 0; i < vertexCount; i++)
            {
                var tc = billboardGeometry.TextureCoordinates[i];
                var vtx = new BillboardVertex
                {
                    Position = new Vector4(position[i], 1.0f),
                    Color = billboardGeometry.Colors[i],
                    TexCoord = new Vector4(tc.X,tc.Y, allOffsets[i].X, allOffsets[i].Y)
                };

                result.Add(vtx);
            }

            return result.ToArray();
        }

        #endregion
    }
}
