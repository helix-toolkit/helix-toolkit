/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
using HelixToolkit.UWP.Render;
using HelixToolkit.UWP.Shaders;
using HelixToolkit.UWP.Utilities;
using SharpDX;

namespace HelixToolkit.UWP.Model
#endif
{
    public sealed class PointMaterialVariable : MaterialVariable
    {        
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public float Width
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.X, value);
            }
            get { return modelStruct.Params.X; }
        }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Y, value);
            }
            get { return modelStruct.Params.Y; }
        }
        /// <summary>
        /// Gets or sets the figure.
        /// </summary>
        /// <value>
        /// The figure.
        /// </value>
        public PointFigure Figure
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Z, (int)value);
            }
            get { return (PointFigure)modelStruct.Params.Z; }
        }
        /// <summary>
        /// Gets or sets the figure ratio.
        /// </summary>
        /// <value>
        /// The figure ratio.
        /// </value>
        public float FigureRatio
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.W, value);
            }
            get { return modelStruct.Params.W; }
        }
        /// <summary>
        /// Final Point Color = PointColor * PerVertexPointColor
        /// </summary>
        public Color4 PointColor
        {
            set
            {
                SetAffectsRender(ref modelStruct.Color, value);
            }
            get
            {
                return modelStruct.Color.ToColor4();
            }
        }

        private PointLineModelStruct modelStruct;

        public ShaderPass PointPass { get; }
        public ShaderPass ShadowPass { get; }

        public PointMaterialVariable(IEffectsManager manager, IRenderTechnique technique) :
            base(manager, technique, DefaultPointLineConstantBufferDesc)
        {
            PointPass = technique[DefaultPassNames.Default];
            ShadowPass = technique[DefaultPassNames.ShadowPass];
        }

        public override void Draw(DeviceContextProxy deviceContext, IElementsBufferProxy indexBuffer, IElementsBufferModel instanceModel)
        {
            DrawPoints(deviceContext, indexBuffer, instanceModel);
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

        protected override bool OnBindMaterialTextures(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            return true;
        }

        protected override void UpdateInternalVariables(DeviceContextProxy deviceContext)
        {

        }

        protected override void WriteMaterialDataToConstantBuffer(DataStream cbStream)
        {
            cbStream.Write(modelStruct);
        }
    }
}
