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
    using global::SharpDX.Direct3D11;
    using HelixToolkit.Wpf.SharpDX.Utilities;
    using Render;
    using Shaders;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public interface IPostEffectMeshXRay : IPostEffect
    {
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        Color4 Color { set; get; }
        /// <summary>
        /// Gets or sets the outline fading factor.
        /// </summary>
        /// <value>
        /// The outline fading factor.
        /// </value>
        float OutlineFadingFactor { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [double pass]. Double pass uses stencil buffer to reduce overlapping artifacts
        /// </summary>
        /// <value>
        ///   <c>true</c> if [double pass]; otherwise, <c>false</c>.
        /// </value>
        bool DoublePass { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class PostEffectMeshXRayCore : RenderCoreBase<BorderEffectStruct>, IPostEffectMeshXRay
    {
        /// <summary>
        /// Gets or sets the name of the effect.
        /// </summary>
        /// <value>
        /// The name of the effect.
        /// </value>
        public string EffectName
        {
            set; get;
        } = DefaultRenderTechniqueNames.PostEffectMeshXRay;
        /// <summary>
        /// Outline color
        /// </summary>
        public Color4 Color
        {
            set
            {
                SetAffectsRender(ref modelStruct.Color, value);
            }
            get
            {
                return modelStruct.Color;
            }
        }

        /// <summary>
        /// Outline fading
        /// </summary>
        public float OutlineFadingFactor
        {
            set
            {
                SetAffectsRender(ref modelStruct.Param.X, value);
            }
            get { return modelStruct.Param.X; }
        }

        private bool doublePass = false;
        /// <summary>
        /// Gets or sets a value indicating whether [double pass]. Double pass uses stencil buffer to reduce overlapping artifacts
        /// </summary>
        /// <value>
        ///   <c>true</c> if [double pass]; otherwise, <c>false</c>.
        /// </value>
        public bool DoublePass
        {
            set
            {
                SetAffectsRender(ref doublePass, value);
            }
            get
            {
                return doublePass;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostEffectMeshXRayCore"/> class.
        /// </summary>
        public PostEffectMeshXRayCore() : base(RenderType.PostProc)
        {
            Color = global::SharpDX.Color.Blue;
        }

        private readonly List<IRenderCore> currentCores = new List<IRenderCore>();

        /// <summary>
        /// Gets the model constant buffer description.
        /// </summary>
        /// <returns></returns>
        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes);
        }

        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            DepthStencilView dsView;
            var renderTargets = deviceContext.DeviceContext.OutputMerger.GetRenderTargets(1, out dsView);
            if (dsView == null)
            {
                return;
            }
            deviceContext.DeviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Stencil, 0, 0);
            dsView.Dispose();
            foreach (var t in renderTargets)
            { t.Dispose(); }
            context.IsCustomPass = true;
            if (DoublePass)
            {
                currentCores.Clear();
                foreach (var mesh in context.RenderHost.PerFrameGeneralCoresWithPostEffect)
                {
                    if (mesh.HasPostEffect(EffectName))
                    {
                        currentCores.Add(mesh);
                        context.CustomPassName = DefaultPassNames.EffectMeshXRayP1;
                        var pass = mesh.EffectTechnique[DefaultPassNames.EffectMeshXRayP1];
                        if (pass.IsNULL) { continue; }
                        pass.BindShader(deviceContext);
                        pass.BindStates(deviceContext, StateType.BlendState);
                        deviceContext.DeviceContext.OutputMerger.SetDepthStencilState(pass.DepthStencilState, 0);
                        mesh.Render(context, deviceContext);
                    }
                }
                foreach (var mesh in currentCores)
                {
                    context.CustomPassName = DefaultPassNames.EffectMeshXRayP2;
                    var pass = mesh.EffectTechnique[DefaultPassNames.EffectMeshXRayP2];
                    if (pass.IsNULL) { continue; }
                    pass.BindShader(deviceContext);
                    pass.BindStates(deviceContext, StateType.BlendState);
                    deviceContext.DeviceContext.OutputMerger.SetDepthStencilState(pass.DepthStencilState, 1);
                    mesh.Render(context, deviceContext);
                }
                currentCores.Clear();
            }
            else
            {
                foreach (var mesh in context.RenderHost.PerFrameGeneralCoresWithPostEffect)
                {
                    if (mesh.HasPostEffect(EffectName))
                    {
                        context.CustomPassName = DefaultPassNames.EffectMeshXRayP2;
                        var pass = mesh.EffectTechnique[DefaultPassNames.EffectMeshXRayP2];
                        if (pass.IsNULL) { continue; }
                        pass.BindShader(deviceContext);
                        pass.BindStates(deviceContext, StateType.BlendState);
                        deviceContext.DeviceContext.OutputMerger.SetDepthStencilState(pass.DepthStencilState, 0);
                        mesh.Render(context, deviceContext);
                    }
                }
            }
            context.IsCustomPass = false;
        }

        protected override void OnUpdatePerModelStruct(ref BorderEffectStruct model, IRenderContext context)
        {
            
        }
    }
}
