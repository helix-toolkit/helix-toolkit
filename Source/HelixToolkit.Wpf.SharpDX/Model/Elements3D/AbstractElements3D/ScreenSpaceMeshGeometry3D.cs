// <copyright file="ScreenSpaceMeshGeometry3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>

using SharpDX;
using SharpDX.Direct3D11;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    using Core;
    /// <summary>
    /// Base class for screen space rendering, such as Coordinate System or ViewBox
    /// </summary>
    public abstract class ScreenSpaceMeshGeometry3D : MeshGeometryModel3D
    {        
        /// <summary>
        /// <see cref="RelativeScreenLocationX"/>
        /// </summary>
        public static readonly DependencyProperty RelativeScreenLocationXProperty = DependencyProperty.Register("RelativeScreenLocationX", typeof(double), typeof(ScreenSpaceMeshGeometry3D),
            new AffectsRenderPropertyMetadata(-0.8,
                (d, e) =>
                {
                    (d as ScreenSpaceMeshGeometry3D).screenSpaceCore.RelativeScreenLocationX = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// <see cref="RelativeScreenLocationY"/>
        /// </summary>
        public static readonly DependencyProperty RelativeScreenLocationYProperty = DependencyProperty.Register("RelativeScreenLocationY", typeof(double), typeof(ScreenSpaceMeshGeometry3D),
            new AffectsRenderPropertyMetadata(-0.8,
                (d, e) =>
                {
                    (d as ScreenSpaceMeshGeometry3D).screenSpaceCore.RelativeScreenLocationY = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// <see cref="SizeScale"/>
        /// </summary>
        public static readonly DependencyProperty SizeScaleProperty = DependencyProperty.Register("SizeScale", typeof(double), typeof(ScreenSpaceMeshGeometry3D),
            new AffectsRenderPropertyMetadata(1.0,
                (d, e) =>
                {
                    (d as ScreenSpaceMeshGeometry3D).screenSpaceCore.SizeScale = (float)(double)e.NewValue;
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

        protected override bool CanHitTest(IRenderMatrices context)
        {
            return false;
        }


        private ScreenSpacedMeshRenderCore screenSpaceCore;

        protected override IRenderCore OnCreateRenderCore()
        {
            screenSpaceCore = new ScreenSpacedMeshRenderCore();
            return screenSpaceCore;
        }

        protected virtual DepthStencilState CreateDepthStencilState(global::SharpDX.Direct3D11.Device device)
        {
            return new DepthStencilState(device, new DepthStencilStateDescription() { IsDepthEnabled = true, IsStencilEnabled = false, DepthWriteMask = DepthWriteMask.All, DepthComparison = Comparison.LessEqual });
        }

        protected override bool CheckBoundingFrustum(ref BoundingFrustum boundingFrustum)
        {
            return true;
        }
    }
}
