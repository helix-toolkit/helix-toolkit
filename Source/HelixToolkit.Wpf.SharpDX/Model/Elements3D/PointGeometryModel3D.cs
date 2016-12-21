namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using global::SharpDX;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;

    using HelixToolkit.Wpf.SharpDX.Extensions;
    using HelixToolkit.Wpf.SharpDX.Utilities;

    using Color = global::SharpDX.Color;
    using System.Runtime.CompilerServices;

    public class PointGeometryModel3D : GeometryModel3D
    {
        private Geometry3D.PointsVertex[] vertexArrayBuffer;
        protected InputLayout vertexLayout;
        protected Buffer vertexBuffer;
        protected EffectTechnique effectTechnique;
        protected EffectTransformVariables effectTransforms;
        protected EffectVectorVariable vViewport;
        protected EffectVectorVariable vPointParams;

        public override int VertexSizeInBytes
        {
            get { return Geometry3D.PointsVertex.SizeInBytes; }
        }

        [TypeConverter(typeof(ColorConverter))]
        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(PointGeometryModel3D),
                new UIPropertyMetadata(Color.Black, (o, e) => ((PointGeometryModel3D)o).OnColorChanged()));

        public Size Size
        {
            get { return (Size)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(PointGeometryModel3D), new UIPropertyMetadata(new Size(1.0, 1.0)));

        public PointFigure Figure
        {
            get { return (PointFigure)this.GetValue(FigureProperty); }
            set { this.SetValue(FigureProperty, value); }
        }

        public static readonly DependencyProperty FigureProperty =
            DependencyProperty.Register("Figure", typeof(PointFigure), typeof(PointGeometryModel3D), new UIPropertyMetadata(PointFigure.Rect));

        public double FigureRatio
        {
            get { return (double)this.GetValue(FigureRatioProperty); }
            set { this.SetValue(FigureRatioProperty, value); }
        }

        public static readonly DependencyProperty FigureRatioProperty =
            DependencyProperty.Register("FigureRatio", typeof(double), typeof(PointGeometryModel3D), new UIPropertyMetadata(0.25));

        public double HitTestThickness
        {
            get { return (double)this.GetValue(HitTestThicknessProperty); }
            set { this.SetValue(HitTestThicknessProperty, value); }
        }

        public static readonly DependencyProperty HitTestThicknessProperty =
            DependencyProperty.Register("HitTestThickness", typeof(double), typeof(PointGeometryModel3D), new UIPropertyMetadata(4.0));

        public static double DistanceRayToPoint(Ray r, Vector3 p)
        {
            Vector3 v = r.Direction;
            Vector3 w = p - r.Position;

            float c1 = Vector3.Dot(w, v);
            float c2 = Vector3.Dot(v, v);
            float b = c1 / c2;

            Vector3 pb = r.Position + v * b;
            return (p - pb).Length();
        }

        /// <summary>
        /// Checks if the ray hits the geometry of the model.
        /// If there a more than one hit, result returns the hit which is nearest to the ray origin.
        /// </summary>
        /// <param name="rayWS">Hitring ray from the camera.</param>
        /// <param name="result">results of the hit.</param>
        /// <returns>True if the ray hits one or more times.</returns>
        public override bool HitTest(Ray rayWS, ref List<HitTestResult> hits)
        {
            PointGeometry3D pointGeometry3D;
            Viewport3DX viewport;

            if (this.Visibility == Visibility.Collapsed ||
                this.IsHitTestVisible == false ||
                (viewport = FindVisualAncestor<Viewport3DX>(this.renderHost as DependencyObject)) == null ||
                (pointGeometry3D = this.Geometry as PointGeometry3D) == null)
            {
                return false;
            }

            var svpm = viewport.GetScreenViewProjectionMatrix();
            var smvpm = this.modelMatrix * svpm;

            var clickPoint4 = new Vector4(rayWS.Position + rayWS.Direction, 1);
            var pos4 = new Vector4(rayWS.Position, 1);
            var dir3 = new Vector3();
            Vector4.Transform(ref clickPoint4, ref svpm, out clickPoint4);
            Vector4.Transform(ref pos4, ref svpm, out pos4);
            Vector3.TransformNormal(ref rayWS.Direction, ref svpm, out dir3);
            dir3.Normalize();

            var clickPoint = clickPoint4.ToVector3();

            var result = new HitTestResult { IsValid = false, Distance = double.MaxValue };
            var maxDist = this.HitTestThickness;
            var lastDist = double.MaxValue;
            var index = 0;

            foreach (var point in pointGeometry3D.Points)
            {
                var p0 = Vector3.TransformCoordinate(point.P0, smvpm);
                var pv = p0 - clickPoint;
                var dist = pv.Length();
                if (dist < lastDist && dist <= maxDist)
                {
                    lastDist = dist;
                    Vector4 res;
                    var lp0 = point.P0;
                    Vector3.Transform(ref lp0, ref this.modelMatrix, out res);
                    var pvv = res.ToVector3();
                    var dst = DistanceRayToPoint(rayWS, pvv);
                    result.Distance = dst;
                    result.PointHit = pvv.ToPoint3D();
                    result.ModelHit = this;
                    result.IsValid = true;
                    result.Tag = index;
                }

                index++;
            }

            if (result.IsValid)
            {
                hits.Add(result);
            }

            return result.IsValid;
        }

        protected override void OnRasterStateChanged()
        {
            Disposer.RemoveAndDispose(ref this.rasterState);
            /// --- set up rasterizer states
            var rasterStateDesc = new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                DepthBias = DepthBias,
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

        private void OnColorChanged()
        {
            CreateVertexBuffer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateVertexBuffer()
        {
            var geometry = Geometry as LineGeometry3D;
            if (geometry != null && geometry.Positions != null)
            {
                Disposer.RemoveAndDispose(ref vertexBuffer);
                /// --- set up buffers            
                var data = CreateVertexArray();
                if (data != null)
                {
                    this.vertexBuffer = Device.CreateBuffer(BindFlags.VertexBuffer, VertexSizeInBytes, data, geometry.Positions.Count);
                }
            }
        }

        protected override void OnGeometryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnGeometryPropertyChanged(sender, e);
            if (sender is PointGeometry3D)
            {
                if (e.PropertyName.Equals(nameof(PointGeometry3D.Positions)))
                {
                    OnUpdateVertexBuffer(CreateVertexArray);
                }
                else if (e.PropertyName.Equals(nameof(PointGeometry3D.Colors)))
                {
                    OnUpdateVertexBuffer(CreateVertexArray);
                }
                else if (e.PropertyName.Equals(Geometry3D.VertexBuffer))
                {
                    OnUpdateVertexBuffer(CreateVertexArray);
                }
            }
        }

        private void OnUpdateVertexBuffer(System.Func<Geometry3D.PointsVertex[]> updateFunction)
        {
            CreateVertexBuffer();
            this.InvalidateRender();
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Points];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        protected override bool OnAttach(IRenderHost host)
        {
            if (!base.OnAttach(host))
            {
                return false;
            }

            if (renderHost.RenderTechnique == renderHost.RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.Deferred) ||
                renderHost.RenderTechnique == renderHost.RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.GBuffer))
                return false;

            // --- get device
            vertexLayout = renderHost.EffectsManager.GetLayout(renderTechnique);
            effectTechnique = effect.GetTechniqueByName(renderTechnique.Name);

            effectTransforms = new EffectTransformVariables(effect);

            CreateVertexBuffer();

            /// --- set up const variables
            vViewport = effect.GetVariableByName("vViewport").AsVector();
            //this.vFrustum = effect.GetVariableByName("vFrustum").AsVector();
            vPointParams = effect.GetVariableByName("vPointParams").AsVector();

            /// --- set effect per object const vars
            var pointParams = new Vector4((float)Size.Width, (float)Size.Height, (float)Figure, (float)FigureRatio);
            vPointParams.Set(pointParams);

            /// --- create raster state
            OnRasterStateChanged();

            /// --- flush
            //Device.ImmediateContext.Flush();
            return true;
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

        protected override bool CanRender(RenderContext context)
        {
            if(base.CanRender(context))
            {
                if (renderHost.RenderTechnique == renderHost.RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.Deferred) ||
                    renderHost.RenderTechnique == renderHost.RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.GBuffer))
                    return false;
                else return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnRender(RenderContext renderContext)
        {       
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

            /// --- set effect per object const vars
            var pointParams = new Vector4((float)this.Size.Width, (float)this.Size.Height, (float)this.Figure, (float)this.FigureRatio);
            this.vPointParams.Set(pointParams);

            /// --- set context
            this.Device.ImmediateContext.InputAssembler.InputLayout = this.vertexLayout;
            this.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;

            /// --- set rasterstate            
            this.Device.ImmediateContext.Rasterizer.State = this.rasterState;

            /// --- bind buffer                
            this.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0,
                new VertexBufferBinding(this.vertexBuffer, VertexSizeInBytes, 0));

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
        private Geometry3D.PointsVertex[] CreateVertexArray()
        {
            var positions = this.Geometry.Positions.Array;
            var vertexCount = this.Geometry.Positions.Count;
            var color = this.Color;
            if (!ReuseVertexArrayBuffer || vertexArrayBuffer == null || vertexArrayBuffer.Length < vertexCount)
                vertexArrayBuffer = new Geometry3D.PointsVertex[vertexCount];

            if (this.Geometry.Colors != null && this.Geometry.Colors.Any())
            {
                var colors = this.Geometry.Colors;
                for (var i = 0; i < vertexCount; i++)
                {
                    vertexArrayBuffer[i].Position = new Vector4(positions[i], 1f);
                    vertexArrayBuffer[i].Color = color * colors[i];
                }
            }
            else
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    vertexArrayBuffer[i].Position = new Vector4(positions[i], 1f);
                    vertexArrayBuffer[i].Color = color;
                }
            }

            return vertexArrayBuffer;
        }

        public enum PointFigure
        {
            Rect,
            Ellipse,
            Cross,
        }
    }

}
