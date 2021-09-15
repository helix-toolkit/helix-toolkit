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
        public static class DefaultHullShaders
        {
            /// <summary>
            /// 
            /// </summary>
            public static string HSMeshTessellation
            {
                get;
            } = "hsMeshTriTessellation";
        }

        public static class DefaultHullShaderDescriptions
        {
            public static readonly ShaderDescription HSMeshTessellation = new ShaderDescription(nameof(HSMeshTessellation), ShaderStage.Hull, new ShaderReflector(),
                DefaultHullShaders.HSMeshTessellation);
        }
    }

}
