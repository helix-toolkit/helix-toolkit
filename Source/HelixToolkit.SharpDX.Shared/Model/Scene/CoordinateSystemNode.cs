/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using global::SharpDX.Direct3D11;
using SharpDX;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene
    {
        /// <summary>
        ///
        /// </summary>
        public class CoordinateSystemNode : ScreenSpacedNode
        {
            private Color4 axisXColor = Color.Red;

            /// <summary>
            /// Gets or sets the color of the axis x.
            /// </summary>
            /// <value>
            /// The color of the axis x.
            /// </value>
            public Color4 AxisXColor
            {
                set
                {
                    if (Set(ref axisXColor, value))
                    {
                        UpdateAxisColor(0, value);
                    }
                }
                get { return axisXColor; }
            }

            private Color4 axisYColor = Color.Green;
            /// <summary>
            /// Gets or sets the color of the axis y.
            /// </summary>
            /// <value>
            /// The color of the axis y.
            /// </value>
            public Color4 AxisYColor
            {
                set
                {
                    if (Set(ref axisYColor, value))
                    {
                        UpdateAxisColor(1, value);
                    }
                }
                get { return axisYColor; }
            }

            private Color4 axisZColor = Color.Blue;
            /// <summary>
            /// Gets or sets the color of the axis z.
            /// </summary>
            /// <value>
            /// The color of the axis z.
            /// </value>
            public Color4 AxisZColor
            {
                set
                {
                    if (Set(ref axisZColor, value))
                    {
                        UpdateAxisColor(2, value);
                    }
                }
                get { return axisZColor; }
            }

            private Color4 labelColor = Color.Gray;
            /// <summary>
            /// Gets or sets the color of the label.
            /// </summary>
            /// <value>
            /// The color of the label.
            /// </value>
            public Color4 LabelColor
            {
                set
                {
                    if (Set(ref labelColor, value))
                    {
                        UpdateLabelColor(value);
                    }
                }
                get { return labelColor; }
            }

            private string labelX = "X";
            /// <summary>
            /// Gets or sets the label x.
            /// </summary>
            /// <value>
            /// The label x.
            /// </value>
            public string LabelX
            {
                set
                {
                    if (Set(ref labelX, value))
                    {
                        UpdateAxisLabel(0, value);
                    }
                }
                get { return labelX; }
            }

            private string labelY = "Y";
            /// <summary>
            /// Gets or sets the label y.
            /// </summary>
            /// <value>
            /// The label y.
            /// </value>
            public string LabelY
            {
                set
                {
                    if (Set(ref labelY, value))
                    {
                        UpdateAxisLabel(1, value);
                    }
                }
                get { return labelY; }
            }

            private string labelZ = "Z";
            /// <summary>
            /// Gets or sets the label z.
            /// </summary>
            /// <value>
            /// The label z.
            /// </value>
            public string LabelZ
            {
                set
                {
                    if (Set(ref labelZ, value))
                    {
                        UpdateAxisLabel(2, value);
                    }
                }
                get { return labelZ; }
            }

            private readonly BillboardNode axisBillboard = new BillboardNode() { Material = new BillboardMaterialCore() };
            private readonly MeshNode arrowMeshModel = new MeshNode() { EnableViewFrustumCheck = false };
            private static readonly float arrowSize = 5.5f;
            private static readonly float arrowWidth = 0.6f;
            private static readonly float arrowHead = 1.7f;

            /// <summary>
            ///
            /// </summary>
            public CoordinateSystemNode()
            {          
                arrowMeshModel.Material = new ColorMaterialCore();           
                arrowMeshModel.CullMode = CullMode.Back;
                arrowMeshModel.IsHitTestVisible = false;
                arrowMeshModel.RenderType = RenderType.ScreenSpaced;

                axisBillboard.IsHitTestVisible = false;
                axisBillboard.RenderType = RenderType.ScreenSpaced;
                axisBillboard.EnableViewFrustumCheck = false;
                var axisLabel = new BillboardText3D();
                axisLabel.TextInfo.Add(new TextInfo());
                axisLabel.TextInfo.Add(new TextInfo());
                axisLabel.TextInfo.Add(new TextInfo());
                axisBillboard.Geometry = axisLabel;
                this.AddChildNode(arrowMeshModel);
                this.AddChildNode(axisBillboard);
                UpdateModel();
            }

            private void UpdateModel()
            {
                var builder = new MeshBuilder(true, false, false);

                builder.AddArrow(Vector3.Zero, new Vector3(arrowSize, 0, 0), arrowWidth, arrowHead, 8);
                builder.AddArrow(Vector3.Zero, new Vector3(0, arrowSize, 0), arrowWidth, arrowHead, 8);
                builder.AddArrow(Vector3.Zero, new Vector3(0, 0, arrowSize), arrowWidth, arrowHead, 8);

                var mesh = builder.ToMesh();
                arrowMeshModel.Geometry = mesh;
                UpdateAxisColor(arrowMeshModel.Geometry, 0, AxisXColor, LabelX, LabelColor);
                UpdateAxisColor(arrowMeshModel.Geometry, 1, AxisYColor, LabelY, LabelColor);
                UpdateAxisColor(arrowMeshModel.Geometry, 2, AxisZColor, LabelZ, LabelColor);
            }

            private void UpdateAxisColor(int which, Color4 color)
            {
                string label = "";
                switch (which)
                {
                    case 0:
                        label = LabelX;
                        break;

                    case 1:
                        label = LabelY;
                        break;

                    case 2:
                        label = LabelZ;
                        break;
                }
                UpdateAxisColor(arrowMeshModel.Geometry, which, color, label, LabelColor);
            }

            private void UpdateAxisLabel(int which, string label)
            {
                Color4 color = Color.Red;
                switch (which)
                {
                    case 0:
                        color = AxisXColor;
                        break;

                    case 1:
                        color = AxisYColor;
                        break;

                    case 2:
                        color = AxisZColor;
                        break;
                }
                UpdateAxisColor(arrowMeshModel.Geometry, which, color, label, LabelColor);
            }

            private void UpdateLabelColor(Color4 color)
            {
                UpdateAxisColor(arrowMeshModel.Geometry, 0, AxisXColor, LabelX, LabelColor);
                UpdateAxisColor(arrowMeshModel.Geometry, 1, AxisYColor, LabelY, LabelColor);
                UpdateAxisColor(arrowMeshModel.Geometry, 2, AxisZColor, LabelZ, LabelColor);
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
                var colors = new Color4Collection(mesh.Colors == null ? Enumerable.Repeat<Color4>(Color.Black, mesh.Positions.Count) : mesh.Colors);
                for (int i = segment * which; i < segment * (which + 1); ++i)
                {
                    colors[i] = color;
                }
                mesh.Colors = colors;
            }
            /// <summary>
            /// Determines whether this instance [can hit test] the specified context.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <returns>
            ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
            /// </returns>
            protected override bool CanHitTest(RenderContext context)
            {
                return false;
            }
        }
    }

}