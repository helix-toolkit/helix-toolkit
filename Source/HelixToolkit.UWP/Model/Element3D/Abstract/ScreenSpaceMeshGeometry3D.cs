// <copyright file="ScreenSpaceMeshGeometry3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>
/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using Windows.UI.Xaml;
using Point3D = SharpDX.Vector3;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Point3D = System.Windows.Media.Media3D.Point3D;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Model.Scene;

    /// <summary>
    /// ScreenSpacedElement3D uses a fixed camera to render model (Mainly used for view box and coordinate system rendering) onto screen which is separated from viewport camera.
    /// <para>
    /// Default fix camera is perspective camera with FOV 45 degree and camera distance = 20. Look direction is always looking at (0,0,0).
    /// </para>
    /// <para>
    /// User must properly scale the model to fit into the camera frustum. The usual maximum size is from (5,5,5) to (-5,-5,-5) bounding box.
    /// </para>
    /// <para>
    /// User can use <see cref="ScreenSpacedNode.SizeScale"/> to scale the size of the rendering.
    /// </para>
    /// </summary>
    public abstract class ScreenSpacedElement3D : GroupModel3D
    {
        /// <summary>
        /// <see cref="RelativeScreenLocationX"/>
        /// </summary>
        public static readonly DependencyProperty RelativeScreenLocationXProperty = DependencyProperty.Register("RelativeScreenLocationX", typeof(double), typeof(ScreenSpacedElement3D),
            new PropertyMetadata(-0.8,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ScreenSpacedNode).RelativeScreenLocationX = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// <see cref="RelativeScreenLocationY"/>
        /// </summary>
        public static readonly DependencyProperty RelativeScreenLocationYProperty = DependencyProperty.Register("RelativeScreenLocationY", typeof(double), typeof(ScreenSpacedElement3D),
            new PropertyMetadata(-0.8,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ScreenSpacedNode).RelativeScreenLocationY = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// <see cref="SizeScale"/>
        /// </summary>
        public static readonly DependencyProperty SizeScaleProperty = DependencyProperty.Register("SizeScale", typeof(double), typeof(ScreenSpacedElement3D),
            new PropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ScreenSpacedNode).SizeScale = (float)(double)e.NewValue;
                }));

        /// <summary>
        /// The enable mover property
        /// </summary>
        public static readonly DependencyProperty EnableMoverProperty =
            DependencyProperty.Register("EnableMover", typeof(bool), typeof(ScreenSpacedElement3D), new PropertyMetadata(true));

        /// <summary>
        /// The mode property
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(ScreenSpacedMode), typeof(ScreenSpacedElement3D), new PropertyMetadata(ScreenSpacedMode.RelativeScreenSpaced,
                (d,e)=> 
                {
                    ((d as Element3DCore).SceneNode as ScreenSpacedNode).Mode = (ScreenSpacedMode)e.NewValue;
                }));

        // Using a DependencyProperty as the backing store for CameraType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CameraTypeProperty =
            DependencyProperty.Register("CameraType", typeof(ScreenSpacedCameraType), typeof(ScreenSpacedElement3D), new PropertyMetadata(ScreenSpacedCameraType.Auto,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ScreenSpacedNode).CameraType = (ScreenSpacedCameraType)e.NewValue;
                }));

        /// <summary>
        /// The absolute position3 d property
        /// </summary>
        public static readonly DependencyProperty AbsolutePosition3DProperty =
            DependencyProperty.Register("AbsolutePosition3D", typeof(Point3D), typeof(ScreenSpacedElement3D), new PropertyMetadata(new Point3D(), (d, e) =>
            {
#if NETFX_CORE
                ((d as Element3DCore).SceneNode as ScreenSpacedNode).AbsolutePosition3D = (Point3D)e.NewValue;
#else
                ((d as Element3DCore).SceneNode as ScreenSpacedNode).AbsolutePosition3D = ((Point3D)e.NewValue).ToVector3();
#endif
            }));

        /// <summary>
        /// Relative Location X on screen. Range from -1~1
        /// </summary>
        public double RelativeScreenLocationX
        {
            set
            {
                SetValue(RelativeScreenLocationXProperty, value);
            }
            get
            {
                return (double)GetValue(RelativeScreenLocationXProperty);
            }
        }

        /// <summary>
        /// Relative Location Y on screen. Range from -1~1
        /// </summary>
        public double RelativeScreenLocationY
        {
            set
            {
                SetValue(RelativeScreenLocationYProperty, value);
            }
            get
            {
                return (double)GetValue(RelativeScreenLocationYProperty);
            }
        }

        /// <summary>
        /// Size scaling
        /// </summary>
        public double SizeScale
        {
            set
            {
                SetValue(SizeScaleProperty, value);
            }
            get
            {
                return (double)GetValue(SizeScaleProperty);
            }
        }
        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public ScreenSpacedMode Mode
        {
            get { return (ScreenSpacedMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Only being used when <see cref="Mode"/> is RelativeScreenSpaced
        /// </summary>
        /// <value>
        /// The type of the camera.
        /// </value>
        public ScreenSpacedCameraType CameraType
        {
            get
            {
                return (ScreenSpacedCameraType)GetValue(CameraTypeProperty);
            }
            set
            {
                SetValue(CameraTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the absolute position in 3d. Use by <see cref="Mode"/> = <see cref="ScreenSpacedMode.AbsolutePosition3D"/>
        /// </summary>
        /// <value>
        /// The absolute position3 d.
        /// </value>
        public Point3D AbsolutePosition3D
        {
            get { return (Point3D)GetValue(AbsolutePosition3DProperty); }
            set { SetValue(AbsolutePosition3DProperty, value); }
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            if (node is ScreenSpacedNode n)
            {
                n.RelativeScreenLocationX = (float)this.RelativeScreenLocationX;
                n.RelativeScreenLocationY = (float)this.RelativeScreenLocationY;
                n.SizeScale = (float)this.SizeScale;
                n.Mode = Mode;
#if NETFX_CORE
                n.AbsolutePosition3D = AbsolutePosition3D;
#else
                n.AbsolutePosition3D = AbsolutePosition3D.ToVector3();
#endif
            }
            base.AssignDefaultValuesToSceneNode(node);
        }
    }
}