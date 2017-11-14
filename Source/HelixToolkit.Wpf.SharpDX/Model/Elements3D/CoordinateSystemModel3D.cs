// <copyright file="CoordinateSystemModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Linq;
using System.Windows;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    public class CoordinateSystemModel3D : ScreenSpaceMeshGeometry3D
    {
        /// <summary>
        /// <see cref="AxisXColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisXColorProperty = DependencyProperty.Register("AxisXColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new AffectsRenderPropertyMetadata(Media.Colors.Red,
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisColor(0, ((Media.Color)e.NewValue).ToColor4());
                }));
        /// <summary>
        /// <see cref="AxisYColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisYColorProperty = DependencyProperty.Register("AxisYColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new AffectsRenderPropertyMetadata(Media.Colors.Green,
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisColor(1, ((Media.Color)e.NewValue).ToColor4());
                }));
        /// <summary>
        /// <see cref="AxisZColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisZColorProperty = DependencyProperty.Register("AxisZColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new AffectsRenderPropertyMetadata(Media.Colors.Blue,
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisColor(2, ((Media.Color)e.NewValue).ToColor4());
                }));

        /// <summary>
        /// Axis X Color
        /// </summary>
        public Media.Color AxisXColor
        {
            set
            {
                SetValue(AxisXColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(AxisXColorProperty);
            }
        }
        /// <summary>
        /// Axis Y Color
        /// </summary>
        public Media.Color AxisYColor
        {
            set
            {
                SetValue(AxisYColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(AxisYColorProperty);
            }
        }
        /// <summary>
        /// Axis Z Color
        /// </summary>
        public Media.Color AxisZColor
        {
            set
            {
                SetValue(AxisZColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(AxisZColorProperty);
            }
        }

        private readonly BillboardTextModel3D[] axisBillboards = new BillboardTextModel3D[3];

        public CoordinateSystemModel3D()
        {
            var builder = new MeshBuilder(true, false, false);
            builder.AddArrow(Vector3.Zero, new Vector3(10, 0, 0), 1, 2, 10);
            builder.AddArrow(Vector3.Zero, new Vector3(0, 10, 0), 1, 2, 10);
            builder.AddArrow(Vector3.Zero, new Vector3(0, 0, 10), 1, 2, 10);
            var mesh = builder.ToMesh();
            this.Material = PhongMaterials.White;
            axisBillboards[0] = new BillboardTextModel3D() { IsHitTestVisible = false };
            axisBillboards[1] = new BillboardTextModel3D() { IsHitTestVisible = false };
            axisBillboards[2] = new BillboardTextModel3D() { IsHitTestVisible = false };
            UpdateAxisColor(mesh, 0, AxisXColor.ToColor4());
            UpdateAxisColor(mesh, 1, AxisYColor.ToColor4());
            UpdateAxisColor(mesh, 2, AxisZColor.ToColor4());
            Geometry = mesh;
            CullMode = CullMode.Back;
        }

        private void UpdateAxisColor(int which, Color4 color)
        {
            UpdateAxisColor(geometryInternal, which, color);
        }

        private void UpdateAxisColor(Geometry3D mesh, int which, Color4 color)
        {
            switch (which)
            {
                case 0:
                    axisBillboards[which].Geometry = new BillboardSingleText3D()
                    { TextInfo = new TextInfo("X", new Vector3(14, 0, 0)), BackgroundColor = Color.Transparent, FontSize = 14, FontColor = color };
                    break;
                case 1:
                    axisBillboards[which].Geometry = new BillboardSingleText3D()
                    { TextInfo = new TextInfo("Y", new Vector3(0, 14, 0)), BackgroundColor = Color.Transparent, FontSize = 14, FontColor = color };
                    break;
                case 2:
                    axisBillboards[which].Geometry = new BillboardSingleText3D()
                    { TextInfo = new TextInfo("Z", new Vector3(0, 0, 14)), BackgroundColor = Color.Transparent, FontSize = 14, FontColor = color };
                    break;
            }
            int segment = mesh.Positions.Count / 3;
            var colors = new Core.Color4Collection(mesh.Colors == null ? Enumerable.Repeat<Color4>(Color.Black, mesh.Positions.Count) : mesh.Colors);
            for (int i = segment * which; i < segment * (which + 1); ++i)
            {
                colors[i] = color;
            }
            mesh.Colors = colors;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                foreach (var billboard in axisBillboards)
                {
                    billboard.Attach(host);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            foreach (var billboard in axisBillboards)
            {
                billboard.Detach();
            }
            base.OnDetach();
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Colors];
        }

        protected override void OnRender(RenderContext renderContext)
        {
            UpdateProjectionMatrix(renderContext.ActualWidth, renderContext.ActualHeight);
            // --- set constant paramerers             
            var worldMatrix = renderContext.worldMatrix;
            worldMatrix.Row4 = new Vector4(0, 0, 0, 1);
            this.EffectTransforms.World.SetMatrix(ref worldMatrix);
            this.viewMatrixVar.SetMatrix(CreateViewMatrix(renderContext));
            this.projectionMatrixVar.SetMatrix(projectionMatrix);
            this.effectMaterial.bHasShadowMapVariable.Set(false);

            // --- set material params      
            this.effectMaterial.AttachMaterial(geometryInternal as MeshGeometry3D);

            this.bHasInstances.Set(false);
            int depthStateRef;
            var depthStateBack = renderContext.DeviceContext.OutputMerger.GetDepthStencilState(out depthStateRef);
            renderContext.DeviceContext.ClearDepthStencilView(renderContext.Canvas.DepthStencilBufferView, DepthStencilClearFlags.Depth, 1f, 0);
            // --- set context
            renderContext.DeviceContext.InputAssembler.InputLayout = this.vertexLayout;
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderContext.DeviceContext.InputAssembler.SetIndexBuffer(this.IndexBuffer.Buffer, Format.R32_UInt, 0);

            // --- set rasterstate            
            renderContext.DeviceContext.Rasterizer.State = this.RasterState;

            // --- bind buffer                
            renderContext.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.VertexBuffer.Buffer, this.VertexBuffer.StructureSize, 0));

            var pass = this.effectTechnique.GetPassByIndex(0);
            pass.Apply(renderContext.DeviceContext);
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(depthStencil);
            // --- draw
            renderContext.DeviceContext.DrawIndexed(this.geometryInternal.Indices.Count, 0, 0);

            foreach (var billboard in axisBillboards)
            {
                billboard.Render(renderContext);
            }

            this.viewMatrixVar.SetMatrix(renderContext.ViewMatrix);
            this.projectionMatrixVar.SetMatrix(renderContext.ProjectionMatrix);
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(depthStateBack);
        }
    }
}
