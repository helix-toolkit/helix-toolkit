/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public static class DefaultDomainShaders
    {        
        /// <summary>
        /// 
        /// </summary>
        public static byte[] DSMeshTessellation
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.dsMeshTriTessellation;
#else
                throw new NotImplementedException();
#endif
            }
        }
    }

    public static class DefaultDomainShaderDescriptions
    {
        public static ShaderDescription DSMeshTessellation = new ShaderDescription(nameof(DSMeshTessellation), ShaderStage.Domain, FeatureLevel.Level_11_0,
            DefaultDomainShaders.DSMeshTessellation,
            new ConstantBufferMapping[]
            {
                DefaultConstantBufferDescriptions.GlobalTransformCB.CreateMapping(0),
                DefaultConstantBufferDescriptions.ModelCB.CreateMapping(1),
                DefaultConstantBufferDescriptions.MaterialCB.CreateMapping(3)
            },
            null);
    }
}
