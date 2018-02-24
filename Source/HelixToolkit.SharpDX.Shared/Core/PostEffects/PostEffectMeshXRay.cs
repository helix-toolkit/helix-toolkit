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

        /// <summary>
        /// Initializes a new instance of the <see cref="PostEffectMeshXRayCore"/> class.
        /// </summary>
        public PostEffectMeshXRayCore() : base(RenderType.PostProc)
        {
            Color = global::SharpDX.Color.Blue;
        }

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
            context.IsCustomPass = true;
            foreach (var mesh in context.RenderHost.PerFrameGeneralCoresWithPostEffect)
            {
                if (mesh.HasPostEffect(EffectName))
                {
                    context.CustomPassName = DefaultPassNames.EffectMeshXRay;
                    var pass = mesh.EffectTechnique[DefaultPassNames.EffectMeshXRay];
                    if (pass.IsNULL) { continue; }
                    pass.BindShader(deviceContext);
                    pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                    mesh.Render(context, deviceContext);
                }
            }
            context.IsCustomPass = false;
        }

        protected override void OnUpdatePerModelStruct(ref BorderEffectStruct model, IRenderContext context)
        {
            
        }
    }
}
