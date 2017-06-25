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
    using System;

    public class PointGeometryModel3D : GeometryModel3D
    {
        private PointsVertex[] vertexArrayBuffer;
        protected InputLayout vertexLayout;
        protected EffectTechnique effectTechnique;
        protected EffectTransformVariables effectTransforms;
        protected EffectVectorVariable vPointParams;
        private readonly ImmutableBufferProxy<PointsVertex> vertexBuffer = new ImmutableBufferProxy<PointsVertex>(PointsVertex.SizeInBytes, BindFlags.VertexBuffer);
        protected Vector4 pointParams = new Vector4();
        /// <summary>
        /// For subclass override
        /// </summary>
        public virtual IBufferProxy VertexBuffer
        {
            get
            {
                return vertexBuffer;
            }
        }

        [TypeConverter(typeof(ColorConverter))]
        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(PointGeometryModel3D),
                new AffectsRenderPropertyMetadata(Color.Black, (o, e) => ((PointGeometryModel3D)o).OnColorChanged()));

        public Size Size
        {
            get { return (Size)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(PointGeometryModel3D), new AffectsRenderPropertyMetadata(new Size(1.0, 1.0),
                (d,e)=> 
                {
                    var size = (Size)e.NewValue;
                    var model = (d as PointGeometryModel3D);
                    model.pointParams.X = (float)size.Width;
                    model.pointParams.Y = (float)size.Height;
                }));

        public PointFigure Figure
        {
            get { return (PointFigure)this.GetValue(FigureProperty); }
            set { this.SetValue(FigureProperty, value); }
        }

        public static readonly DependencyProperty FigureProperty =
            DependencyProperty.Register("Figure", typeof(PointFigure), typeof(PointGeometryModel3D), new AffectsRenderPropertyMetadata(PointFigure.Rect,
                (d, e)=> 
                {
                    var figure = (PointFigure)e.NewValue;
                    var model = (d as PointGeometryModel3D);
                    model.pointParams.Z = (float)figure;
                }));

        public double FigureRatio
        {
            get { return (double)this.GetValue(FigureRatioProperty); }
            set { this.SetValue(FigureRatioProperty, value); }
        }

        public static readonly DependencyProperty FigureRatioProperty =
            DependencyProperty.Register("FigureRatio", typeof(double), typeof(PointGeometryModel3D), new AffectsRenderPropertyMetadata(0.25,
                (d, e)=> 
                {
                    var ratio = (double)e.NewValue;
                    var model = (d as PointGeometryModel3D);
                    model.pointParams.W = (float)ratio;
                }));

        public double HitTestThickness
        {
            get { return (double)this.GetValue(HitTestThicknessProperty); }
            set { this.SetValue(HitTestThicknessProperty, value); }
        }

        public static readonly DependencyProperty HitTestThicknessProperty =
            DependencyProperty.Register("HitTestThickness", typeof(double), typeof(PointGeometryModel3D), new UIPropertyMetadata(4.0));

        public PointGeometryModel3D() : base()
        {
            pointParams.X = (float)Size.Width;
            pointParams.Y = (float)Size.Height;
            pointParams.Z = (float)Figure;
            pointParams.W = (float)FigureRatio;
        }

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

        protected override bool CanHitTest(IRenderMatrices context)
        {
            return base.CanHitTest(context) && geometryInternal != null && geometryInternal.Positions != null && geometryInternal.Positions.Count > 0 && geometryInternal is PointGeometry3D && context != null;
        }

        /// <summary>
        /// Checks if the ray hits the geometry of the model.
        /// If there a more than one hit, result returns the hit which is nearest to the ray origin.
        /// </summary>
        /// <param name="rayWS">Hitring ray from the camera.</param>
        /// <param name="hits">results of the hit.</param>
        /// <returns>True if the ray hits one or more times.</returns>
        protected override bool OnHitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            if (geometryInternal.Octree != null)
            {
                return geometryInternal.Octree.HitTest(context, this, ModelMatrix, rayWS, ref hits);
            }
            else
            {
                PointGeometry3D pointGeometry3D = this.geometryInternal as PointGeometry3D;
                var svpm =  context.ScreenViewProjectionMatrix;
                var smvpm = this.modelMatrix * svpm;

                var clickPoint4 = new Vector4(rayWS.Position + rayWS.Direction, 1);
                var pos4 = new Vector4(rayWS.Position, 1);
               // var dir3 = new Vector3();
                Vector4.Transform(ref clickPoint4, ref svpm, out clickPoint4);
                Vector4.Transform(ref pos4, ref svpm, out pos4);
                //Vector3.TransformNormal(ref rayWS.Direction, ref svpm, out dir3);
                //dir3.Normalize();

                var clickPoint = clickPoint4.ToVector3();

                var result = new HitTestResult { IsValid = false, Distance = double.MaxValue };
                var maxDist = this.HitTestThickness;
                var lastDist = double.MaxValue;
                var index = 0;

                foreach (var point in pointGeometry3D.Positions)
                {
                    var p0 = Vector3.TransformCoordinate(point, smvpm);
                    var pv = p0 - clickPoint;
                    var dist = pv.Length();
                    if (dist < lastDist && dist <= maxDist)
                    {
                        lastDist = dist;
                        Vector4 res;
                        var lp0 = point;
                        Vector3.Transform(ref lp0, ref this.modelMatrix, out res);
                        var pvv = res.ToVector3();
                        result.Distance = (rayWS.Position - res.ToVector3()).Length();
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
        }

        protected override void OnRasterStateChanged()
        {
            Disposer.RemoveAndDispose(ref this.rasterState);
            if (!IsAttached) { return; }
            // --- set up rasterizer states
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
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled
            };

            try { this.rasterState = new RasterizerState(this.Device, rasterStateDesc); }
            catch (System.Exception)
            {
            }
        }

        private void OnColorChanged()
        {
            if(IsAttached)
                CreateVertexBuffer();
        }

        protected override void OnCreateGeometryBuffers()
        {
            CreateVertexBuffer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateVertexBuffer()
        {
            var geometry = geometryInternal as PointGeometry3D;
            if (geometry != null && geometry.Positions != null)
            {
                // --- set up buffers            
                var data = CreateVertexArray();
                vertexBuffer.CreateBufferFromDataArray(this.Device, data);
            }
            InvalidateRender();
        }

        protected override void OnGeometryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnGeometryPropertyChanged(sender, e);
            if (sender is PointGeometry3D)
            {
                if (e.PropertyName.Equals(nameof(PointGeometry3D.Positions)) || e.PropertyName.Equals(nameof(PointGeometry3D.Colors))
                    || e.PropertyName.Equals(Geometry3D.VertexBuffer))
                {
                    CreateVertexBuffer();
                }
            }
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Points];
        }

        protected override bool CheckGeometry()
        {
            if (this.geometryInternal == null || this.geometryInternal.Positions == null || this.geometryInternal.Positions.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
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

            if (renderHost.IsDeferredLighting)
                return false;

            // --- get device
            vertexLayout = renderHost.EffectsManager.GetLayout(renderTechnique);
            effectTechnique = effect.GetTechniqueByName(renderTechnique.Name);

            effectTransforms = new EffectTransformVariables(effect);

            OnCreateGeometryBuffers();

            // --- set up const variables
            vPointParams = effect.GetVariableByName("vPointParams").AsVector();

            // --- set effect per object const vars
            var pointParams = new Vector4((float)Size.Width, (float)Size.Height, (float)Figure, (float)FigureRatio);
            vPointParams.Set(pointParams);

            // --- flush
            //Device.ImmediateContext.Flush();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDetach()
        {
            vertexBuffer.Dispose();
            Disposer.RemoveAndDispose(ref this.rasterState);
            Disposer.RemoveAndDispose(ref this.vPointParams);
            this.renderTechnique = null;
            this.effectTechnique = null;
            this.vertexLayout = null;

            base.OnDetach();
        }

        protected override bool CanRender(RenderContext context)
        {
            if(base.CanRender(context))
            {
                return !renderHost.IsDeferredLighting;
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
            // --- set transform paramerers             
            var worldMatrix = this.modelMatrix * renderContext.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            // --- set effect per object const vars
            this.vPointParams.Set(pointParams);

            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = this.vertexLayout;
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;

            // --- set rasterstate            
            renderContext.DeviceContext.Rasterizer.State = this.rasterState;

            // --- bind buffer                
            renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0,
                new VertexBufferBinding(this.VertexBuffer.Buffer, this.VertexBuffer.StructureSize, 0));

            // --- render the geometry
            this.effectTechnique.GetPassByIndex(0).Apply(renderContext.DeviceContext);

            renderContext.DeviceContext.Draw(this.geometryInternal.Positions.Count, 0);
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
        private PointsVertex[] CreateVertexArray()
        {
            var positions = this.geometryInternal.Positions;
            var vertexCount = this.geometryInternal.Positions.Count;
            var color = this.Color;
            if (!ReuseVertexArrayBuffer || vertexArrayBuffer == null || vertexArrayBuffer.Length < vertexCount)
                vertexArrayBuffer = new PointsVertex[vertexCount];

            if (this.geometryInternal.Colors != null && this.geometryInternal.Colors.Any())
            {
                var colors = this.geometryInternal.Colors;
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
