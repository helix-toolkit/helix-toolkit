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
    /// <summary>
    /// 
    /// </summary>
    public static class DefaultGSShaderByteCodes
    {
        /// <summary>
        /// 
        /// </summary>
        public static byte[] GSPoint
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.gsPoint;
#else
                return UWPShaderBytePool.Read("gsPoint");
#endif

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] GSLine
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.gsLine;
#else
                return UWPShaderBytePool.Read("gsLine");
#endif

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] GSBillboard
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.gsBillboard;
#else
                return UWPShaderBytePool.Read("gsBillboard");
#endif

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] GSParticle
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.gsParticle;
#else
                return UWPShaderBytePool.Read("gsParticle");
#endif

            }
        }
    }


    /// <summary>
    /// Default Geometry Shaders
    /// </summary>
    public static class DefaultGSShaderDescriptions
    {
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription GSPoint = new ShaderDescription(nameof(GSPoint), ShaderStage.Geometry, new ShaderReflector(),
            DefaultGSShaderByteCodes.GSPoint);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription GSLine = new ShaderDescription(nameof(GSLine), ShaderStage.Geometry, new ShaderReflector(),
            DefaultGSShaderByteCodes.GSLine);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription GSBillboard = new ShaderDescription(nameof(GSBillboard), ShaderStage.Geometry, new ShaderReflector(),
            DefaultGSShaderByteCodes.GSBillboard);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription GSParticle = new ShaderDescription(nameof(GSParticle), ShaderStage.Geometry, new ShaderReflector(),
            DefaultGSShaderByteCodes.GSParticle);
    }
}
