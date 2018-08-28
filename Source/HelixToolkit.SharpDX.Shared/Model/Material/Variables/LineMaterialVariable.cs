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
    public sealed class LineMaterialVariable : MaterialVariable
    {
        private PointLineMaterialStruct materialStruct;
        private readonly LineMaterialCore materialCore;

        public ShaderPass LinePass { get; }
        public ShaderPass ShadowPass { get; }

        public LineMaterialVariable(IEffectsManager manager, IRenderTechnique technique, LineMaterialCore core) 
            : base(manager, technique, DefaultPointLineConstantBufferDesc)
        {
            LinePass = technique[DefaultPassNames.Default];
            ShadowPass = technique[DefaultPassNames.ShadowPass];
            materialCore = core;
            materialStruct.Color = core.LineColor;
            materialStruct.Params.X = core.Thickness;
            materialStruct.Params.Y = core.Smoothness;
            core.PropertyChanged += Core_PropertyChanged;
        }

        private void Core_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(LineMaterialCore.LineColor)))
            {
                materialStruct.Color = materialCore.LineColor;
            }
            else if (e.PropertyName.Equals(nameof(LineMaterialCore.Thickness)))
            {
                materialStruct.Params.X = materialCore.Thickness;
            }
            else if (e.PropertyName.Equals(nameof(LineMaterialCore.Smoothness)))
            {
                materialStruct.Params.Y = materialCore.Smoothness;
            }
            InvalidateRenderer();
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

        protected override void WriteMaterialDataToConstantBuffer(DataStream cbStream)
        {
            cbStream.Write(materialStruct);
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            materialCore.PropertyChanged -= Core_PropertyChanged;
            base.OnDispose(disposeManagedResources);
        }
    }
}
