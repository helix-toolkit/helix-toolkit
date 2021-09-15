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
    namespace Core
    {
        using Model;

        /// <summary>
        ///
        /// </summary>
        public class PointLightCore : LightCoreBase
        {
            private Vector3 position;
            /// <summary>
            /// Gets or sets the position.
            /// </summary>
            /// <value>
            /// The position.
            /// </value>
            public Vector3 Position
            {
                set
                {
                    SetAffectsRender(ref position, value);
                }
                get { return position; }
            }

            private Vector3 attenuation = new Vector3(1, 0, 0);
            /// <summary>
            /// Gets or sets the attenuation.
            /// </summary>
            /// <value>
            /// The attenuation.
            /// </value>
            public Vector3 Attenuation
            {
                set { SetAffectsRender(ref attenuation, value); }
                get { return attenuation; }
            }

            private float range = 1000;
            /// <summary>
            /// Gets or sets the range.
            /// </summary>
            /// <value>
            /// The range.
            /// </value>
            public float Range
            {
                set { SetAffectsRender(ref range, value); }
                get { return range; }
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="PointLightCore"/> class.
            /// </summary>
            public PointLightCore()
            {
                LightType = LightType.Point;
            }
            /// <summary>
            /// Called when [render].
            /// </summary>
            /// <param name="lightScene">The light scene.</param>
            /// <param name="index">The index.</param>
            protected override void OnRender(Light3DSceneShared lightScene, int index)
            {
                base.OnRender(lightScene, index);
                lightScene.LightModels.Lights[index].LightPos = (position + ModelMatrix.Row4.ToVector3()).ToVector4();
                lightScene.LightModels.Lights[index].LightAtt = attenuation.ToVector4(range);
            }
        }
    }

}