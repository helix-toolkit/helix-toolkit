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
    public sealed class NullShaderPass : IShaderPass
    {
        public static readonly NullShaderPass NullPass = new NullShaderPass();

        public bool IsNULL { get; } = true;

        public BlendStateProxy BlendState
        {
            get
            {
                return null;
            }
        }

        public DepthStencilStateProxy DepthStencilState
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

        public RasterizerStateProxy RasterState
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

        public void BindShader(IDeviceContext context)
        {
            
        }

        public void BindStates(IDeviceContext context, StateType type)
        {

        }

        public void Dispose()
        {
        }

        public IShader GetShader(ShaderStage type)
        {
            switch (type)
            {
                case ShaderStage.Pixel:
                    return NullShader.PixelNull;
                case ShaderStage.Vertex:
                    return NullShader.VertexNull;
                case ShaderStage.Geometry:
                    return NullShader.GeometryNull;
                case ShaderStage.Compute:
                    return NullShader.ComputeNull;
                case ShaderStage.Domain:
                    return NullShader.DomainNull;
                case ShaderStage.Hull:
                    return NullShader.HullNull;
                default:
                    return new NullShader(type);
            }            
        }
    }

    /// <summary>
    /// Shader Pass
    /// </summary>
    public sealed class ShaderPass : DisposeObject, IShaderPass
    {
        /// <summary>
        /// <see cref="IShaderPass.Name"/>
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsNULL { get; } = false;
        /// <summary>
        /// 
        /// </summary>
        public const int VertexIdx = 0, HullIdx = 1, DomainIdx = 2, GeometryIdx = 3, PixelIdx = 4, ComputeIdx = 5;
        private readonly IShader[] shaders = new IShader[6];
        /// <summary>
        /// <see cref="IShaderPass.Shaders"/>
        /// </summary>
        public IEnumerable<IShader> Shaders { get { return shaders; } }
        /// <summary>
        /// <see cref="IShaderPass.BlendState"/>
        /// </summary>
        public BlendStateProxy BlendState { private set; get; } = null;
        /// <summary>
        /// <see cref="IShaderPass.DepthStencilState"/>
        /// </summary>
        public DepthStencilStateProxy DepthStencilState { private set; get; } = null;
        /// <summary>
        /// <see cref="IShaderPass.RasterState"/>
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
                    shaders[GetShaderArrayIndex(shader.ShaderType)] = manager.ShaderManager.RegisterShader(shader);
                }
            }
            for(int i=0; i<shaders.Length; ++i)
            {
                if (shaders[i] == null)
                {
                    var type = GetShaderStageByArrayIndex(i);
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

            BlendState = passDescription.BlendStateDescription != null ? Collect(new BlendStateProxy(manager.StateManager, ((BlendStateDescription)passDescription.BlendStateDescription))) : null;

            DepthStencilState = passDescription.DepthStencilStateDescription != null ? Collect(new DepthStencilStateProxy(manager.StateManager, ((DepthStencilStateDescription)passDescription.DepthStencilStateDescription))) : null;

            RasterState = passDescription.RasterStateDescription != null ? Collect(new RasterizerStateProxy(manager.StateManager, ((RasterizerStateDescription)passDescription.RasterStateDescription))) : null;
        }

        /// <summary>
        /// Convert shader stage to internal array index
        /// </summary>
        /// <param name="stage"></param>
        /// <returns></returns>
        public static int GetShaderArrayIndex(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    return VertexIdx;
                case ShaderStage.Domain:
                    return DomainIdx;
                case ShaderStage.Hull:
                    return HullIdx;
                case ShaderStage.Geometry:
                    return GeometryIdx;
                case ShaderStage.Pixel:
                    return PixelIdx;
                case ShaderStage.Compute:
                    return ComputeIdx;
                default:
                    return -1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrayIndex"></param>
        /// <returns></returns>
        public static ShaderStage GetShaderStageByArrayIndex(int arrayIndex)
        {
            switch (arrayIndex)
            {
                case VertexIdx:
                    return ShaderStage.Vertex;
                case DomainIdx:
                    return ShaderStage.Domain;
                case HullIdx:
                    return ShaderStage.Hull;
                case GeometryIdx:
                    return ShaderStage.Geometry;
                case PixelIdx:
                    return ShaderStage.Pixel;
                case ComputeIdx:
                    return ShaderStage.Compute;
                default:
                    return ShaderStage.None;
            }
        }

        /// <summary>
        /// Bind shaders and its constant buffer for this technique
        /// </summary>
        /// <param name="context"></param>
        public void BindShader(IDeviceContext context)
        {
            if (context.LastShaderPass == this)
            {
                return;
            }
            foreach (var shader in Shaders)
            {
                shader.Bind(context.DeviceContext);
                shader.BindConstantBuffers(context.DeviceContext);
            }
            context.LastShaderPass = this;
        }
        /// <summary>
        /// <see cref="IShaderPass.GetShader(ShaderStage)"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IShader GetShader(ShaderStage type)
        {
            return shaders[GetShaderArrayIndex(type)];
        }

        /// <summary>
        /// Binds the states.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="type">The type.</param>
        public void BindStates(IDeviceContext context, StateType type)
        {
            if (type == StateType.None)
            {
                return;
            }
            if (type.HasFlag(StateType.BlendState))
            {
                context.DeviceContext.OutputMerger.BlendState = BlendState;
            }
            if (type.HasFlag(StateType.DepthStencilState))
            {
                context.DeviceContext.OutputMerger.DepthStencilState = DepthStencilState;
            }
            if (type.HasFlag(StateType.RasterState))
            {
                context.DeviceContext.Rasterizer.State = RasterState;
            }
        }
    }
}
