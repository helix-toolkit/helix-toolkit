/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

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
        
        using Render;
        using Shaders;
        using System.Runtime.CompilerServices;
        using Utilities;

        /// <summary>
        ///
        /// </summary>
        public class PostEffectBlurCore : DisposeObject
        {
            #region Variables
            private const int NumPingPongBlurBuffer = 2;
            private ShaderPass screenBlurPassVertical;
            private ShaderPass screenBlurPassHorizontal;
            private readonly int textureSlot;
            private readonly int samplerSlot;

            #region Texture Resources
            private readonly ShaderResourceViewProxy[] renderTargetBlur = new ShaderResourceViewProxy[NumPingPongBlurBuffer];
            private readonly SamplerStateProxy sampler;
            #endregion Texture Resources
            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="PostEffectMeshOutlineBlurCore"/> class.
            /// </summary>
            public PostEffectBlurCore(ShaderPass blurVerticalPass, ShaderPass blurHorizontalPass, int textureSlot, int samplerSlot,
                SamplerStateDescription sampler, IEffectsManager manager)
            {
                screenBlurPassVertical = blurVerticalPass;
                screenBlurPassHorizontal = blurHorizontalPass;
                this.textureSlot = textureSlot;
                this.samplerSlot = samplerSlot;
                this.sampler = Collect(manager.StateManager.Register(sampler));
            }

            /// <summary>
            /// Runs the specified device context.
            /// </summary>
            /// <param name="deviceContext">The device context.</param>
            /// <param name="iteration">The iteration.</param>
            /// <param name="initVerticalIter">The initialize vertical iter.</param>
            /// <param name="initHorizontalIter">The initialize horizontal iter.</param>
            /// <param name="height"></param>
            /// <param name="width"></param>
            /// <param name="source"></param>
            /// <param name="target"></param>
            /// <returns>Current target with image blurred</returns>
            public virtual ShaderResourceViewProxy Run(DeviceContextProxy deviceContext, 
                ShaderResourceViewProxy source, ShaderResourceViewProxy target, int width, int height,
                int iteration, int initVerticalIter = 0, int initHorizontalIter = 0)
            {
                renderTargetBlur[0] = source;
                renderTargetBlur[1] = target;
                deviceContext.SetSampler(PixelShader.Type, samplerSlot, sampler);
                if (!screenBlurPassVertical.IsNULL)
                {
                    screenBlurPassVertical.BindShader(deviceContext);
                    screenBlurPassVertical.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
                    for (int i = initVerticalIter; i < iteration; ++i)
                    {
                        SwapTargets();
                        deviceContext.SetRenderTargetNoClear(renderTargetBlur[0], width, height);
                        screenBlurPassVertical.PixelShader.BindTexture(deviceContext, textureSlot, renderTargetBlur[1]);
                        deviceContext.Draw(4, 0);
                    }
                }

                if (!screenBlurPassHorizontal.IsNULL)
                {
                    screenBlurPassHorizontal.BindShader(deviceContext);
                    screenBlurPassHorizontal.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
                    for (int i = initHorizontalIter; i < iteration; ++i)
                    {
                        SwapTargets();
                        deviceContext.SetRenderTargetNoClear(renderTargetBlur[0], width, height);
                        screenBlurPassHorizontal.PixelShader.BindTexture(deviceContext, textureSlot, renderTargetBlur[1]);
                        deviceContext.Draw(4, 0);
                    }
                }
                deviceContext.SetShaderResource(PixelShader.Type, textureSlot, null);
                return renderTargetBlur[0];
            }
            /// <summary>
            /// Swaps the targets.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SwapTargets()
            {
                //swap buffer
                var current = renderTargetBlur[0];
                renderTargetBlur[0] = renderTargetBlur[1];
                renderTargetBlur[1] = current;
            }

            /// <summary>
            /// Clears the targets.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="c">The c.</param>
            public void ClearTargets(DeviceContextProxy context, Color c)
            {
                foreach (var target in renderTargetBlur)
                {
                    context.ClearRenderTargetView(target, c);
                }
            }
        }
    }

}