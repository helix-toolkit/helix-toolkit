/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#if DEBUG
//#define OUTPUTDEBUGGING
//#endif
using HelixToolkit.Mathematics;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.IO;
using System.Numerics;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Utilities;
    using Shaders;
    using Render;
    using Components;
    /// <summary>
    /// 
    /// </summary>
    public class ParticleRenderCore : RenderCoreBase<PointLineModelStruct>
    {
#pragma warning disable 1591
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
#pragma warning restore
        #region variables
        /// <summary>
        /// Texture tile columns
        /// </summary>
        public uint NumTextureColumn
        {
            set
            {
                FrameVariables.NumTexCol = value;
            }
            get { return FrameVariables.NumTexCol; }
        } 
        /// <summary>
        /// Texture tile rows
        /// </summary>
        public uint NumTextureRow
        {
            set
            {
                FrameVariables.NumTexRow = value;
            }
            get
            {
                return FrameVariables.NumTexRow;
            }
        }

        /// <summary>
        /// Change Sprite based on particle energy, sequence from (1,1) to (NumTextureRow, NumTextureColumn) evenly divided by tile counts
        /// </summary>
        public bool AnimateSpriteByEnergy
        {
            set
            {
                FrameVariables.AnimateByEnergyLevel = (value ? 1 : 0);
            }
            get
            {
                return FrameVariables.AnimateByEnergyLevel == 1 ? true : false;
            }
        }

        public float Turbulance
        {
            set
            {
                FrameVariables.Turbulance = value;
            }
            get { return FrameVariables.Turbulance; }
        }

        /// <summary>
        /// Minimum time elapse to insert new particles
        /// </summary>
        public float InsertElapseThrottle { private set; get; } = 0;

        private double prevTimeMillis = 0;

        /// <summary>
        /// Random generator, used to generate particle for different direction, etc
        /// </summary>
        public IRandomVector VectorGenerator { get; set; } = new UniformRandomVectorGenerator();

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

        private Stream particleTexture;

        /// <summary>
        /// Particle Texture
        /// </summary>
        public Stream ParticleTexture
        {
            set
            {
                if(Set(ref particleTexture, value) && IsAttached)
                {
                    OnTextureChanged();
                }
            }
            get
            {
                return particleTexture;
            }
        }

        private SamplerStateDescription samplerDescription = DefaultSamplers.LinearSamplerWrapAni1;
        /// <summary>
        /// Particle texture sampler description.
        /// </summary>
        public SamplerStateDescription SamplerDescription
        {
            set
            {
                if(Set(ref samplerDescription, value) && IsAttached)
                {
                    RemoveAndDispose(ref textureSampler);
                    textureSampler = Collect(EffectTechnique.EffectsManager.StateManager.Register(value));
                }
            }
            get
            {
                return samplerDescription;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has texture.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has texture; otherwise, <c>false</c>.
        /// </value>
        public bool HasTexture { get { return particleTexture != null; } }

        /// <summary>
        /// Particle Size
        /// </summary>
        public Vector2 ParticleSize
        {
            set
            {
                FrameVariables.ParticleSize = value;
            }
            get
            {
                return FrameVariables.ParticleSize;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 EmitterLocation
        {
            set
            {
                InsertVariables.EmitterLocation = value;
            }
            get
            {
                return InsertVariables.EmitterLocation;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool CumulateAtBound
        {
            set
            {
                FrameVariables.CumulateAtBound = (value ? 1u : 0);
            }
            get
            {
                return FrameVariables.CumulateAtBound == 1 ? true : false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 ExtraAcceleration
        {
            set
            {
                FrameVariables.ExtraAcceleration = value;
            }
            get
            {
                return FrameVariables.ExtraAcceleration;
            }
        }
        /// <summary>
        /// Gets or sets the domain bound maximum.
        /// </summary>
        /// <value>
        /// The domain bound maximum.
        /// </value>
        public Vector3 DomainBoundMax
        {
            set
            {
                FrameVariables.DomainBoundsMax = value;
            }
            get
            {
                return FrameVariables.DomainBoundsMax;
            }
        }
        /// <summary>
        /// Gets or sets the domain bound minimum.
        /// </summary>
        /// <value>
        /// The domain bound minimum.
        /// </value>
        public Vector3 DomainBoundMin
        {
            set
            {
                if (FrameVariables.DomainBoundsMin != value)
                {
                    FrameVariables.DomainBoundsMin = value;
                    InvalidateRenderer();
                }
            }
            get
            {
                return FrameVariables.DomainBoundsMin;
            }
        }
        /// <summary>
        /// Gets or sets the consumer gravity.
        /// </summary>
        /// <value>
        /// The consumer gravity.
        /// </value>
        public float ConsumerGravity
        {
            set
            {
                FrameVariables.ConsumerGravity = value;
            }
            get
            {
                return FrameVariables.ConsumerGravity;
            }
        }
        /// <summary>
        /// Gets or sets the consumer location.
        /// </summary>
        /// <value>
        /// The consumer location.
        /// </value>
        public Vector3 ConsumerLocation
        {
            set
            {
                FrameVariables.ConsumerLocation = value;
            }
            get
            {
                return FrameVariables.ConsumerLocation;
            }
        }
        /// <summary>
        /// Gets or sets the consumer radius.
        /// </summary>
        /// <value>
        /// The consumer radius.
        /// </value>
        public float ConsumerRadius
        {
            set
            {
                FrameVariables.ConsumerRadius = value;
            }
            get
            {
                return FrameVariables.ConsumerRadius;
            }
        }
        /// <summary>
        /// Gets or sets the energy dissipation rate.
        /// </summary>
        /// <value>
        /// The energy dissipation rate.
        /// </value>
        public float EnergyDissipationRate
        {
            set
            {
                InsertVariables.EnergyDissipationRate = value;
            }
            get { return InsertVariables.EnergyDissipationRate; }
        }
        /// <summary>
        /// Gets or sets the initial acceleration.
        /// </summary>
        /// <value>
        /// The initial acceleration.
        /// </value>
        public Vector3 InitialAcceleration
        {
            set { InsertVariables.InitialAcceleration = value; }
            get { return InsertVariables.InitialAcceleration; }
        }
        /// <summary>
        /// Gets or sets the initial energy.
        /// </summary>
        /// <value>
        /// The initial energy.
        /// </value>
        public float InitialEnergy
        {
            set { InsertVariables.InitialEnergy = value; }
            get { return InsertVariables.InitialEnergy; }
        }
        /// <summary>
        /// Gets or sets the initial velocity.
        /// </summary>
        /// <value>
        /// The initial velocity.
        /// </value>
        public float InitialVelocity
        {
            set { InsertVariables.InitialVelocity = value; }
            get { return InsertVariables.InitialVelocity; }
        }
        /// <summary>
        /// Gets or sets the color of the particle blend.
        /// </summary>
        /// <value>
        /// The color of the particle blend.
        /// </value>
        public Color4 ParticleBlendColor
        {
            set { InsertVariables.ParticleBlendColor = value; }
            get { return InsertVariables.ParticleBlendColor; }
        }
        /// <summary>
        /// Gets or sets the emitter radius.
        /// </summary>
        /// <value>
        /// The emitter radius.
        /// </value>
        public float EmitterRadius
        {
            set { InsertVariables.EmitterRadius = value; }
            get { return InsertVariables.EmitterRadius; }
        }
        /// <summary>
        /// Particle per frame parameters
        /// </summary>
        private ParticlePerFrame FrameVariables = new ParticlePerFrame() { ExtraAcceleration = DefaultAcceleration, CumulateAtBound = 0,
            DomainBoundsMax = DefaultBoundMaximum, DomainBoundsMin = DefaultBoundMinimum,
            ConsumerGravity = DefaultConsumerGravity, ConsumerLocation = DefaultConsumerLocation, ConsumerRadius = DefaultConsumerRadius };

        /// <summary>
        /// Particle insert parameters
        /// </summary>
        private ParticleInsertParameters InsertVariables = new ParticleInsertParameters() { EmitterLocation = DefaultEmitterLocation, EmitterRadius = DefaultConsumerRadius,
            EnergyDissipationRate = DefaultEnergyDissipationRate, InitialAcceleration = DefaultAcceleration, InitialEnergy = DefaultInitialEnergy,
            InitialVelocity = DefaultInitialVelocity, ParticleBlendColor = Color.White.ToColor4() };

        #region ShaderVariables
        private ShaderPass updatePass;
        private ShaderPass insertPass;
        private ShaderPass renderPass;

        private readonly ConstantBufferComponent perFrameCB;
        private readonly ConstantBufferComponent insertCB;
        private readonly ConstantBufferComponent modelCB;

        private ShaderResourceViewProxy textureView;
        private SamplerStateProxy textureSampler;
        private BlendStateProxy blendState;
        private double totalElapsed = 0;
        #endregion
        #region Buffers        
        /// <summary>
        /// Gets or sets the instance buffer.
        /// </summary>
        /// <value>
        /// The instance buffer.
        /// </value>
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
            = new ConstantBufferProxy(ParticleCountIndirectArgs.SizeInBytes, BindFlags.None, CpuAccessFlags.None, 
                ResourceOptionFlags.DrawIndirectArguments);

        private ConstantBufferProxy particleCountStaging
            = new ConstantBufferProxy(4 * sizeof(int), BindFlags.None, CpuAccessFlags.Read, ResourceOptionFlags.None, ResourceUsage.Staging);

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
        /// <summary>
        /// Gets or sets the buffer proxies.
        /// </summary>
        /// <value>
        /// The buffer proxies.
        /// </value>
        protected UAVBufferViewProxy[] BufferProxies { private set; get; } = new UAVBufferViewProxy[2];
        private ParticleCountIndirectArgs drawArgument = new ParticleCountIndirectArgs();
        #endregion

        private BlendStateDescription blendDesc = new BlendStateDescription() { IndependentBlendEnable = false, AlphaToCoverageEnable = false };
        /// <summary>
        /// Particle blend state description
        /// </summary>
        public BlendStateDescription BlendDescription
        {
            set
            {
                if(Set(ref blendDesc, value) && IsAttached)
                {
                    OnBlendStateChanged();
                }
            }
            get { return blendDesc; }
        }

        private Color4 blendFactor = Color4.White;
        /// <summary>
        /// Gets or sets the blend factor used for blending.
        /// </summary>
        /// <value>
        /// The blend factor.
        /// </value>
        public Color4 BlendFactor
        {
            set
            {
                SetAffectsRender(ref blendFactor, value);
            }
            get { return blendFactor; }
        }

        private int sampleMask = -1;
        /// <summary>
        /// Gets or sets the sample mask used for blending.
        /// </summary>
        /// <value>
        /// The sample mask.
        /// </value>
        public int SampleMask
        {
            set
            {
                SetAffectsRender(ref sampleMask, value);
            }
            get { return sampleMask; }
        }
        /// <summary>
        /// Gets or sets the vertex layout.
        /// </summary>
        /// <value>
        /// The vertex layout.
        /// </value>
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

        private int currentStateSlot;
        private int newStateSlot;
        private int renderStateSlot;
        private int textureSlot;
        private int samplerSlot;
        #endregion

        private readonly object lockObject = new object();

        public ParticleRenderCore() : base(RenderType.Particle)
        {
            modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.PointLineModelCB, PointLineModelStruct.SizeInBytes)));
            perFrameCB = AddComponent(new ConstantBufferComponent(DefaultBufferNames.ParticleFrameCB, ParticlePerFrame.SizeInBytes));
            insertCB = AddComponent(new ConstantBufferComponent(DefaultBufferNames.ParticleCreateParameters, ParticleInsertParameters.SizeInBytes));
            NeedUpdate = true;
        }


        /// <summary>
        /// Called when [update per model structure].
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected override void OnUpdatePerModelStruct(ref PointLineModelStruct model, RenderContext context)
        {
            model.World = ModelMatrix;
            model.HasInstances = InstanceBuffer == null ? 0 : InstanceBuffer.HasElements ? 1 : 0;
            model.BoolParams.X = HasTexture;
            FrameVariables.RandomVector = VectorGenerator.RandomVector3;
        }

        /// <summary>
        /// Called when [attach].
        /// </summary>
        /// <param name="technique">The technique.</param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderTechnique technique)
        {
            VertexLayout = technique.Layout;
            updatePass = technique[DefaultParticlePassNames.Update];
            insertPass = technique[DefaultParticlePassNames.Insert];
            renderPass = technique[DefaultParticlePassNames.Default];
            #region Get binding slots
            currentStateSlot = updatePass.GetShader(ShaderStage.Compute).UnorderedAccessViewMapping.TryGetBindSlot(CurrentSimStateUAVBufferName);
            newStateSlot = updatePass.GetShader(ShaderStage.Compute).UnorderedAccessViewMapping.TryGetBindSlot(NewSimStateUAVBufferName);

            renderStateSlot = renderPass.GetShader(ShaderStage.Vertex).ShaderResourceViewMapping.TryGetBindSlot(SimStateBufferName);
            textureSlot = renderPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(ShaderTextureBufferName);
            samplerSlot = renderPass.PixelShader.SamplerMapping.TryGetBindSlot(ShaderTextureSamplerName);
            #endregion
            if (isInitialParticleChanged)
            {
                OnInitialParticleChanged(ParticleCount);
            }
            textureSampler = Collect(technique.EffectsManager.StateManager.Register(SamplerDescription));
            OnTextureChanged();
            OnBlendStateChanged();
            return true;
        }

        /// <summary>
        /// Updates the insert throttle.
        /// </summary>
        public void UpdateInsertThrottle()
        {
            InsertElapseThrottle = (8.0f * InsertVariables.InitialEnergy / InsertVariables.EnergyDissipationRate / System.Math.Max(0, (particleCount + 8)));
        }

        private void UpdateTime(RenderContext context, ref double totalElapsed)
        {
            double timeElapsed = Math.Max(0, (context.TimeStamp.TotalMilliseconds - prevTimeMillis) / 1000);
            prevTimeMillis = context.TimeStamp.TotalMilliseconds;
            totalElapsed += timeElapsed;
            //Update perframe variables
            FrameVariables.TimeFactors = (float)timeElapsed;
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
                DisposeBuffers();
                InitializeBuffers(count);
            }
            UpdateInsertThrottle();
            isInitialParticleChanged = false;
            isRestart = true;
            UpdateCanRenderFlag();
        }

        private void DisposeBuffers()
        {
            particleCountGSIABuffer.DisposeAndClear();

            particleCountStaging.DisposeAndClear();

            if (BufferProxies != null)
            {
                for (int i = 0; i < BufferProxies.Length; ++i)
                {
                    BufferProxies[i]?.Dispose();
                    BufferProxies[i] = null;
                }
            }
        }
        /// <summary>
        /// Called when [detach].
        /// </summary>
        protected override void OnDetach()
        {
            DisposeBuffers();
            textureSampler = null;
            blendState = null;
            textureView = null;
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

            particleCountStaging.CreateBuffer(this.Device); 
            particleCountGSIABuffer.CreateBuffer(this.Device);
        }

        private void OnTextureChanged()
        {
            RemoveAndDispose(ref textureView);
            textureView = Collect(this.EffectTechnique.EffectsManager.MaterialTextureManager.Register(ParticleTexture));
        }

        private void OnBlendStateChanged()
        {
            RemoveAndDispose(ref blendState);
            blendState = Collect(EffectTechnique.EffectsManager.StateManager.Register(blendDesc));
        }


        protected override bool OnUpdateCanRenderFlag()
        {
            return base.OnUpdateCanRenderFlag() && !isInitialParticleChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext"></param>
        protected override void OnUpdate(RenderContext context, DeviceContextProxy deviceContext)
        {        
            UpdateTime(context, ref totalElapsed);
            //Set correct instance count from instance buffer
            drawArgument.InstanceCount = InstanceBuffer == null || !InstanceBuffer.HasElements ? 1 : (uint)InstanceBuffer.Buffer.ElementCount;
            //Upload the draw argument
            particleCountGSIABuffer.UploadDataToBuffer(deviceContext, ref drawArgument);

            updatePass.BindShader(deviceContext);
            updatePass.ComputeShader.BindUAV(deviceContext, currentStateSlot, BufferProxies[0]);
            updatePass.ComputeShader.BindUAV(deviceContext, newStateSlot, BufferProxies[1]);

            if (isRestart)
            {
                FrameVariables.NumParticles = 0;
                perFrameCB.Upload(deviceContext, ref FrameVariables);
                // Call ComputeShader to add initial particles
                deviceContext.Dispatch(1, 1, 1);
                isRestart = false;
            }
            else
            {
                #region Get consume buffer count
                // Get consume buffer count.
                //Due to some intel integrated graphic card having issue copy structure count directly into constant buffer.
                //Has to use staging buffer to read and pass into constant buffer              
                FrameVariables.NumParticles = (uint)ReadCount("", deviceContext, BufferProxies[0]);
                perFrameCB.Upload(deviceContext, ref FrameVariables);
                #endregion

                deviceContext.Dispatch(Math.Max(1, (int)Math.Ceiling((double)FrameVariables.NumParticles / 512)), 1, 1);
                // Get append buffer count
                BufferProxies[1].CopyCount(deviceContext, particleCountGSIABuffer.Buffer, 0);
            }

#if OUTPUTDEBUGGING
            ReadCount("UAV 0", deviceContext, BufferProxies[0].UAV);
#endif


            if (totalElapsed > InsertElapseThrottle)
            {
                insertCB.Upload(deviceContext, ref InsertVariables);
                // Add more particles 
                insertPass.BindShader(deviceContext);
                insertPass.ComputeShader.BindUAV(deviceContext, newStateSlot, BufferProxies[1]);
                deviceContext.Dispatch(1, 1, 1);
                totalElapsed = 0;
#if OUTPUTDEBUGGING
                ReadCount("UAV 1", deviceContext, BufferProxies[1].UAV);
#endif
            }

            // Swap UAV buffers for next frame
            var bproxy = BufferProxies[0];
            BufferProxies[0] = BufferProxies[1];
            BufferProxies[1] = bproxy;
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            perFrameCB.Upload(deviceContext, ref FrameVariables);
            modelCB.Upload(deviceContext, ref modelStruct);
            // Clear binding
            updatePass.ComputeShader.BindUAV(deviceContext, currentStateSlot, null);
            updatePass.ComputeShader.BindUAV(deviceContext, newStateSlot, null);

            // Render existing particles
            renderPass.BindShader(deviceContext);
            renderPass.BindStates(deviceContext, StateType.RasterState | StateType.DepthStencilState);

            renderPass.VertexShader.BindTexture(deviceContext, renderStateSlot, BufferProxies[0]);
            renderPass.PixelShader.BindTexture(deviceContext, textureSlot, textureView);
            renderPass.PixelShader.BindSampler(deviceContext, samplerSlot, textureSampler);
            deviceContext.InputLayout = VertexLayout;
            int firstSlot = 0;
            InstanceBuffer?.AttachBuffer(deviceContext, ref firstSlot);
            deviceContext.SetBlendState(blendState, blendFactor, sampleMask);
            deviceContext.DrawInstancedIndirect(particleCountGSIABuffer.Buffer, 0);
            InvalidateRenderer();//Since particle is running all the time. Invalidate once finished rendering
        }


        private int ReadCount(string src, DeviceContextProxy context, UnorderedAccessView uav)
        {
            context.CopyStructureCount(particleCountStaging, 0, uav);
            var db = context.MapSubresource(particleCountStaging, MapMode.Read, MapFlags.None, out DataStream ds);
            int CurrentParticleCount = 0;
            using (ds)
            {
                CurrentParticleCount = ds.ReadInt();
#if OUTPUTDEBUGGING
                Debug.WriteLine("{0}: {1}", src, CurrentParticleCount);
#endif
            }
            context.UnmapSubresource(particleCountStaging, 0);
            return CurrentParticleCount;
        }

        public sealed override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public sealed override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }
    }
}
