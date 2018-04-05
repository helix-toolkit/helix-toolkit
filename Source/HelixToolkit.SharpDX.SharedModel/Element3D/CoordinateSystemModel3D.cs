// <copyright file="CoordinateSystemModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>

using SharpDX;
using System.Collections.Generic;

#if NETFX_CORE
using Media = Windows.UI;
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Media = System.Windows.Media;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Model.Scene;

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
                    ((d as Element3DCore).SceneNode as CoordinateSystemNode).AxisXColor = ((Media.Color)e.NewValue).ToColor4();
                }));
        /// <summary>
        /// <see cref="AxisYColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisYColorProperty = DependencyProperty.Register("AxisYColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Green,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as CoordinateSystemNode).AxisYColor = ((Media.Color)e.NewValue).ToColor4();
                }));
        /// <summary>
        /// <see cref="AxisZColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisZColorProperty = DependencyProperty.Register("AxisZColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Blue,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as CoordinateSystemNode).AxisZColor = ((Media.Color)e.NewValue).ToColor4();
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelColorProperty = DependencyProperty.Register("LabelColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new PropertyMetadata(Media.Colors.Gray,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as CoordinateSystemNode).LabelColor = ((Media.Color)e.NewValue).ToColor4();
                }));

        /// <summary>
        /// The coordinate system label x property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelXProperty = DependencyProperty.Register(
                "CoordinateSystemLabelX", typeof(string), typeof(CoordinateSystemModel3D), new PropertyMetadata("X",
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as CoordinateSystemNode).LabelX = e.NewValue as string;
                }));

        /// <summary>
        /// The coordinate system label Y property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelYProperty = DependencyProperty.Register(
                "CoordinateSystemLabelY", typeof(string), typeof(CoordinateSystemModel3D), new PropertyMetadata("Y",
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as CoordinateSystemNode).LabelY = e.NewValue as string;
                }));

        /// <summary>
        /// The coordinate system label Z property
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemLabelZProperty = DependencyProperty.Register(
                "CoordinateSystemLabelZ", typeof(string), typeof(CoordinateSystemModel3D), new PropertyMetadata("Z",
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as CoordinateSystemNode).LabelZ = e.NewValue as string;
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


        protected override SceneNode OnCreateSceneNode()
        {
            return new CoordinateSystemNode();
        }

        public override bool HitTest(IRenderContext context, Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
