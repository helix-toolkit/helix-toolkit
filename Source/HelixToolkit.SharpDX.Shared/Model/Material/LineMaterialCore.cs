/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Runtime.Serialization;
using SharpDX.Direct3D11;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model
    {
        using Core;
        using Shaders;

        [DataContract]
        public class LineMaterialCore : MaterialCore, ILineRenderParams
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

            private bool fixedSize = true;
            /// <summary>
            /// Gets or sets a value indicating whether [fixed size].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [fixed size]; otherwise, <c>false</c>.
            /// </value>
            public bool FixedSize
            {
                set { Set(ref fixedSize, value); }
                get { return fixedSize; }
            }

            private TextureModel texture;
            /// <summary>
            /// Gets or sets the texture.
            /// </summary>
            /// <value>
            /// The texture.
            /// </value>
            public TextureModel Texture
            {
                set { Set(ref texture, value); }
                get { return texture; }
            }

            private float textureScale = 1;
            /// <summary>
            /// Gets or sets the texture scale.
            /// </summary>
            /// <value>
            /// The texture scale.
            /// </value>
            public float TextureScale
            {
                set { Set(ref textureScale, value); }
                get { return textureScale; }
            }

            private float alphaThreshold = 0.2f;
            /// <summary>
            /// Gets or sets the alpha threshold. Pixel with color alpha value smaller than threshold will be set to transparent.
            /// <para>This is used to avoid sampler color interpolation effects.</para>
            /// </summary>
            /// <value>
            /// The alpha threshold
            /// </value>
            public float AlphaThreshold
            {
                set { Set(ref alphaThreshold, value); }
                get { return alphaThreshold; }
            }


            private SamplerStateDescription samplerDescription = DefaultSamplers.LineSamplerUWrapVClamp;
            /// <summary>
            /// Billboard texture sampler description
            /// </summary>
            public SamplerStateDescription SamplerDescription
            {
                set
                {
                    Set(ref samplerDescription, value);
                }
                get
                {
                    return samplerDescription;
                }
            }
            #endregion

            public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
            {
                return new LineMaterialVariable(manager, technique, this);
            }
        }
    }

}
