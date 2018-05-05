/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Utilities;
    using Render;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Shader Pass
    /// </summary>
    public sealed class ShaderPass : DisposeObject
    {
        public static readonly ShaderPass NullPass = new ShaderPass();
        /// <summary>
        /// <see cref="ShaderPass.Name"/>
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsNULL { get; } = false;

        private readonly ShaderBase[] shaders = new ShaderBase[Constants.NumShaderStages];
        /// <summary>
        /// <see cref="ShaderPass.Shaders"/>
        /// </summary>
        public IReadOnlyList<ShaderBase> Shaders { get { return shaders; } }
        /// <summary>
        /// <see cref="ShaderPass.BlendState"/>
        /// </summary>
        public BlendStateProxy BlendState { private set; get; } = null;
        /// <summary>
        /// <see cref="ShaderPass.DepthStencilState"/>
        /// </summary>
        public DepthStencilStateProxy DepthStencilState { private set; get; } = null;
        /// <summary>
        /// <see cref="ShaderPass.RasterState"/>
        /// </summary>
        public RasterizerStateProxy RasterState { private set; get; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passDescription"></param>
        /// <param name="manager"></param>
        public ShaderPass(ShaderPassDescription passDescription, IEffectsManager manager)
        {
            Name = passDescription.Name;

            if (passDescription.ShaderList != null)
            {
                foreach (var shader in passDescription.ShaderList)
                {
                    shaders[shader.ShaderType.ToIndex()] = manager.ShaderManager.RegisterShader(shader);
                }
            }
            for(int i=0; i<shaders.Length; ++i)
            {
                if (shaders[i] == null)
                {
                    var type = i.ToShaderStage();
                    switch (type)
                    {
                        case ShaderStage.Vertex:
                            shaders[i] = NullShader.VertexNull;
                            break;
                        case ShaderStage.Hull:
                            shaders[i] = NullShader.HullNull;
                            break;
                        case ShaderStage.Domain:
                            shaders[i] = NullShader.DomainNull;
                            break;
                        case ShaderStage.Geometry:
                            shaders[i] = NullShader.GeometryNull;
                            break;
                        case ShaderStage.Pixel:
                            shaders[i] = NullShader.PixelNull;
                            break;
                        case ShaderStage.Compute:
                            shaders[i] = NullShader.ComputeNull;
                            break;
                    }
                }
            }

            BlendState = passDescription.BlendStateDescription != null ? 
                Collect(manager.StateManager.Register((BlendStateDescription)passDescription.BlendStateDescription)) : BlendStateProxy.Empty;

            DepthStencilState = passDescription.DepthStencilStateDescription != null ?
                Collect(manager.StateManager.Register((DepthStencilStateDescription)passDescription.DepthStencilStateDescription)) : DepthStencilStateProxy.Empty;

            RasterState = passDescription.RasterStateDescription != null ?
                Collect(manager.StateManager.Register((RasterizerStateDescription)passDescription.RasterStateDescription)) : RasterizerStateProxy.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderPass"/> class.
        /// </summary>
        private ShaderPass()
        {
            IsNULL = true;
            for (int i = 0; i < shaders.Length; ++i)
            {
                if (shaders[i] == null)
                {
                    var type = i.ToShaderStage();
                    switch (type)
                    {
                        case ShaderStage.Vertex:
                            shaders[i] = NullShader.VertexNull;
                            break;
                        case ShaderStage.Hull:
                            shaders[i] = NullShader.HullNull;
                            break;
                        case ShaderStage.Domain:
                            shaders[i] = NullShader.DomainNull;
                            break;
                        case ShaderStage.Geometry:
                            shaders[i] = NullShader.GeometryNull;
                            break;
                        case ShaderStage.Pixel:
                            shaders[i] = NullShader.PixelNull;
                            break;
                        case ShaderStage.Compute:
                            shaders[i] = NullShader.ComputeNull;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Bind shaders and its constant buffer for this technique
        /// </summary>
        /// <param name="context"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindShader(DeviceContextProxy context)
        {
            BindShader(context, true);
        }

        /// <summary>
        /// Bind shaders and its constant buffer for this technique
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindShader(DeviceContextProxy context, bool bindConstantBuffer)
        {
            if (context.LastShaderPass == this || IsNULL)
            {
                return;
            }
            for (int i = 0; i < shaders.Length; ++i)
            {
                shaders[i].Bind(context.DeviceContext);
                if (bindConstantBuffer)
                {
                    shaders[i].BindConstantBuffers(context.DeviceContext);
                }
            }
            context.LastShaderPass = this;
        }
        /// <summary>
        /// <see cref="ShaderPass.GetShader(ShaderStage)"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderBase GetShader(ShaderStage type)
        {
            return shaders[type.ToIndex()];
        }

        /// <summary>
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>        
        public void SetShader(ShaderBase shader)
        {
            shaders[shader.ShaderType.ToIndex()] = shader;
        }

        /// <summary>
        /// Binds the states.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="type">The type.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindStates(DeviceContextProxy context, StateType type)
        {
            if (type == StateType.None || IsNULL)
            {
                return;
            }
            if (EnumHelper.HasFlag(type, StateType.BlendState))
            {
                context.SetBlendState(BlendState);
            }
            if (EnumHelper.HasFlag(type, StateType.DepthStencilState))
            {
                context.SetDepthStencilState(DepthStencilState);
            }
            if (EnumHelper.HasFlag(type, StateType.RasterState))
            {
                context.SetRasterState(RasterState);
            }
        }
    }
}
