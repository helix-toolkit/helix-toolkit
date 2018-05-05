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
    public class MeshRenderCore : MaterialGeometryRenderCore, IMeshRenderParams, IDynamicReflectable
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
        /// Gets or sets the dynamic reflector.
        /// </summary>
        /// <value>
        /// The dynamic reflector.
        /// </value>
        public IDynamicReflector DynamicReflector
        {
            set; get;
        }

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
                if(SetAffectsRender(ref transparentPassName, value) && IsAttached)
                {
                    TransparentPass = EffectTechnique[value];
                }
            }
            get
            {
                return transparentPassName;
            }
        }

        /// <summary>
        /// Gets the raster state wireframe.
        /// </summary>
        /// <value>
        /// The raster state wireframe.
        /// </value>
        protected RasterizerStateProxy RasterStateWireframe { get { return rasterStateWireframe; } }

        protected ShaderPass WireframePass { private set; get; } = ShaderPass.NullPass;
        protected ShaderPass WireframeOITPass { private set; get; } = ShaderPass.NullPass;
        protected ShaderPass TransparentPass { private set; get; } = ShaderPass.NullPass;

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
                WireframeOITPass = technique.GetPass(DefaultPassNames.WireframeOITPass);
                TransparentPass = technique.GetPass(TransparentPassName);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            DynamicReflector = null;
            base.OnDetach();
        }

        protected override void OnDefaultPassChanged(ShaderPass pass)
        {
            base.OnDefaultPassChanged(pass);
            shadowMapSlot = pass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(ShaderShadowMapTextureName);
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, RenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.RenderOIT = context.IsOITPass ? 1 : 0;
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
            if (!BindMaterialTextures(deviceContext, pass))
            {
                return;
            }
            if (context.RenderHost.IsShadowMapEnabled)
            {
                pass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, shadowMapSlot, context.SharedResource.ShadowView);
            }
            DynamicReflector?.BindCubeMap(deviceContext);
            OnDraw(deviceContext, InstanceBuffer);
            DynamicReflector?.UnBindCubeMap(deviceContext);
            if (RenderWireframe && WireframePass != ShaderPass.NullPass)
            {
                if (RenderType == RenderType.Transparent && context.IsOITPass)
                {
                    pass = WireframeOITPass;
                }
                else
                {
                    pass = WireframePass;
                }
                pass.BindShader(deviceContext, false);
                pass.BindStates(deviceContext, DefaultStateBinding);
                deviceContext.SetRasterState(RasterStateWireframe);
                OnDraw(deviceContext, InstanceBuffer);
            }
        }
    }
}
