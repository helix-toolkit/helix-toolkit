using SharpDX.Direct3D;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public static class DefaultHullShaders
    {
        /// <summary>
        /// 
        /// </summary>
        public static byte[] HSMeshTessellation
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.hsMeshTriTessellation;
#else
                throw new NotImplementedException();
#endif
            }
        }
    }

    public static class DefaultHullShaderDescriptions
    {
        public static ShaderDescription HSMeshTessellation = new ShaderDescription(nameof(HSMeshTessellation), ShaderStage.Hull, FeatureLevel.Level_11_0,
            DefaultHullShaders.HSMeshTessellation,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1),
            },
            null);
    }
}
