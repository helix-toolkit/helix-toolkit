/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Render;
    using Shaders;
    /// <summary>
    /// 
    /// </summary>
    public sealed class LineMaterialVariable : MaterialVariable
    {
        private readonly LineMaterialCore materialCore;

        public ShaderPass LinePass { get; }
        public ShaderPass ShadowPass { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="LineMaterialVariable"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="technique">The technique.</param>
        /// <param name="core">The core.</param>
        /// <param name="linePassName">Name of the line pass.</param>
        /// <param name="shadowPassName">Name of the shadow pass.</param>
        public LineMaterialVariable(IEffectsManager manager, IRenderTechnique technique, LineMaterialCore core,
            string linePassName = DefaultPassNames.Default, string shadowPassName = DefaultPassNames.ShadowPass) 
            : base(manager, technique, DefaultPointLineConstantBufferDesc)
        {
            LinePass = technique[linePassName];
            ShadowPass = technique[shadowPassName];
            materialCore = core;
            core.PropertyChanged += Core_PropertyChanged;
        }

        private void Core_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(LineMaterialCore.LineColor)))
            {
                WriteValue(PointLineMaterialStruct.ColorStr, materialCore.LineColor);
            }
            else if (e.PropertyName.Equals(nameof(LineMaterialCore.Thickness)) || e.PropertyName.Equals(nameof(LineMaterialCore.Smoothness)))
            {
                WriteValue(PointLineMaterialStruct.ParamsStr, new Vector2(materialCore.Thickness, materialCore.Smoothness));
            }
            InvalidateRenderer();
        }

        protected override void OnInitializeParameters()
        {
            base.OnInitializeParameters();
            WriteValue(PointLineMaterialStruct.ColorStr, materialCore.LineColor);
            WriteValue(PointLineMaterialStruct.ParamsStr, new Vector2(materialCore.Thickness, materialCore.Smoothness));
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

        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            return true;
        }

        protected override void UpdateInternalVariables(DeviceContextProxy deviceContext)
        {

        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            materialCore.PropertyChanged -= Core_PropertyChanged;
            base.OnDispose(disposeManagedResources);
        }
    }
}
