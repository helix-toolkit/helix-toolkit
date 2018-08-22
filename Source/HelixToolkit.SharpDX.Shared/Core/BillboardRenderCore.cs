/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Utilities;
    using Render;
    using Components;
    /// <summary>
    /// 
    /// </summary>
    public class BillboardRenderCore : GeometryRenderCore<PointLineModelStruct>, IBillboardRenderParams
    {
        #region Private Variables
        private IBillboardBufferModel billboardBuffer;
        private int textureSamplerSlot;
        private int shaderTextureSlot;
        private SamplerStateProxy textureSampler;
        private readonly ConstantBufferComponent modelCB;
        #endregion
        private bool fixedSize = true;
        /// <summary>
        /// Gets or sets a value indicating whether [fixed size].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixed size]; otherwise, <c>false</c>.
        /// </value>
        public bool FixedSize
        {
            set
            {
                SetAffectsRender(ref fixedSize, value);
            }
            get { return fixedSize; }
        }

        private SamplerStateDescription samplerDescription = DefaultSamplers.LinearSamplerClampAni1;
        /// <summary>
        /// Billboard texture sampler description
        /// </summary>
        public SamplerStateDescription SamplerDescription
        {
            set
            {
                if(SetAffectsRender(ref samplerDescription, value) && IsAttached)
                {
                    RemoveAndDispose(ref textureSampler);
                    textureSampler = Collect(EffectTechnique.EffectsManager.StateManager.Register(value));
                }
            }
            get
            {
                return samplerDescription;
            }
        }
        /// <summary>
        /// Set texture variable name insider shader for binding
        /// </summary>
        public string ShaderTextureName { set; get; } = DefaultBufferNames.BillboardTB;
        /// <summary>
        /// Set texture sampler variable name inside shader for binding
        /// </summary>
        public string ShaderTextureSamplerName { set; get; } = DefaultSamplerStateNames.BillboardTextureSampler;

        private string transparentPassName = DefaultPassNames.OITPass; 
        /// <summary>
        /// Gets or sets the name of the mesh transparent pass.
        /// </summary>
        /// <value>
        /// The name of the transparent pass.
        /// </value>
        public string TransparentPassName
        {
            set
            {
                if (SetAffectsRender(ref transparentPassName, value) && IsAttached)
                {
                    TransparentPass = EffectTechnique[value];
                }
            }
            get
            {
                return transparentPassName;
            }
        }

        protected ShaderPass TransparentPass { private set; get; } = ShaderPass.NullPass;

        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardRenderCore"/> class.
        /// </summary>
        public BillboardRenderCore()
        {
            modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB, PointLineModelStruct.SizeInBytes)));
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                textureSampler = Collect(technique.EffectsManager.StateManager.Register(SamplerDescription));
                TransparentPass = technique.GetPass(TransparentPassName);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDefaultPassChanged(ShaderPass pass)
        {
            base.OnDefaultPassChanged(pass);
            shaderTextureSlot = pass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderTextureName);
            textureSamplerSlot = pass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderTextureSamplerName);
        }
        protected override void OnDetach()
        {
            textureSampler = null;
            base.OnDetach();
        }

        protected override void OnGeometryBufferChanged(IAttachableBufferModel buffer)
        {
            billboardBuffer = buffer as IBillboardBufferModel;
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            ShaderPass pass = DefaultShaderPass;
            if (RenderType == RenderType.Transparent && context.IsOITPass)
            {
                pass = TransparentPass;
            }
            modelCB.Upload(deviceContext, ref modelStruct);
            pass.BindShader(deviceContext);
            pass.BindStates(deviceContext, DefaultStateBinding);
            BindBillboardTexture(deviceContext, pass.PixelShader);
            DrawPoints(deviceContext, GeometryBuffer.VertexBuffer[0], InstanceBuffer);
        }

        protected override void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {

        }

        protected override void OnRenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
            BindBillboardTexture(deviceContext, DefaultShaderPass.PixelShader);
            DrawPoints(deviceContext, GeometryBuffer.VertexBuffer[0], InstanceBuffer);
        }

        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, RenderContext context)
        {
            model.World = ModelMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
            model.BoolParams.X = FixedSize;
            var type = billboardBuffer.Type;
            model.Params.X = (int)type;
        }

        private void BindBillboardTexture(DeviceContextProxy context, PixelShader shader)
        {
            var buffer = billboardBuffer;
            shader.BindTexture(context, shaderTextureSlot, buffer.TextureView);
            shader.BindSampler(context, textureSamplerSlot, textureSampler);
        }
    }
}
