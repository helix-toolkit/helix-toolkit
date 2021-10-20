/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Runtime.CompilerServices;
using SharpDX.Direct3D11;
using global::SharpDX;

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
    namespace Shaders
    {
        using Render;
        using Utilities;

        /// <summary>
        /// Shader Pass
        /// </summary>
        public sealed class ShaderPass : DisposeObject
        {
            public static readonly ShaderPass NullPass = new ShaderPass();
            /// <summary>
            /// <see cref="ShaderPass.Name"/>
            /// </summary>
            public string Name
            {
                private set; get;
            }
            /// <summary>
            /// 
            /// </summary>
            public bool IsNULL { get; } = false;
            private VertexShader vertexShader = VertexShader.NullVertexShader;
            public VertexShader VertexShader => vertexShader;
            private DomainShader domainShader = DomainShader.NullDomainShader;
            public DomainShader DomainShader => domainShader;
            private HullShader hullShader = HullShader.NullHullShader;
            public HullShader HullShader => hullShader;
            private PixelShader pixelShader = PixelShader.NullPixelShader;
            public PixelShader PixelShader => pixelShader;
            private GeometryShader geometryShader = GeometryShader.NullGeometryShader;
            public GeometryShader GeometryShader => geometryShader;
            private ComputeShader computeShader = ComputeShader.NullComputeShader;
            public ComputeShader ComputeShader => computeShader;

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

            private BlendStateProxy blendState = BlendStateProxy.Empty;
            /// <summary>
            /// <see cref="ShaderPass.BlendState"/>
            /// </summary>
            public BlendStateProxy BlendState => blendState;

            private DepthStencilStateProxy depthStencilState = DepthStencilStateProxy.Empty;
            /// <summary>
            /// <see cref="ShaderPass.DepthStencilState"/>
            /// </summary>
            public DepthStencilStateProxy DepthStencilState => depthStencilState;

            private RasterizerStateProxy rasterState = RasterizerStateProxy.Empty;
            /// <summary>
            /// <see cref="ShaderPass.RasterState"/>
            /// </summary>
            public RasterizerStateProxy RasterState => rasterState;

            private InputLayoutProxy layout;
            /// <summary>
            /// Gets or sets the input layout. This is customized layout used for this ShaderPass only.
            /// If this is not set, default is using <see cref="Technique.Layout"/> from <see cref="TechniqueDescription.InputLayoutDescription"/>
            /// </summary>
            /// <value>
            /// The input layout.
            /// </value>
            public InputLayoutProxy Layout => layout;
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

            private readonly IEffectsManager effectsManager;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="passDescription"></param>
            /// <param name="layout"></param>
            /// <param name="manager"></param>
            public ShaderPass(ShaderPassDescription passDescription, IEffectsManager manager)
            {
                Name = passDescription.Name;
                effectsManager = manager;
                if (passDescription.ShaderList != null)
                {
                    foreach (var shader in passDescription.ShaderList)
                    {
                        var s = manager.ShaderManager.RegisterShader(shader);
                        switch (shader.ShaderType)
                        {
                            case ShaderStage.Vertex:
                                vertexShader = s as VertexShader;
                                break;
                            case ShaderStage.Domain:
                                domainShader = s as DomainShader;
                                break;
                            case ShaderStage.Hull:
                                hullShader = s as HullShader;
                                break;
                            case ShaderStage.Geometry:
                                geometryShader = s as GeometryShader;
                                break;
                            case ShaderStage.Pixel:
                                pixelShader = s as PixelShader;
                                break;
                            case ShaderStage.Compute:
                                computeShader = s as ComputeShader;
                                break;
                        }
                    }
                }

                blendState = passDescription.BlendStateDescription != null ?
                    manager.StateManager.Register((BlendStateDescription)passDescription.BlendStateDescription) : BlendStateProxy.Empty;

                depthStencilState = passDescription.DepthStencilStateDescription != null ?
                    manager.StateManager.Register((DepthStencilStateDescription)passDescription.DepthStencilStateDescription) : DepthStencilStateProxy.Empty;

                rasterState = passDescription.RasterStateDescription != null ?
                    manager.StateManager.Register((RasterizerStateDescription)passDescription.RasterStateDescription) : RasterizerStateProxy.Empty;

                BlendFactor = passDescription.BlendFactor;

                StencilRef = passDescription.StencilRef;

                SampleMask = passDescription.SampleMask;

                Topology = passDescription.Topology;
                if (passDescription.InputLayoutDescription != null)
                {
                    layout = manager.ShaderManager.RegisterInputLayout(passDescription.InputLayoutDescription);
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
                if (Layout != null)
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
                        RemoveAndDispose(ref vertexShader);
                        vertexShader = shader as VertexShader ?? VertexShader.NullVertexShader;
                        break;
                    case ShaderStage.Pixel:
                        RemoveAndDispose(ref pixelShader);
                        pixelShader = shader as PixelShader ?? PixelShader.NullPixelShader;
                        break;
                    case ShaderStage.Compute:
                        RemoveAndDispose(ref computeShader);
                        computeShader = shader as ComputeShader ?? ComputeShader.NullComputeShader;
                        break;
                    case ShaderStage.Hull:
                        RemoveAndDispose(ref hullShader);
                        hullShader = shader as HullShader ?? HullShader.NullHullShader;
                        break;
                    case ShaderStage.Domain:
                        RemoveAndDispose(ref domainShader);
                        domainShader = shader as DomainShader ?? DomainShader.NullDomainShader;
                        break;
                    case ShaderStage.Geometry:
                        RemoveAndDispose(ref geometryShader);
                        geometryShader = shader as GeometryShader ?? GeometryShader.NullGeometryShader;
                        break;
                }
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
            /// <summary>
            /// Sets the state.
            /// </summary>
            /// <param name="blendStateDesc">The blend state desc.</param>
            public void SetState(BlendStateDescription? blendStateDesc)
            {
                if (IsNULL)
                {
                    return;
                }
                if (BlendState != BlendStateProxy.Empty)
                {
                    RemoveAndDispose(ref blendState);
                }
                blendState = blendStateDesc != null ?
                    effectsManager.StateManager.Register(blendStateDesc.Value) : BlendStateProxy.Empty;
            }
            /// <summary>
            /// Sets the state.
            /// </summary>
            /// <param name="depthStencilStateDesc">The depth stencil state desc.</param>
            public void SetState(DepthStencilStateDescription? depthStencilStateDesc)
            {
                if (IsNULL)
                {
                    return;
                }
                if (DepthStencilState != DepthStencilStateProxy.Empty)
                {
                    RemoveAndDispose(ref depthStencilState);
                }
                depthStencilState = depthStencilStateDesc != null ?
                    effectsManager.StateManager.Register(depthStencilStateDesc.Value) : DepthStencilStateProxy.Empty;
            }
            /// <summary>
            /// Sets the state.
            /// </summary>
            /// <param name="rasterizerStateDesc">The rasterizer state desc.</param>
            public void SetState(RasterizerStateDescription? rasterizerStateDesc)
            {
                if (IsNULL)
                {
                    return;
                }
                if (RasterState != RasterizerStateProxy.Empty)
                {
                    RemoveAndDispose(ref rasterState);
                }
                rasterState = rasterizerStateDesc != null ?
                    effectsManager.StateManager.Register(rasterizerStateDesc.Value) : RasterizerStateProxy.Empty;
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                if (BlendState != BlendStateProxy.Empty)
                {
                    RemoveAndDispose(ref blendState);
                }
                if (DepthStencilState != DepthStencilStateProxy.Empty)
                {
                    RemoveAndDispose(ref depthStencilState);
                }
                if (RasterState != RasterizerStateProxy.Empty)
                {
                    RemoveAndDispose(ref rasterState);
                }
                RemoveAndDispose(ref layout);
                RemoveAndDispose(ref vertexShader);
                RemoveAndDispose(ref domainShader);
                RemoveAndDispose(ref hullShader);
                RemoveAndDispose(ref geometryShader);
                RemoveAndDispose(ref pixelShader);
                RemoveAndDispose(ref computeShader);
                base.OnDispose(disposeManagedResources);
            }
        }
    }
}
