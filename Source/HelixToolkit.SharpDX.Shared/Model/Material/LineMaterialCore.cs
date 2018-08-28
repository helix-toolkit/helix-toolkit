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

        #endregion

        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new LineMaterialVariable(manager, technique, this);
        }
    }
}
