/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using Matrix = System.Numerics.Matrix4x4;
using HelixToolkit.Mathematics;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;

    /// <summary>
    ///
    /// </summary>
    public class AxisPlaneGridNode : SceneNode
    {
        /// <summary>
        /// Gets or sets a value indicating whether [automatic spacing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic spacing]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoSpacing
        {
            set
            {
                (RenderCore as AxisPlaneGridCore).AutoSpacing = value;
            }
            get { return (RenderCore as AxisPlaneGridCore).AutoSpacing; }
        }
        /// <summary>
        /// Gets or sets the automatic spacing rate.
        /// </summary>
        /// <value>
        /// The automatic spacing rate.
        /// </value>
        public float AutoSpacingRate
        {
            set { (RenderCore as AxisPlaneGridCore).AutoSpacingRate = value; }
            get { return (RenderCore as AxisPlaneGridCore).AutoSpacingRate; }
        }

        /// <summary>
        /// Gets the acutal spacing.
        /// </summary>
        /// <value>
        /// The acutal spacing.
        /// </value>
        public float AcutalSpacing
        {
            get { return (RenderCore as AxisPlaneGridCore).AcutalSpacing; }
        }

        /// <summary>
        /// Gets or sets the grid spacing.
        /// </summary>
        /// <value>
        /// The grid spacing.
        /// </value>
        public float GridSpacing
        {
            set
            {
                (RenderCore as AxisPlaneGridCore).GridSpacing = value;
            }
            get
            {
                return (RenderCore as AxisPlaneGridCore).GridSpacing;
            }
        }

        /// <summary>
        /// Gets or sets the grid thickness.
        /// </summary>
        /// <value>
        /// The grid thickness.
        /// </value>
        public float GridThickness
        {
            set
            {
                (RenderCore as AxisPlaneGridCore).GridThickness = value;
            }
            get
            {
                return (RenderCore as AxisPlaneGridCore).GridThickness;
            }
        }

        /// <summary>
        /// Gets or sets the fading factor.
        /// </summary>
        /// <value>
        /// The fading factor.
        /// </value>
        public float FadingFactor
        {
            set
            {
                (RenderCore as AxisPlaneGridCore).FadingFactor = value;
            }
            get { return (RenderCore as AxisPlaneGridCore).FadingFactor; }
        }

        /// <summary>
        /// Gets or sets the color of the plane.
        /// </summary>
        /// <value>
        /// The color of the plane.
        /// </value>
        public Color4 PlaneColor
        {
            set
            {
                (RenderCore as AxisPlaneGridCore).PlaneColor = value;
            }
            get { return (RenderCore as AxisPlaneGridCore).PlaneColor; }
        }

        /// <summary>
        /// Gets or sets the color of the grid.
        /// </summary>
        /// <value>
        /// The color of the grid.
        /// </value>
        public Color4 GridColor
        {
            set { (RenderCore as AxisPlaneGridCore).GridColor = value; }
            get { return (RenderCore as AxisPlaneGridCore).GridColor; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [render shadow map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render shadow map]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderShadowMap
        {
            set { (RenderCore as AxisPlaneGridCore).RenderShadowMap = value; }
            get { return (RenderCore as AxisPlaneGridCore).RenderShadowMap; }
        }

        /// <summary>
        /// Gets or sets up axis.
        /// </summary>
        /// <value>
        /// Up axis.
        /// </value>
        public Axis UpAxis
        {
            set { (RenderCore as AxisPlaneGridCore).UpAxis = value; }
            get { return (RenderCore as AxisPlaneGridCore).UpAxis; }
        }

        /// <summary>
        /// Gets or sets the axis plane offset.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        public float Offset
        {
            set { (RenderCore as AxisPlaneGridCore).Offset = value; }
            get { return (RenderCore as AxisPlaneGridCore).Offset; }
        }
        /// <summary>
        /// Gets or sets the type of the grid.
        /// </summary>
        /// <value>
        /// The type of the grid.
        /// </value>
        public GridPattern GridPattern
        {
            set { (RenderCore as AxisPlaneGridCore).GridPattern = value; }
            get { return (RenderCore as AxisPlaneGridCore).GridPattern; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AxisPlaneGridNode"/> class.
        /// </summary>
        public AxisPlaneGridNode()
        {
            RenderOrder = 1000;
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new AxisPlaneGridCore();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.PlaneGrid];
        }

        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}