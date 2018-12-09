/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core
    {
        using Utilities;
        using Render;
        using Shaders;
        using Model;
        using Components;

        /// <summary>
        /// 
        /// </summary>
        public interface IPostEffectOutlineBlur : IPostEffect
        {
            /// <summary>
            /// Gets or sets the color of the border.
            /// </summary>
            /// <value>
            /// The color of the border.
            /// </value>
            Color4 Color { set; get; }
            /// <summary>
            /// Gets or sets the scale x.
            /// </summary>
            /// <value>
            /// The scale x.
            /// </value>
            float ScaleX { set; get; }
            /// <summary>
            /// Gets or sets the scale y.
            /// </summary>
            /// <value>
            /// The scale y.
            /// </value>
            float ScaleY { set; get; }
            /// <summary>
            /// Gets or sets the number of blur pass.
            /// </summary>
            /// <value>
            /// The number of blur pass.
            /// </value>
            int NumberOfBlurPass { set; get; }
        }

        /// <summary>
        /// Outline blur effect
        /// <para>Must not put in shared model across multiple viewport, otherwise may causes performance issue if each viewport sizes are different.</para>
        /// </summary>
        public class PostEffectMeshOutlineBlurCore : RenderCore, IPostEffectOutlineBlur
        {
            #region Variables
            private SamplerStateProxy sampler;
            private PostEffectBlurCore blurCore;
            private ShaderPass screenQuadPass;

            private ShaderPass blurPassVertical;

            private ShaderPass blurPassHorizontal;

            private ShaderPass smoothPass;

            private ShaderPass screenOutlinePass;

            private int textureSlot;

            private int samplerSlot;

            private readonly ConstantBufferComponent modelCB;
            private BorderEffectStruct modelStruct;
            private static readonly OffScreenTextureSize TextureSize = OffScreenTextureSize.Full;
            private static readonly Color4 Transparent = new Color4(0, 0, 0, 0);

            private readonly bool useBlurCore = true;
            #endregion
            #region Properties
            private string effectName = DefaultRenderTechniqueNames.PostEffectMeshOutlineBlur;
            /// <summary>
            /// Gets or sets the name of the effect.
            /// </summary>
            /// <value>
            /// The name of the effect.
            /// </value>
            public string EffectName
            {
                set
                {
                    SetAffectsCanRenderFlag(ref effectName, value);
                }
                get
                {
                    return effectName;
                }
            }

            private Color4 color = global::SharpDX.Color.Red;
            /// <summary>
            /// Gets or sets the color of the border.
            /// </summary>
            /// <value>
            /// The color of the border.
            /// </value>
            public Color4 Color
            {
                set
                {
                    SetAffectsRender(ref color, value);
                }
                get { return color; }
            }

            private float scaleX = 1;
            /// <summary>
            /// Gets or sets the scale x.
            /// </summary>
            /// <value>
            /// The scale x.
            /// </value>
            public float ScaleX
            {
                set
                {
                    SetAffectsRender(ref scaleX, value);
                }
                get { return scaleX; }
            }

            private float scaleY = 1;
            /// <summary>
            /// Gets or sets the scale y.
            /// </summary>
            /// <value>
            /// The scale y.
            /// </value>
            public float ScaleY
            {
                set
                {
                    SetAffectsRender(ref scaleY, value);
                }
                get { return scaleY; }
            }

            private int numberOfBlurPass = 1;
            /// <summary>
            /// Gets or sets the number of blur pass.
            /// </summary>
            /// <value>
            /// The number of blur pass.
            /// </value>
            public int NumberOfBlurPass
            {
                set
                {
                    SetAffectsRender(ref numberOfBlurPass, value);
                }
                get { return numberOfBlurPass; }
            }

            private OutlineMode drawMode = OutlineMode.Merged;
        
            public OutlineMode DrawMode
            {
                set
                {
                    SetAffectsRender(ref drawMode, value);
                }
                get { return drawMode; }
            }
            #endregion        

            /// <summary>
            /// Initializes a new instance of the <see cref="PostEffectMeshOutlineBlurCore"/> class.
            /// </summary>
            public PostEffectMeshOutlineBlurCore(bool useBlurCore = true) : base(RenderType.PostProc)
            {
                this.useBlurCore = useBlurCore;
                Color = global::SharpDX.Color.Red;
                modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes)));
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {               
                screenQuadPass = technique.GetPass(DefaultPassNames.ScreenQuad);
                blurPassVertical = technique.GetPass(DefaultPassNames.EffectBlurVertical);
                blurPassHorizontal = technique.GetPass(DefaultPassNames.EffectBlurHorizontal);
                smoothPass = technique.GetPass(DefaultPassNames.EffectOutlineSmooth);
                screenOutlinePass = technique.GetPass(DefaultPassNames.MeshOutline);
                textureSlot = screenOutlinePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
                samplerSlot = screenOutlinePass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SurfaceSampler);
                sampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni1));
                if (useBlurCore)
                {
                    blurCore = Collect(new PostEffectBlurCore(blurPassVertical,
                        blurPassHorizontal, textureSlot, samplerSlot, DefaultSamplers.LinearSamplerClampAni1, technique.EffectsManager));
                }
                return true;
            }

            protected override bool OnUpdateCanRenderFlag()
            {
                return IsAttached && !string.IsNullOrEmpty(EffectName);
            }

            public override void Render(RenderContext context, DeviceContextProxy deviceContext)
            {
                int width = 0;
                int height = 0;
                using (var depthStencilBuffer = context.GetOffScreenDS(TextureSize, global::SharpDX.DXGI.Format.D32_Float_S8X24_UInt, out width, out height))
                {
                    using (var renderTargetBuffer = context.GetOffScreenRT(TextureSize, global::SharpDX.DXGI.Format.R8G8B8A8_UNorm))
                    {
                        OnUpdatePerModelStruct(context);
                        if (drawMode == OutlineMode.Separated)
                        {
                            for (int i = 0; i < context.RenderHost.PerFrameNodesWithPostEffect.Count; ++i)
                            {                    
                                #region Render objects onto offscreen texture
                                var mesh = context.RenderHost.PerFrameNodesWithPostEffect[i];
                                deviceContext.SetRenderTarget(depthStencilBuffer, renderTargetBuffer, width, height, true,
                                    global::SharpDX.Color.Transparent, true, DepthStencilClearFlags.Stencil, 0, 0);
                                modelCB.Upload(deviceContext, ref modelStruct);
                                if (mesh.TryGetPostEffect(EffectName, out IEffectAttributes effect))
                                {
                                    var color = Color;
                                    if (effect.TryGetAttribute(EffectAttributeNames.ColorAttributeName, out object attribute) && attribute is string colorStr)
                                    {
                                        color = colorStr.ToColor4();
                                    }
                                    if (modelStruct.Color != color)
                                    {
                                        modelStruct.Color = color;
                                        modelCB.Upload(deviceContext, ref modelStruct);
                                    }
                                    context.CustomPassName = DefaultPassNames.EffectOutlineP1;
                                    var pass = mesh.EffectTechnique[DefaultPassNames.EffectOutlineP1];
                                    if (pass.IsNULL) { continue; }
                                    pass.BindShader(deviceContext);
                                    pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                                    mesh.RenderCustom(context, deviceContext);
                                    DrawOutline(context, deviceContext, depthStencilBuffer, renderTargetBuffer, width, height);
                                }                             
                                #endregion   
                            }
                        }
                        else
                        {
                            deviceContext.SetRenderTarget(depthStencilBuffer, renderTargetBuffer, width, height, true,
                                    Transparent, true, DepthStencilClearFlags.Stencil, 0, 0);
                            #region Render objects onto offscreen texture
                            bool hasMesh = false;
                            for (int i = 0; i < context.RenderHost.PerFrameNodesWithPostEffect.Count; ++i)
                            {
                                var mesh = context.RenderHost.PerFrameNodesWithPostEffect[i];
                                if (mesh.TryGetPostEffect(EffectName, out IEffectAttributes effect))
                                {
                                    var color = Color;
                                    if (effect.TryGetAttribute(EffectAttributeNames.ColorAttributeName, out object attribute) && attribute is string colorStr)
                                    {
                                        color = colorStr.ToColor4();
                                    }
                                    if (modelStruct.Color != color)
                                    {
                                        modelStruct.Color = color;
                                        modelCB.Upload(deviceContext, ref modelStruct);
                                    }
                                    context.CustomPassName = DefaultPassNames.EffectOutlineP1;
                                    var pass = mesh.EffectTechnique[DefaultPassNames.EffectOutlineP1];
                                    if (pass.IsNULL) { continue; }
                                    pass.BindShader(deviceContext);
                                    pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                                    mesh.RenderCustom(context, deviceContext);
                                    hasMesh = true;
                                }
                            }
                            #endregion
                            if (hasMesh)
                            {
                                DrawOutline(context, deviceContext, depthStencilBuffer, renderTargetBuffer, width, height);
                            }
                        }
                    }
            

                }
            }

            private void DrawOutline(RenderContext context, DeviceContextProxy deviceContext, 
                ShaderResourceViewProxy depthStencilBuffer, ShaderResourceViewProxy source, int width, int height)
            {
                var buffer = context.RenderHost.RenderBuffer;

                #region Do Blur Pass
                if (useBlurCore)
                {
                    for(int i = 0; i < numberOfBlurPass; ++i)
                    {
                        blurCore.Run(context, deviceContext, source, width, height, PostEffectBlurCore.BlurDepth.One, ref modelStruct);
                    }
                }
                else
                {
                    blurPassHorizontal.PixelShader.BindSampler(deviceContext, samplerSlot, sampler);
                    for (int i = 0; i < numberOfBlurPass; ++i)
                    {
                        deviceContext.SetRenderTarget(context.RenderHost.RenderBuffer.FullResPPBuffer.NextRTV, width, height);
                        blurPassHorizontal.PixelShader.BindTexture(deviceContext, textureSlot, source);                       
                        blurPassHorizontal.BindShader(deviceContext);
                        blurPassHorizontal.BindStates(deviceContext, StateType.All);
                        deviceContext.Draw(4, 0);

                        deviceContext.SetRenderTarget(source, width, height);
                        blurPassVertical.PixelShader.BindTexture(deviceContext, textureSlot, context.RenderHost.RenderBuffer.FullResPPBuffer.NextRTV);
                        blurPassVertical.BindShader(deviceContext);
                        blurPassVertical.BindStates(deviceContext, StateType.All);
                        deviceContext.Draw(4, 0);
                    }
                }

                #region Draw back with stencil test
                deviceContext.SetRenderTarget(depthStencilBuffer, context.RenderHost.RenderBuffer.FullResPPBuffer.NextRTV, width, height,
                    true, global::SharpDX.Color.Transparent, false);
                screenQuadPass.PixelShader.BindTexture(deviceContext, textureSlot, source);
                screenQuadPass.BindShader(deviceContext);
                screenQuadPass.BindStates(deviceContext, StateType.All);
                deviceContext.Draw(4, 0);
                #endregion

                #region Draw outline onto original target
                deviceContext.SetRenderTarget(buffer.FullResPPBuffer.CurrentRTV, buffer.TargetWidth, buffer.TargetHeight);
                screenOutlinePass.PixelShader.BindTexture(deviceContext, textureSlot, context.RenderHost.RenderBuffer.FullResPPBuffer.NextRTV);
                screenOutlinePass.BindShader(deviceContext);
                screenOutlinePass.BindStates(deviceContext, StateType.All);
                deviceContext.Draw(4, 0);
                screenOutlinePass.PixelShader.BindTexture(deviceContext, textureSlot, null);
                #endregion
                #endregion
            }

            protected override void OnDetach()
            {
                blurCore = null;
                sampler = null;
                base.OnDetach();
            }

            private void OnUpdatePerModelStruct(RenderContext context)
            {
                modelStruct.Param.M11 = scaleX;
                modelStruct.Param.M12 = ScaleY;
                modelStruct.Color = color;
                modelStruct.ViewportScale = (int)TextureSize;
            }
        }
    }

}
