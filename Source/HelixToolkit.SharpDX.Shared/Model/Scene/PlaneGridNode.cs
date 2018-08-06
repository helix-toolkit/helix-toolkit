/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;
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
    public class PlaneGridNode : SceneNode
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
                (RenderCore as PlaneGridCore).AutoSpacing = value;
            }
            get { return (RenderCore as PlaneGridCore).AutoSpacing; }
        }

        /// <summary>
        /// Gets the acutal spacing.
        /// </summary>
        /// <value>
        /// The acutal spacing.
        /// </value>
        public float AcutalSpacing
        {
            get { return (RenderCore as PlaneGridCore).AcutalSpacing; }
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
                (RenderCore as PlaneGridCore).GridSpacing = value;
            }
            get
            {
                return (RenderCore as PlaneGridCore).GridSpacing;
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
                (RenderCore as PlaneGridCore).GridThickness = value;
            }
            get
            {
                return (RenderCore as PlaneGridCore).GridThickness;
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
                (RenderCore as PlaneGridCore).FadingFactor = value;
            }
            get { return (RenderCore as PlaneGridCore).FadingFactor; }
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
                (RenderCore as PlaneGridCore).PlaneColor = value;
            }
            get { return (RenderCore as PlaneGridCore).PlaneColor; }
        }

        /// <summary>
        /// Gets or sets the color of the grid.
        /// </summary>
        /// <value>
        /// The color of the grid.
        /// </value>
        public Color4 GridColor
        {
            set { (RenderCore as PlaneGridCore).GridColor = value; }
            get { return (RenderCore as PlaneGridCore).GridColor; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [render shadow map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render shadow map]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderShadowMap
        {
            set { (RenderCore as PlaneGridCore).RenderShadowMap = value; }
            get { return (RenderCore as PlaneGridCore).RenderShadowMap; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaneGridNode"/> class.
        /// </summary>
        public PlaneGridNode()
        {
            RenderOrder = 1000;
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new PlaneGridCore();
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