/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Runtime.Serialization;
#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model
    {
        public class LineArrowHeadMaterialCore : LineMaterialCore
        {
            private float arrowSize = 0.1f;
            /// <summary>
            /// Gets or sets the size of the arrow.
            /// </summary>
            /// <value>
            /// The size of the arrow.
            /// </value>
            public float ArrowSize
            {
                set { Set(ref arrowSize, value); }
                get { return arrowSize; }
            }

            public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
            {
                return new LineArrowMaterialVariable(manager, manager.GetTechnique(DefaultRenderTechniqueNames.LinesArrowHead), this);
            }
        }

        public class LineArrowHeadTailMaterialCore : LineArrowHeadMaterialCore
        {
            public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
            {
                return new LineArrowMaterialVariable(manager, manager.GetTechnique(DefaultRenderTechniqueNames.LinesArrowHeadTail), this);
            }
        }
    }
}
