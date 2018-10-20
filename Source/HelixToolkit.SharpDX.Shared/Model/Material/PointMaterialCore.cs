/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Core;
    public sealed class PointMaterialCore : MaterialCore, IPointRenderParams
    {
        private float width = 0.5f;
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public float Width
        {
            set
            {
                Set(ref width, value);
            }
            get { return width; }
        }

        private float height = 0.5f;
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height
        {
            set
            {
                Set(ref height, value);
            }
            get { return height; }
        }

        private PointFigure figure = PointFigure.Rect;
        /// <summary>
        /// Gets or sets the figure.
        /// </summary>
        /// <value>
        /// The figure.
        /// </value>
        public PointFigure Figure
        {
            set
            {
                Set(ref figure, value);
            }
            get { return figure; }
        }

        private float figureRatio = 0.25f;
        /// <summary>
        /// Gets or sets the figure ratio.
        /// </summary>
        /// <value>
        /// The figure ratio.
        /// </value>
        public float FigureRatio
        {
            set
            {
                Set(ref figureRatio, value);
            }
            get { return figureRatio; }
        }

        private Color4 pointColor = Color.Black;
        /// <summary>
        /// Final Point Color = PointColor * PerVertexPointColor
        /// </summary>
        public Color4 PointColor
        {
            set
            {
                Set(ref pointColor, value);
            }
            get
            {
                return pointColor;
            }
        }
        private bool enableDistanceFading = false;
        public bool EnableDistanceFading
        {
            set
            {
                Set(ref enableDistanceFading, value);
            }
            get { return enableDistanceFading; }
        }

        private float fadingNearDistance = 0;
        public float FadingNearDistance
        {
            set { Set(ref fadingNearDistance, value); }
            get { return fadingNearDistance; }
        }

        private float fadingFarDistance = 100;
        public float FadingFarDistance
        {
            set { Set(ref fadingFarDistance, value); }
            get { return fadingFarDistance; }
        }
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new PointMaterialVariable(manager, technique, this);
        }
    }
}
