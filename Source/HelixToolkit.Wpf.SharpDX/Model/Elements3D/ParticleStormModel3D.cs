using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.Windows;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;

namespace HelixToolkit.Wpf.SharpDX
{
    public class ParticleStormModel3D : InstanceGeometryModel3D
    {
        public static DependencyProperty InitialParticlesProperty = DependencyProperty.Register("InitialParticles", typeof(IList<Particle>), typeof(ParticleStormModel3D), new PropertyMetadata(null,
            (d, e) =>
            {
                (d as ParticleStormModel3D).OnInitialParticleChanged((IList<Particle>)e.NewValue);
            }
            ));

        public IList<Particle> InitialParticles
        {
            set
            {
                SetValue(InitialParticlesProperty, value);
            }
            get
            {
                return (IList<Particle>)GetValue(InitialParticlesProperty);
            }
        }

        public static DependencyProperty EmitterLocationProperty = DependencyProperty.Register("EmitterLocation", typeof(Vector3), typeof(ParticleStormModel3D), new PropertyMetadata(Vector3.Zero,
            (d, e) =>
            {
                (d as ParticleStormModel3D).OnEmitterLocationChanged((Vector3)e.NewValue);
            }
            ));

        public Vector3 EmitterLocation
        {
            set
            {
                SetValue(EmitterLocationProperty, value);
            }
            get
            {
                return (Vector3)GetValue(EmitterLocationProperty);
            }
        }

        public static DependencyProperty ConsumerLocationProperty = DependencyProperty.Register("ConsumerLocation", typeof(Vector3), typeof(ParticleStormModel3D),
            new PropertyMetadata(new Vector3(0,10,0),
            (d, e) =>
            {
                (d as ParticleStormModel3D).OnConsumerLocationChanged((Vector3)e.NewValue);
            }
            ));

        public Vector3 ConsumerLocation
        {
            set
            {
                SetValue(ConsumerLocationProperty, value);
            }
            get
            {
                return (Vector3)GetValue(ConsumerLocationProperty);
            }
        }

        protected int particleCountInternal = 0;

        private float insertThrottle = 0;

        private float prevTimeMillis = 0;

        private float totalElapsed = 0;

        private bool isInitialParticleChanged = true;

        protected Vector3 emitterLocationInternal = Vector3.Zero;

        protected Vector3 consumerLocationInternal = new Vector3(0, 100, 0);

        protected Vector3 randomVector = new Vector3(0,0.01f,0);

        protected EffectVectorVariable emitterLocationVar;

        protected EffectVectorVariable consumerLocationVar;

        protected EffectScalarVariable numberOfParticlesVar;

        protected EffectVectorVariable randomVectorVar;

        protected EffectScalarVariable timeFactorsVar;

        protected EffectUnorderedAccessViewVariable currentSimulationStateVar;

        protected EffectUnorderedAccessViewVariable newSimulationStateVar;

        protected EffectShaderResourceVariable simulationStateVar;

        protected EffectTransformVariables effectTransforms;

        private BufferDescription bufferDesc = new BufferDescription()
        {
            BindFlags = BindFlags.UnorderedAccess | BindFlags.ShaderResource,
            OptionFlags = ResourceOptionFlags.BufferStructured,
            StructureByteStride = Particle.SizeInBytes,
            CpuAccessFlags = CpuAccessFlags.None,
            Usage = ResourceUsage.Default
        };

        private UnorderedAccessViewDescription UAVBufferViewDesc = new UnorderedAccessViewDescription()
        {
            Dimension = UnorderedAccessViewDimension.Buffer
        };

        private ShaderResourceViewDescription SRVBufferViewDesc = new ShaderResourceViewDescription()
        {
            Dimension = ShaderResourceViewDimension.Buffer
        };

        protected readonly BufferViewProxy[] BufferProxies = new BufferViewProxy[2];

        protected EffectTechnique effectTechnique;

        private void OnInitialParticleChanged(IList<Particle> particles)
        {
            isInitialParticleChanged = true;
            DisposeBuffers();
            if (particles == null || particles.Count == 0 || !IsAttached)
            {
                return;
            }
            particleCountInternal = particles.Count;
            bufferDesc.SizeInBytes = particleCountInternal * Particle.SizeInBytes;
            var array = particles.ToArray();

            UAVBufferViewDesc.Buffer.ElementCount = particleCountInternal;
            UAVBufferViewDesc.Buffer.Flags = UnorderedAccessViewBufferFlags.Append;
            UAVBufferViewDesc.Buffer.FirstElement = 0;

            SRVBufferViewDesc.Buffer.ElementCount = particleCountInternal;
            SRVBufferViewDesc.Buffer.FirstElement = 0;

            for(int i=0; i < BufferProxies.Length; ++i)
            {
                BufferProxies[i] = new BufferViewProxy(array, Device, ref bufferDesc, ref UAVBufferViewDesc, ref SRVBufferViewDesc);
            }

            insertThrottle = (long)(8.0 * 30.0 / particleCountInternal);
            isInitialParticleChanged = false;
        }

        private void DisposeBuffers()
        {
            if (BufferProxies != null)
            {
                for (int i = 0; i < BufferProxies.Length; ++i)
                {
                    BufferProxies[i]?.Dispose();
                    BufferProxies[i] = null;
                }
            }
        }

        private void OnEmitterLocationChanged(Vector3 location)
        {
            emitterLocationInternal = location;
        }

        private void OnConsumerLocationChanged(Vector3 location)
        {
            consumerLocationInternal = location;
        }

        protected override bool CheckGeometry()
        {
            return true;
        }

        protected override void OnRasterStateChanged()
        {
            Disposer.RemoveAndDispose(ref this.rasterState);
            if (!IsAttached) { return; }
            // --- set up rasterizer states
            var rasterStateDesc = new RasterizerStateDescription()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = -2,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled
            };

            try { this.rasterState = new RasterizerState(this.Device, rasterStateDesc); }
            catch (System.Exception)
            {
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (!base.OnAttach(host))
            {
                return false;
            }
            emitterLocationVar = effect.GetVariableByName("EmitterLocation").AsVector();
            consumerLocationVar = effect.GetVariableByName("ConsumerLocation").AsVector();
            numberOfParticlesVar = effect.GetVariableByName("NumParticles").AsScalar();
            randomVectorVar = effect.GetVariableByName("RandomVector").AsVector();
            timeFactorsVar = effect.GetVariableByName("TimeFactors").AsScalar();
            currentSimulationStateVar = effect.GetVariableByName("CurrentSimulationState").AsUnorderedAccessView();
            newSimulationStateVar = effect.GetVariableByName("NewSimulationState").AsUnorderedAccessView();
            simulationStateVar = effect.GetVariableByName("SimulationState").AsShaderResource();
            effectTechnique = effect.GetTechniqueByName(this.renderTechnique.Name);
            this.effectTransforms = new EffectTransformVariables(this.effect);
            return true;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (isInitialParticleChanged)
            {
                OnInitialParticleChanged(InitialParticles);
            }
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref emitterLocationVar);
            Disposer.RemoveAndDispose(ref consumerLocationVar);
            Disposer.RemoveAndDispose(ref numberOfParticlesVar);
            Disposer.RemoveAndDispose(ref randomVectorVar);
            Disposer.RemoveAndDispose(ref timeFactorsVar);
            Disposer.RemoveAndDispose(ref currentSimulationStateVar);
            Disposer.RemoveAndDispose(ref newSimulationStateVar);
            Disposer.RemoveAndDispose(ref simulationStateVar);
            Disposer.RemoveAndDispose(ref effectTransforms);
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

        protected override void OnRender(RenderContext context)
        {
            var worldMatrix = this.modelMatrix * context.worldMatrix;
            this.effectTransforms.mWorld.SetMatrix(ref worldMatrix);
            var bproxy = BufferProxies[0];
            BufferProxies[0] = BufferProxies[1];
            BufferProxies[1] = bproxy;

            emitterLocationVar.Set(emitterLocationInternal);
            consumerLocationVar.Set(consumerLocationInternal);
            numberOfParticlesVar.Set(particleCountInternal);
            randomVectorVar.Set(randomVector);

            float timeElapsed = ((float)context.TimeStamp.TotalMilliseconds - prevTimeMillis) /1000;
            prevTimeMillis = (float)context.TimeStamp.TotalMilliseconds;
            timeFactorsVar.Set(timeElapsed);

            EffectPass pass;

            //totalElapsed += timeElapsed;
            //if (totalElapsed > insertThrottle)
            //{
            //    newSimulationStateVar.Set(BufferProxies[0].UAV);
            //    pass = this.effectTechnique.GetPassByIndex(0);
            //    pass.Apply(context.DeviceContext);
            //    context.DeviceContext.Dispatch(1, 1, 1);
            //    totalElapsed = 0;
            //}
            currentSimulationStateVar.Set(BufferProxies[0].UAV);
            newSimulationStateVar.Set(BufferProxies[1].UAV);
            pass = this.effectTechnique.GetPassByIndex(1);
            pass.Apply(context.DeviceContext);
            // --- draw
            context.DeviceContext.Dispatch(particleCountInternal / 512, 1, 1);

            simulationStateVar.SetResource(BufferProxies[1].SRV);
            context.DeviceContext.InputAssembler.InputLayout = null;
            context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            context.DeviceContext.Rasterizer.State = this.rasterState;
            pass = this.effectTechnique.GetPassByIndex(2);
            pass.Apply(context.DeviceContext);
            context.DeviceContext.Draw(particleCountInternal, 0);
        }

        protected override bool CanHitTest(IRenderMatrices context)
        {
            return false;
        }

        protected override bool OnHitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class BufferViewProxy : System.IDisposable
    {
        public Buffer Buffer;
        public UnorderedAccessView UAV;
        public ShaderResourceView SRV;

        public BufferViewProxy(Particle[] array, Device device, ref BufferDescription bufferDesc, ref UnorderedAccessViewDescription uavDesc, ref ShaderResourceViewDescription srvDesc)
        {
            Buffer = Buffer.Create(device, array, bufferDesc);
            SRV = new ShaderResourceView(device, Buffer, srvDesc);
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
