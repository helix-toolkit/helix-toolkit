/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
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
    namespace Model.Scene
    {
        using Core;

        /// <summary>
        /// 
        /// </summary>
        public abstract class LightNode : SceneNode, ILight3D
        {
            /// <summary>
            /// Gets or sets the color.
            /// </summary>
            /// <value>
            /// The color.
            /// </value>
            public Color4 Color
            {
                set { (RenderCore as LightCoreBase).Color = value; }
                get { return (RenderCore as LightCoreBase).Color; }
            }
            /// <summary>
            /// Gets the type of the light.
            /// </summary>
            /// <value>
            /// The type of the light.
            /// </value>
            public LightType LightType
            {
                get { return (RenderCore as LightCoreBase).LightType; }
            }

            public sealed override bool HitTest(RenderContext context, Ray ray, ref List<HitTestResult> hits)
            {
                return false;
            }

            protected sealed override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
            {
                return false;
            }
        }
    }

}
