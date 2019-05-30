/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
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
    namespace Model
    {
        using Render;
        using Shaders;
        /// <summary>
        /// 
        /// </summary>
        public sealed class LineMaterialVariable : MaterialVariable
        {
            private readonly LineMaterialCore material;

            public ShaderPass LinePass { get; }
            public ShaderPass ShadowPass { get; }
            public ShaderPass DepthPass { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="LineMaterialVariable"/> class.
            /// </summary>
            /// <param name="manager">The manager.</param>
            /// <param name="technique">The technique.</param>
            /// <param name="materialCore">The material core.</param>
            /// <param name="linePassName">Name of the line pass.</param>
            /// <param name="shadowPassName">Name of the shadow pass.</param>
            /// <param name="depthPassName">Name of the depth pass</param>
            public LineMaterialVariable(IEffectsManager manager, IRenderTechnique technique, LineMaterialCore materialCore,
                string linePassName = DefaultPassNames.Default, string shadowPassName = DefaultPassNames.ShadowPass,
                string depthPassName = DefaultPassNames.DepthPrepass) 
                : base(manager, technique, DefaultPointLineConstantBufferDesc, materialCore)
            {
                LinePass = technique[linePassName];
                ShadowPass = technique[shadowPassName];
                DepthPass = technique[depthPassName];
                this.material = materialCore;
            }

            protected override void OnInitialPropertyBindings()
            {
                AddPropertyBinding(nameof(LineMaterialCore.LineColor), () => { WriteValue(PointLineMaterialStruct.ColorStr, material.LineColor); });
                AddPropertyBinding(nameof(LineMaterialCore.Thickness), () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector2(material.Thickness, material.Smoothness)); });
                AddPropertyBinding(nameof(LineMaterialCore.Smoothness), () => { WriteValue(PointLineMaterialStruct.ParamsStr, new Vector2(material.Thickness, material.Smoothness)); });
                AddPropertyBinding(nameof(LineMaterialCore.EnableDistanceFading), () => { WriteValue(PointLineMaterialStruct.EnableDistanceFading, material.EnableDistanceFading ? 1 : 0); });
                AddPropertyBinding(nameof(LineMaterialCore.FadingNearDistance), () => { WriteValue(PointLineMaterialStruct.FadeNearDistance, material.FadingNearDistance); });
                AddPropertyBinding(nameof(LineMaterialCore.FadingFarDistance), () => { WriteValue(PointLineMaterialStruct.FadeFarDistance, material.FadingFarDistance); });
                AddPropertyBinding(nameof(LineMaterialCore.FixedSize), () => { WriteValue(PointLineMaterialStruct.FixedSize, material.FixedSize); });
            }

            public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
            {
                DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
            }

            public override ShaderPass GetPass(RenderType renderType, RenderContext context)
            {
                return LinePass;
            }

            public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
            {
                return ShadowPass;
            }

            public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
            {
                return ShaderPass.NullPass;
            }

            public override ShaderPass GetDepthPass(RenderType renderType, RenderContext context)
            {
                return DepthPass;
            }

            public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
            {
                return true;
            }
        }
    }

}
