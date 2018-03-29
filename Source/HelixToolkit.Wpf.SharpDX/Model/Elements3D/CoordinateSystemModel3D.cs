// <copyright file="CoordinateSystemModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>

using SharpDX;
using SharpDX.Direct3D11;
using System.Linq;
using System.Windows;
using Media = System.Windows.Media;
using System;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// 
    /// </summary>
    public class CoordinateSystemModel3D : ScreenSpacedElement3D
    {
        /// <summary>
        /// <see cref="AxisXColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisXColorProperty = DependencyProperty.Register("AxisXColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Red,
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisColor(0, ((Media.Color)e.NewValue).ToColor4());
                }));
        /// <summary>
        /// <see cref="AxisYColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisYColorProperty = DependencyProperty.Register("AxisYColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Green,
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisColor(1, ((Media.Color)e.NewValue).ToColor4());
                }));
        /// <summary>
        /// <see cref="AxisZColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisZColorProperty = DependencyProperty.Register("AxisZColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Blue,
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisColor(2, ((Media.Color)e.NewValue).ToColor4());
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelColorProperty = DependencyProperty.Register("LabelColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Gray,
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateLabelColor(((Media.Color)e.NewValue).ToColor4());
                }));

        /// <summary>
        /// The coordinate system label x property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelXProperty = DependencyProperty.Register(
                "CoordinateSystemLabelX", typeof(string), typeof(CoordinateSystemModel3D), new PropertyMetadata("X",
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisLabel(0, (string)e.NewValue);
                }));

        /// <summary>
        /// The coordinate system label Y property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelYProperty = DependencyProperty.Register(
                "CoordinateSystemLabelY", typeof(string), typeof(CoordinateSystemModel3D), new PropertyMetadata("Y",
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisLabel(1, (string)e.NewValue);
                }));

        /// <summary>
        /// The coordinate system label Z property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelZProperty = DependencyProperty.Register(
                "CoordinateSystemLabelZ", typeof(string), typeof(CoordinateSystemModel3D), new PropertyMetadata("Z",
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisLabel(2, (string)e.NewValue);
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
        /// <summary>
        /// Label Color
        /// </summary>
        public Media.Color LabelColor
        {
            set
            {
                SetValue(LabelColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(LabelColorProperty);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CoordinateSystemLabelX
        {
            set
            {
                SetValue(CoordinateSystemLabelXProperty, value);
            }
            get
            {
                return (string)GetValue(CoordinateSystemLabelXProperty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CoordinateSystemLabelY
        {
            set
            {
                SetValue(CoordinateSystemLabelYProperty, value);
            }
            get
            {
                return (string)GetValue(CoordinateSystemLabelYProperty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CoordinateSystemLabelZ
        {
            set
            {
                SetValue(CoordinateSystemLabelZProperty, value);
            }
            get
            {
                return (string)GetValue(CoordinateSystemLabelZProperty);
            }
        }

        private readonly BillboardTextModel3D axisBillboard = new BillboardTextModel3D();
        private readonly MeshGeometryModel3D arrowMeshModel = new MeshGeometryModel3D(){ EnableViewFrustumCheck = false };
        private static readonly float arrowSize = 5.5f;
        private static readonly float arrowWidth = 0.6f;
        private static readonly float arrowHead = 1.7f;
        /// <summary>
        /// 
        /// </summary>
        public CoordinateSystemModel3D()
        {
            var builder = new MeshBuilder(true, false, false);
            builder.AddArrow(Vector3.Zero, new Vector3(arrowSize, 0, 0), arrowWidth, arrowHead, 8);
            builder.AddArrow(Vector3.Zero, new Vector3(0, arrowSize, 0), arrowWidth, arrowHead, 8);
            builder.AddArrow(Vector3.Zero, new Vector3(0, 0, arrowSize), arrowWidth, arrowHead, 8);
            var mesh = builder.ToMesh();
          
            arrowMeshModel.Material = PhongMaterials.White;
            arrowMeshModel.Geometry = mesh;
            arrowMeshModel.CullMode = CullMode.Back;
            arrowMeshModel.SceneNode.OnSetRenderTechnique += (host) => { return host.EffectsManager[DefaultRenderTechniqueNames.Colors]; };
            arrowMeshModel.IsHitTestVisible = false;
            arrowMeshModel.SceneNode.RenderCore.RenderType = RenderType.ScreenSpaced;

            axisBillboard.IsHitTestVisible = false;
            axisBillboard.SceneNode.RenderCore.RenderType = RenderType.ScreenSpaced;
            axisBillboard.EnableViewFrustumCheck = false;
            var axisLabel = new BillboardText3D();
            axisLabel.TextInfo.Add(new TextInfo());
            axisLabel.TextInfo.Add(new TextInfo());
            axisLabel.TextInfo.Add(new TextInfo());
            axisBillboard.Geometry = axisLabel;
            UpdateAxisColor(mesh, 0, AxisXColor.ToColor4(), CoordinateSystemLabelX, LabelColor.ToColor4());
            UpdateAxisColor(mesh, 1, AxisYColor.ToColor4(), CoordinateSystemLabelY, LabelColor.ToColor4());
            UpdateAxisColor(mesh, 2, AxisZColor.ToColor4(), CoordinateSystemLabelZ, LabelColor.ToColor4());

            Children.Add(arrowMeshModel);
            Children.Add(axisBillboard);
        }

        protected override void UpdateModel(Vector3 upDirection)
        {
            
        }

        private void UpdateAxisColor(int which, Color4 color)
        {
            string label = "";
            switch (which)
            {
                case 0:
                    label = CoordinateSystemLabelX;
                    break;
                case 1:
                    label = CoordinateSystemLabelY;
                    break;
                case 2:
                    label = CoordinateSystemLabelZ;
                    break;
            }
            UpdateAxisColor(arrowMeshModel.Geometry, which, color, label, LabelColor.ToColor4());
        }

        private void UpdateAxisLabel(int which, string label)
        {
            Color4 color = Color.Red;
            switch (which)
            {
                case 0:
                    color = AxisXColor.ToColor4();
                    break;
                case 1:
                    color = AxisYColor.ToColor4();
                    break;
                case 2:
                    color = AxisZColor.ToColor4();
                    break;
            }
            UpdateAxisColor(arrowMeshModel.Geometry, which, color, label, LabelColor.ToColor4());
        }

        private void UpdateLabelColor(Color4 color)
        {
            UpdateAxisColor(arrowMeshModel.Geometry, 0, AxisXColor.ToColor4(), CoordinateSystemLabelX, LabelColor.ToColor4());
            UpdateAxisColor(arrowMeshModel.Geometry, 1, AxisYColor.ToColor4(), CoordinateSystemLabelY, LabelColor.ToColor4());
            UpdateAxisColor(arrowMeshModel.Geometry, 2, AxisZColor.ToColor4(), CoordinateSystemLabelZ, LabelColor.ToColor4());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="which"></param>
        /// <param name="color"></param>
        /// <param name="label"></param>
        /// <param name="labelColor"></param>
        protected void UpdateAxisColor(Geometry3D mesh, int which, Color4 color, string label, Color4 labelColor)
        {
            var labelText = axisBillboard.Geometry as BillboardText3D;
            switch (which)
            {
                case 0:
                    labelText.TextInfo[which] = new TextInfo(label, new Vector3(arrowSize + 1.5f, 0, 0)) { Foreground = labelColor, Scale = 0.5f };
                    break;
                case 1:
                    labelText.TextInfo[which] = new TextInfo(label, new Vector3(0, arrowSize + 1.5f, 0)) { Foreground = labelColor, Scale = 0.5f };
                    break;
                case 2:
                    labelText.TextInfo[which] = new TextInfo(label, new Vector3(0, 0, arrowSize + 1.5f)) { Foreground = labelColor, Scale = 0.5f };
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
    }
}
