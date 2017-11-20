// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineGeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using Core;
    using Media = System.Windows.Media;
    using global::SharpDX;
    using global::SharpDX.Direct3D11;

    public class LineGeometryModel3D : InstanceGeometryModel3D
    {
        #region Dependency Properties
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Media.Color), typeof(LineGeometryModel3D), new AffectsRenderPropertyMetadata(Media.Colors.Black, (d, e) =>
            {
                ((d as LineGeometryModel3D).RenderCore as LineRenderCore).LineColor = ((Media.Color)e.NewValue).ToColor4();
            }));

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LineGeometryModel3D), new AffectsRenderPropertyMetadata(1.0, (d, e) =>
            {
                ((d as LineGeometryModel3D).RenderCore as LineRenderCore).LineParams.X = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty SmoothnessProperty =
            DependencyProperty.Register("Smoothness", typeof(double), typeof(LineGeometryModel3D), new AffectsRenderPropertyMetadata(0.0,
            (d, e) =>
            {
                ((d as LineGeometryModel3D).RenderCore as LineRenderCore).LineParams.Y = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty HitTestThicknessProperty =
            DependencyProperty.Register("HitTestThickness", typeof(double), typeof(LineGeometryModel3D), new UIPropertyMetadata(1.0));

        public Media.Color Color
        {
            get { return (Media.Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public double Thickness
        {
            get { return (double)this.GetValue(ThicknessProperty); }
            set { this.SetValue(ThicknessProperty, value); }
        }


        public double Smoothness
        {
            get { return (double)this.GetValue(SmoothnessProperty); }
            set { this.SetValue(SmoothnessProperty, value); }
        }

        public double HitTestThickness
        {
            get { return (double)this.GetValue(HitTestThicknessProperty); }
            set { this.SetValue(HitTestThicknessProperty, value); }
        }
        #endregion
        [ThreadStatic]
        private static LinesVertex[] vertexArrayBuffer = null;

        protected override GeometryBufferModel OnCreateBufferModel()
        {
            var buffer = new LineGeometryBufferModel<LinesVertex>(LinesVertex.SizeInBytes);
            buffer.OnBuildVertexArray = CreateLinesVertexArray;
            return buffer;
        }

        protected override IRenderCore OnCreateRenderCore()
        {
            var core = new LineRenderCore();
            core.LineColor = Color.ToColor4();
            core.LineParams.X = (float)Thickness;
            core.LineParams.Y = (float)Smoothness;
            return core;
        }

        protected override bool CheckGeometry()
        {
            return base.CheckGeometry() && geometryInternal is LineGeometry3D;
        }

        protected override bool CanHitTest(IRenderMatrices context)
        {
            return base.CanHitTest(context) && context != null;
        }

        protected override bool OnHitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            var lineGeometry3D = this.geometryInternal as LineGeometry3D;
            var result = new LineHitTestResult { IsValid = false, Distance = double.MaxValue };
            var lastDist = double.MaxValue;
            var lineIndex = 0;
            foreach (var line in lineGeometry3D.Lines)
            {
                var t0 = Vector3.TransformCoordinate(line.P0, this.ModelMatrix);
                var t1 = Vector3.TransformCoordinate(line.P1, this.ModelMatrix);
                Vector3 sp, tp;
                float sc, tc;
                var rayToLineDistance = LineBuilder.GetRayToLineDistance(rayWS, t0, t1, out sp, out tp, out sc, out tc);
                var svpm = context.ScreenViewProjectionMatrix;
                Vector4 sp4;
                Vector4 tp4;
                Vector3.Transform(ref sp, ref svpm, out sp4);
                Vector3.Transform(ref tp, ref svpm, out tp4);
                var sp3 = sp4.ToVector3();
                var tp3 = tp4.ToVector3();
                var tv2 = new Vector2(tp3.X - sp3.X, tp3.Y - sp3.Y);
                var dist = tv2.Length();
                if (dist < lastDist && dist <= this.HitTestThickness)
                {
                    lastDist = dist;
                    result.PointHit = sp.ToPoint3D();
                    result.NormalAtHit = (sp - tp).ToVector3D(); // not normalized to get length
                    result.Distance = (rayWS.Position - sp).Length();
                    result.RayToLineDistance = rayToLineDistance;
                    result.ModelHit = this;
                    result.IsValid = true;
                    result.Tag = lineIndex; // For compatibility
                    result.LineIndex = lineIndex;
                    result.TriangleIndices = null; // Since triangles are shader-generated
                    result.RayHitPointScalar = sc;
                    result.LineHitPointScalar = tc;
                }

                lineIndex++;
            }

            if (result.IsValid)
            {
                hits.Add(result);
            }

            return result.IsValid;
        }

        protected override RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
            {
                FillMode = FillMode,
                CullMode = CullMode.None,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = -2,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,

                IsMultisampleEnabled = IsMultisampleEnabled,
                //IsAntialiasedLineEnabled = true, // Intel HD 3000 doesn't like this (#10051) and it's not needed
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled
            };
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Lines];
        }

        protected override bool OnAttach(IRenderHost host)
        {
            // --- attach                        
            if (!base.OnAttach(host))
            {
                return false;
            }

            if (renderHost.IsDeferredLighting)
                return false;
            return true;
        }

        protected override bool CanRender(RenderContext context)
        {
            if (base.CanRender(context))
            {
                return !renderHost.IsDeferredLighting;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a <see cref="T:LinesVertex[]"/>.
        /// </summary>
        private LinesVertex[] CreateLinesVertexArray(LineGeometry3D geometry)
        {
            var positions = geometry.Positions;
            var vertexCount = geometry.Positions.Count;
            var array = ReuseVertexArrayBuffer && vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new LinesVertex[vertexCount];
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
                    array[i].Color = global::SharpDX.Color.White;
                }
            }

            return array;
        }
    }
}
