using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.Windows;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using HelixToolkit.Wpf.SharpDX.Randoms;
using System.IO;
using Media3D = System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.SharpDX
{
    public class ParticleStormModel3D : Model3D
    {
        #region Dependency Properties
        public static DependencyProperty ParticleCountProperty = DependencyProperty.Register("ParticleCount", typeof(int), typeof(ParticleStormModel3D),
            new PropertyMetadata(512,
            (d, e) =>
            {
                (d as ParticleStormModel3D).OnInitialParticleChanged((int)e.NewValue);
            }
            ));

        public int ParticleCount
        {
            set
            {
                SetValue(ParticleCountProperty, value);
            }
            get
            {
                return (int)GetValue(ParticleCountProperty);
            }
        }

        public static DependencyProperty EmitterLocationProperty = DependencyProperty.Register("EmitterLocation", typeof(Media3D.Point3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(new Media3D.Point3D(),
            (d, e) =>
            {
                (d as ParticleStormModel3D).OnEmitterLocationChanged(((Media3D.Point3D)e.NewValue).ToVector3());
            }
            ));

        public Media3D.Point3D EmitterLocation
        {
            set
            {
                SetValue(EmitterLocationProperty, value);
            }
            get
            {
                return (Media3D.Point3D)GetValue(EmitterLocationProperty);
            }
        }

        public static DependencyProperty ConsumerLocationProperty = DependencyProperty.Register("ConsumerLocation", typeof(Media3D.Point3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(new Media3D.Point3D(0, 10, 0),
            (d, e) =>
            {
                (d as ParticleStormModel3D).OnConsumerLocationChanged(((Media3D.Point3D)e.NewValue).ToVector3());
            }
            ));

        public Media3D.Point3D ConsumerLocation
        {
            set
            {
                SetValue(ConsumerLocationProperty, value);
            }
            get
            {
                return (Media3D.Point3D)GetValue(ConsumerLocationProperty);
            }
        }

        public static DependencyProperty InitialEnergyProperty = DependencyProperty.Register("InitialEnergy", typeof(float), typeof(ParticleStormModel3D),
            new PropertyMetadata(5f,
            (d, e) =>
            {
                (d as ParticleStormModel3D).initialEnergy = (float)e.NewValue;
                (d as ParticleStormModel3D).UpdateInsertThrottle();
            }
            ));

        public float InitialEnergy
        {
            set
            {
                SetValue(InitialEnergyProperty, value);
            }
            get
            {
                return (float)GetValue(InitialEnergyProperty);
            }
        }

        public static DependencyProperty EnergyDissipationRateProperty = DependencyProperty.Register("EnergyDissipationRate", typeof(float), typeof(ParticleStormModel3D),
            new PropertyMetadata(0.1f,
            (d, e) =>
            {
                (d as ParticleStormModel3D).energyDissipationRate = (float)e.NewValue;
            }
            ));

        public float EnergyDissipationRate
        {
            set
            {
                SetValue(EnergyDissipationRateProperty, value);
            }
            get
            {
                return (float)GetValue(EnergyDissipationRateProperty);
            }
        }

        public static DependencyProperty RandomVectorGeneratorProperty = DependencyProperty.Register("RandomVectorGenerator", typeof(IRandomVector), typeof(ParticleStormModel3D),
            new PropertyMetadata(new UniformRandomVectorGenerator(),
            (d, e) =>
            {
                (d as ParticleStormModel3D).vectorGenerator = (IRandomVector)e.NewValue;
            }
            ));

        public IRandomVector RandomVectorGenerator
        {
            set
            {
                SetValue(RandomVectorGeneratorProperty, value);
            }
            get
            {
                return (IRandomVector)GetValue(RandomVectorGeneratorProperty);
            }
        }

        public static DependencyProperty ParticleTextureProperty = DependencyProperty.Register("ParticleTexture", typeof(Stream), typeof(ParticleStormModel3D),
            new AffectsRenderPropertyMetadata(null,
            (d, e) =>
            {
                (d as ParticleStormModel3D).isTextureChanged = true;
            }
            ));

        public Stream ParticleTexture
        {
            set
            {
                SetValue(ParticleTextureProperty, value);
            }
            get
            {
                return (Stream)GetValue(ParticleTextureProperty);
            }
        }

        public static DependencyProperty ParticleSizeProperty = DependencyProperty.Register("ParticleSizeProperty", typeof(Size), typeof(ParticleStormModel3D),
            new AffectsRenderPropertyMetadata(new Size(1, 1),
                (d, e) =>
                {
                    var size = (Size)e.NewValue;
                    (d as ParticleStormModel3D).particleSize = new Vector2((float)size.Width, (float)size.Height);
                }));

        public Size ParticleSize
        {
            set
            {
                SetValue(ParticleSizeProperty, value);
            }
            get
            {
                return (Size)GetValue(ParticleSizeProperty);
            }
        }


        public static DependencyProperty InitialVelocityProperty = DependencyProperty.Register("InitialVelocity", typeof(Media3D.Vector3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(new Media3D.Vector3D(1, 1, 1),
            (d, e) =>
            {
                (d as ParticleStormModel3D).initialVelocity = ((Media3D.Vector3D)e.NewValue).ToVector3();
            }
            ));

        public Media3D.Vector3D InitialVelocity
        {
            set
            {
                SetValue(InitialVelocityProperty, value);
            }
            get
            {
                return (Media3D.Vector3D)GetValue(InitialVelocityProperty);
            }
        }

        public static DependencyProperty AccelerationProperty = DependencyProperty.Register("Acceleration", typeof(Media3D.Vector3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(new Media3D.Vector3D(0, 0.1, 0),
            (d, e) =>
            {
                (d as ParticleStormModel3D).acceleration = ((Media3D.Vector3D)e.NewValue).ToVector3();
            }
            ));

        public Media3D.Vector3D Acceleration
        {
            set
            {
                SetValue(AccelerationProperty, value);
            }
            get
            {
                return (Media3D.Vector3D)GetValue(AccelerationProperty);
            }
        }

        public static DependencyProperty ParticleBoundsProperty = DependencyProperty.Register("ParticleBounds", typeof(Media3D.Rect3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(new Media3D.Rect3D(0, 0, 0, 10, 10, 10),
            (d, e) =>
            {
                var bound = (Media3D.Rect3D)e.NewValue;
                (d as ParticleStormModel3D).boundMaximum = new Vector3((float)(bound.SizeX / 2 + bound.Location.X), (float)(bound.SizeY / 2 + bound.Location.Y), (float)(bound.SizeZ / 2 + bound.Location.Z));
                (d as ParticleStormModel3D).boundMinimum = new Vector3((float)(bound.Location.X - bound.SizeX / 2), (float)(bound.Location.Y - bound.SizeY / 2), (float)(bound.Location.Z - bound.SizeZ / 2));
            }));

        public Media3D.Rect3D ParticleBounds
        {
            set
            {
                SetValue(ParticleBoundsProperty, value);
            }
            get
            {
                return (Media3D.Rect3D)GetValue(ParticleBoundsProperty);
            }
        }

        //public static DependencyProperty BoundMaximumProperty = DependencyProperty.Register("BoundMaximum", typeof(Media3D.Point3D), typeof(ParticleStormModel3D),
        //    new PropertyMetadata(new Media3D.Point3D(10, 10, 10),
        //        (d, e) =>
        //        {
        //            (d as ParticleStormModel3D).boundMaximum = ((Media3D.Point3D)e.NewValue).ToVector3();
        //        }));

        //public Media3D.Point3D BoundMaximum
        //{
        //    set
        //    {
        //        SetValue(BoundMaximumProperty, value);
        //    }
        //    get
        //    {
        //        return (Media3D.Point3D)GetValue(BoundMaximumProperty);
        //    }
        //}

        //public static DependencyProperty BoundMinimumProperty = DependencyProperty.Register("BoundMinimum", typeof(Media3D.Point3D), typeof(ParticleStormModel3D),
        //    new PropertyMetadata(new Media3D.Point3D(-10, -10, -10),
        //        (d, e) =>
        //        {
        //            (d as ParticleStormModel3D).boundMinimum = ((Media3D.Point3D)e.NewValue).ToVector3();
        //        }));

        //public Media3D.Point3D BoundMinimum
        //{
        //    set
        //    {
        //        SetValue(BoundMinimumProperty, value);
        //    }
        //    get
        //    {
        //        return (Media3D.Point3D)GetValue(BoundMinimumProperty);
        //    }
        //}
        #endregion
        #region variables
        protected int particleCountInternal = 0;

        private float insertThrottle = 0;

        private float prevTimeMillis = 0;

        private float totalElapsed = 0;

        private float initialEnergy = 5;

        private float energyDissipationRate = 0.1f;

        private IRandomVector vectorGenerator = new UniformRandomVectorGenerator();

        private bool isInitialParticleChanged = true;

        private bool isRestart = true;

        private bool hasTexture = false;

        private bool isTextureChanged = true;

        private ParticlePerFrame frameVariables = new ParticlePerFrame();

        private Vector3 initialVelocity = new Vector3(1, 1, 1);

        private Vector3 acceleration = new Vector3(0, 0.1f, 0);

        protected Vector2 particleSize = new Vector2(1, 1);

        protected Vector3 emitterLocationInternal = Vector3.Zero;

        protected Vector3 consumerLocationInternal = new Vector3(0, 10, 0);

        protected Vector3 boundMaximum = new Vector3(10, 10, 10);

        protected Vector3 boundMinimum = new Vector3(-10, -10, -10);

        protected EffectVectorVariable emitterLocationVar;

        protected EffectVectorVariable consumerLocationVar;

        protected EffectScalarVariable initialEnergyVar;

        protected EffectScalarVariable energyDissipationRateVar;

        protected EffectUnorderedAccessViewVariable currentSimulationStateVar;

        protected EffectUnorderedAccessViewVariable newSimulationStateVar;

        protected EffectShaderResourceVariable simulationStateVar;

        protected EffectTransformVariables effectTransforms;

        protected EffectScalarVariable bHasTextureVar;

        protected EffectShaderResourceVariable textureViewVar;

        protected ShaderResourceView textureView;

        protected EffectVectorVariable particleSizeVar;

        protected EffectVectorVariable initialVelocityVar;

        protected EffectVectorVariable accelerationVar;

        protected EffectVectorVariable boundMaximumVar;

        protected EffectVectorVariable boundMinimumVar;

        private BufferDescription bufferDesc = new BufferDescription()
        {
            BindFlags = BindFlags.UnorderedAccess | BindFlags.ShaderResource,
            OptionFlags = ResourceOptionFlags.BufferStructured,
            StructureByteStride = Particle.SizeInBytes,
            CpuAccessFlags = CpuAccessFlags.None,
            Usage = ResourceUsage.Default
        };

        //Buffer indirectArgsBuffer;
        Buffer particleCountGSIABuffer;
        Buffer frameConstBuffer;
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

        protected readonly BufferViewProxy[] BufferProxies = new BufferViewProxy[2];

        protected EffectTechnique effectTechnique;

        #endregion

        private void OnInitialParticleChanged(int count)
        {
            isInitialParticleChanged = true;
            DisposeBuffers();
            if (count <= 0 || !IsAttached)
            {
                return;
            }
            InitializeBuffers(count);
            UpdateInsertThrottle();
            isInitialParticleChanged = false;
            isRestart = true;
        }

        private void UpdateInsertThrottle()
        {
            insertThrottle = (8.0f * initialEnergy / System.Math.Max(0, (particleCountInternal + 8)));
        }

        private void DisposeBuffers()
        {
            Disposer.RemoveAndDispose(ref particleCountGSIABuffer);
            Disposer.RemoveAndDispose(ref frameConstBuffer);
            // Disposer.RemoveAndDispose(ref indirectArgsBuffer);
            if (BufferProxies != null)
            {
                for (int i = 0; i < BufferProxies.Length; ++i)
                {
                    BufferProxies[i]?.Dispose();
                    BufferProxies[i] = null;
                }
            }
        }

        private void InitializeBuffers(int count)
        {
            particleCountInternal = count;
            bufferDesc.SizeInBytes = particleCountInternal * Particle.SizeInBytes;

            UAVBufferViewDesc.Buffer.ElementCount = particleCountInternal;

            for (int i = 0; i < BufferProxies.Length; ++i)
            {
                BufferProxies[i] = new BufferViewProxy(Device, ref bufferDesc, ref UAVBufferViewDesc, ref SRVBufferViewDesc);
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
            particleCountStaging = new Buffer(this.Device, stagingbufferDesc);
#endif
            particleCountGSIABuffer = Buffer.Create(this.Device, new uint[4] { 0, 1, 0, 0 }, renderIndirectArgsBufDesc);

            frameConstBuffer = new Buffer(this.Device, ParticlePerFrame.SizeInBytes, ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

        private void OnEmitterLocationChanged(Vector3 location)
        {
            emitterLocationInternal = location;
        }

        private void OnConsumerLocationChanged(Vector3 location)
        {
            consumerLocationInternal = location;
        }

        private void OnTextureChanged(Stream texture)
        {
            Disposer.RemoveAndDispose(ref textureView);
            if (!IsAttached)
            {
                return;
            }
            if (texture == null)
            {
                hasTexture = false;
            }
            else
            {
                textureView = TextureLoader.FromMemoryAsShaderResourceView(this.Device, texture);
                hasTexture = true;
            }
            isTextureChanged = false;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            emitterLocationVar = effect.GetVariableByName("EmitterLocation").AsVector();
            consumerLocationVar = effect.GetVariableByName("ConsumerLocation").AsVector();
            currentSimulationStateVar = effect.GetVariableByName("CurrentSimulationState").AsUnorderedAccessView();
            newSimulationStateVar = effect.GetVariableByName("NewSimulationState").AsUnorderedAccessView();
            simulationStateVar = effect.GetVariableByName("SimulationState").AsShaderResource();
            initialEnergyVar = effect.GetVariableByName("InitialEnergy").AsScalar();
            energyDissipationRateVar = effect.GetVariableByName("EnergyDissipationRate").AsScalar();
            bHasTextureVar = effect.GetVariableByName("bHasDiffuseMap").AsScalar();
            textureViewVar = effect.GetVariableByName("texDiffuseMap").AsShaderResource();
            particleSizeVar = effect.GetVariableByName("ParticleSize").AsVector();
            initialVelocityVar = effect.GetVariableByName("InitialVelocity").AsVector();
            accelerationVar = effect.GetVariableByName("Acceleration").AsVector();
            boundMaximumVar = effect.GetVariableByName("DomainBoundsMax").AsVector();
            boundMinimumVar = effect.GetVariableByName("DomainBoundsMin").AsVector();
            effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);
            this.effectTransforms = new EffectTransformVariables(this.effect);
            System.Windows.Media.CompositionTarget.Rendering += CompositionTarget_Rendering;
            return true;
        }

        private void CompositionTarget_Rendering(object sender, System.EventArgs e)
        {
            InvalidateRender();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (isInitialParticleChanged)
            {
                OnInitialParticleChanged(ParticleCount);
            }
        }

        protected override void OnDetach()
        {
            System.Windows.Media.CompositionTarget.Rendering -= CompositionTarget_Rendering;
            Disposer.RemoveAndDispose(ref emitterLocationVar);
            Disposer.RemoveAndDispose(ref consumerLocationVar);
            Disposer.RemoveAndDispose(ref currentSimulationStateVar);
            Disposer.RemoveAndDispose(ref newSimulationStateVar);
            Disposer.RemoveAndDispose(ref simulationStateVar);
            Disposer.RemoveAndDispose(ref effectTransforms);
            Disposer.RemoveAndDispose(ref bHasTextureVar);
            Disposer.RemoveAndDispose(ref textureViewVar);
            Disposer.RemoveAndDispose(ref particleSizeVar);
            Disposer.RemoveAndDispose(ref accelerationVar);
            Disposer.RemoveAndDispose(ref initialVelocityVar);
            Disposer.RemoveAndDispose(ref energyDissipationRateVar);
            Disposer.RemoveAndDispose(ref initialEnergyVar);
            Disposer.RemoveAndDispose(ref boundMaximumVar);
            Disposer.RemoveAndDispose(ref boundMinimumVar);
            DisposeBuffers();
            base.OnDetach();
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.ParticleStorm];
        }

        protected override bool CanRender(RenderContext context)
        {
            return BufferProxies != null && IsAttached && visibleInternal && isRenderingInternal;
        }

        private void SetVariables()
        {
            bHasTextureVar.Set(hasTexture);
            textureViewVar.SetResource(hasTexture ? textureView : null);
            emitterLocationVar.Set(emitterLocationInternal);
            consumerLocationVar.Set(consumerLocationInternal);
            initialEnergyVar.Set(initialEnergy);
            particleSizeVar.Set(particleSize);
            initialVelocityVar.Set(initialVelocity);
            accelerationVar.Set(acceleration);
            energyDissipationRateVar.Set(energyDissipationRate);
            boundMaximumVar.Set(boundMaximum);
            boundMinimumVar.Set(boundMinimum);
        }

        protected override void OnRender(RenderContext context)
        {
            var worldMatrix = this.modelMatrix * context.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);

            if (isTextureChanged)
            {
                OnTextureChanged(ParticleTexture);
            }

            SetVariables();

            float timeElapsed = ((float)context.TimeStamp.TotalMilliseconds - prevTimeMillis) / 1000;
            prevTimeMillis = (float)context.TimeStamp.TotalMilliseconds;
            //timeFactorsVar.Set(timeElapsed);
            totalElapsed += timeElapsed;
            //Update perframe variables
            frameVariables.TimeFactors = timeElapsed;
            frameVariables.RandomVector = vectorGenerator.RandomVector3;
            frameVariables.RandomSeed = vectorGenerator.Seed;
            //upload to const buffer
            context.DeviceContext.UpdateSubresource(ref frameVariables, frameConstBuffer);

            EffectPass pass;

            if (isRestart)
            {
                pass = this.effectTechnique.GetPassByIndex(1);
                pass.Apply(context.DeviceContext);
                // Reset Both UAV buffers
                context.DeviceContext.ComputeShader.SetUnorderedAccessView(0, BufferProxies[0].UAV, 0);
                context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, BufferProxies[1].UAV, 0);
                context.DeviceContext.ComputeShader.SetConstantBuffer(1, frameConstBuffer);
                // Call ComputeShader to add initial particles
                context.DeviceContext.Dispatch(1, 1, 1);
                isRestart = false;
            }
            else
            {
                // Get consume buffer count
                context.DeviceContext.CopyStructureCount(frameConstBuffer, ParticlePerFrame.NumParticlesOffset, BufferProxies[0].UAV);
                // Calculate existing particles
                pass = this.effectTechnique.GetPassByIndex(1);
                pass.Apply(context.DeviceContext);
                context.DeviceContext.ComputeShader.SetUnorderedAccessView(0, BufferProxies[0].UAV);
                context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, BufferProxies[1].UAV, 0);
                context.DeviceContext.ComputeShader.SetConstantBuffer(1, frameConstBuffer);
                context.DeviceContext.Dispatch(particleCountInternal / 512, 1, 1);
                // Get append buffer count
                context.DeviceContext.CopyStructureCount(particleCountGSIABuffer, 0, BufferProxies[1].UAV);
            }

            //#if DEBUG
            //            DebugCount("UAV 0", context.DeviceContext, BufferProxies[0].UAV);
            //#endif


            if (totalElapsed > insertThrottle)
            {
                // Add more particles 
                pass = this.effectTechnique.GetPassByIndex(0);
                pass.Apply(context.DeviceContext);
                context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, BufferProxies[1].UAV);
                context.DeviceContext.ComputeShader.SetConstantBuffer(1, frameConstBuffer);
                context.DeviceContext.Dispatch(1, 1, 1);
                totalElapsed = 0;
#if DEBUG
                //     DebugCount("UAV 1", context.DeviceContext, BufferProxies[1].UAV);
#endif
            }

            // Clear
            context.DeviceContext.ComputeShader.SetUnorderedAccessView(0, null);
            context.DeviceContext.ComputeShader.SetUnorderedAccessView(1, null);

            // Swap UAV buffers for next frame
            var bproxy = BufferProxies[0];
            BufferProxies[0] = BufferProxies[1];
            BufferProxies[1] = bproxy;

            // Render existing particles
            simulationStateVar.SetResource(BufferProxies[0].SRV);
            context.DeviceContext.InputAssembler.InputLayout = null;
            context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            pass = this.effectTechnique.GetPassByIndex(2);
            pass.Apply(context.DeviceContext);
            context.DeviceContext.DrawInstancedIndirect(particleCountGSIABuffer, 0);
        }

#if DEBUG
        private void DebugCount(string src, DeviceContext context, UnorderedAccessView uav)
        {
            context.CopyStructureCount(particleCountStaging, 0, uav);
            DataStream ds;
            var db = context.MapSubresource(particleCountStaging, MapMode.Read, MapFlags.None, out ds);
            int CurrentParticleCount = ds.ReadInt();
            System.Diagnostics.Debug.WriteLine("{0}: {1}", src, CurrentParticleCount);
            context.UnmapSubresource(particleCountStaging, 0);
        }
#endif
    }

    public sealed class BufferViewProxy : System.IDisposable
    {
        public Buffer Buffer;
        public UnorderedAccessView UAV;
        public ShaderResourceView SRV;

        public BufferViewProxy(Device device, ref BufferDescription bufferDesc, ref UnorderedAccessViewDescription uavDesc, ref ShaderResourceViewDescription srvDesc)
        {
            Buffer = new Buffer(device, bufferDesc);
            SRV = new ShaderResourceView(device, Buffer);
            UAV = new UnorderedAccessView(device, Buffer, uavDesc);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disposer.RemoveAndDispose(ref UAV);
                    Disposer.RemoveAndDispose(ref SRV);
                    Disposer.RemoveAndDispose(ref Buffer);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BufferViewProxy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
