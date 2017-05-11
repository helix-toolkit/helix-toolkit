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

        public static DependencyProperty ConsumerLocationProperty = DependencyProperty.Register("ConsumerLocation", typeof(Vector3), typeof(ParticleStormModel3D), new PropertyMetadata(Vector3.One,
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

        private bool isInitialParticleChanged = true;

        protected Vector3 emitterLocationInternal = Vector3.Zero;

        protected Vector3 consumerLocationInternal = Vector3.Zero;

        private EffectVectorVariable emitterLocationVar;

        private EffectVectorVariable consumerLocationVar;

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

        protected override void OnAttached()
        {
            base.OnAttached();
            emitterLocationVar = effect.GetVariableByName("EmitterLocation").AsVector();
            consumerLocationVar = effect.GetVariableByName("ConsumerLocation").AsVector();
            if (isInitialParticleChanged)
            {
                OnInitialParticleChanged(InitialParticles);
            }
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref emitterLocationVar);
            Disposer.RemoveAndDispose(ref consumerLocationVar);
            DisposeBuffers();
            base.OnDetach();
        }

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.ParticleStorm];
        }

        protected override void OnRender(RenderContext context)
        {
            emitterLocationVar.Set(emitterLocationInternal);
            consumerLocationVar.Set(consumerLocationInternal);
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
