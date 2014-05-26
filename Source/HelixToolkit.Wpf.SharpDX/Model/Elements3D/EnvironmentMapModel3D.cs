namespace HelixToolkit.Wpf.SharpDX
{
    using System.Linq;
    using System.Windows;

    using global::SharpDX;
    using global::SharpDX.Direct3D11;

    using global::SharpDX.DXGI;

    using Direct3D = global::SharpDX.Direct3D;
    using Matrix = global::SharpDX.Matrix;
    using Texture2D = global::SharpDX.Direct3D11.Texture2D;

    public sealed class EnvironmentMap3D : Model3D
    {
        // members to dispose          
        private Buffer vertexBuffer;
        private Buffer indexBuffer;
        private InputLayout vertexLayout;
        private EffectTechnique effectTechnique;

        private ShaderResourceView texCubeMapView;
        private EffectShaderResourceVariable texCubeMap;
        private EffectScalarVariable bHasCubeMap;

        private RasterizerState rasterState;
        private DepthStencilState depthStencilState;

        private EffectTransformVariables effectTransforms;
        private MeshGeometry3D geometry;

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FilenameProperty =
            DependencyProperty.Register("Filename", typeof(string), typeof(EnvironmentMap3D), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the current environment map texture image
        /// (expects DDS Environment Map image)
        /// </summary>
        public string Filename
        {
            get { return (string)this.GetValue(FilenameProperty); }
            set { this.SetValue(FilenameProperty, value); }
        }

        /// <summary>
        /// Indicates, if this element is active, if not, the model will be not 
        /// rendered and not reflected.
        /// default is true.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(Element3D), new UIPropertyMetadata(true, IsActiveChanged));

        /// <summary>
        /// Indicates, if this element is active, if not, the model will be not 
        /// rendered and not reflected.
        /// default is true.
        /// </summary>
        public bool IsActive
        {
            get { return (bool)this.GetValue(IsActiveProperty); }
            set { this.SetValue(IsActiveProperty, value); }
        }

        private static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = ((EnvironmentMap3D)d);
            if (obj.IsAttached)
                obj.bHasCubeMap.Set((bool)e.NewValue);
        }

        public override void Attach(IRenderHost host)
        {
            /// --- attach
            this.renderTechnique = Techniques.RenderCubeMap;
            base.Attach(host);

            /// --- get variables               
            this.vertexLayout = EffectsManager.Instance.GetLayout(this.renderTechnique);
            this.effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);
            this.effectTransforms = new EffectTransformVariables(this.effect);

            /// -- attach cube map 
            if (this.Filename != null)
            {
                /// -- attach texture
                using (var texture = Texture2D.FromFile<Texture2D>(this.Device, this.Filename))
                {
                    this.texCubeMapView = new ShaderResourceView(this.Device, texture);
                }
                this.texCubeMap = effect.GetVariableByName("texCubeMap").AsShaderResource();
                this.texCubeMap.SetResource(this.texCubeMapView);
                this.bHasCubeMap = effect.GetVariableByName("bHasCubeMap").AsScalar();
                this.bHasCubeMap.Set(true);

                /// --- set up geometry
                var sphere = new MeshBuilder(false,true,false);
                sphere.AddSphere(new Vector3(0, 0, 0));
                this.geometry = sphere.ToMeshGeometry3D();

                /// --- set up vertex buffer
                this.vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, CubeVertex.SizeInBytes, this.geometry.Positions.Select((x, ii) => new CubeVertex() { Position = new Vector4(x, 1f) }).ToArray());

                /// --- set up index buffer
                this.indexBuffer = Device.CreateBuffer(BindFlags.IndexBuffer, sizeof(int), geometry.Indices.Array);

                /// --- set up rasterizer states
                var rasterStateDesc = new RasterizerStateDescription()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.Back,
                    IsMultisampleEnabled = true,
                    IsAntialiasedLineEnabled = true,
                    IsFrontCounterClockwise = false,
                };
                this.rasterState = new RasterizerState(this.Device, rasterStateDesc);

                /// --- set up depth stencil state
                var depthStencilDesc = new DepthStencilStateDescription()
                {
                    DepthComparison = Comparison.LessEqual,
                    DepthWriteMask = global::SharpDX.Direct3D11.DepthWriteMask.All,
                    IsDepthEnabled = true,
                };
                this.depthStencilState = new DepthStencilState(this.Device, depthStencilDesc);
            }

            /// --- flush
            this.Device.ImmediateContext.Flush();
        }

        public override void Detach()
        {
            if (!this.IsAttached)
                return;

            this.bHasCubeMap.Set(false);

            this.effectTechnique = null;
            this.effectTechnique = null;
            this.vertexLayout = null;
            this.geometry = null;

            Disposer.RemoveAndDispose(ref this.vertexBuffer);
            Disposer.RemoveAndDispose(ref this.indexBuffer);
            Disposer.RemoveAndDispose(ref this.texCubeMap);
            Disposer.RemoveAndDispose(ref this.texCubeMapView);
            Disposer.RemoveAndDispose(ref this.bHasCubeMap);
            Disposer.RemoveAndDispose(ref this.rasterState);
            Disposer.RemoveAndDispose(ref this.depthStencilState);
            Disposer.RemoveAndDispose(ref this.effectTransforms);

            base.Detach();
        }

        public override void Render(RenderContext context)
        {
            if (!this.IsRendering) return;

            /// --- set context
            this.Device.ImmediateContext.InputAssembler.InputLayout = this.vertexLayout;
            this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = Direct3D.PrimitiveTopology.TriangleList;
            this.Device.ImmediateContext.InputAssembler.SetIndexBuffer(this.indexBuffer, Format.R32_UInt, 0);
            this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, CubeVertex.SizeInBytes, 0));

            this.Device.ImmediateContext.Rasterizer.State = rasterState;
            this.Device.ImmediateContext.OutputMerger.DepthStencilState = depthStencilState;

            /// --- set constant paramerers 
            var worldMatrix = Matrix.Translation(((PerspectiveCamera)context.Camera).Position.ToVector3());
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            /// --- render the geometry
            this.effectTechnique.GetPassByIndex(0).Apply(Device.ImmediateContext);
            this.Device.ImmediateContext.DrawIndexed(this.geometry.Indices.Count, 0, 0);
        }
    }
}
