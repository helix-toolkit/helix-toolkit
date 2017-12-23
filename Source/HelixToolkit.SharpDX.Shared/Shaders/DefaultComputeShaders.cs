#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public static class DefaultComputeShaders
    {
        /// <summary>
        /// 
        /// </summary>
        public static byte[] CSParticleInsert
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.csParticleInsert;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] CSParticleUpdate
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.csParticleUpdate;
#else
                throw new NotImplementedException();
#endif
            }
        }
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
