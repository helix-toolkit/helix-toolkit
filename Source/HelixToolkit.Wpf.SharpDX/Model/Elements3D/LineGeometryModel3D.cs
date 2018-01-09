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
    using System.Linq;
    using System.Windows;
    using Core;
    using global::SharpDX;
    using global::SharpDX.Direct3D11;
    using Media = System.Windows.Media;

    public class LineGeometryModel3D : GeometryModel3D
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
                ((d as LineGeometryModel3D).RenderCore as LineRenderCore).Thickness = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty SmoothnessProperty =
            DependencyProperty.Register("Smoothness", typeof(double), typeof(LineGeometryModel3D), new AffectsRenderPropertyMetadata(0.0,
            (d, e) =>
            {
                ((d as LineGeometryModel3D).RenderCore as LineRenderCore).Smoothness = (float)(double)e.NewValue;
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
        private static LinesVertex[] vertexArrayBuffer = null;

        protected override IGeometryBufferModel OnCreateBufferModel()
        {
            var buffer = new LineGeometryBufferModel<LinesVertex>(LinesVertex.SizeInBytes);
            buffer.OnBuildVertexArray = CreateLinesVertexArray;
            return buffer;
        }

        protected override IRenderCore OnCreateRenderCore()
        {
            return new LineRenderCore();
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            var c = core as ILineRenderParams;
            c.LineColor = Color.ToColor4();
            c.Thickness = (float)Thickness;
            c.Smoothness = (float)Smoothness;
            base.AssignDefaultValuesToCore(core);
        }

        protected override RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
            {
                FillMode = FillMode,
                CullMode = CullMode.None,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = (float)SlopeScaledDepthBias,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = false,

                IsMultisampleEnabled = IsMultisampleEnabled,
                //IsAntialiasedLineEnabled = true, // Intel HD 3000 doesn't like this (#10051) and it's not needed
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled
            };
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.Lines];
        }

        protected override bool CanRender(IRenderContext context)
        {
            if (base.CanRender(context))
            {
                return !RenderHost.IsDeferredLighting;
            }
            else
            {
                return false;
            }
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return (Geometry as LineGeometry3D).HitTest(context, totalModelMatrix, ref ray, ref hits, this, (float)HitTestThickness);
        }

        /// <summary>
        /// Creates a <see cref="T:LinesVertex[]"/>.
        /// </summary>
        private LinesVertex[] CreateLinesVertexArray(LineGeometry3D geometry)
        {
            var positions = geometry.Positions;
            var vertexCount = geometry.Positions.Count;
            var array = ReuseVertexArrayBuffer && vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new LinesVertex[vertexCount];
            var colors = geometry.Colors != null ? geometry.Colors.GetEnumerator() : Enumerable.Repeat(Color4.White, vertexCount).GetEnumerator();
            if (ReuseVertexArrayBuffer)
            {
                vertexArrayBuffer = array;
            }

            for (var i = 0; i < vertexCount; i++)
            {
                colors.MoveNext();
                array[i].Position = new Vector4(positions[i], 1f);
                array[i].Color = colors.Current;
            }           
            return array;
        }
    }
}
