/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/


#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
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
        public static ShaderDescription HSMeshTessellation = new ShaderDescription(nameof(HSMeshTessellation), ShaderStage.Hull, new ShaderReflector(),
            DefaultHullShaders.HSMeshTessellation);
    }
}
