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
    public static class DefaultComputeShaders
    {
        /// <summary>
        /// 
        /// </summary>
        public static string CSParticleInsert
        {
            get;
        } = "csParticleInsert";

        /// <summary>
        /// 
        /// </summary>
        public static string CSParticleUpdate
        {
            get;
        } = "csParticleUpdate";
    }


    public static class DefaultComputeShaderDescriptions
    {        
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription CSParticleInsert = new ShaderDescription(nameof(CSParticleInsert), ShaderStage.Compute, new ShaderReflector(),
            DefaultComputeShaders.CSParticleInsert);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription CSParticleUpdate = new ShaderDescription(nameof(CSParticleUpdate), ShaderStage.Compute, new ShaderReflector(),
            DefaultComputeShaders.CSParticleUpdate);
    }
}
