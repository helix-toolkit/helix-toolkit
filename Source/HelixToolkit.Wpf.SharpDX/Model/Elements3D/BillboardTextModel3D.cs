using System.Windows;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace HelixToolkit.Wpf.SharpDX.Model.Elements3D
{
    class BillboardTextModel3D : MeshGeometryModel3D
    {
        #region Dependency Properties

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color),
            typeof(BillboardTextModel3D),
            new UIPropertyMetadata(Color.Black, (o, e) => ((BillboardTextModel3D)o).OnColorChanged()));

        public Vector2 ScreenPixelSize
        {
            get { return (Vector2)GetValue(ScreenPixelSizeProperty); }
            set { SetValue(ScreenPixelSizeProperty, value); }
        }

        public static readonly DependencyProperty ScreenPixelSizeProperty =
            DependencyProperty.Register("ScreenPixelSizeProperty", typeof(Vector2),
            typeof(BillboardTextModel3D), new UIPropertyMetadata(new Vector2(320, 240)));

        #endregion

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
            var result = new BillboardVertex[vertexCount];

            for (var i = 0; i < vertexCount; i++)
            {
                result[i] = new BillboardVertex
                {
                    Position = new Vector4(position[i], 1.0f),
                    Color = billboardGeometry.Colors[i],
                    Offset = billboardGeometry.Offsets[i],
                    TexCoord = billboardGeometry.TextureCoordinates[i]
                };
            }

            return null;
        }

        private void OnColorChanged()
        {
            if (IsAttached)
            {
                /// --- set up buffers            
                vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer,
                    BillboardVertex.SizeInBytes, CreateBillboardVertexArray());
            }
        }

        #endregion
    }
}
