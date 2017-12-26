/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if DEBUG
//#define OUTPUTDEBUGGING
#endif
using SharpDX;
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX.Direct3D;
    using System.Diagnostics;
    using System.IO;
    using Utilities;
    using Shaders;

    public class ParticleRenderCore : RenderCoreBase<ModelStruct>
    {
        public static readonly int DefaultParticleCount = 512;
        public static readonly float DefaultInitialVelocity = 1f;
        public static readonly Vector3 DefaultAcceleration = new Vector3(0, 0.1f, 0);
        public static readonly Vector2 DefaultParticleSize = new Vector2(1, 1);
        public static readonly Vector3 DefaultEmitterLocation = Vector3.Zero;
        public static readonly Vector3 DefaultConsumerLocation = new Vector3(0, 10, 0);
        public static readonly float DefaultConsumerGravity = 0;
        public static readonly float DefaultConsumerRadius = 0;
        public static readonly Vector3 DefaultBoundMaximum = new Vector3(5, 5, 5);
        public static readonly Vector3 DefaultBoundMinimum = new Vector3(-5, -5, -5);
        public static readonly float DefaultInitialEnergy = 5;
        public static readonly float DefaultEnergyDissipationRate = 1f;

        #region variables
        /// <summary>
        /// Texture tile columns
        /// </summary>
        public uint NumTextureColumn { set; get; } = 1;
        /// <summary>
        /// Texture tile rows
        /// </summary>
        public uint NumTextureRow { set; get; } = 1;

        /// <summary>
        /// Change Sprite based on particle energy, sequence from (1,1) to (NumTextureRow, NumTextureColumn) evenly divided by tile counts
        /// </summary>
        public bool AnimateSpriteByEnergy { set; get; } = false;

        /// <summary>
        /// Minimum time elapse to insert new particles
        /// </summary>
        public float InsertElapseThrottle { private set; get; } = 0;

        private float prevTimeMillis = 0;

        /// <summary>
        /// Random generator, used to generate particle for different direction, etc
        /// </summary>
        public Utility.IRandomVector VectorGenerator { get; set; } = new Utility.UniformRandomVectorGenerator();

        private bool isRestart = true;

        private bool isInitialParticleChanged = true;

        private int particleCount = DefaultParticleCount;
        /// <summary>
        /// Maximum Particle count
        /// </summary>
        public int ParticleCount
        {
            set
            {
                if (particleCount == value)
                {
                    return;
                }
                particleCount = value;
                if (IsAttached)
                { OnInitialParticleChanged(value); }
            }
            get
            {
                return particleCount;
            }
        }

        private bool isTextureChanged = true;
        private Stream particleTexture;

        /// <summary>
        /// Particle Texture
        /// </summary>
        public Stream ParticleTexture
        {
            set
            {
                if (particleTexture == value)
                {
                    return;
                }
                particleTexture = value;
                isTextureChanged = true;
            }
            get
            {
                return particleTexture;
            }
        }

        private SamplerStateDescription samplerDescription = DefaultSamplers.LinearSamplerWrapAni2;
        /// <summary>
        /// Particle texture sampler description.
        /// </summary>
        public SamplerStateDescription SamplerDescription
        {
            set
            {
                samplerDescription = value;
                if (textureSampler == null)
                {
                    return;
                }
                textureSampler.Description = value;
            }
            get
            {
                return samplerDescription;
            }
        }

        private SamplerProxy textureSampler;

        private float totalElapsed = 0;              

        public bool HasTexture { get { return particleTexture != null; } }

        /// <summary>
        /// Particle Size
        /// </summary>
        public Vector2 ParticleSize = DefaultParticleSize;

        /// <summary>
        /// Particle per frame parameters
        /// </summary>
        public ParticlePerFrame FrameVariables = new ParticlePerFrame() { ExtraAcceleration = DefaultAcceleration, CumulateAtBound = 0, DomainBoundsMax = DefaultBoundMaximum, DomainBoundsMin = DefaultBoundMinimum, ConsumerGravity = DefaultConsumerGravity, ConsumerLocation = DefaultConsumerLocation, ConsumerRadius = DefaultConsumerRadius };

        /// <summary>
        /// Particle insert parameters
        /// </summary>
        public ParticleInsertParameters InsertVariables = new ParticleInsertParameters() { EmitterLocation = DefaultEmitterLocation, EnergyDissipationRate = DefaultEnergyDissipationRate, InitialAcceleration = DefaultAcceleration, InitialEnergy = DefaultInitialEnergy, InitialVelocity = DefaultInitialVelocity, ParticleBlendColor = Color.White.ToColor4() };

        #region ShaderVariables
        private IShaderPass updatePass;
        private IShaderPass insertPass;
        private IShaderPass renderPass;

        private IBufferProxy perFrameCB;
        private IBufferProxy insertCB;

        private ShaderResourceView textureView;
        #endregion
        #region Buffers
        public IElementsBufferModel InstanceBuffer { set; get; }

        private BufferDescription bufferDesc = new BufferDescription()
        {
            BindFlags = BindFlags.UnorderedAccess | BindFlags.ShaderResource,
            OptionFlags = ResourceOptionFlags.BufferStructured,
            StructureByteStride = Particle.SizeInBytes,
            CpuAccessFlags = CpuAccessFlags.None,
            Usage = ResourceUsage.Default
        };

        //Buffer indirectArgsBuffer;
        private readonly ConstantBufferProxy particleCountGSIABuffer 
            = new ConstantBufferProxy(ParticleCountIndirectArgs.SizeInBytes, BindFlags.None, CpuAccessFlags.None, ResourceOptionFlags.DrawIndirectArguments);

#if OUTPUTDEBUGGING
        private Buffer particleCountStaging;
#endif
        private UnorderedAccessViewDescription UAVBufferViewDesc = new UnorderedAccessViewDescription()
        {
            Dimension = UnorderedAccessViewDimension.Buffer,
            Format = global::SharpDX.DXGI.Format.Unknown,
            Buffer = new UnorderedAccessViewDescription.BufferResource { FirstElement = 0, Flags = UnorderedAccessViewBufferFlags.Append }
        };

        private ShaderResourceViewDescription SRVBufferViewDesc = new ShaderResourceViewDescription()
        {
            Dimension = ShaderResourceViewDimension.Buffer
        };

        private BufferDescription renderIndirectArgsBufDesc = new BufferDescription
        {
            BindFlags = BindFlags.None,
            SizeInBytes = 4 * global::SharpDX.Utilities.SizeOf<uint>(),
            StructureByteStride = 0,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.DrawIndirectArguments
        };

        protected UAVBufferViewProxy[] BufferProxies { private set; get; } = new UAVBufferViewProxy[2];
        private ParticleCountIndirectArgs drawArgument = new ParticleCountIndirectArgs();
        #endregion
        private bool isBlendChanged = true;
        private BlendState blendState;
        private BlendStateDescription blendDesc = new BlendStateDescription() { IndependentBlendEnable = false, AlphaToCoverageEnable = false };
        /// <summary>
        /// Particle blend state description
        /// </summary>
        public BlendStateDescription BlendDescription
        {
            set
            {
                blendDesc = value;
                isBlendChanged = true;
            }
            get { return blendDesc; }
        }

        public InputLayout VertexLayout { private set; get; }
        #region Shader Variable Names
        /// <summary>
        /// Set current sim state variable name inside compute shader for binding
        /// </summary>
        public string CurrentSimStateUAVBufferName
        {
            set; get;
        } = DefaultBufferNames.CurrentSimulationStateUB;
        /// <summary>
        /// Set new sim state variable name inside compute shader for binding
        /// </summary>
        public string NewSimStateUAVBufferName
        {
            set; get;
        } = DefaultBufferNames.NewSimulationStateUB;
        /// <summary>
        /// Set sim state name inside vertex shader for binding
        /// </summary>
        public string SimStateBufferName
        {
            set; get;
        } = DefaultBufferNames.SimulationStateTB;
        /// <summary>
        /// Set texture variable name inside shader for binding
        /// </summary>
        public string ShaderTextureBufferName
        {
            set;
            get;
        } = DefaultBufferNames.ParticleMapTB;
        /// <summary>
        /// Set texture sampler variable name inside shader for binding
        /// </summary>
        public string ShaderTextureSamplerName
        {
            set;
            get;
        } = DefaultSamplerStateNames.ParticleTextureSampler;
        #endregion
        #endregion

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ModelCB, ModelStruct.SizeInBytes);
        }

        

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            model.World = ModelMatrix * context.WorldMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
            model.BoolParams.X = HasTexture;
            FrameVariables.ParticleSize = ParticleSize;
            FrameVariables.RandomVector = VectorGenerator.RandomVector3;
            FrameVariables.RandomSeed = VectorGenerator.Seed;
            FrameVariables.NumTexCol = NumTextureColumn;
            FrameVariables.NumTexRow = NumTextureRow;
            FrameVariables.AnimateByEnergyLevel = AnimateSpriteByEnergy ? 1 : 0;
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
            base.OnUploadPerModelConstantBuffers(context);
            perFrameCB.UploadDataToBuffer(context, ref FrameVariables);
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                VertexLayout = technique.Layout;
                updatePass = technique[DefaultParticlePassNames.Update];
                insertPass = technique[DefaultParticlePassNames.Insert];
                renderPass = technique[DefaultParticlePassNames.Default];

                perFrameCB = technique.ConstantBufferPool.Register(DefaultBufferNames.ParticleFrameCB, ParticlePerFrame.SizeInBytes);
                insertCB = technique.ConstantBufferPool.Register(DefaultBufferNames.ParticleCreateParameters, ParticleInsertParameters.SizeInBytes);

                isBlendChanged = true;
                if (isInitialParticleChanged)
                {
                    OnInitialParticleChanged(ParticleCount);
                }
                textureSampler = new SamplerProxy(technique.EffectsManager);
                textureSampler.Description = SamplerDescription;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateInsertThrottle()
        {
            InsertElapseThrottle = (8.0f * InsertVariables.InitialEnergy / InsertVariables.EnergyDissipationRate / System.Math.Max(0, (particleCount + 8)));
        }

        private void UpdateTime(IRenderMatrices context, ref float totalElapsed)
        {
            float timeElapsed = ((float)context.TimeStamp.TotalMilliseconds - prevTimeMillis) / 1000;
            prevTimeMillis = (float)context.TimeStamp.TotalMilliseconds;
            totalElapsed += timeElapsed;
            //Update perframe variables
            FrameVariables.TimeFactors = timeElapsed;
        }


        private void OnInitialParticleChanged(int count)
        {
            isInitialParticleChanged = true;
            if (count <= 0)
            {
                return;
            }
            else if (bufferDesc.SizeInBytes < count * Particle.SizeInBytes) // Create new buffer, otherwise reuse existing buffers
            {
                Debug.WriteLine("Create buffers");
                DisposeBuffers();
                InitializeBuffers(count);
            }
            UpdateInsertThrottle();
            isInitialParticleChanged = false;
            isRestart = true;
        }

        private void DisposeBuffers()
        {
            particleCountGSIABuffer.Dispose();
#if OUTPUTDEBUGGING
            RemoveAndDispose(ref particleCountStaging);
#endif
            if (BufferProxies != null)
            {
                for (int i = 0; i < BufferProxies.Length; ++i)
                {
                    BufferProxies[i]?.Dispose();
                    BufferProxies[i] = null;
                }
            }
        }

        protected override void OnDetach()
        {
            textureSampler = null;
            DisposeBuffers();
            base.OnDetach();
        }

        private void InitializeBuffers(int count)
        {
            bufferDesc.SizeInBytes = particleCount * Particle.SizeInBytes;
            UAVBufferViewDesc.Buffer.ElementCount = particleCount;

            for (int i = 0; i < BufferProxies.Length; ++i)
            {
                BufferProxies[i] = new UAVBufferViewProxy(Device, ref bufferDesc, ref UAVBufferViewDesc, ref SRVBufferViewDesc);
            }

#if OUTPUTDEBUGGING
            var stagingbufferDesc = new BufferDescription()
            {
                BindFlags = BindFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 4 * global::SharpDX.Utilities.SizeOf<uint>(),
                CpuAccessFlags = CpuAccessFlags.Read,
                Usage = ResourceUsage.Staging
            };
            particleCountStaging = Collect(new Buffer(this.Device, stagingbufferDesc));
#endif
            particleCountGSIABuffer.CreateBuffer(this.Device);
        }

        private void OnTextureChanged()
        {
            if (isTextureChanged)
            {
                RemoveAndDispose(ref textureView);
                if (!IsAttached)
                {
                    return;
                }
                if (ParticleTexture != null)
                {
                    textureView = Collect(TextureLoader.FromMemoryAsShaderResourceView(this.Device, ParticleTexture));
                }
                isTextureChanged = false;
            }
        }

        private void OnBlendStateChanged()
        {
            if (isBlendChanged)
            {
                RemoveAndDispose(ref blendState);
                blendState = Collect(new BlendState(this.Device, blendDesc));
                isBlendChanged = false;
            }
        }

        protected override bool CanRender()
        {
            return base.CanRender() && BufferProxies != null && !isInitialParticleChanged;
        }

        protected override void OnRender(IRenderMatrices context)
        {
            OnTextureChanged();
            OnBlendStateChanged();

            UpdateTime(context, ref totalElapsed);
            //Set correct instance count from instance buffer
            drawArgument.InstanceCount = InstanceBuffer == null || !InstanceBuffer.HasElements ? 1 : (uint)InstanceBuffer.Buffer.Count;
            //Upload the draw argument
            particleCountGSIABuffer.UploadDataToBuffer(context.DeviceContext, ref drawArgument);

            updatePass.BindShader(context.DeviceContext);
            updatePass.GetShader(ShaderStage.Compute).BindUAV(context.DeviceContext, CurrentSimStateUAVBufferName, BufferProxies[0].UAV);
            updatePass.GetShader(ShaderStage.Compute).BindUAV(context.DeviceContext, NewSimStateUAVBufferName, BufferProxies[1].UAV);

            if (isRestart)
            {
                // Call ComputeShader to add initial particles
                context.DeviceContext.Dispatch(1, 1, 1);
                isRestart = false;
            }
            else
            {
                // Get consume buffer count
                BufferProxies[0].CopyCount(context.DeviceContext, perFrameCB.Buffer, ParticlePerFrame.NumParticlesOffset);
                context.DeviceContext.Dispatch(System.Math.Max(1, particleCount / 512), 1, 1);
                // Get append buffer count
                BufferProxies[1].CopyCount(context.DeviceContext, particleCountGSIABuffer.Buffer, 0);
            }

#if OUTPUTDEBUGGING
            DebugCount("UAV 0", context.DeviceContext, BufferProxies[0].UAV);
#endif


            if (totalElapsed > InsertElapseThrottle)
            {
                insertCB.UploadDataToBuffer(context.DeviceContext, ref InsertVariables);
                // Add more particles 
                insertPass.BindShader(context.DeviceContext);
                updatePass.GetShader(ShaderStage.Compute).BindUAV(context.DeviceContext, NewSimStateUAVBufferName, BufferProxies[1].UAV);
                context.DeviceContext.Dispatch(1, 1, 1);
                totalElapsed = 0;
#if OUTPUTDEBUGGING
                DebugCount("UAV 1", context.DeviceContext, BufferProxies[1].UAV);
#endif
            }

            // Clear
            updatePass.GetShader(ShaderStage.Compute).BindUAV(context.DeviceContext, CurrentSimStateUAVBufferName, null);
            updatePass.GetShader(ShaderStage.Compute).BindUAV(context.DeviceContext, NewSimStateUAVBufferName, null);

            // Swap UAV buffers for next frame
            var bproxy = BufferProxies[0];
            BufferProxies[0] = BufferProxies[1];
            BufferProxies[1] = bproxy;

            // Render existing particles
            renderPass.BindShader(context.DeviceContext);
            renderPass.BindStates(context.DeviceContext, StateType.RasterState | StateType.DepthStencilState);

            renderPass.GetShader(ShaderStage.Vertex).BindTexture(context.DeviceContext, SimStateBufferName, BufferProxies[0].SRV);
            renderPass.GetShader(ShaderStage.Pixel).BindTexture(context.DeviceContext, ShaderTextureBufferName, textureView);
            renderPass.GetShader(ShaderStage.Pixel).BindSampler(context.DeviceContext, ShaderTextureSamplerName, textureSampler);
            context.DeviceContext.InputAssembler.InputLayout = VertexLayout;
            context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            InstanceBuffer?.AttachBuffer(context.DeviceContext, 0);
            context.DeviceContext.OutputMerger.SetBlendState(blendState, null, 0xFFFFFFFF);
            context.DeviceContext.DrawInstancedIndirect(particleCountGSIABuffer.Buffer, 0);
        }

#if OUTPUTDEBUGGING
        private void DebugCount(string src, DeviceContext context, UnorderedAccessView uav)
        {
            context.CopyStructureCount(particleCountStaging, 0, uav);
            DataStream ds;
            var db = context.MapSubresource(particleCountStaging, MapMode.Read, MapFlags.None, out ds);
            int CurrentParticleCount = ds.ReadInt();
            Debug.WriteLine("{0}: {1}", src, CurrentParticleCount);
            context.UnmapSubresource(particleCountStaging, 0);
        }
#endif
    }
}
