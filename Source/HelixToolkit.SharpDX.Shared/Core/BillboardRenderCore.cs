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

    public class BillboardRenderCore : GeometryRenderCore<PointLineModelStruct>, IBillboardRenderParams
    {
        private bool fixedSize = true;
        public bool FixedSize
        {
            set
            {
                SetAffectsRender(ref fixedSize, value);
            }
            get { return fixedSize; }
        }
        private SamplerStateDescription samplerDescription = DefaultSamplers.LinearSamplerWrapAni4;
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

        private SamplerStateProxy textureSampler;


        private int shaderTextureSlot;
        private int textureSamplerSlot;

        protected override void OnDefaultPassChanged(ShaderPass pass)
        {
            base.OnDefaultPassChanged(pass);
            shaderTextureSlot = pass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(ShaderTextureName);
            textureSamplerSlot = pass.GetShader(ShaderStage.Pixel).SamplerMapping.TryGetBindSlot(ShaderTextureSamplerName);
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

        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, RenderContext context)
        {
            model.World = ModelMatrix * context.WorldMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
            model.BoolParams.X = FixedSize;
            var type = (GeometryBuffer as IBillboardBufferModel).Type;
            model.Params.X = (int)type;
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB, PointLineModelStruct.SizeInBytes);
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            ShaderPass pass = DefaultShaderPass;
            if (RenderType == RenderType.Transparent && context.IsOITPass)
            {
                pass = TransparentPass;
            }
            pass.BindShader(deviceContext);
            pass.BindStates(deviceContext, DefaultStateBinding);
            BindBillboardTexture(deviceContext, pass.GetShader(ShaderStage.Pixel));
            OnDraw(deviceContext, InstanceBuffer);
        }        

        protected virtual void BindBillboardTexture(DeviceContext context, ShaderBase shader)
        {
            var buffer = GeometryBuffer as IBillboardBufferModel;
            shader.BindTexture(context, shaderTextureSlot, buffer.TextureView);
            shader.BindSampler(context, textureSamplerSlot, textureSampler);
        }

        protected override void OnDraw(DeviceContext context, IElementsBufferModel instanceModel)
        {
            var billboardGeometry = GeometryBuffer.Geometry as IBillboardText;
            var vertexCount = billboardGeometry.BillboardVertices.Count;
            if (instanceModel == null || !instanceModel.HasElements)
            {
                context.Draw(vertexCount, 0);
            }
            else
            {
                context.DrawInstanced(vertexCount, instanceModel.Buffer.ElementCount, 0, 0);
            }
        }

        protected override void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
            
        }
    }
}
