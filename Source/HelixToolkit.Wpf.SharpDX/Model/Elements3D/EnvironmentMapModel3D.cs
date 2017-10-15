// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentMapModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
            DependencyProperty.Register("Filename", typeof(string), typeof(EnvironmentMap3D), new AffectsRenderPropertyMetadata(null));

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
            DependencyProperty.Register("IsActive", typeof(bool), typeof(Element3D), new AffectsRenderPropertyMetadata(true, IsActiveChanged));

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

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.CubeMap];
        }
        protected override bool OnAttach(IRenderHost host)
        {           
            // --- get variables               
            this.vertexLayout = renderHost.EffectsManager.GetLayout(this.renderTechnique);
            this.effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);
            this.effectTransforms = new EffectTransformVariables(this.effect);

            // -- attach cube map 
            if (this.Filename != null)
            {
                // -- attach texture
                using (var texture = TextureLoader.FromFileAsResource(this.Device, this.Filename))
                {
                    this.texCubeMapView = new ShaderResourceView(this.Device, texture);
                }
                this.texCubeMap = effect.GetVariableByName("texCubeMap").AsShaderResource();
                this.texCubeMap.SetResource(this.texCubeMapView);
                this.bHasCubeMap = effect.GetVariableByName("bHasCubeMap").AsScalar();
                this.bHasCubeMap.Set(true);

                // --- set up geometry
                var sphere = new MeshBuilder(false,true,false);
                sphere.AddSphere(new Vector3(0, 0, 0));
                this.geometry = sphere.ToMeshGeometry3D();

                // --- set up vertex buffer
                this.vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, CubeVertex.SizeInBytes, this.geometry.Positions.Select((x, ii) => new CubeVertex() { Position = new Vector4(x, 1f) }).ToArray());

                // --- set up index buffer
                this.indexBuffer = Device.CreateBuffer(BindFlags.IndexBuffer, sizeof(int), geometry.Indices.Array, geometry.Indices.Count);

                // --- set up rasterizer states
                var rasterStateDesc = new RasterizerStateDescription()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.Back,
                    IsMultisampleEnabled = true,
                    IsAntialiasedLineEnabled = true,
                    IsFrontCounterClockwise = false,
                };
                this.rasterState = new RasterizerState(this.Device, rasterStateDesc);

                // --- set up depth stencil state
                var depthStencilDesc = new DepthStencilStateDescription()
                {
                    DepthComparison = Comparison.LessEqual,
                    DepthWriteMask = global::SharpDX.Direct3D11.DepthWriteMask.All,
                    IsDepthEnabled = true,
                };
                this.depthStencilState = new DepthStencilState(this.Device, depthStencilDesc);
                // --- flush
                //this.Device.ImmediateContext.Flush();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            if (!this.IsAttached)
                return;

            this.bHasCubeMap.Set(false);

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

            base.OnDetach();
        }

        protected override void OnRender(RenderContext renderContext)
        {
            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = this.vertexLayout;
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = Direct3D.PrimitiveTopology.TriangleList;
            renderContext.DeviceContext.InputAssembler.SetIndexBuffer(this.indexBuffer, Format.R32_UInt, 0);
            renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, CubeVertex.SizeInBytes, 0));

            renderContext.DeviceContext.Rasterizer.State = rasterState;
            renderContext.DeviceContext.OutputMerger.DepthStencilState = depthStencilState;

            // --- set constant paramerers 
            var worldMatrix = Matrix.Translation(renderContext.Camera.Position.ToVector3());
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            // --- render the geometry
            this.effectTechnique.GetPassByIndex(0).Apply(renderContext.DeviceContext);
            renderContext.DeviceContext.DrawIndexed(this.geometry.Indices.Count, 0, 0);
        }
    }
}