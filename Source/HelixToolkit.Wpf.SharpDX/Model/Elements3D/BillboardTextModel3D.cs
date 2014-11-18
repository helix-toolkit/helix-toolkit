using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace HelixToolkit.Wpf.SharpDX
{
    public class BillboardTextModel3D : MeshGeometryModel3D
    {
        #region Overridable Methods

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

            // --- material 
            this.AttachMaterial();

            // --- get geometry
            var geometry = this.Geometry as BillboardText3D;
            if (geometry == null)
                throw new System.Exception("Geometry must not be null");

            // -- set geometry if given
            vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer,
                DefaultVertex.SizeInBytes, CreateBillboardVertexArray());

            /// --- set rasterstate
            this.OnRasterStateChanged(this.DepthBias);

            /// --- flush
            this.Device.ImmediateContext.Flush();
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

            /// --- set constant paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            /// --- check shadowmaps
            this.hasShadowMap = this.renderHost.IsShadowMapEnabled;
            this.effectMaterial.bHasShadowMapVariable.Set(this.hasShadowMap);

            /// --- set context
            this.Device.ImmediateContext.InputAssembler.InputLayout = this.vertexLayout;
            this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            /// --- set rasterstate            
            this.Device.ImmediateContext.Rasterizer.State = this.rasterState;

            /// --- bind buffer                
            this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, DefaultVertex.SizeInBytes, 0));
            /// --- render the geometry
            this.effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
            /// --- draw
            this.Device.ImmediateContext.DrawIndexed(this.Geometry.Indices.Count, 0, 0);
        }

        #endregion

        #region Private Helper Methdos

        private BillboardVertex[] CreateBillboardVertexArray()
        {
            var billboardGeometry = Geometry as BillboardText3D;
            var position = billboardGeometry.Positions.Array;
            var vertexCount = billboardGeometry.Positions.Count;
            var result = new List<BillboardVertex>();

            // Gather all of the textInfo offsets.
            // These should be equal in number to the positions.
            var allOffsets = billboardGeometry.TextInfo.SelectMany(ti => ti.Offsets).ToArray();

            for (var i = 0; i < vertexCount; i++)
            {
                var vtx = new BillboardVertex
                {
                    Position = new Vector4(position[i], 1.0f),
                    Color = billboardGeometry.Colors[i],
                    Offset = allOffsets[i],
                    TexCoord = billboardGeometry.TextureCoordinates[i]
                };

                result.Add(vtx);
            }

            return result.ToArray();
        }

        #endregion
    }
}
