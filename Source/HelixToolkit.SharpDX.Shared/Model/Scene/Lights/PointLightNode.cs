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
        public class PointLightNode : LightNode
        {
            /// <summary>
            /// Gets or sets the position.
            /// </summary>
            /// <value>
            /// The position.
            /// </value>
            public Vector3 Position
            {
                set { (RenderCore as PointLightCore).Position = value; }
                get { return (RenderCore as PointLightCore).Position; }
            }
            /// <summary>
            /// Gets or sets the attenuation.
            /// </summary>
            /// <value>
            /// The attenuation.
            /// </value>
            public Vector3 Attenuation
            {
                set { (RenderCore as PointLightCore).Attenuation = value; }
                get { return (RenderCore as PointLightCore).Attenuation; }
            }
            /// <summary>
            /// Gets or sets the range.
            /// </summary>
            /// <value>
            /// The range.
            /// </value>
            public float Range
            {
                set { (RenderCore as PointLightCore).Range = value; }
                get { return (RenderCore as PointLightCore).Range; }
            }
            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                return new PointLightCore();
            }
        }
    }

}
