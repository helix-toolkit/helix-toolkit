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
            public static readonly ShaderDescription CSParticleInsert = new ShaderDescription(nameof(CSParticleInsert), ShaderStage.Compute, new ShaderReflector(),
                DefaultComputeShaders.CSParticleInsert);
            /// <summary>
            /// 
            /// </summary>
            public static readonly ShaderDescription CSParticleUpdate = new ShaderDescription(nameof(CSParticleUpdate), ShaderStage.Compute, new ShaderReflector(),
                DefaultComputeShaders.CSParticleUpdate);
        }
    }

}
