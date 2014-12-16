using System.Linq;
using HelixToolkit.Wpf.SharpDX.Model.Geometry;

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Windows;
    using global::SharpDX;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using Color = global::SharpDX.Color;

    public class PointGeometryModel3D : GeometryModel3D
    {
        private InputLayout vertexLayout;
        private Buffer vertexBuffer;
        private EffectTechnique effectTechnique;
        private EffectTransformVariables effectTransforms;
        private EffectVectorVariable vViewport;

        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(PointGeometryModel3D),
                new UIPropertyMetadata(Color.Black, (o, e) => ((PointGeometryModel3D)o).OnColorChanged()));

        public double HitTestThickness
        {
            get { return (double)this.GetValue(HitTestThicknessProperty); }
            set { this.SetValue(HitTestThicknessProperty, value); }
        }

        public static readonly DependencyProperty HitTestThicknessProperty =
            DependencyProperty.Register("HitTestThickness", typeof(double), typeof(PointGeometryModel3D), new UIPropertyMetadata(1.0));

        public override bool HitTest(Ray rayWS, ref List<HitTestResult> hits)
        {
            return false; // No hit testing on point geometry -- yet.
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
                this.vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, Geometry3D.PointsVertex.SizeInBytes, this.CreatePointVertexArray());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        public override void Attach(IRenderHost host)
        {
            this.renderTechnique = Techniques.RenderPoints;
            base.Attach(host);

            if (this.Geometry == null)
                return;

#if DEFERRED
            if (renderHost.RenderTechnique == Techniques.RenderDeferred || renderHost.RenderTechnique == Techniques.RenderGBuffer)
                return;
#endif

            // --- get device
            this.vertexLayout = EffectsManager.Instance.GetLayout(this.renderTechnique);
            this.effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);

            this.effectTransforms = new EffectTransformVariables(this.effect);

            // --- get geometry
            var geometry = this.Geometry as PointGeometry3D;

            // -- set geometry if given
            if (geometry != null)
            {
                /// --- set up buffers            
                this.vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, Geometry3D.PointsVertex.SizeInBytes, this.CreatePointVertexArray());
            }

            /// --- set up const variables
            this.vViewport = effect.GetVariableByName("vViewport").AsVector();

            /// --- create raster state
            this.OnRasterStateChanged(this.DepthBias);

            /// --- flush
            this.Device.ImmediateContext.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Detach()
        {
            Disposer.RemoveAndDispose(ref this.vertexBuffer);
            Disposer.RemoveAndDispose(ref this.vViewport);
            Disposer.RemoveAndDispose(ref this.rasterState);

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

#if DEFERRED
            if (renderHost.RenderTechnique == Techniques.RenderDeferred || renderHost.RenderTechnique == Techniques.RenderGBuffer)
                return;
#endif

            if (renderContext.IsShadowPass)
                if (!this.IsThrowingShadow)
                    return;

            /// --- since these values are changed only per window resize, we set them only once here
            if (renderContext.Camera is ProjectionCamera)
            {
                var c = renderContext.Camera as ProjectionCamera;
                // viewport: W,H,0,0   
                var viewport = new Vector4((float)renderContext.Canvas.ActualWidth, (float)renderContext.Canvas.ActualHeight, 0, 0);
                var ar = viewport.X / viewport.Y;
                this.vViewport.Set(ref viewport);
            }
            
            /// --- set transform paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            /// --- set context
            this.Device.ImmediateContext.InputAssembler.InputLayout = this.vertexLayout;
            this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;

            /// --- set rasterstate            
            this.Device.ImmediateContext.Rasterizer.State = this.rasterState;

            /// --- bind buffer                
            this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0,
                new VertexBufferBinding(this.vertexBuffer, Geometry3D.PointsVertex.SizeInBytes, 0));

            /// --- render the geometry
            this.effectTechnique.GetPassByIndex(0).Apply(this.Device.ImmediateContext);

            this.Device.ImmediateContext.Draw(this.Geometry.Positions.Count, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            this.Detach();
        }

        /// <summary>
        /// Creates a <see cref="T:PointsVertex[]"/>.
        /// </summary>
        private Geometry3D.PointsVertex[] CreatePointVertexArray()
        {
            var positions = this.Geometry.Positions.Array;
            var vertexCount = this.Geometry.Positions.Count;
            var color = this.Color;
            var result = new Geometry3D.PointsVertex[vertexCount];

            if (this.Geometry.Colors != null && this.Geometry.Colors.Any())
            {
                var colors = this.Geometry.Colors;
                for (var i = 0; i < vertexCount; i++)
                {
                    result[i] = new Geometry3D.PointsVertex
                    {
                        Position = new Vector4(positions[i], 1f),
                        Color = color * colors[i],
                    };
                }
            }
            else
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    result[i] = new Geometry3D.PointsVertex
                    {
                        Position = new Vector4(positions[i], 1f),
                        Color = color,
                    };
                }
            }

            return result;
        }
    }

}
