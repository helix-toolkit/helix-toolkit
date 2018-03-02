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

        public static byte[] PSMeshDiffuseMap
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psDiffuseMap;
#else
                throw new NotImplementedException();
#endif

            }
        }

        public static byte[] PSMeshViewCube
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psViewCube;
#else
                throw new NotImplementedException();
#endif

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSShadow
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psShadow;
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
        public static byte[] PSLineColor
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psLineColor;
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSParticle
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psParticle;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSSkybox
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psSkybox;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSMeshWireframe
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psWireframe;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSDepthStencilTestOnly
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psDepthStencilOnly;
#else
                throw new NotImplementedException();
#endif
            }
        }
        /// <summary>
        /// Gets the ps mesh outline screen quad.
        /// </summary>
        /// <value>
        /// The ps mesh outline screen quad.
        /// </value>
        public static byte[] PSEffectOutlineScreenQuad
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectOutlineQuad;
#else
                throw new NotImplementedException();
#endif
            }
        }
        /// <summary>
        /// Gets the ps effect full screen blur vertical.
        /// </summary>
        /// <value>
        /// The ps effect full screen blur vertical.
        /// </value>
        public static byte[] PSEffectFullScreenBlurVertical
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectGaussianBlurVertical;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// Gets the ps effect full screen blur horizontal.
        /// </summary>
        /// <value>
        /// The ps effect full screen blur horizontal.
        /// </value>
        public static byte[] PSEffectFullScreenBlurHorizontal
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectGaussianBlurHorizontal;
#else
                throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// Gets the ps mesh border highlight
        /// </summary>
        /// <value>
        /// The ps mesh mesh border highlight
        /// </value>
        public static byte[] PSEffectMeshBorderHighlight
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectMeshBorderHighlight;
#else
                throw new NotImplementedException();
#endif
            }
        }
        /// <summary>
        /// Gets the ps mesh outline screen quad stencil.
        /// </summary>
        /// <value>
        /// The ps mesh outline screen quad stencil.
        /// </value>
        public static byte[] PSEffectOutlineScreenQuadStencil
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectOutlineQuadStencil;
#else
                throw new NotImplementedException();
#endif
            }
        }
        /// <summary>
        /// Gets the ps mesh outline quad final.
        /// </summary>
        /// <value>
        /// The ps mesh outline quad final.
        /// </value>
        public static byte[] PSEffectOutlineQuadFinal
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectOutlineQualFinal;
#else
                throw new NotImplementedException();
#endif
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public static byte[] PSEffectMeshXRay
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectMeshXRay;
#else
                throw new NotImplementedException();
#endif
            }
        }
        /// <summary>
        /// Gets the ps effect bloom extract.
        /// </summary>
        /// <value>
        /// The ps effect bloom extract.
        /// </value>
        public static byte[] PSEffectBloomExtract
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectBloomExtract;
#else
                throw new NotImplementedException();
#endif
            }
        }
        /// <summary>
        /// Gets the ps effect bloom vertical blur.
        /// </summary>
        /// <value>
        /// The ps effect bloom vertical blur.
        /// </value>
        public static byte[] PSEffectBloomVerticalBlur
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectBloomBlurVertical;
#else
                throw new NotImplementedException();
#endif
            }
        }
        /// <summary>
        /// Gets the ps effect bloom horizontal blur.
        /// </summary>
        /// <value>
        /// The ps effect bloom horizontal blur.
        /// </value>
        public static byte[] PSEffectBloomHorizontalBlur
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectBloomBlurHorizontal;
#else
                throw new NotImplementedException();
#endif
            }
        }
        /// <summary>
        /// Gets the ps effect bloom combine.
        /// </summary>
        /// <value>
        /// The ps effect bloom combine.
        /// </value>
        public static byte[] PSEffectBloomCombine
        {
            get
            {
#if !NETFX_CORE
                return Properties.Resources.psEffectBloomCombine;
#else
                throw new NotImplementedException();
#endif
            }
        }
#if !NETFX_CORE
        /// <summary>
        /// 
        /// </summary>
        public static byte[] PSScreenDup
        {
            get
            {

                return Properties.Resources.psScreenDup;
            }
        }
#endif
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
        public static ShaderDescription PSMeshDiffuseMap = new ShaderDescription(nameof(PSMeshDiffuseMap), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshDiffuseMap);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshViewCube = new ShaderDescription(nameof(PSMeshViewCube), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshViewCube);
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
        public static ShaderDescription PSLineColor = new ShaderDescription(nameof(PSLineColor), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSLineColor);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSBillboardText = new ShaderDescription(nameof(PSBillboardText), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSBillboardText);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshXRay = new ShaderDescription(nameof(PSMeshXRay), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshXRay);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSShadow = new ShaderDescription(nameof(PSShadow), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSShadow);
        #region Mesh Clipping
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshClipPlane = new ShaderDescription(nameof(PSMeshClipPlane), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshClipPlane);
        /// <summary>
        /// /
        /// </summary>
        public static ShaderDescription PSMeshClipBackface = new ShaderDescription(nameof(PSMeshClipBackface), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshClipPlaneBackface);
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshClipScreenQuad = new ShaderDescription(nameof(PSMeshClipScreenQuad), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshClipPlaneQuad);
        #endregion
        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSParticle = new ShaderDescription(nameof(PSParticle), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSParticle);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSSkybox = new ShaderDescription(nameof(PSSkybox), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSSkybox);

        /// <summary>
        /// 
        /// </summary>
        public static ShaderDescription PSMeshWireframe = new ShaderDescription(nameof(PSMeshWireframe), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSMeshWireframe);

        /// <summary>
        /// The ps depth stencil only
        /// </summary>
        public static ShaderDescription PSDepthStencilOnly = new ShaderDescription(nameof(PSDepthStencilOnly), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSDepthStencilTestOnly);

        /// <summary>
        ///
        /// </summary>
        public static ShaderDescription PSMeshOutlineScreenQuad = new ShaderDescription(nameof(PSMeshOutlineScreenQuad), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectOutlineScreenQuad);

        /// <summary>
        ///
        /// </summary>
        public static ShaderDescription PSEffectFullScreenBlurVertical = new ShaderDescription(nameof(PSEffectFullScreenBlurVertical), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectFullScreenBlurVertical);
        /// <summary>
        ///
        /// </summary>
        public static ShaderDescription PSEffectFullScreenBlurHorizontal = new ShaderDescription(nameof(PSEffectFullScreenBlurHorizontal), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectFullScreenBlurHorizontal);
        /// <summary>
        ///
        /// </summary>
        public static ShaderDescription PSEffectMeshBorderHighlight = new ShaderDescription(nameof(PSEffectMeshBorderHighlight), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectMeshBorderHighlight);
        /// <summary>
        ///
        /// </summary>
        public static ShaderDescription PSMeshOutlineQuadStencil = new ShaderDescription(nameof(PSMeshOutlineQuadStencil), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectOutlineScreenQuadStencil);

        /// <summary>
        ///
        /// </summary>
        public static ShaderDescription PSMeshOutlineQuadFinal = new ShaderDescription(nameof(PSMeshOutlineQuadFinal), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectOutlineQuadFinal);

        /// <summary>
        ///
        /// </summary>
        public static ShaderDescription PSEffectMeshXRay = new ShaderDescription(nameof(PSEffectMeshXRay), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectMeshXRay);

        /// <summary>
        ///
        /// </summary>
        public static ShaderDescription PSEffectBloomExtract = new ShaderDescription(nameof(PSEffectBloomExtract), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectBloomExtract);
        /// <summary>
        /// The ps effect bloom vertical blur
        /// </summary>
        public static ShaderDescription PSEffectBloomVerticalBlur = new ShaderDescription(nameof(PSEffectBloomVerticalBlur), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectBloomVerticalBlur);
        /// <summary>
        /// The ps effect bloom horizontal blur
        /// </summary>
        public static ShaderDescription PSEffectBloomHorizontalBlur = new ShaderDescription(nameof(PSEffectBloomHorizontalBlur), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectBloomHorizontalBlur);

        /// <summary>
        /// The ps effect bloom combine
        /// </summary>
        public static ShaderDescription PSEffectBloomCombine = new ShaderDescription(nameof(PSEffectBloomCombine), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSEffectBloomCombine);
#if !NETFX_CORE
        /// <summary>
        /// The ps screen dup
        /// </summary>
        public static ShaderDescription PSScreenDup = new ShaderDescription(nameof(PSScreenDup), ShaderStage.Pixel, new ShaderReflector(),
            DefaultPSShaderByteCodes.PSScreenDup);
#endif
    }
}
