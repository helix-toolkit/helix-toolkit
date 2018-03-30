/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Render;
    using global::SharpDX.Direct3D11;
    using global::SharpDX;
    using Utilities;
    public class MeshRenderCore : MaterialGeometryRenderCore, IMeshRenderParams
    {
        /// <summary>
        /// 
        /// </summary>
        public bool InvertNormal
        {
            set
            {
                SetAffectsRender(ref modelStruct.InvertNormal, (value ? 1 : 0));
            }
            get
            {
                return modelStruct.InvertNormal == 1 ? true : false;
            }
        }
        private bool renderWireframe = false;
        /// <summary>
        /// Gets or sets a value indicating whether [render wireframe].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render wireframe]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderWireframe
        {
            set
            {
                SetAffectsRender(ref renderWireframe, value);
            }
            get
            {
                return renderWireframe;
            }
        }

        /// <summary>
        /// Gets or sets the color of the wireframe.
        /// </summary>
        /// <value>
        /// The color of the wireframe.
        /// </value>
        public Color4 WireframeColor
        {
            set
            {
                SetAffectsRender(ref modelStruct.WireframeColor, value);
            }
            get { return modelStruct.WireframeColor; }
        }

        private RasterizerStateProxy rasterStateWireframe = null;
        /// <summary>
        /// Gets the raster state wireframe.
        /// </summary>
        /// <value>
        /// The raster state wireframe.
        /// </value>
        protected RasterizerStateProxy RasterStateWireframe { get { return rasterStateWireframe; } }

        protected IShaderPass WireframePass { private set; get; }

        public string ShaderShadowMapTextureName { set; get; } = DefaultBufferNames.ShadowMapTB;

        private int shadowMapSlot;

        protected override bool CreateRasterState(RasterizerStateDescription description, bool force)
        {
            if (base.CreateRasterState(description, force))
            {
                RemoveAndDispose(ref rasterStateWireframe);
                var wireframeDesc = description;
                wireframeDesc.FillMode = FillMode.Wireframe;
                wireframeDesc.DepthBias = -100;
                wireframeDesc.SlopeScaledDepthBias = -2f;
                wireframeDesc.DepthBiasClamp = -0.00008f;
                rasterStateWireframe = Collect(EffectTechnique.EffectsManager.StateManager.Register(wireframeDesc));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                WireframePass = technique.GetPass(DefaultPassNames.Wireframe);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDefaultPassChanged(IShaderPass pass)
        {
            base.OnDefaultPassChanged(pass);
            shadowMapSlot = pass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(ShaderShadowMapTextureName);
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            DefaultShaderPass.BindShader(deviceContext);
            DefaultShaderPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
            if (!BindMaterialTextures(deviceContext, DefaultShaderPass))
            {
                return;
            }
            if (context.RenderHost.IsShadowMapEnabled)
            {
                DefaultShaderPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, shadowMapSlot, context.SharedResource.ShadowView);
            }
            OnDraw(deviceContext, InstanceBuffer);
            if (RenderWireframe && WireframePass != NullShaderPass.NullPass)
            {
                WireframePass.BindShader(deviceContext, false);
                WireframePass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                deviceContext.SetRasterState(RasterStateWireframe);
                OnDraw(deviceContext, InstanceBuffer);
            }
        }
    }
}
