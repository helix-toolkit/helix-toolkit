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
    using Utilities;
    using Core;

    public sealed class PointMaterialVariable : MaterialVariable
    {        
        private PointMaterialCore materialCore;

        public ShaderPass PointPass { get; }
        public ShaderPass ShadowPass { get; }

        public PointMaterialVariable(IEffectsManager manager, IRenderTechnique technique, PointMaterialCore core)
            : base(manager, technique, DefaultPointLineConstantBufferDesc)
        {
            PointPass = technique[DefaultPassNames.Default];
            ShadowPass = technique[DefaultPassNames.ShadowPass];
            materialCore = core;
            core.PropertyChanged += Core_PropertyChanged;
        }

        private void Core_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(PointMaterialCore.PointColor)))
            {
                WriteValue(PointLineMaterialStruct.ColorStr, materialCore.PointColor);
            }
            else if (e.PropertyName.Equals(nameof(PointMaterialCore.Width)) || e.PropertyName.Equals(nameof(PointMaterialCore.Height))
                || e.PropertyName.Equals(nameof(PointMaterialCore.PointColor)) || e.PropertyName.Equals(nameof(PointMaterialCore.PointColor)))
            {
                WriteValue(PointLineMaterialStruct.ParamsStr, new Vector4(materialCore.Width, materialCore.Height, (int)materialCore.Figure, materialCore.FigureRatio));
            }
            InvalidateRenderer();
        }

        protected override void OnInitializeParameters()
        {
            base.OnInitializeParameters();
            WriteValue(PointLineMaterialStruct.ColorStr, materialCore.PointColor);
            WriteValue(PointLineMaterialStruct.ParamsStr, new Vector4(materialCore.Width, materialCore.Height, (int)materialCore.Figure, materialCore.FigureRatio));
        }

        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
            DrawPoints(deviceContext, bufferModel.VertexBuffer[0].ElementCount, instanceCount);
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return PointPass;
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
