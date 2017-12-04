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
    using System;
    using Core;

    public class PointGeometryModel3D : InstanceGeometryModel3D
    {
        #region Dependency Properties
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(PointGeometryModel3D),
                new AffectsRenderPropertyMetadata(Color.Black, (d, e) =>
                {
                    (d as PointGeometryModel3D).pointRenderCore.PointColor = (Color)e.NewValue;
                }));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(PointGeometryModel3D), new AffectsRenderPropertyMetadata(new Size(1.0, 1.0),
                (d,e)=> 
                {
                    var size = (Size)e.NewValue;
                    (d as PointGeometryModel3D).pointRenderCore.PointParams.X = (float)size.Width;
                    (d as PointGeometryModel3D).pointRenderCore.PointParams.Y = (float)size.Height;
                }));

        public static readonly DependencyProperty FigureProperty =
            DependencyProperty.Register("Figure", typeof(PointFigure), typeof(PointGeometryModel3D), new AffectsRenderPropertyMetadata(PointFigure.Rect,
                (d, e)=> 
                {
                    var figure = (PointFigure)e.NewValue;
                    (d as PointGeometryModel3D).pointRenderCore.PointParams.Z = (float)figure;
                }));

        public static readonly DependencyProperty FigureRatioProperty =
            DependencyProperty.Register("FigureRatio", typeof(double), typeof(PointGeometryModel3D), new AffectsRenderPropertyMetadata(0.25,
                (d, e)=> 
                {
                    var ratio = (double)e.NewValue;
                    (d as PointGeometryModel3D).pointRenderCore.PointParams.W = (float)ratio;
                }));

        public static readonly DependencyProperty HitTestThicknessProperty =
            DependencyProperty.Register("HitTestThickness", typeof(double), typeof(PointGeometryModel3D), new UIPropertyMetadata(4.0));

        [TypeConverter(typeof(ColorConverter))]
        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public Size Size
        {
            get { return (Size)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        public PointFigure Figure
        {
            get { return (PointFigure)this.GetValue(FigureProperty); }
            set { this.SetValue(FigureProperty, value); }
        }

        public double FigureRatio
        {
            get { return (double)this.GetValue(FigureRatioProperty); }
            set { this.SetValue(FigureRatioProperty, value); }
        }

        public double HitTestThickness
        {
            get { return (double)this.GetValue(HitTestThicknessProperty); }
            set { this.SetValue(HitTestThicknessProperty, value); }
        }
        #endregion
        [ThreadStatic]
        private static PointsVertex[] vertexArrayBuffer;

        private PointRenderCore pointRenderCore
        {
            get
            {
                return (PointRenderCore)RenderCore;
            }
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

        protected override IGeometryBufferModel OnCreateBufferModel()
        {
            var buffer = new PointGeometryBufferModel<PointsVertex>(PointsVertex.SizeInBytes);
            buffer.OnBuildVertexArray = CreateVertexArray;
            return buffer;
        }

        protected override IRenderCore OnCreateRenderCore()
        {
            return new PointRenderCore();
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            pointRenderCore.PointParams.X = (float)Size.Width;
            pointRenderCore.PointParams.Y = (float)Size.Height;
            pointRenderCore.PointParams.Z = (float)Figure;
            pointRenderCore.PointParams.W = (float)FigureRatio;
            pointRenderCore.PointColor = Color;
        }

        protected override bool CanHitTest(IRenderMatrices context)
        {
            return base.CanHitTest(context) && context != null;
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

        protected override RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
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
        }

        protected override IRenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Points];
        }

        protected override bool CheckGeometry()
        {
            return geometryInternal is PointGeometry3D && this.geometryInternal != null && this.geometryInternal.Positions != null && this.geometryInternal.Positions.Count > 0;
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
        /// Creates a <see cref="T:PointsVertex[]"/>.
        /// </summary>
        private PointsVertex[] CreateVertexArray(PointGeometry3D geometry)
        {
            var positions = geometry.Positions;
            var vertexCount = geometry.Positions.Count;
            var array = ReuseVertexArrayBuffer && vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new PointsVertex[vertexCount];
            if (ReuseVertexArrayBuffer)
            {
                vertexArrayBuffer = array;
            }
            if (geometry.Colors != null && geometry.Colors.Any())
            {
                var colors = geometry.Colors;
                for (var i = 0; i < vertexCount; i++)
                {
                    array[i].Position = new Vector4(positions[i], 1f);
                    array[i].Color = colors[i];
                }
            }
            else
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    array[i].Position = new Vector4(positions[i], 1f);
                    array[i].Color = Color.White;
                }
            }

            return array;
        }

        public enum PointFigure
        {
            Rect,
            Ellipse,
            Cross,
        }
    }

}
