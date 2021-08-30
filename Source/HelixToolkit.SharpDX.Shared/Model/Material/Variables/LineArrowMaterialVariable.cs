/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
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
        using Shaders;
        public class LineArrowMaterialVariable : LineMaterialVariable
        {
            private readonly LineArrowHeadMaterialCore material;
            /// <summary>
            /// Initializes a new instance of the <see cref="LineMaterialVariable"/> class.
            /// </summary>
            /// <param name="manager">The manager.</param>
            /// <param name="technique">The technique.</param>
            /// <param name="materialCore">The material core.</param>
            /// <param name="linePassName">Name of the line pass.</param>
            /// <param name="shadowPassName">Name of the shadow pass.</param>
            /// <param name="depthPassName">Name of the depth pass</param>
            public LineArrowMaterialVariable(IEffectsManager manager, IRenderTechnique technique, LineArrowHeadMaterialCore materialCore,
                string linePassName = DefaultPassNames.Default, string shadowPassName = DefaultPassNames.ShadowPass,
                string depthPassName = DefaultPassNames.DepthPrepass)
                : base(manager, technique, materialCore, linePassName, shadowPassName, depthPassName)
            {
                this.material = materialCore;          
            }

            protected override void OnInitialPropertyBindings()
            {
                base.OnInitialPropertyBindings();
                AddPropertyBinding(nameof(LineArrowHeadMaterialCore.ArrowSize),
                    () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector3(material.Thickness, material.Smoothness, material.ArrowSize)); });
            }
        }
    }
}
