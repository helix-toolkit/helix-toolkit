/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

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
        public class SpotLightNode : PointLightNode
        {
            /// <summary>
            /// Gets or sets the direction.
            /// </summary>
            /// <value>
            /// The direction.
            /// </value>
            public Vector3 Direction
            {
                set { (RenderCore as SpotLightCore).Direction = value; }
                get { return (RenderCore as SpotLightCore).Direction; }
            }
            /// <summary>
            /// Gets or sets the fall off.
            /// </summary>
            /// <value>
            /// The fall off.
            /// </value>
            public float FallOff
            {
                set { (RenderCore as SpotLightCore).FallOff = value; }
                get { return (RenderCore as SpotLightCore).FallOff; }
            }
            /// <summary>
            /// Gets or sets the inner angle.
            /// </summary>
            /// <value>
            /// The inner angle.
            /// </value>
            public float InnerAngle
            {
                set { (RenderCore as SpotLightCore).InnerAngle = value; }
                get { return (RenderCore as SpotLightCore).InnerAngle; }
            }
            /// <summary>
            /// Gets or sets the outer angle.
            /// </summary>
            /// <value>
            /// The outer angle.
            /// </value>
            public float OuterAngle
            {
                set { (RenderCore as SpotLightCore).OuterAngle = value; }
                get { return (RenderCore as SpotLightCore).OuterAngle; }
            }
            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                return new SpotLightCore();
            }
        }
    }

}
