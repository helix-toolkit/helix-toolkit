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
        public static ShaderDescription DSMeshTessellation = new ShaderDescription(nameof(DSMeshTessellation), ShaderStage.Domain, new ShaderReflector(),
            DefaultDomainShaders.DSMeshTessellation);
    }
}
