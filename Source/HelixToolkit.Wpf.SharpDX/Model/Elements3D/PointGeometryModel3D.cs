namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using global::SharpDX;
    using global::SharpDX.Direct3D11;
    using Utilities;
    using System;
    using Core;
    using Media = System.Windows.Media;

    public class PointGeometryModel3D : GeometryModel3D
    {
        #region Dependency Properties
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Media.Color), typeof(PointGeometryModel3D),
                new PropertyMetadata(Media.Colors.Black, (d, e) =>
                {
                    (d as PointGeometryModel3D).pointRenderCore.PointColor = ((Media.Color)e.NewValue).ToColor4();
                }));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(PointGeometryModel3D), new PropertyMetadata(new Size(1.0, 1.0),
                (d,e)=> 
                {
                    var size = (Size)e.NewValue;
                    (d as PointGeometryModel3D).pointRenderCore.Width = (float)size.Width;
                    (d as PointGeometryModel3D).pointRenderCore.Height = (float)size.Height;
                }));

        public static readonly DependencyProperty FigureProperty =
            DependencyProperty.Register("Figure", typeof(PointFigure), typeof(PointGeometryModel3D), new PropertyMetadata(PointFigure.Rect,
                (d, e)=> 
                {
                    (d as PointGeometryModel3D).pointRenderCore.Figure = (PointFigure)e.NewValue;
                }));

        public static readonly DependencyProperty FigureRatioProperty =
            DependencyProperty.Register("FigureRatio", typeof(double), typeof(PointGeometryModel3D), new PropertyMetadata(0.25,
                (d, e)=> 
                {
                    (d as PointGeometryModel3D).pointRenderCore.FigureRatio = (float)(double)e.NewValue;
                }));

        public static readonly DependencyProperty HitTestThicknessProperty =
            DependencyProperty.Register("HitTestThickness", typeof(double), typeof(PointGeometryModel3D), new UIPropertyMetadata(4.0));

        public Media.Color Color
        {
            get { return (Media.Color)this.GetValue(ColorProperty); }
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

        /// <summary>
        /// Used only for point/line hit test
        /// </summary>
        public double HitTestThickness
        {
            get { return (double)this.GetValue(HitTestThicknessProperty); }
            set { this.SetValue(HitTestThicknessProperty, value); }
        }
        #endregion
        [ThreadStatic]
        private static PointsVertex[] vertexArrayBuffer;

        private IPointRenderParams pointRenderCore
        {
            get
            {
                return (IPointRenderParams)RenderCore;
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
            var c = core as IPointRenderParams;
            c.Width = (float)Size.Width;
            c.Height = (float)Size.Height;
            c.Figure = Figure;
            c.FigureRatio = (float)FigureRatio;
            c.PointColor = Color.ToColor4();
        }

        protected override RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = (float)SlopeScaledDepthBias,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = false,
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled
            };
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.Points];
        }


        protected override bool CanRender(IRenderContext context)
        {
            if(base.CanRender(context))
            {
                return !RenderHost.IsDeferredLighting;
            }
            else
            {
                return false;
            }
        }

        protected override bool OnCheckGeometry(Geometry3D geometry)
        {
            return base.OnCheckGeometry(geometry) && geometry is PointGeometry3D;
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return (Geometry as PointGeometry3D).HitTest(context, totalModelMatrix, ref ray, ref hits, this, (float)HitTestThickness);
        }

        /// <summary>
        /// Creates a <see cref="T:PointsVertex[]"/>.
        /// </summary>
        private PointsVertex[] CreateVertexArray(PointGeometry3D geometry)
        {
            var positions = geometry.Positions;
            var vertexCount = geometry.Positions.Count;
            var array = ReuseVertexArrayBuffer && vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new PointsVertex[vertexCount];
            var colors = geometry.Colors != null ? geometry.Colors.GetEnumerator() : Enumerable.Repeat(Color4.White, vertexCount).GetEnumerator();
            if (ReuseVertexArrayBuffer)
            {
                vertexArrayBuffer = array;
            }
            if (geometry.Colors != null && geometry.Colors.Any())
            {
                for (var i = 0; i < vertexCount; i++)
                {
                    colors.MoveNext();
                    array[i].Position = new Vector4(positions[i], 1f);
                    array[i].Color = colors.Current;
                }
            }
            colors.Dispose();
            return array;
        }
    }

}
