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
    namespace Render
    {
        /// <summary>
        /// 
        /// </summary>
        public sealed class DX11RenderHostConfiguration
        {
            /// <summary>
            /// The render d2d
            /// </summary>
            public bool RenderD2D = true;
            /// <summary>
            /// The update global variable
            /// </summary>
            public bool UpdatePerFrameData = true;
            /// <summary>
            /// Gets or sets a value indicating whether [render lights].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [render lights]; otherwise, <c>false</c>.
            /// </value>
            public bool RenderLights = true;
            /// <summary>
            /// Gets or sets a value indicating whether [clear render target before each frame].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [clear render target before each frame]; otherwise, <c>false</c>.
            /// </value>
            public bool ClearEachFrame = true;

            /// <summary>
            /// Auto update octree in geometry during rendering. 
            /// </summary>
            public bool AutoUpdateOctree = false;
            /// <summary>
            /// Gets or sets a value indicating whether [enable oit rendering].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable oit rendering]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableOITRendering = true;
            /// <summary>
            /// Gets or sets the OIT weight power used for color weight calculation. Default = 3.
            /// </summary>
            /// <value>
            /// The OIT weight power.
            /// </value>
            public float OITWeightPower = 3;

            /// <summary>
            /// Gets or sets the oit weight depth slope. Used to increase resolution for particular range of depth values. 
            /// <para>If value = 2, the depth range from 0-0.5 expands to 0-1 to increase resolution. However, values from 0.5 - 1 will be pushed to 1</para>
            /// </summary>
            /// <value>
            /// The oit weight depth slope.
            /// </value>
            public float OITWeightDepthSlope = 1;
            /// <summary>
            /// Gets or sets the oit weight mode.
            /// <para>Please refer to http://jcgt.org/published/0002/02/09/ </para>
            /// <para>Linear0: eq7; Linear1: eq8; Linear2: eq9; NonLinear: eq10</para>
            /// </summary>
            /// <value>
            /// The oit weight mode.
            /// </value>
            public OITWeightMode OITWeightMode = OITWeightMode.Linear1;
            /// <summary>
            /// Enable FXAA. If MSAA used, FXAA will be disabled automatically
            /// </summary>
            public FXAALevel FXAALevel = FXAALevel.None;

            /// <summary>
            /// Gets or sets a value indicating whether [enable render order] specified by user.
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable render order]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableRenderOrder = false;
            /// <summary>
            /// Gets or sets a value indicating whether [enable vertical synchronize].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable v synchronize]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableVSync = true;
            /// <summary>
            /// Gets or sets a value indicating whether [enable SSAO].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable SSAO]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableSSAO = false;

            /// <summary>
            /// The SSAO sampling radius
            /// </summary>
            public float SSAORadius = 0.5f;
            /// <summary>
            /// The ssao bias
            /// </summary>
            public float SSAOBias = 1e-3f;
            /// <summary>
            /// The ssao intensity
            /// </summary>
            public float SSAOIntensity = 1f;
            /// <summary>
            /// The ssao quality
            /// </summary>
            public SSAOQuality SSAOQuality = SSAOQuality.High;

            /// <summary>
            /// The update count. Used to render at least N frames for each InvalidateRenderer. 
            /// D3DImage sometimes not getting refresh if only render once.
            /// Default = 6.
            /// </summary>
            public uint MinimumUpdateCount = 6;
        }
    }
}
