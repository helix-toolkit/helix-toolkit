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
    /// <summary>
    /// 
    /// </summary>
    public static class DefaultPSShaderByteCodes
    {
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSMeshBinnPhong
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psMeshBlinnPhong;
#else
                throw new NotImplementedException();
#endif

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSMeshVertColor
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psColor;
#else
                throw new NotImplementedException();
#endif

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSMeshVertPosition
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psPositions;
#else
                throw new NotImplementedException();
#endif

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSMeshNormal
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psNormals;
#else
                throw new NotImplementedException();
#endif

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSPoint
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psPoint;
#else
                throw new NotImplementedException();
#endif

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSLine
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psLine;
#else
                throw new NotImplementedException();
#endif

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSBillboardText
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psBillboardText;
#else
                throw new NotImplementedException();
#endif

            }
        }

        public static byte[] PSMeshXRay
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psMeshXRay;
#else
                throw new NotImplementedException();
#endif
            }
        }

        public static byte[] PSMeshClipPlane
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psMeshClipPlane;
#else
                throw new NotImplementedException();
#endif
            }
        }

        public static byte[] PSMeshClipPlaneBackface
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psMeshClipPlaneBackface;
#else
                throw new NotImplementedException();
#endif
            }
        }

        public static byte[] PSMeshClipPlaneQuad
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psMeshClipPlaneQuad;
#else
                throw new NotImplementedException();
#endif
            }
        }
    }


    /// <summary>
    /// Default Pixel Shaders
    /// </summary>
    public static class DefaultPSShaderDescriptions
    {
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshBlinnPhong = new ShaderDescription(nameof(PSMeshBlinnPhong), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshBinnPhong);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshVertColor = new ShaderDescription(nameof(PSMeshVertColor), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshVertColor);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshVertNormal = new ShaderDescription(nameof(PSMeshVertNormal), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshNormal);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshVertPosition = new ShaderDescription(nameof(PSMeshVertPosition), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshVertPosition);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSPoint = new ShaderDescription(nameof(PSPoint), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSPoint);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSLine = new ShaderDescription(nameof(PSLine), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSLine);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSBillboardText = new ShaderDescription(nameof(PSBillboardText), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSBillboardText);

        public static ShaderDescription PSMeshXRay = new ShaderDescription(nameof(PSMeshXRay), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshXRay);

        #region Mesh Clipping
        public static ShaderDescription PSMeshClipPlane = new ShaderDescription(nameof(PSMeshClipPlane), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshClipPlane);

        public static ShaderDescription PSMeshClipBackface = new ShaderDescription(nameof(PSMeshClipBackface), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshClipPlaneBackface);

        public static ShaderDescription PSMeshClipScreenQuad = new ShaderDescription(nameof(PSMeshClipScreenQuad), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshClipPlaneQuad);
        #endregion
    }
}
