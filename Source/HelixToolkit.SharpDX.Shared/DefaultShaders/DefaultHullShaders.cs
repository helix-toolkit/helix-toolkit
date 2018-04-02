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
                return UWPShaderBytePool.Read("hsMeshTriTessellation");
#endif
            }
        }
    }

    public static class DefaultHullShaderDescriptions
    {
        public static ShaderDescription HSMeshTessellation = new ShaderDescription(nameof(HSMeshTessellation), ShaderStage.Hull, new ShaderReflector(),
            DefaultHullShaders.HSMeshTessellation);
    }
}
