/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
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
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public abstract class MaterialCore : ObservableObject, IMaterial
        {
            private string name = "Material";
            public string Name
            {
                set
                {
                    Set(ref name, value);
                }
                get
                {
                    return name;
                }
            }

            public Guid Guid { get; } = Guid.NewGuid();
            /// <summary>
            /// Creates the material variables.
            /// </summary>
            /// <param name="manager">The manager.</param>
            /// <param name="technique">The technique.</param>
            /// <returns></returns>
            public abstract MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique);
        }
    }

}
