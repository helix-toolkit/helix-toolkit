// <copyright file="ScreenSpaceMeshGeometry3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>
using System.Windows;
using Media3D = System.Windows.Media.Media3D;
namespace HelixToolkit.Wpf.SharpDX
{
    using Core;
    using global::SharpDX;

    /// <summary>
    /// Base class for screen space rendering, such as Coordinate System or ViewBox
    /// </summary>
    public abstract class ScreenSpacedElement3D : GroupModel3D
    {        
        /// <summary>
        /// <see cref="RelativeScreenLocationX"/>
        /// </summary>
        public static readonly DependencyProperty RelativeScreenLocationXProperty = DependencyProperty.Register("RelativeScreenLocationX", typeof(double), typeof(ScreenSpacedElement3D),
            new AffectsRenderPropertyMetadata(-0.8,
                (d, e) =>
                {
                   ((d as ScreenSpacedElement3D).RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationX = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// <see cref="RelativeScreenLocationY"/>
        /// </summary>
        public static readonly DependencyProperty RelativeScreenLocationYProperty = DependencyProperty.Register("RelativeScreenLocationY", typeof(double), typeof(ScreenSpacedElement3D),
            new AffectsRenderPropertyMetadata(-0.8,
                (d, e) =>
                {
                    ((d as ScreenSpacedElement3D).RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationY = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// <see cref="SizeScale"/>
        /// </summary>
        public static readonly DependencyProperty SizeScaleProperty = DependencyProperty.Register("SizeScale", typeof(double), typeof(ScreenSpacedElement3D),
            new AffectsRenderPropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as ScreenSpacedElement3D).RenderCore as IScreenSpacedRenderParams).SizeScale = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register("UpDirection", typeof(Media3D.Vector3D), typeof(ScreenSpacedElement3D),
            new AffectsRenderPropertyMetadata(new Media3D.Vector3D(0, 1, 0),
            (d, e) =>
            {
                (d as ScreenSpacedElement3D).UpdateModel(((Media3D.Vector3D)e.NewValue).ToVector3());
            }));

        public Media3D.Vector3D UpDirection
        {
            set
            {
                SetValue(UpDirectionProperty, value);
            }
            get
            {
                return (Media3D.Vector3D)GetValue(UpDirectionProperty);
            }
        }

        public static readonly DependencyProperty LeftHandedProperty = DependencyProperty.Register("LeftHanded", typeof(bool), typeof(ScreenSpacedElement3D),
            new AffectsRenderPropertyMetadata(false,
            (d, e) =>
            {
                (d as ScreenSpacedElement3D).screenSpaceCore.IsRightHand = !(bool)e.NewValue;
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

        protected bool NeedClearDepthBuffer { set; get; } = true;


        protected IScreenSpacedRenderParams screenSpaceCore { get { return (IScreenSpacedRenderParams)RenderCore; } }

        protected abstract void UpdateModel(Vector3 upDirection);

        protected override IRenderCore OnCreateRenderCore()
        {
            return new ScreenSpacedMeshRenderCore();
        }      

        protected override bool OnAttach(IRenderHost host)
        {
            RenderCore.Attach(renderTechnique);
            screenSpaceCore.RelativeScreenLocationX = (float)this.RelativeScreenLocationX;
            screenSpaceCore.RelativeScreenLocationY = (float)this.RelativeScreenLocationY;
            screenSpaceCore.SizeScale = (float)this.SizeScale;
            return base.OnAttach(host);
        }

        protected override void OnDetach()
        {
            RenderCore.Detach();
            base.OnDetach();
        }

        protected override void OnRender(IRenderContext renderContext)
        {
            screenSpaceCore.SetScreenSpacedCoordinates(renderContext, NeedClearDepthBuffer);
            base.OnRender(renderContext);
        }
    }
}
