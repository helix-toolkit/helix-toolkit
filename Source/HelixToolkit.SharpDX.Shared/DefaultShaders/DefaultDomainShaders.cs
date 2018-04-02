/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX.Direct3D;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
using HelixToolkit.UWP.Helper;
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
                return UWPShaderBytePool.Read("dsMeshTriTessellation");
#endif
            }
        }
    }

    public static class DefaultDomainShaderDescriptions
    {
        public static ShaderDescription DSMeshTessellation = new ShaderDescription(nameof(DSMeshTessellation), ShaderStage.Domain, new ShaderReflector(),
            DefaultDomainShaders.DSMeshTessellation);
    }
}
