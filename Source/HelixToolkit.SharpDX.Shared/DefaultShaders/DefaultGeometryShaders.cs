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
    public static class DefaultGSShaderByteCodes
    {
        /// <summary>
        /// 
        /// </summary>
        public static byte[] GSPoint
        {
            get
            {
                return UWPShaderBytePool.Read("gsPoint");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] GSLine
        {
            get
            {
                return UWPShaderBytePool.Read("gsLine");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] GSBillboard
        {
            get
            {
                return UWPShaderBytePool.Read("gsBillboard");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] GSParticle
        {
            get
            {
                return UWPShaderBytePool.Read("gsParticle");
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
