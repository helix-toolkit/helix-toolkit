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
        public uint AnimateSpriteByEnergy { set; get; } = 0;

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
        //private EffectScalarVariable bHasTextureVar;
        //private EffectShaderResourceVariable textureViewVar;

        //private EffectVectorVariable particleSizeVar;
        //private EffectVectorVariable randomVectorVar;

        //private EffectScalarVariable randomSeedVar;
        //private EffectScalarVariable numTextureColumnVar;
        //private EffectScalarVariable numTextureRowVar;

        //private EffectUnorderedAccessViewVariable currentSimulationStateVar;
        //private EffectUnorderedAccessViewVariable newSimulationStateVar;

        //private EffectShaderResourceVariable simulationStateVar;
        //private EffectScalarVariable animateSpriteByEnergyVar;

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
        private readonly ConstantBufferProxy<ParticleCountIndirectArgs> particleCountGSIABuffer 
            = new ConstantBufferProxy<ParticleCountIndirectArgs>(ParticleCountIndirectArgs.SizeInBytes, BindFlags.None, CpuAccessFlags.None, ResourceOptionFlags.DrawIndirectArguments);
        private readonly ConstantBufferProxy<ParticlePerFrame> frameConstBuffer
            = new ConstantBufferProxy<ParticlePerFrame>(ParticlePerFrame.SizeInBytes);
        private readonly ConstantBufferProxy<ParticleInsertParameters> particleInsertBuffer
            = new ConstantBufferProxy<ParticleInsertParameters>(ParticleInsertParameters.SizeInBytes);
#if DEBUG
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
        #endregion

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return DefaultConstantBufferDescriptions.ModelCB;
        }

        protected override void OnUpdateModelStruct(IRenderMatrices context)
        {
            
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                VertexLayout = technique.Layout;
                //bHasTextureVar = Collect(Effect.GetVariableByName(ShaderVariableNames.HasDiffuseMapVariable).AsScalar());
                //textureViewVar = Collect(Effect.GetVariableByName(ShaderVariableNames.TextureDiffuseMapVariable).AsShaderResource());
                //currentSimulationStateVar = Collect(Effect.GetVariableByName(ShaderVariableNames.CurrentSimulationStateVariable).AsUnorderedAccessView());
                //newSimulationStateVar = Collect(Effect.GetVariableByName(ShaderVariableNames.NewSimulationStateVariable).AsUnorderedAccessView());
                //simulationStateVar = Collect(Effect.GetVariableByName(ShaderVariableNames.SimulationStateVariable).AsShaderResource());
                //particleSizeVar = Collect(Effect.GetVariableByName(ShaderVariableNames.ParticleSizeVariable).AsVector());
                //randomVectorVar = Collect(Effect.GetVariableByName(ShaderVariableNames.RandomVectorVariable).AsVector());
                //randomSeedVar = Collect(Effect.GetVariableByName(ShaderVariableNames.RandomSeedVariable).AsScalar());
                //numTextureColumnVar = Collect(Effect.GetVariableByName(ShaderVariableNames.NumTextureColumnVariable).AsScalar());
                //numTextureRowVar = Collect(Effect.GetVariableByName(ShaderVariableNames.NumTextureRowVariable).AsScalar());
                //animateSpriteByEnergyVar = Collect(Effect.GetVariableByName(ShaderVariableNames.AnimateByEnergyLevelVariable).AsScalar());
                isBlendChanged = true;
                if (isInitialParticleChanged)
                {
                    OnInitialParticleChanged(ParticleCount);
                }
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
            frameConstBuffer.Dispose();
            particleInsertBuffer.Dispose();
#if DEBUG
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

#if DEBUG
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
            frameConstBuffer.CreateBuffer(this.Device);
            particleInsertBuffer.CreateBuffer(this.Device);
        }

        //protected override void SetShaderVariables(IRenderMatrices matrices)
        //{
        //    base.SetShaderVariables(matrices);
        //    bHasTextureVar.Set(HasTexture);
        //    textureViewVar.SetResource(HasTexture ? textureView : null);
        //    particleSizeVar.Set(ParticleSize);
        //    randomVectorVar.Set(VectorGenerator.RandomVector3);
        //    randomSeedVar.Set(VectorGenerator.Seed);
        //    numTextureColumnVar.Set(NumTextureColumn);
        //    numTextureRowVar.Set(NumTextureRow);
        //    animateSpriteByEnergyVar.Set(AnimateSpriteByEnergy);
            
        //}

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
//            OnTextureChanged();
//            OnBlendStateChanged();

//            UpdateTime(context, ref totalElapsed);
//            //Set correct instance count from instance buffer
//            drawArgument.InstanceCount = InstanceBuffer == null || !InstanceBuffer.HasElements ? 1 : (uint)InstanceBuffer.Buffer.Count;
//            //Upload the draw argument
//            particleCountGSIABuffer.UploadDataToBuffer(context.DeviceContext, ref drawArgument);

//            EffectPass pass;

//            if (isRestart)
//            {
//                pass = this.EffectTechnique.GetPassByIndex(1);
//                pass.Apply(context.DeviceContext);
//                // Reset Both UAV buffers
//                context.DeviceContext.ComputeShader.SetUnorderedAccessView(0, BufferProxies[0].UAV, 0);
//                context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, BufferProxies[1].UAV, 0);
//                context.DeviceContext.ComputeShader.SetConstantBuffer(1, frameConstBuffer.Buffer);
//                // Call ComputeShader to add initial particles
//                context.DeviceContext.Dispatch(1, 1, 1);
//                isRestart = false;
//            }
//            else
//            {
//                //upload framebuffer
//                frameConstBuffer.UploadDataToBuffer(context.DeviceContext, ref FrameVariables);
//                // Get consume buffer count
//                BufferProxies[0].CopyCount(context.DeviceContext, frameConstBuffer.Buffer, ParticlePerFrame.NumParticlesOffset);
//                // Calculate existing particles
//                pass = this.EffectTechnique.GetPassByIndex(1);
//                pass.Apply(context.DeviceContext);
//                context.DeviceContext.ComputeShader.SetUnorderedAccessView(0, BufferProxies[0].UAV);
//                context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, BufferProxies[1].UAV, 0);
//                context.DeviceContext.ComputeShader.SetConstantBuffer(1, frameConstBuffer.Buffer);
//                context.DeviceContext.Dispatch(System.Math.Max(1, particleCount / 512), 1, 1);
//                // Get append buffer count
//                BufferProxies[1].CopyCount(context.DeviceContext, particleCountGSIABuffer.Buffer, 0);
//            }

//            //#if DEBUG
//            //            DebugCount("UAV 0", context.DeviceContext, BufferProxies[0].UAV);
//            //#endif


//            if (totalElapsed > InsertElapseThrottle)
//            {
//                particleInsertBuffer.UploadDataToBuffer(context.DeviceContext, ref InsertVariables);
//                //context.DeviceContext.UpdateSubresource(ref InsertVariables, particleInsertBuffer.Buffer);
//                // Add more particles 
//                pass = this.EffectTechnique.GetPassByIndex(0);
//                pass.Apply(context.DeviceContext);
//                context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, BufferProxies[1].UAV);
//                context.DeviceContext.ComputeShader.SetConstantBuffer(1, particleInsertBuffer.Buffer);
//                context.DeviceContext.Dispatch(1, 1, 1);
//                totalElapsed = 0;
//#if DEBUG
//               // DebugCount("UAV 1", context.DeviceContext, BufferProxies[1].UAV);
//#endif
//            }

//            // Clear
//            context.DeviceContext.ComputeShader.SetUnorderedAccessView(0, null);
//            context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, null);

//            // Swap UAV buffers for next frame
//            var bproxy = BufferProxies[0];
//            BufferProxies[0] = BufferProxies[1];
//            BufferProxies[1] = bproxy;

//            // Render existing particles
//            simulationStateVar.SetResource(BufferProxies[0].SRV);
//            context.DeviceContext.InputAssembler.InputLayout = VertexLayout;
//            context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
//            InstanceBuffer?.AttachBuffer(context.DeviceContext, 0);
//            pass = this.EffectTechnique.GetPassByIndex(2);
//            pass.Apply(context.DeviceContext);
//            context.DeviceContext.OutputMerger.SetBlendState(blendState, null, 0xFFFFFFFF);          
//            context.DeviceContext.DrawInstancedIndirect(particleCountGSIABuffer.Buffer, 0);
        }

#if DEBUG
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
