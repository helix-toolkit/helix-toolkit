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
        public static string GSPoint
        {
            get;
        } = "gsPoint";
        /// <summary>
        /// 
        /// </summary>
        public static string GSLine
        {
            get;
        } = "gsLine";
        /// <summary>
        /// 
        /// </summary>
        public static string GSBillboard
        {
            get;
        } = "gsBillboard";

        /// <summary>
        /// 
        /// </summary>
        public static string GSParticle
        {
            get;
        } = "gsParticle";
        /// <summary>
        /// Gets the gs mesh normal vector.
        /// </summary>
        /// <value>
        /// The gs mesh normal vector.
        /// </value>
        public static string GSMeshNormalVector
        {
            get;
        } = "gsMeshNormalVector";
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
        /// <summary>
        /// The gs mesh normal vector
        /// </summary>
        public static ShaderDescription GSMeshNormalVector = new ShaderDescription(nameof(GSMeshNormalVector), ShaderStage.Geometry, new ShaderReflector(),
            DefaultGSShaderByteCodes.GSMeshNormalVector);
    }
}
