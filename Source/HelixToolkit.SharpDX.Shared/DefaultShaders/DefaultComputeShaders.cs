#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
using HelixToolkit.UWP.Helper;

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
                return UWPShaderBytePool.Read("csParticleInsert");
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
                return UWPShaderBytePool.Read("csParticleUpdate");
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
