/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Runtime.CompilerServices;
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{    
    using Render;
    using Utilities;
    using Mathematics;
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
        /// <summary>
        /// Gets or sets the blend factor.
        /// </summary>
        /// <value>
        /// The blend factor.
        /// </value>
        public Color4 BlendFactor { private set; get; } = Color4.White;
        /// <summary>
        /// Gets or sets the sample mask.
        /// </summary>
        /// <value>
        /// The sample mask.
        /// </value>
        public int SampleMask { private set; get; } = -1;
        /// <summary>
        /// Gets or sets the stencil reference.
        /// </summary>
        /// <value>
        /// The stencil reference.
        /// </value>
        public int StencilRef { private set; get; } = 0;
        /// <summary>
        /// <see cref="ShaderPass.DepthStencilState"/>
        /// </summary>
        public DepthStencilStateProxy DepthStencilState { private set; get; } = null;
        /// <summary>
        /// <see cref="ShaderPass.RasterState"/>
        /// </summary>
        public RasterizerStateProxy RasterState { private set; get; } = null;
        /// <summary>
        /// Gets or sets the input layout. This is customized layout used for this ShaderPass only.
        /// If this is not set, default is using <see cref="Technique.Layout"/> from <see cref="TechniqueDescription.InputLayoutDescription"/>
        /// </summary>
        /// <value>
        /// The input layout.
        /// </value>
        public InputLayout Layout { private set; get; } = null;
        /// <summary>
        /// Gets or sets the topology.
        /// </summary>
        /// <value>
        /// The topology.
        /// </value>
        public global::SharpDX.Direct3D.PrimitiveTopology Topology
        {
            set; get;
        } = global::SharpDX.Direct3D.PrimitiveTopology.Undefined;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="passDescription"></param>
        /// <param name="layout"></param>
        /// <param name="manager"></param>
        public ShaderPass(ShaderPassDescription passDescription, InputLayout layout, IEffectsManager manager)
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

            StencilRef = passDescription.StencilRef;

            SampleMask = passDescription.SampleMask;

            Topology = passDescription.Topology;
            if(passDescription.InputLayoutDescription != null)
            {
                Layout = manager.ShaderManager.RegisterInputLayout(passDescription.InputLayoutDescription);
            }
            else
            {
                Layout = layout;
            }
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
            context.SetShaderPass(this, bindConstantBuffer);
            if(Layout != null)
            {
                context.InputLayout = Layout;
            }
        }

        #region Get Shaders
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
        /// Gets the shader.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VertexShader GetShader(VertexShaderType type)
        {
            return VertexShader;
        }
        /// <summary>
        /// Gets the shader.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HullShader GetShader(HullShaderType type)
        {
            return HullShader;
        }
        /// <summary>
        /// Gets the shader.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DomainShader GetShader(DomainShaderType type)
        {
            return DomainShader;
        }
        /// <summary>
        /// Gets the shader.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GeometryShader GetShader(GeometryShaderType type)
        {
            return GeometryShader;
        }
        /// <summary>
        /// Gets the shader.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PixelShader GetShader(PixelShaderType type)
        {
            return PixelShader;
        }
        /// <summary>
        /// Gets the shader.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComputeShader GetShader(ComputeShaderType type)
        {
            return ComputeShader;
        }
        #endregion

        #region Set Shaders
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
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public void SetShader(VertexShader shader)
        {
            VertexShader = shader;
        }
        /// <summary>
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public void SetShader(HullShader shader)
        {
            HullShader = shader;
        }
        /// <summary>
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public void SetShader(DomainShader shader)
        {
            DomainShader = shader;
        }
        /// <summary>
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public void SetShader(GeometryShader shader)
        {
            GeometryShader = shader;
        }
        /// <summary>
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public void SetShader(PixelShader shader)
        {
            PixelShader = shader;
        }
        /// <summary>
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public void SetShader(ComputeShader shader)
        {
            ComputeShader = shader;
        }
        #endregion

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
                context.SetDepthStencilState(DepthStencilState, StencilRef);
            }
            if (EnumHelper.HasFlag(type, StateType.RasterState))
            {
                context.SetRasterState(RasterState);
            }
        }
    }
}
