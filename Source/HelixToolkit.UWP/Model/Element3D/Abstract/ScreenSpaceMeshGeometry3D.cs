// <copyright file="ScreenSpaceMeshGeometry3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>
using Windows.UI.Xaml;

namespace HelixToolkit.UWP
{
    using Model;
    using Model.Scene;



    /// <summary>
    /// Base class for screen space rendering, such as Coordinate System or ViewBox
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
        /// The left handed property
        /// </summary>
        public static readonly DependencyProperty LeftHandedProperty = DependencyProperty.Register("LeftHanded", typeof(bool), typeof(ScreenSpacedElement3D),
            new PropertyMetadata(false,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ScreenSpacedNode).IsRightHand = !(bool)e.NewValue;
            }));

        public bool LeftHanded
        {
            set
            {
                SetValue(LeftHandedProperty, value);
            }
            get
            {
                return (bool)GetValue(LeftHandedProperty);
            }
        }
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


        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            if (node is ScreenSpacedNode n)
            {
                n.RelativeScreenLocationX = (float)this.RelativeScreenLocationX;
                n.RelativeScreenLocationY = (float)this.RelativeScreenLocationY;
                n.SizeScale = (float)this.SizeScale;
            }
            base.AssignDefaultValuesToSceneNode(node);
        }
    }
}