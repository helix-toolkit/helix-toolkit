/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Components;
    using Utilities;
    using System;

    public class AxisPlaneGridCore : RenderCoreBase<PlaneGridModelStruct>
    {
        private ShaderPass DefaultShaderPass;
        private SamplerStateProxy shadowSampler;
        private int samplerSlot;
        private int shadowMapSlot;
        private Vector3 upDirection = Vector3.UnitY;
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
                        modelStruct.GridSpacing = GridSpacing;
                    }
                }
            }
            get { return autoSpacing; }
        }

        private float autoSpacingChangeRate = 5;

        /// <summary>
        /// Gets or sets the automatic spacing rate. Default is 5 for perspective camera. If using orthographic camera, increase the rate value to for example > 15.
        /// </summary>
        /// <value>
        /// The automatic spacing rate.
        /// </value>
        public float AutoSpacingRate
        {
            set { SetAffectsRender(ref autoSpacingChangeRate, value); }
            get { return autoSpacingChangeRate; }
        }
        /// <summary>
        /// Gets the acutal spacing.
        /// </summary>
        /// <value>
        /// The acutal spacing.
        /// </value>
        public float AcutalSpacing
        {
            get { return modelStruct.GridSpacing; }
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
                    modelStruct.GridSpacing = value;
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
                SetAffectsRender(ref modelStruct.GridThickenss, value);
            }
            get
            {
                return modelStruct.GridThickenss;
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
                SetAffectsRender(ref modelStruct.FadingFactor, value);
            }
            get { return modelStruct.FadingFactor; }
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
            set { SetAffectsRender(ref modelStruct.HasShadowMap, value); }
            get { return modelStruct.HasShadowMap; }
        }

        private Axis upAxis = Axis.Y;
        /// <summary>
        /// Gets or sets Up Axis. Default is Y
        /// </summary>
        /// <value>
        /// The plane.
        /// </value>
        public Axis UpAxis
        {
            set
            {
                if (SetAffectsRender(ref upAxis, value))
                {
                    modelStruct.Axis = (int)value;
                    switch (value)
                    {
                        case Axis.X:
                            upDirection = Vector3.UnitX;
                            break;
                        case Axis.Y:
                            upDirection = Vector3.UnitY;
                            break;
                        case Axis.Z:
                            upDirection = Vector3.UnitZ;
                            break;
                    }
                }
            }
            get { return upAxis; }
        }

        /// <summary>
        /// Gets or sets the axis plane offset.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        public float Offset
        {
            set { SetAffectsRender(ref modelStruct.PlaneD, value); }
            get { return modelStruct.PlaneD; }
        }

        private GridPattern gridType = GridPattern.Tile;
        /// <summary>
        /// Gets or sets the grid pattern.
        /// </summary>
        /// <value>
        /// The grid pattern.
        /// </value>
        public GridPattern GridPattern
        {
            set {
                if (SetAffectsRender(ref gridType, value))
                {
                    modelStruct.Type = (int)value;
                }
            }
            get { return gridType; }
        }

        private readonly ConstantBufferComponent modelCB;
        /// <summary>
        /// Initializes a new instance of the <see cref="AxisPlaneGridCore"/> class.
        /// </summary>
        public AxisPlaneGridCore() : base(RenderType.Particle)
        {
            modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.PlaneGridModelCB, PlaneGridModelStruct.SizeInBytes)));
            modelStruct = new PlaneGridModelStruct()
            {
                World = Matrix.Identity,
                Axis = 1,
            };
            GridSpacing = 10;
            GridThickness = 0.05f;
            FadingFactor = 0.2f;
            PlaneColor = Color.Gray;
            GridColor = Color.DarkGray;
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            DefaultShaderPass = technique[DefaultPassNames.Default];
            samplerSlot = DefaultShaderPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.ShadowMapSampler);
            shadowMapSlot = DefaultShaderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.ShadowMapTB);
            shadowSampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.ShadowSampler));
            return true;
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            modelCB.Upload(deviceContext, ref modelStruct);
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
            if (autoSpacing)
            {                
                //Disable auto spacing if view angle larger than 60 degree of plane normal
                var lookDir = Vector3.Normalize(context.Camera.LookDirection);
                var angle = Math.Acos(Math.Abs(Vector3.Dot(upDirection, lookDir)));
                if (angle > Math.PI / 3)
                {
                    return;
                }
                var r = new Ray(context.Camera.Position, Vector3.Normalize(context.Camera.LookDirection));
                var plane = new Plane(upDirection, model.PlaneD);
                if (r.Intersects(ref plane, out float l))
                {
                    l /= autoSpacingChangeRate;
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
                    model.GridSpacing = n;
                }
            }
        }

        public override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }
    }
}
