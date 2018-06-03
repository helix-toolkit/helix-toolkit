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
    using global::SharpDX;

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
        public VertexShader VertexShader { private set; get; } = VertexShader.NullVertexShader;
        public DomainShader DomainShader { private set; get; } = DomainShader.NullDomainShader;
        public HullShader HullShader { private set; get; } = HullShader.NullHullShader;
        public PixelShader PixelShader { private set; get; } = PixelShader.NullPixelShader;
        public GeometryShader GeometryShader { private set; get; } = GeometryShader.NullGeometryShader;
        public ComputeShader ComputeShader { private set; get; } = ComputeShader.NullComputeShader;
        /// <summary>
        /// <see cref="ShaderPass.BlendState"/>
        /// </summary>
        public BlendStateProxy BlendState { private set; get; } = null;

        public Color4 BlendFactor { private set; get; } = Color4.White;

        public int SampleMask { private set; get; } = -1;
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
                    var s = manager.ShaderManager.RegisterShader(shader);
                    switch (shader.ShaderType)
                    {
                        case ShaderStage.Vertex:
                            VertexShader = s as VertexShader;
                            break;
                        case ShaderStage.Domain:
                            DomainShader = s as DomainShader;
                            break;
                        case ShaderStage.Hull:
                            HullShader = s as HullShader;
                            break;
                        case ShaderStage.Geometry:
                            GeometryShader = s as GeometryShader;
                            break;
                        case ShaderStage.Pixel:
                            PixelShader = s as PixelShader;
                            break;
                        case ShaderStage.Compute:
                            ComputeShader = s as ComputeShader;
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

            BlendFactor = passDescription.BlendFactor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderPass"/> class.
        /// </summary>
        private ShaderPass()
        {
            IsNULL = true;
        }

        /// <summary>
        /// Bind shaders and its constant buffer for this technique
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindShader(DeviceContextProxy context, bool bindConstantBuffer = true)
        {
            if (context.LastShaderPass == this || IsNULL)
            {
                return;
            }
            VertexShader.Bind(context, bindConstantBuffer);
            PixelShader.Bind(context);
            ComputeShader.Bind(context);
            HullShader.Bind(context);
            DomainShader.Bind(context);
            GeometryShader.Bind(context);
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
            switch (type)
            {
                case ShaderStage.Vertex:
                    return VertexShader;
                case ShaderStage.Pixel:
                    return PixelShader;
                case ShaderStage.Compute:
                    return ComputeShader;
                case ShaderStage.Hull:
                    return HullShader;
                case ShaderStage.Domain:
                    return DomainShader;
                case ShaderStage.Geometry:
                    return GeometryShader;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>        
        public void SetShader(ShaderBase shader)
        {
            switch (shader.ShaderType)
            {
                case ShaderStage.Vertex:
                    VertexShader = shader as VertexShader;
                    break;
                case ShaderStage.Pixel:
                    PixelShader = shader as PixelShader;
                    break;
                case ShaderStage.Compute:
                    ComputeShader = shader as ComputeShader;
                    break;
                case ShaderStage.Hull:
                    HullShader = shader as HullShader;
                    break;
                case ShaderStage.Domain:
                    DomainShader = shader as DomainShader;
                    break;
                case ShaderStage.Geometry:
                    GeometryShader = shader as GeometryShader;
                    break;
            }
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
                context.SetBlendState(BlendState, BlendFactor, SampleMask);
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
