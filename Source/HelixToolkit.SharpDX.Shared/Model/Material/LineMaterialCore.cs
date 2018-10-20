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
    public sealed class LineMaterialCore : MaterialCore, ILineRenderParams
    {
        #region Properties
        private float thickness = 0.5f;
        /// <summary>
        /// 
        /// </summary>
        public float Thickness
        {
            set
            {
                Set(ref thickness, value);
            }
            get
            {
                return thickness;
            }
        }

        private float smoothness;
        /// <summary>
        /// 
        /// </summary>
        public float Smoothness
        {
            set
            {
                Set(ref smoothness, value);
            }
            get { return smoothness; }
        }

        private Color4 lineColor = Color.Blue;
        /// <summary>
        /// Final Line Color = LineColor * PerVertexLineColor
        /// </summary>
        public Color4 LineColor
        {
            set
            {
                Set(ref lineColor, value);
            }
            get { return lineColor; }
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

        private float fadingNearDistance = 100;
        public float FadingNearDistance
        {
            set { Set(ref fadingNearDistance, value); }
            get { return fadingNearDistance; }
        }

        private float fadingFarDistance = 0;
        public float FadingFarDistance
        {
            set { Set(ref fadingFarDistance, value); }
            get { return fadingFarDistance; }
        }
        #endregion

        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new LineMaterialVariable(manager, technique, this);
        }
    }
}
