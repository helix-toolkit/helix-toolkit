namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using global::SharpDX;

    using global::SharpDX.Direct3D;

    using global::SharpDX.Direct3D11;

    using global::SharpDX.DXGI;

    using Color = global::SharpDX.Color;

    public class LineGeometryModel3D : GeometryModel3D
    {
        private InputLayout vertexLayout;        
        private Buffer vertexBuffer;
        private Buffer indexBuffer;
        private Buffer instanceBuffer;
        private EffectTechnique effectTechnique;
        private EffectTransformVariables effectTransforms;
        private EffectVectorVariable vFrustum, vViewport, vLineParams;                
        //private DepthStencilState depthStencilState;
        //private LineGeometry3D geometry;
        private EffectScalarVariable bHasInstances;
        private Matrix[] instanceArray;
        private bool hasInstances = false;
        private bool isChanged = true;

        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(LineGeometryModel3D), new UIPropertyMetadata(Color.Black, (o, e) => ((LineGeometryModel3D)o).OnColorChanged()));

        public double Thickness
        {
            get { return (double)this.GetValue(ThicknessProperty); }
            set { this.SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LineGeometryModel3D), new UIPropertyMetadata(1.0));

        public double Smoothness
        {
            get { return (double)this.GetValue(SmoothnessProperty); }
            set { this.SetValue(SmoothnessProperty, value); }
        }

        public static readonly DependencyProperty SmoothnessProperty =
            DependencyProperty.Register("Smoothness", typeof(double), typeof(LineGeometryModel3D), new UIPropertyMetadata(0.0));



        public IEnumerable<Matrix> Instances
        {
            get { return (IEnumerable<Matrix>)this.GetValue(InstancesProperty); }
            set { this.SetValue(InstancesProperty, value); }
        }

        public static readonly DependencyProperty InstancesProperty =
            DependencyProperty.Register("Instances", typeof(IEnumerable<Matrix>), typeof(LineGeometryModel3D), new UIPropertyMetadata(null, InstancesChanged));

        protected static void InstancesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (LineGeometryModel3D)d;
            if (e.NewValue != null)
            {
                model.instanceArray = ((IEnumerable<Matrix>)e.NewValue).ToArray();
            }
            else
            {
                model.instanceArray = null;
            }
            model.isChanged = true;
        }


        protected override void OnRasterStateChanged(int depthBias)
        {
            if (this.IsAttached)
            {
                Disposer.RemoveAndDispose(ref this.rasterState);
                /// --- set up rasterizer states
                var rasterStateDesc = new RasterizerStateDescription()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.None,
                    DepthBias = depthBias,
                    DepthBiasClamp = -1000,
                    SlopeScaledDepthBias = -2,
                    IsDepthClipEnabled = true,
                    IsFrontCounterClockwise = false,

                    IsMultisampleEnabled = true,
                    IsAntialiasedLineEnabled = true,
                    //IsScissorEnabled = true,
                };

                try { this.rasterState = new RasterizerState(this.Device, rasterStateDesc); }
                catch (System.Exception)
                {
                }
            }
        }

        private void OnColorChanged()
        {
            if (this.IsAttached)
            {
                /// --- set up buffers            
                this.vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, DefaultVertex.SizeInBytes, this.Geometry.Positions.Select((x, ii) => new LinesVertex()
                {
                    Position = new Vector4(x, 1f),
                    Color = this.Color,
                }).ToArray());
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        public override void Attach(IRenderHost host)
        {
            /// --- attach            
            this.renderTechnique = Techniques.RenderLines;
            base.Attach(host);

            if (this.Geometry == null)            
                return;

            if (renderHost.RenderTechnique == Techniques.RenderDeferred || renderHost.RenderTechnique == Techniques.RenderGBuffer)
                return;  

            // --- get device
            this.vertexLayout = EffectsManager.Instance.GetLayout(this.renderTechnique);
            this.effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);

            this.effectTransforms = new EffectTransformVariables(this.effect);
            
            // --- get geometry
            var geometry = this.Geometry as LineGeometry3D;

            // -- set geometry if given
            if (geometry != null)
            {
                /// --- set up buffers            
                this.vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, DefaultVertex.SizeInBytes, this.Geometry.Positions.Select((x, ii) => new LinesVertex()
                {
                    Position = new Vector4(x, 1f),
                    Color = this.Color,
                }).ToArray());

                /// --- set up indexbuffer
                this.indexBuffer = Device.CreateBuffer(BindFlags.IndexBuffer, sizeof(int), geometry.Indices.Array);
            }

            /// --- init instances buffer            
            this.hasInstances = (this.Instances != null)&&(this.Instances.Any());
            this.bHasInstances = this.effect.GetVariableByName("bHasInstances").AsScalar();
            if (this.hasInstances)
            {
                this.instanceBuffer = Buffer.Create(this.Device, this.instanceArray, new BufferDescription(Matrix.SizeInBytes * this.instanceArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
            }

            /// --- set up const variables
            this.vViewport = effect.GetVariableByName("vViewport").AsVector();
            this.vFrustum = effect.GetVariableByName("vFrustum").AsVector();
            this.vLineParams = effect.GetVariableByName("vLineParams").AsVector();

            /// --- set effect per object const vars
            var lineParams = new Vector4((float)this.Thickness, (float)this.Smoothness, 0, 0);
            this.vLineParams.Set(lineParams);

            /// === debug hack
            //{
            //    var texDiffuseMapView = ShaderResourceView.FromFile(device, @"G:\Projects\Deformation Project\FrameworkWPF2012\Externals\HelixToolkit-SharpDX\Source\Examples\SharpDX.Wpf\LightingDemo\TextureCheckerboard2.jpg");
            //    var texDiffuseMap = effect.GetVariableByName("texDiffuseMap").AsShaderResource();
            //    texDiffuseMap.SetResource(texDiffuseMapView);                
            //}

            /// --- create raster state
            this.OnRasterStateChanged(this.DepthBias);
            

            
            //this.rasterState = new RasterizerState(this.device, rasterStateDesc);

            /// --- set up depth stencil state
            //var depthStencilDesc = new DepthStencilStateDescription()
            //{
            //    DepthComparison = Comparison.Less,
            //    DepthWriteMask = global::SharpDX.Direct3D11.DepthWriteMask.All,
            //    IsDepthEnabled = true,
            //};
            //this.depthStencilState = new DepthStencilState(this.device, depthStencilDesc);   
           

            /// --- flush
            this.Device.ImmediateContext.Flush();
        }        

        /// <summary>
        /// 
        /// </summary>
        public override void Detach()
        {
            Disposer.RemoveAndDispose(ref this.vertexBuffer);
            Disposer.RemoveAndDispose(ref this.indexBuffer);
            Disposer.RemoveAndDispose(ref this.instanceBuffer);
            Disposer.RemoveAndDispose(ref this.vFrustum);
            Disposer.RemoveAndDispose(ref this.vViewport);
            Disposer.RemoveAndDispose(ref this.vLineParams);            
            Disposer.RemoveAndDispose(ref this.rasterState);
            //Disposer.RemoveAndDispose(ref this.depthStencilState);
            Disposer.RemoveAndDispose(ref this.bHasInstances);

            this.renderTechnique = null;
            this.effectTechnique = null;
            this.vertexLayout = null;

            base.Detach();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Render(RenderContext renderContext)
        {
            /// --- do not render, if not enabled
            if (!this.IsRendering)
                return;

            if (this.Geometry == null)
                return;

            if (this.Visibility != System.Windows.Visibility.Visible)
                return;

            if (renderHost.RenderTechnique == Techniques.RenderDeferred || renderHost.RenderTechnique == Techniques.RenderGBuffer)
                return;

            if (renderContext.IsShadowPass)
                if (!this.IsThrowingShadow)
                    return;

            /// --- since these values are changed only per window resize, we set them only once here
            //if (this.isResized || renderContext.Camera != this.lastCamera)
            {
                //this.isResized = false;
                //this.lastCamera = renderContext.Camera;

                if (renderContext.Camera is ProjectionCamera)
                {
                    var c = renderContext.Camera as ProjectionCamera;
                    // viewport: W,H,0,0   
                    var viewport = new Vector4((float)renderContext.Canvas.ActualWidth, (float)renderContext.Canvas.ActualHeight, 0, 0);
                    var ar = viewport.X / viewport.Y;
                    var fov = 100.0; // this is a fake value, since the line shader does not use it!
                    var zn = c.NearPlaneDistance > 0 ? c.NearPlaneDistance : 0.1;
                    var zf = c.FarPlaneDistance + 0.0;
                    // frustum: FOV,AR,N,F
                    var frustum = new Vector4((float)fov, (float)ar, (float)zn, (float)zf);
                    this.vViewport.Set(ref viewport);
                    this.vFrustum.Set(ref frustum);
                }
            }
            /// --- set transform paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            /// --- set effect per object const vars
            var lineParams = new Vector4((float)this.Thickness, (float)this.Smoothness, 0, 0);
            this.vLineParams.Set(lineParams);
            
            /// --- set context
            this.Device.ImmediateContext.InputAssembler.InputLayout = this.vertexLayout;
            this.Device.ImmediateContext.InputAssembler.SetIndexBuffer(this.indexBuffer, Format.R32_UInt, 0);
            this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;

            /// --- check instancing
            this.hasInstances = (this.Instances != null)&&(this.Instances.Any());
            this.bHasInstances.Set(this.hasInstances);

            /// --- set rasterstate            
            this.Device.ImmediateContext.Rasterizer.State = this.rasterState;

            if (this.hasInstances)
            {
                /// --- update instance buffer
                if (this.isChanged)
                {
                    this.instanceBuffer = Buffer.Create(this.Device, this.instanceArray, new BufferDescription(Matrix.SizeInBytes * this.instanceArray.Length, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
                    DataStream stream;
                    Device.ImmediateContext.MapSubresource(this.instanceBuffer, MapMode.WriteDiscard, global::SharpDX.Direct3D11.MapFlags.None, out stream);
                    stream.Position = 0;
                    stream.WriteRange(this.instanceArray, 0, this.instanceArray.Length);
                    Device.ImmediateContext.UnmapSubresource(this.instanceBuffer, 0);
                    stream.Dispose();
                    this.isChanged = false;
                }

                /// --- INSTANCING: need to set 2 buffers            
                this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new[] 
                {
                    new VertexBufferBinding(this.vertexBuffer, LinesVertex.SizeInBytes, 0),
                    new VertexBufferBinding(this.instanceBuffer, Matrix.SizeInBytes, 0),
                });

                /// --- render the geometry
                for (int i = 0; i < this.effectTechnique.Description.PassCount; i++)
                {
                    this.effectTechnique.GetPassByIndex(i).Apply(Device.ImmediateContext);
                    this.Device.ImmediateContext.DrawIndexedInstanced(this.Geometry.Indices.Count, this.instanceArray.Length, 0, 0, 0);
                }
            }
            else
            {
                /// --- bind buffer                
                this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.vertexBuffer, LinesVertex.SizeInBytes, 0));

                /// --- render the geometry
                this.effectTechnique.GetPassByIndex(0).Apply(this.Device.ImmediateContext);
                this.Device.ImmediateContext.DrawIndexed(this.Geometry.Indices.Count, 0, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            this.Detach();
        }
    }
}
