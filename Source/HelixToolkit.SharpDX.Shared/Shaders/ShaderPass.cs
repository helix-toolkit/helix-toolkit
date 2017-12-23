/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public sealed class NullShaderPass : IShaderPass
    {
        public BlendState BlendState
        {
            get
            {
                return null;
            }
        }

        public DepthStencilState DepthStencilState
        {
            get
            {
                return null;
            }
        }

        public string Name
        {
            get
            {
                return "NULL";
            }
        }

        public RasterizerState RasterState
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<IShader> Shaders
        {
            get
            {
                return new IShader[0];
            }
        }

        public void BindShader(DeviceContext context)
        {
            
        }

        public void BindStates(DeviceContext context, StateType type)
        {

        }

        public IShader GetShader(ShaderStage type)
        {
            return new NullShader(type);
        }
    }

    /// <summary>
    /// Shader Pass
    /// </summary>
    public sealed class ShaderPass : IShaderPass
    {
        /// <summary>
        /// <see cref="IShaderPass.Name"/>
        /// </summary>
        public string Name { private set; get; }
        private readonly Dictionary<ShaderStage, IShader> shaders = new Dictionary<ShaderStage, IShader>();
        /// <summary>
        /// <see cref="IShaderPass.Shaders"/>
        /// </summary>
        public IEnumerable<IShader> Shaders { get { return shaders.Values; } }
        /// <summary>
        /// <see cref="IShaderPass.BlendState"/>
        /// </summary>
        public BlendState BlendState { private set; get; } = null;
        /// <summary>
        /// <see cref="IShaderPass.DepthStencilState"/>
        /// </summary>
        public DepthStencilState DepthStencilState { private set; get; } = null;
        /// <summary>
        /// <see cref="IShaderPass.RasterState"/>
        /// </summary>
        public RasterizerState RasterState { private set; get; } = null;

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
                    shaders.Add(shader.ShaderType, manager.ShaderManager.RegisterShader(shader));
                }
            }

            if (!shaders.ContainsKey(ShaderStage.Domain))
            {
                shaders.Add(ShaderStage.Domain, new NullShader(ShaderStage.Domain));
            }
            if (!shaders.ContainsKey(ShaderStage.Hull))
            {
                shaders.Add(ShaderStage.Hull, new NullShader(ShaderStage.Hull));
            }
            if (!shaders.ContainsKey(ShaderStage.Geometry))
            {
                shaders.Add(ShaderStage.Geometry, new NullShader(ShaderStage.Geometry));
            }
            if (!shaders.ContainsKey(ShaderStage.Compute))
            {
                shaders.Add(ShaderStage.Compute, new NullShader(ShaderStage.Compute));
            }
            if (!shaders.ContainsKey(ShaderStage.Vertex))
            {
                shaders.Add(ShaderStage.Vertex, new NullShader(ShaderStage.Vertex));
            }
            if (!shaders.ContainsKey(ShaderStage.Pixel))
            {
                shaders.Add(ShaderStage.Pixel, new NullShader(ShaderStage.Pixel));
            }
            BlendState = passDescription.BlendStateDescription != null ? manager.StateManager.Register((BlendStateDescription)passDescription.BlendStateDescription) : null;

            DepthStencilState = passDescription.DepthStencilStateDescription != null ? manager.StateManager.Register((DepthStencilStateDescription)passDescription.DepthStencilStateDescription) : null;

            RasterState = passDescription.RasterStateDescription != null ? manager.StateManager.Register((RasterizerStateDescription)passDescription.RasterStateDescription) : null;
        }

        /// <summary>
        /// Bind shaders and its constant buffer for this technique
        /// </summary>
        /// <param name="context"></param>
        public void BindShader(DeviceContext context)
        {
            foreach (var shader in Shaders)
            {
                shader.Bind(context);
                shader.BindConstantBuffers(context);
            }
        }
        /// <summary>
        /// <see cref="IShaderPass.GetShader(ShaderStage)"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IShader GetShader(ShaderStage type)
        {
            if (shaders.ContainsKey(type))
            {
                return shaders[type];
            }
            else
            {
                return new NullShader(type);
            }
        }

        /// <summary>
        /// <see cref="IShaderPass.BindStates(DeviceContext, StateType)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        public void BindStates(DeviceContext context, StateType type)
        {
            if (type == StateType.None)
            {
                return;
            }
            if (type.HasFlag(StateType.BlendState))
            {
                context.OutputMerger.BlendState = BlendState;
            }
            if (type.HasFlag(StateType.DepthStencilState))
            {
                context.OutputMerger.DepthStencilState = DepthStencilState;
            }
            if (type.HasFlag(StateType.RasterState))
            {
                context.Rasterizer.State = RasterState;
            }
        }
    }
}
