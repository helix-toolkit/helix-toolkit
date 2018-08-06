/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Utilities;

    public class PlaneGridCore : RenderCoreBase<PlaneGridModelStruct>
    {
        private ShaderPass DefaultShaderPass;
        private SamplerStateProxy shadowSampler;
        private int samplerSlot;
        private int shadowMapSlot;

        private bool autoSpacing = true;
        /// <summary>
        /// Gets or sets a value indicating whether [automatic spacing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic spacing]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoSpacing
        {
            set
            {
                if (SetAffectsRender(ref autoSpacing, value))
                {
                    if (!value)
                    {
                        modelStruct.Params.X = GridSpacing;
                    }
                }
            }
            get { return autoSpacing; }
        }
        /// <summary>
        /// Gets the acutal spacing.
        /// </summary>
        /// <value>
        /// The acutal spacing.
        /// </value>
        public float AcutalSpacing
        {
            get { return modelStruct.Params.X; }
        }

        private float gridSpacing;
        /// <summary>
        /// Gets or sets the grid spacing.
        /// </summary>
        /// <value>
        /// The grid spacing.
        /// </value>
        public float GridSpacing
        {
            set
            {
                if(SetAffectsRender(ref gridSpacing, value))
                {
                    modelStruct.Params.X = value;
                }
            }
            get
            {
                return gridSpacing;
            }
        }
        /// <summary>
        /// Gets or sets the grid thickness.
        /// </summary>
        /// <value>
        /// The grid thickness.
        /// </value>
        public float GridThickness
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Y, value);
            }
            get
            {
                return modelStruct.Params.Y;
            }
        }
        /// <summary>
        /// Gets or sets the fading factor.
        /// </summary>
        /// <value>
        /// The fading factor.
        /// </value>
        public float FadingFactor
        {
            set
            {
                SetAffectsRender(ref modelStruct.Params.Z, value);
            }
            get { return modelStruct.Params.Z; }
        }
        /// <summary>
        /// Gets or sets the color of the plane.
        /// </summary>
        /// <value>
        /// The color of the plane.
        /// </value>
        public Color4 PlaneColor
        {
            set
            {
                SetAffectsRender(ref modelStruct.PlaneColor, value);
            }
            get { return modelStruct.PlaneColor.ToColor4(); }
        }
        /// <summary>
        /// Gets or sets the color of the grid.
        /// </summary>
        /// <value>
        /// The color of the grid.
        /// </value>
        public Color4 GridColor
        {
            set { SetAffectsRender(ref modelStruct.GridColor, value); }
            get { return modelStruct.GridColor.ToColor4(); }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [render shadow map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render shadow map]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderShadowMap
        {
            set { SetAffectsRender(ref modelStruct.hasShadowMap, value); }
            get { return modelStruct.hasShadowMap; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaneGridCore"/> class.
        /// </summary>
        public PlaneGridCore() : base(RenderType.Opaque)
        {
            modelStruct = new PlaneGridModelStruct()
            {
                World = Matrix.Identity,                
            };
            GridSpacing = 10;
            GridThickness = 0.05f;
            FadingFactor = 0.2f;
            PlaneColor = Color.Gray;
            GridColor = new Color4(0.2f, 0.2f, 0.2f, 1);
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                DefaultShaderPass = technique[DefaultPassNames.Default];
                samplerSlot = DefaultShaderPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.ShadowMapSampler);
                shadowMapSlot = DefaultShaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.ShadowMapTB);
                shadowSampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.ShadowSampler));
                return true;
            }
            else { return false; }
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.PlaneGridModelCB, PlaneGridModelStruct.SizeInBytes);
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            DefaultShaderPass.BindShader(deviceContext);
            DefaultShaderPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
            if(RenderShadowMap && context.SharedResource.ShadowView != null)
            {
                DefaultShaderPass.PixelShader.BindTexture(deviceContext, shadowMapSlot, context.SharedResource.ShadowView);
                DefaultShaderPass.PixelShader.BindSampler(deviceContext, samplerSlot, shadowSampler);
            }
            deviceContext.Draw(4, 0);
        }

        protected override void OnUpdatePerModelStruct(ref PlaneGridModelStruct model, RenderContext context)
        {
            model.World = ModelMatrix;
            if (autoSpacing)
            {
                var p = new Plane(Vector3.UnitY, 0);
                var r = new Ray(context.Camera.Position, Vector3.Normalize(context.Camera.LookDirection));
                if (r.Intersects(ref p, out float l))
                {
                    l /= 5;
                    int n = 1;
                    while (n < 1e6)
                    {
                        if(n > l)
                        {
                            n /= 10;
                            break;
                        }
                        n *= 10;
                    }
                    model.Params.X = n;
                }
            }
            model.Params.Y = GridThickness;
        }

        public override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }
    }
}
