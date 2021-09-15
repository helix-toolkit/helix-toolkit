/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/


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
    namespace Shaders
    {
        using Helper;
        /// <summary>
        /// 
        /// </summary>
        public static class DefaultDomainShaders
        {
            /// <summary>
            /// 
            /// </summary>
            public static string DSMeshTessellation
            {
                get;
            } = "dsMeshTriTessellation";
        }

        public static class DefaultDomainShaderDescriptions
        {
            public static readonly ShaderDescription DSMeshTessellation = new ShaderDescription(nameof(DSMeshTessellation), ShaderStage.Domain, new ShaderReflector(),
                DefaultDomainShaders.DSMeshTessellation);
        }
    }

}
