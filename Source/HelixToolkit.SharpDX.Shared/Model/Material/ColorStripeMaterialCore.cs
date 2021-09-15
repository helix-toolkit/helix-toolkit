/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.Runtime.Serialization;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model
    {
        using Shaders;
        [DataContract]
        public class ColorStripeMaterialCore : MaterialCore
        {
            private Color4 diffuseColor = Color.White;
            /// <summary>
            /// Gets or sets the color of the diffuse.
            /// </summary>
            /// <value>
            /// The color of the diffuse.
            /// </value>
            public Color4 DiffuseColor
            {
                set { Set(ref diffuseColor, value); }
                get { return diffuseColor; }
            }

            private IList<Color4> colorStripeX = null;
            /// <summary>
            /// Gets or sets the color stripe x. Use texture coordinate X for sampling
            /// </summary>
            /// <value>
            /// The color stripe x.
            /// </value>
            public IList<Color4> ColorStripeX
            {
                set { Set(ref colorStripeX, value); }
                get { return colorStripeX; }
            }

            private IList<Color4> colorStripeY = null;
            /// <summary>
            /// Gets or sets the color stripe y. Use texture coordinate Y for sampling
            /// </summary>
            /// <value>
            /// The color stripe y.
            /// </value>
            public IList<Color4> ColorStripeY
            {
                set { Set(ref colorStripeY, value); }
                get { return colorStripeY; }
            }

            private bool colorStripeXEnabled = true;
            /// <summary>
            /// Gets or sets a value indicating whether [color stripe x enabled].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [color stripe x enabled]; otherwise, <c>false</c>.
            /// </value>
            public bool ColorStripeXEnabled
            {
                set { Set(ref colorStripeXEnabled, value); }
                get { return colorStripeXEnabled; }
            }

            private bool colorStripeYEnabled = true;
            /// <summary>
            /// Gets or sets a value indicating whether [color stripe y enabled].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [color stripe y enabled]; otherwise, <c>false</c>.
            /// </value>
            public bool ColorStripeYEnabled
            {
                set { Set(ref colorStripeYEnabled, value); }
                get { return colorStripeYEnabled; }
            }

            private global::SharpDX.Direct3D11.SamplerStateDescription colorStripeSampler = DefaultSamplers.LinearSamplerClampAni1;
            /// <summary>
            /// Gets or sets the DiffuseMapSampler.
            /// </summary>
            /// <value>
            /// DiffuseMapSampler
            /// </value>
            public global::SharpDX.Direct3D11.SamplerStateDescription ColorStripeSampler
            {
                set { Set(ref colorStripeSampler, value); }
                get { return colorStripeSampler; }
            }

            public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
            {
                return new ColorStripeMaterialVariables(manager, technique, this);
            }
        }
    }

}
