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
            this.renderTechnique = Techniques.RenderBillboard;
            this.effect = EffectsManager.Instance.GetEffect(renderTechnique);
            this.renderHost = host;            

            // --- get variables
            this.vertexLayout = EffectsManager.Instance.GetLayout(this.renderTechnique);
            this.effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);

            // --- transformations
            this.effectTransforms = new EffectTransformVariables(this.effect);

            // --- shader variables
            this.vViewport = effect.GetVariableByName("vViewport").AsVector();

            // --- get geometry
            var geometry = this.Geometry as BillboardText3D;
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
                DefaultVertex.SizeInBytes, CreateBillboardVertexArray());

            /// --- set rasterstate
            this.OnRasterStateChanged(this.DepthBias);

            /// --- flush
            this.Device.ImmediateContext.Flush();
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
                if (!this.IsRendering)
                    return;

                if (this.Geometry == null)
                    return;

                if (this.Visibility != System.Windows.Visibility.Visible)
                    return;

                if (renderContext.IsShadowPass)
                    if (!this.IsThrowingShadow)
                        return;
            }

            if (renderContext.Camera is ProjectionCamera)
            {
                var c = renderContext.Camera as ProjectionCamera;
                var width = ((float)renderContext.Canvas.ActualWidth);
                var height = ((float) renderContext.Canvas.ActualHeight);
                var viewport = new Vector4(width, height, 0, 0);
                this.vViewport.Set(ref viewport);
            }

            /// --- set constant paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            /// --- check shadowmaps
            //this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            //this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);

            /// --- set context
            this.Device.ImmediateContext.InputAssembler.InputLayout = this.vertexLayout;
            this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            /// --- set rasterstate            
            this.Device.ImmediateContext.Rasterizer.State = this.rasterState;

            /// --- bind buffer                
            this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, BillboardVertex.SizeInBytes, 0));
            /// --- render the geometry
            this.effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
            
            /// --- draw
            this.Device.ImmediateContext.Draw(Geometry.Positions.Count, 0);
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
